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
            
            // Kullanıcının ayarlarını al
            var settings = await _context.Settings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            // Eğer ayar yoksa varsayılan değer olarak 10 kullan
            ViewBag.DailyWordCount = settings?.DailyWordCount ?? 10;

            return View(new Quiz());
        }

        // POST: Quiz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartQuiz()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
            // Kullanıcının ayarlarını al
            var settings = await _context.Settings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            // Eğer ayar yoksa varsayılan değer olarak 10 kullan
            int wordCount = settings?.DailyWordCount ?? 10;
            
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

            // Kullanıcının ayarlarındaki kelime sayısı kadar rastgele kelime seç
            var words = await _context.Words
                .OrderBy(r => Guid.NewGuid())
                .Take(wordCount)
                .ToListAsync();

            // Quiz detaylarını oluştur
            foreach (var word in words)
            {
                var quizDetail = new QuizDetail
                {
                    QuizId = quiz.Id,
                    WordId = word.Id,
                    IsCorrect = false
                };
                _context.QuizDetails.Add(quizDetail);

                // Her kelime için örnek cümle kontrolü yap
                var hasExampleSentence = await _context.WordSampleWords
                    .AnyAsync(wsw => wsw.WordId == word.Id);

                // Eğer kelimeye ait örnek cümle yoksa ekle
                if (!hasExampleSentence)
                {
                    // Kelime için örnek cümle oluştur
                    var sampleText = "";
                    switch (word.English.ToLower())
                    {
                        case "pig":
                            sampleText = "The pig is rolling in the mud.";
                            break;
                        case "bear":
                            sampleText = "I saw a big brown bear in the forest.";
                            break;
                        case "lion":
                            sampleText = "The lion is the king of the jungle.";
                            break;
                        case "dog":
                            sampleText = "My dog loves to play fetch.";
                            break;
                        case "cat":
                            sampleText = "The cat is sleeping on the windowsill.";
                            break;
                        default:
                            sampleText = $"This is an example sentence with the word '{word.English}'.";
                            break;
                    }

                    // Örnek cümleyi ekle
                    var wordSample = new WordSample
                    {
                        SampleText = sampleText
                    };
                    _context.WordSample.Add(wordSample);
                    await _context.SaveChangesAsync();

                    // Kelime-cümle ilişkisini kur
                    var wordSampleWord = new WordSampleWord
                    {
                        WordId = word.Id,
                        WordSampleId = wordSample.Id
                    };
                    _context.WordSampleWords.Add(wordSampleWord);
                }
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
                .FirstOrDefault(qd => !qd.IsCorrect);

            if (currentQuestion == null)
            {
                // Tüm sorular cevaplandı, sonuç sayfasına yönlendir
                quiz.IsCompleted = true;
                quiz.Score = quiz.QuizDetails.Count(qd => qd.IsCorrect) * 10;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = quiz.Id });
            }

            // Debug bilgisi için veritabanı durumunu kontrol edelim
            var totalWordSamples = await _context.WordSample.CountAsync();
            var totalWordSampleWords = await _context.WordSampleWords.CountAsync();
            
            // Kelimeye ait örnek cümleleri getir
            var currentWordSampleWords = await _context.WordSampleWords
                .Include(wsw => wsw.WordSample)
                .Where(wsw => wsw.WordId == currentQuestion.WordId)
                .ToListAsync();

            var debugMessage = $"Veritabanı Durumu:\n" +
                             $"- Toplam örnek cümle sayısı: {totalWordSamples}\n" +
                             $"- Toplam kelime-cümle ilişkisi: {totalWordSampleWords}\n" +
                             $"- Mevcut kelime (ID: {currentQuestion.WordId}, {currentQuestion.Word.English}) için:\n" +
                             $"  * İlişkili cümle sayısı: {currentWordSampleWords.Count}\n";

            if (currentWordSampleWords.Any())
            {
                debugMessage += "  * İlişkili cümleler:\n";
                foreach (var wsw in currentWordSampleWords)
                {
                    debugMessage += $"    - ID: {wsw.WordSampleId}, Text: {wsw.WordSample?.SampleText ?? "NULL"}\n";
                }
            }

            TempData["DebugInfo"] = debugMessage;

            // Eğer örnek cümle varsa rastgele birini seç
            if (currentWordSampleWords.Any())
            {
                var validSentences = currentWordSampleWords
                    .Where(wsw => wsw.WordSample != null && !string.IsNullOrEmpty(wsw.WordSample.SampleText))
                    .Select(wsw => wsw.WordSample.SampleText)
                    .ToList();

                if (validSentences.Any())
                {
                    Random rnd = new Random();
                    ViewBag.SampleSentence = validSentences[rnd.Next(validSentences.Count)];
                }
            }

            return View(currentQuestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(int quizDetailId, string answer)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
            var quizDetail = await _context.QuizDetails
                .Include(qd => qd.Word)
                .Include(qd => qd.Quiz)
                .FirstOrDefaultAsync(qd => qd.Id == quizDetailId && qd.Quiz.UserId == userId);

            if (quizDetail == null)
            {
                return NotFound();
            }

            // Cevabı kontrol et
            bool isCorrect = answer.Trim().ToLower() == quizDetail.Word.Turkish.Trim().ToLower();
            quizDetail.IsCorrect = isCorrect;

            if (isCorrect)
            {
                // Kelime ilerlemesini güncelle
                var wordLog = new WordLog
                {
                    UserId = userId,
                    WordId = quizDetail.WordId,
                    LogDate = DateTime.UtcNow,
                    IsCorrect = true
                };
                _context.WordLogs.Add(wordLog);
            }

            await _context.SaveChangesAsync();

            // Quiz'in ID'sini al ve Take action'ına yönlendir
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
    }
}