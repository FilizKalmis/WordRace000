using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordRace000.Data;
using WordRace000.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordRace000.Controllers
{
    public class WordleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WordleController> _logger;

        public WordleController(ApplicationDbContext context, ILogger<WordleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Play()
        {
            try
            {
                // Veritabanından rastgele bir kelime seç
                var word = await _context.Words
                    .Include(w => w.Category)
                    .OrderBy(w => Guid.NewGuid())
                    .FirstOrDefaultAsync();

                if (word == null)
                {
                    _logger.LogWarning("Veritabanında kelime bulunamadı.");
                    return RedirectToAction("Index");
                }

                // Yeni bir oyun kaydı oluştur
                var game = new WordleGame
                {
                    WordId = word.Id,
                    UserId = 1, // Şimdilik sabit kullanıcı ID
                    CreatedAt = DateTime.UtcNow,
                    AttemptCount = 0,
                    IsCompleted = false,
                    IsSuccessful = false
                };

                await _context.WordleGame.AddAsync(game);
                await _context.SaveChangesAsync();

                // View model oluştur
                var viewModel = new WordleViewModel
                {
                    GameId = game.Id,
                    TargetWord = word.English.ToUpper(),
                    CategoryName = word.Category?.CategoryName ?? "Genel",
                    WordLength = word.English.Length
                };

                return View(viewModel);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Wordle oyunu başlatılırken hata oluştu.");
                
                // Veritabanında kelime yoksa kullanıcıyı bilgilendir
                if (!_context.Words.Any())
                {
                    TempData["Error"] = "Veritabanında kelime bulunamadı. Lütfen sistem yöneticisi ile iletişime geçin.";
                    return RedirectToAction("Index");
                }

                // Test kelimesi ile devam et
                var viewModel = new WordleViewModel
                {
                    GameId = 0,
                    TargetWord = "CHAIR",
                    CategoryName = "Test Kategorisi",
                    WordLength = 5
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckWord([FromBody] GuessRequest request)
        {
            if (string.IsNullOrEmpty(request.Guess) || string.IsNullOrEmpty(request.TargetWord))
            {
                return Json(new { error = "Geçersiz tahmin" });
            }

            var guess = request.Guess.ToUpper();
            var targetWord = request.TargetWord.ToUpper();

            if (guess.Length != targetWord.Length)
            {
                return Json(new { error = $"Lütfen {targetWord.Length} harfli bir kelime girin!" });
            }

            try
            {
                // Oyun kaydını güncelle
                if (request.GameId > 0)
                {
                    var game = await _context.WordleGame.FindAsync(request.GameId);
                    if (game != null)
                    {
                        game.AttemptCount++;
                        game.Guesses = string.IsNullOrEmpty(game.Guesses) 
                            ? guess 
                            : game.Guesses + ";" + guess;
                        
                        if (guess == targetWord)
                        {
                            game.IsCompleted = true;
                            game.IsSuccessful = true;
                            game.CompletedAt = DateTime.UtcNow;
                        }
                        else if (game.AttemptCount >= 6)
                        {
                            game.IsCompleted = true;
                            game.CompletedAt = DateTime.UtcNow;
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                var result = new List<string>();
                var letterCount = new Dictionary<char, int>();

                // Hedef kelimedeki her harfin sayısını hesapla
                foreach (var c in targetWord)
                {
                    if (!letterCount.ContainsKey(c))
                        letterCount[c] = 0;
                    letterCount[c]++;
                }

                // Önce doğru konumdaki harfleri işaretle
                var used = new bool[guess.Length];
                for (int i = 0; i < guess.Length; i++)
                {
                    if (guess[i] == targetWord[i])
                    {
                        result.Add("correct");
                        letterCount[guess[i]]--;
                        used[i] = true;
                    }
                    else
                    {
                        result.Add("");
                    }
                }

                // Sonra yanlış konumdaki harfleri işaretle
                for (int i = 0; i < guess.Length; i++)
                {
                    if (!used[i])
                    {
                        if (letterCount.ContainsKey(guess[i]) && letterCount[guess[i]] > 0)
                        {
                            result[i] = "wrong-position";
                            letterCount[guess[i]]--;
                        }
                        else
                        {
                            result[i] = "wrong";
                        }
                    }
                }

                return Json(new { feedback = result });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Tahmin kontrol edilirken hata oluştu.");
                return Json(new { error = "Bir hata oluştu, lütfen tekrar deneyin." });
            }
        }

        public async Task<IActionResult> History()
        {
            try
            {
                var gameHistory = await _context.WordleGame
                    .Include(w => w.Word)
                    .Include(w => w.Word.Category)
                    .Where(g => g.IsCompleted)
                    .OrderByDescending(g => g.CompletedAt)
                    .Select(g => new WordleHistoryViewModel
                    {
                        CategoryName = g.Word != null && g.Word.Category != null ? g.Word.Category.CategoryName : "Bilinmiyor",
                        Word = g.Word != null ? g.Word.English : "Bilinmiyor",
                        AttemptCount = g.AttemptCount,
                        IsSuccessful = g.IsSuccessful,
                        CompletedAt = g.CompletedAt,
                        Guesses = !string.IsNullOrEmpty(g.Guesses) 
                            ? g.Guesses.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() 
                            : new List<string>()
                    })
                    .ToListAsync();

                return View(gameHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oyun geçmişi yüklenirken hata oluştu.");
                return RedirectToAction("Index");
            }
        }
    }
} 