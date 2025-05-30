using Microsoft.AspNetCore.Mvc;
using WordRace000.Data;
using WordRace000.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace WordRace000.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            
            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre!");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("UserId", user.Id.ToString()),
                new Claim("ProfilePicture", user.ProfilePicture ?? "/images/default-avatar.png")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = "Başarıyla giriş yaptınız!";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            _logger.LogInformation("Register attempt started for email: {Email}", user.Email);

            try 
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid. Errors: {Errors}", 
                        string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)));
                    return View(user);
                }

                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    _logger.LogWarning("Email {Email} is already in use", user.Email);
                    ModelState.AddModelError("Email", "Bu email adresi zaten kullanımda!");
                    return View(user);
                }

                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    _logger.LogWarning("Username {Username} is already in use", user.Username);
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanımda!");
                    return View(user);
                }

                _logger.LogInformation("Adding new user to database: {Username}", user.Username);
                _context.Users.Add(user);
                
                _logger.LogInformation("Saving changes to database");
                var saveResult = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChanges result: {Result}", saveResult);

                TempData["SuccessMessage"] = "Hesabınız başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.";
                _logger.LogInformation("User registration successful, redirecting to Login");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                return View(user);
            }
        }

        // POST: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Başarıyla çıkış yaptınız!";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            
            if (user == null)
            {
                ModelState.AddModelError("", "Bu email adresi ile kayıtlı kullanıcı bulunamadı!");
                return View();
            }

            // TODO: Şifre sıfırlama emaili gönder
            TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı email adresinize gönderildi.";
            return RedirectToAction("Login");
        }
    }
} 