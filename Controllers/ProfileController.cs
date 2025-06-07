using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordRace000.Data;
using WordRace000.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WordRace000.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Get total words count
            var totalWords = await _context.Words.CountAsync();

            // Get learned words count
            var learnedWords = await _context.LearnedWords
                .Where(lw => lw.UserId == userId)
                .CountAsync();

            // Get in progress words count
            var inProgressWords = await _context.QuizSchedules
                .Where(qs => qs.UserId == userId && 
                           (!qs.IsCompleted.HasValue || !qs.IsCompleted.Value))
                .CountAsync();

            // Get total quizzes and last quiz date
            var totalQuizzes = await _context.Quizzes
                .Where(q => q.UserId == userId)
                .CountAsync();

            var lastQuizDate = await _context.Quizzes
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => q.CreatedAt)
                .FirstOrDefaultAsync();

            var categoryProgress = await GetCategoryProgress(userId);
            var viewModel = new ProfileViewModel
            {
                User = user,
                TotalWords = totalWords,
                LearnedWords = learnedWords,
                InProgressWords = inProgressWords,
                TotalQuizzes = totalQuizzes,
                LastQuizDate = lastQuizDate,
                CategoryProgress = categoryProgress
            };

            return View(viewModel);
        }

        private async Task<List<CategoryProgressViewModel>> GetCategoryProgress(int userId)
        {
            return await _context.Category
                .Select(c => new CategoryProgressViewModel
                {
                    CategoryName = c.CategoryName,
                    TotalWords = _context.Words.Count(w => w.CategoryId == c.Id),
                    LearnedWords = _context.LearnedWords
                        .Count(lw => lw.Word.CategoryId == c.Id && lw.UserId == userId),
                    LastUpdated = DateTime.UtcNow
                })
                .ToListAsync();
        }

        // POST: Profile/GenerateProgressReport
        [HttpPost]
        public async Task<IActionResult> GenerateProgressReport()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var progress = await GetCategoryProgress(userId);

                var report = new CategoryProgressReport
                {
                    UserId = userId,
                    GeneratedAt = DateTime.UtcNow,
                    TotalLearnedWords = progress.Sum(p => p.LearnedWords),
                    TotalWords = progress.Sum(p => p.TotalWords),
                    OverallProgress = progress.Sum(p => p.LearnedWords) * 100.0M / progress.Sum(p => p.TotalWords)
                };

                _context.CategoryProgressReports.Add(report);
                await _context.SaveChangesAsync();

                var details = progress.Select(p => new CategoryProgressDetail
                {
                    ReportId = report.Id,
                    CategoryName = p.CategoryName,
                    TotalWords = p.TotalWords,
                    LearnedWords = p.LearnedWords,
                    ProgressPercentage = p.TotalWords > 0 ? (p.LearnedWords * 100.0M / p.TotalWords) : 0,
                    LastUpdated = p.LastUpdated
                }).ToList();

                _context.CategoryProgressDetails.AddRange(details);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(PrintProgressReport), new { id = report.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlerleme raporu oluşturulurken hata oluştu");
                TempData["ErrorMessage"] = "Rapor oluşturulurken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Profile/PrintProgressReport/5
        public async Task<IActionResult> PrintProgressReport(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var report = await _context.CategoryProgressReports
                .Include(r => r.Details)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            // TODO: Gerçek kullanıcı ID'sini Session'dan al
            int userId = 1;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                }
            }
            return View(user);
        }

        // GET: Profile/Settings
        public async Task<IActionResult> Settings()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var settings = await _context.Settings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (settings == null)
            {
                settings = new Settings
                {
                    UserId = userId,
                    DailyWordCount = 10, // Varsayılan değer
                    EmailNotifications = true,
                    DarkMode = false
                };
                _context.Settings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return View(settings);
        }

        // POST: Profile/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(Settings settings)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(settings);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                }
            }
            return View(settings);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            // Mevcut şifreyi kontrol et
            if (user.Password != currentPassword)
            {
                return Json(new { success = false, message = "Mevcut şifre yanlış." });
            }

            // Yeni şifreyi güncelle
            user.Password = newPassword;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: Profile/UploadProfileImage
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Dosya seçilmedi." });
            }

            try
            {
                // Dosya uzantısını kontrol et
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                
                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Sadece resim dosyaları yüklenebilir (.jpg, .jpeg, .png, .gif)" });
                }

                // Dosya adını benzersiz yap
                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                
                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // URL'yi döndür
                var url = $"/uploads/profiles/{fileName}";
                return Json(new { success = true, url = url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Dosya yükleme sırasında bir hata oluştu: " + ex.Message });
            }
        }
    }
} 