using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordRace000.Data;
using WordRace000.Models;

namespace WordRace000.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
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

            var totalWords = await _context.Words.CountAsync();
            var learnedWords = await _context.LearnedWords
                .Where(lw => lw.UserId == userId)
                .CountAsync();

            var inProgressWords = await _context.WordProgresses
                .Where(wp => wp.UserId == userId && !wp.IsLearned)
                .CountAsync();

            var totalQuizzes = await _context.Quizzes
                .Where(q => q.UserId == userId)
                .CountAsync();

            var lastQuiz = await _context.Quizzes
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .FirstOrDefaultAsync();

            var categoryProgress = await _context.Category
                .Select(c => new CategoryProgressViewModel
                {
                    CategoryName = c.CategoryName,
                    TotalWords = _context.Words.Count(w => w.CategoryId == c.Id),
                    LearnedWords = _context.LearnedWords
                        .Count(lw => lw.Word.CategoryId == c.Id && lw.UserId == userId)
                })
                .ToListAsync();

            var viewModel = new ProfileViewModel
            {
                User = user,
                TotalWords = totalWords,
                LearnedWords = learnedWords,
                InProgressWords = inProgressWords,
                TotalQuizzes = totalQuizzes,
                LastQuizDate = lastQuiz?.CreatedAt,
                CategoryProgress = categoryProgress
            };

            return View(viewModel);
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