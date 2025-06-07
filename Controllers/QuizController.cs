using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordRace000.Data;
using WordRace000.Models;

namespace WordRace000.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var quizzes = await _context.Quizzes
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(quizzes);
        }

        // GET: Quiz/Create
        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            
            // Dünkü quizde doğru bilinen kelimeleri al
            var yesterdaysCorrectWords = await _context.QuizDetails
                .Include(qd => qd.Quiz)
                .Where(qd => qd.Quiz.UserId == userId && 
                            qd.Quiz.CreatedAt.Date == yesterday &&
                            qd.IsCorrect)
                .Select(qd => qd.WordId)
                .Distinct()
                .ToListAsync();

            // Bugün tekrar edilmesi gereken kelimeler (sadece dün doğru bilinenler)
            var scheduledWords = await _context.QuizSchedules
                .Where(qs => qs.UserId == userId && 
                            qs.NextTestDate.Date <= today && 
                            yesterdaysCorrectWords.Contains(qs.WordId) &&
                            (!qs.IsCompleted.HasValue || !qs.IsCompleted.Value))
                .Select(qs => qs.WordId)
                .ToListAsync();

            // Kullanıcının ayarlarını al
            var settings = await _context.Settings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            // Eğer ayar yoksa varsayılan değer olarak 10 kullan
            ViewBag.DailyWordCount = settings?.DailyWordCount ?? 10;
            ViewBag.ScheduledWordCount = scheduledWords.Count;

            return View(new Quiz());
        }

        // POST: Quiz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartQuiz()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            
            // Dünkü quizde doğru bilinen kelimeleri al
            var yesterdaysCorrectWords = await _context.QuizDetails
                .Include(qd => qd.Quiz)
                .Where(qd => qd.Quiz.UserId == userId && 
                            qd.Quiz.CreatedAt.Date == yesterday &&
                            qd.IsCorrect)
                .Select(qd => qd.WordId)
                .Distinct()
                .ToListAsync();

            // Bugün tekrar edilmesi gereken kelimeler (sadece dün doğru bilinenler)
            var scheduledWords = await _context.QuizSchedules
                .Where(qs => qs.UserId == userId && 
                            qs.NextTestDate.Date <= today && 
                            yesterdaysCorrectWords.Contains(qs.WordId) &&
                            (!qs.IsCompleted.HasValue || !qs.IsCompleted.Value))
                .Select(qs => qs.WordId)
                .ToListAsync();

            // Kullanıcı ayarlarından yeni kelime sayısını al
            var settings = await _context.Settings
                .FirstOrDefaultAsync(s => s.UserId == userId);
            int newWordCount = settings?.DailyWordCount ?? 10;

            // Yeni kelimeler seç (hiç test edilmemiş veya schedule'da olmayan kelimelerden)
            var newWords = await _context.Words
                .Where(w => !_context.QuizSchedules
                    .Any(qs => qs.WordId == w.Id && qs.UserId == userId))
                .OrderBy(r => Guid.NewGuid())
                .Take(newWordCount)
                .Select(w => w.Id)
                .ToListAsync();

            // Tüm kelime ID'lerini birleştir
            var allWordIds = scheduledWords.Concat(newWords).ToList();

            // Yeni quiz oluştur
            var quiz = new Quiz
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false,
                Score = 0
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            // Quiz detaylarını oluştur
            foreach (var wordId in allWordIds)
            {
                var quizDetail = new QuizDetail
                {
                    QuizId = quiz.Id,
                    WordId = wordId,
                    IsCorrect = false,
                    IsAnswered = false
                };
                _context.QuizDetails.Add(quizDetail);
            }

            // Yeni kelimeler için QuizSchedule kayıtları oluştur
            foreach (var wordId in newWords)
            {
                var schedule = new QuizSchedule
                {
                    UserId = userId,
                    WordId = wordId,
                    NextTestDate = today.AddDays(1), // 1. aşama: 1 gün sonra
                    AttemptCount = 0,
                    IsCompleted = false,
                    CreatedAt = today,
                    LastUpdatedAt = today
                };
                _context.QuizSchedules.Add(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Take), new { id = quiz.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
            var quiz = await _context.Quizzes
                .Include(q => q.QuizDetails)
                .ThenInclude(qd => qd.Word)
                .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (quiz == null)
            {
                return NotFound();
            }

            // Henüz cevaplanmamış ilk soruyu bul
            var currentQuestion = quiz.QuizDetails
                .FirstOrDefault(qd => !qd.IsAnswered);

            if (currentQuestion == null)
            {
                // Tüm sorular cevaplandı, sonuç sayfasına yönlendir
                quiz.IsCompleted = true;
                quiz.Score = quiz.QuizDetails.Count(qd => qd.IsCorrect) * 10;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = quiz.Id });
            }

            return View(currentQuestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(int quizDetailId, string answer, bool moveNext = false)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var today = DateTime.UtcNow.Date;
            
            var quizDetail = await _context.QuizDetails
                .Include(qd => qd.Word)
                .Include(qd => qd.Quiz)
                .FirstOrDefaultAsync(qd => qd.Id == quizDetailId && qd.Quiz.UserId == userId);

            if (quizDetail == null || quizDetail.Word == null)
            {
                return NotFound();
            }

            if (!moveNext)  // Normal cevap verme durumu
            {
                // Cevabı kontrol et
                bool isCorrect = !string.IsNullOrEmpty(answer) && 
                               answer.Trim().ToLower() == quizDetail.Word.Turkish?.Trim().ToLower();
                quizDetail.IsCorrect = isCorrect;

                // Kelime ilerlemesini güncelle (doğru veya yanlış)
                var wordLog = new WordLog
                {
                    UserId = userId,
                    WordId = quizDetail.WordId,
                    LogDate = DateTime.UtcNow,
                    IsCorrect = isCorrect
                };
                _context.WordLogs.Add(wordLog);

                // QuizSchedule'ı güncelle
                var schedule = await _context.QuizSchedules
                    .FirstOrDefaultAsync(qs => 
                        qs.UserId == userId && 
                        qs.WordId == quizDetail.WordId);

                if (schedule != null)
                {
                    if (isCorrect)
                    {
                        schedule.AttemptCount++;
                        schedule.LastUpdatedAt = today;
                        
                        // Bir sonraki test tarihini belirle
                        switch(schedule.AttemptCount)
                        {
                            case 1: 
                                schedule.NextTestDate = today.AddDays(1);     // 1 gün
                                break;
                            case 2: 
                                schedule.NextTestDate = today.AddDays(7);     // 1 hafta
                                break;
                            case 3: 
                                schedule.NextTestDate = today.AddMonths(1);   // 1 ay
                                break;
                            case 4: 
                                schedule.NextTestDate = today.AddMonths(3);   // 3 ay
                                break;
                            case 5: 
                                schedule.NextTestDate = today.AddMonths(6);   // 6 ay
                                                               break;
                            case 6: 
                                schedule.NextTestDate = today.AddYears(1);    // 1 yıl
                                schedule.IsCompleted = true;
                                
                                // LearnedWords tablosuna ekle
                                var learnedWord = new LearnedWord 
                                { 
                                    UserId = userId,
                                    WordId = quizDetail.WordId,
                                    LearnedDate = today
                                };
                                _context.LearnedWords.Add(learnedWord);
                                break;
                        }
                    }
                    else
                    {
                        // Yanlış cevap - süreci başa döndür
                        schedule.AttemptCount = 0;
                        schedule.NextTestDate = today.AddDays(1); // Tekrar 1. aşamadan başla
                        schedule.LastUpdatedAt = today;
                    }
                }
            }
               else // Atlama durumu
            {
                // Soruyu atlama durumunda yanlış olarak işaretle
                quizDetail.IsCorrect = false;
                
                // Kelime ilerlemesini yanlış olarak kaydet
                var wordLog = new WordLog
                {
                    UserId = userId,
                    WordId = quizDetail.WordId,
                    LogDate = DateTime.UtcNow,
                    IsCorrect = false
                };
                _context.WordLogs.Add(wordLog);

                // Schedule'ı sıfırla
                var schedule = await _context.QuizSchedules
                    .FirstOrDefaultAsync(qs => 
                        qs.UserId == userId && 
                        qs.WordId == quizDetail.WordId);

                if (schedule != null)
                {
                    schedule.AttemptCount = 0;
                    schedule.NextTestDate = today.AddDays(1);
                    schedule.LastUpdatedAt = today;
                }
            }

            // Her durumda soruyu işaretlenmiş olarak kaydet

            quizDetail.IsAnswered = true;
            await _context.SaveChangesAsync();

            // QuizIDsini al ve take actiona yönlendir
            return RedirectToAction(nameof(Take), new { id = quizDetail.QuizId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
            var quiz = await _context.Quizzes
                .Include(q => q.QuizDetails)
                .ThenInclude(qd => qd.Word)
                .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpGet]
        public async Task<IActionResult> CheckSampleSentences()
        {
            // Tüm kelime-örnek cümle ilişkilerini getir
            var allWordSamples = await _context.WordSampleWords
                .Include(wsw => wsw.WordSample)
                .Include(wsw => wsw.Word)
                .Take(10) // İlk 10 kaydı al
                .ToListAsync();

             var result = allWordSamples.Select(wsw => new
            {
                WordId = wsw.WordId,
                Word = wsw.Word?.English,
                SampleId = wsw.WordSampleId,
                SampleText = wsw.WordSample?.SampleText
            }).ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCurrentWord(int quizId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
                      var quiz = await _context.Quizzes
                .Include(q => q.QuizDetails)
                .ThenInclude(qd => qd.Word)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.UserId == userId);

            if (quiz == null)
            {
                return Json(new { error = "Quiz bulunamadı" });
            }

            var currentQuestion = quiz.QuizDetails
                .FirstOrDefault(qd => !qd.IsCorrect);

            if (currentQuestion == null)
            {
                return Json(new { error = "Aktif soru bulunamadı" });
            }

            // Kelimeye ait örnek cümleleri getir
            var sampleSentences = await _context.WordSampleWords
                .Include(wsw => wsw.WordSample)
                .Where(wsw => wsw.WordId == currentQuestion.WordId)
                .Select(wsw => new
                {
                    WordSampleWordId = wsw.Id,
                    WordId = wsw.WordId,
                    WordSampleId = wsw.WordSampleId,
                    SampleText = wsw.WordSample.SampleText
                })
                .ToListAsync();

            return Json(new
            {
                wordId = currentQuestion.WordId,
                word = currentQuestion.Word.English,
                sampleSentences = sampleSentences
            });
        }

        [HttpGet]
        public async Task<IActionResult> CheckMissingSentences()
        {
                 // Tüm kelimeleri ve örnek cümle sayılarını getir
            var wordStats = await _context.Words
                .Select(w => new
                {
                    WordId = w.Id,
                    English = w.English,
                    SampleSentenceCount = _context.WordSampleWords
                        .Count(wsw => wsw.WordId == w.Id)
                })
                .OrderBy(w => w.SampleSentenceCount)
                .Take(20)  // İlk 20 kelimeyi göster
                .ToListAsync();

               return Json(new
            {
                totalWords = await _context.Words.CountAsync(),
                wordsWithNoSentences = await _context.Words
                    .CountAsync(w => !_context.WordSampleWords.Any(wsw => wsw.WordId == w.Id)),
                sampleWords = wordStats
            });
        }

        [HttpGet]
        public async Task<IActionResult> ListWordsWithoutSentences()
        {
           // Örnek cümlesi olmayan kelimeleri kategorilerine göre grupla
            var wordsWithoutSentences = await _context.Words
                .Include(w => w.Category)  // Category'yi include et
                .Where(w => !_context.WordSampleWords.Any(wsw => wsw.WordId == w.Id))
                .OrderBy(w => w.English)
                .Select(w => new
                {
                    w.Id,
                    w.English,
                    w.Turkish,
                    CategoryName = w.Category != null ? w.Category.CategoryName : "Uncategorized"
                })
                .ToListAsync();

            // Kelimeleri kategorilerine göre grupla
            var groupedWords = wordsWithoutSentences
                .GroupBy(w => w.CategoryName)
                .Select(g => new
                {
                    Category = g.Key,
                    Words = g.Select(w => new
                    {
                        w.Id,
                        w.English,
                        w.Turkish
                    }).ToList()
                })
                .OrderBy(g => g.Category)
                .ToList();

            // Sonuçları HTML olarak göster
            return View(groupedWords);

        }

        [HttpPost]
        public async Task<IActionResult> AddSampleSentences([FromBody] List<WordSentence> sentences)
        {

            foreach (var sentence in sentences)
            {
                var wordSample = new WordSample
                {
                    SampleText = sentence.SampleText
                };
                _context.WordSample.Add(wordSample);
                await _context.SaveChangesAsync();

                var wordSampleWord = new WordSampleWord
                {
                    WordId = sentence.WordId,
                    WordSampleId = wordSample.Id
                };
                _context.WordSampleWords.Add(wordSampleWord);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });

        }

        public class WordSentence
        {
            public int WordId { get; set; }
            public string SampleText { get; set; }

        }
    }
}