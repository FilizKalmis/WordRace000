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
        public IActionResult Create()
        {
            return View(new Quiz());
        }

        // POST: Quiz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartQuiz()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
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

            // Rastgele 10 kelime seç
            var words = await _context.Words
                .OrderBy(r => Guid.NewGuid())
                .Take(10)
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