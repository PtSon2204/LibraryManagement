using System.Security.Claims;
using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryManagement.MVC.ViewModels.Auth;

namespace LibraryManagement.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _authService;

        public AccountController(IAccountService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _authService.LoginAsync(model);

            if (user == null)
            {
                ViewBag.Error = "Tài khoản và mật khẩu sai. Vui lòng kiểm tra lại";
                return View(model);
            }

            // Lưu JWT vào Claims thay vì Session để sống sót qua browser restart
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("jwt_token", user.Token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            if (user.Role == "Librarian")
            {
                return RedirectToAction("Index", "Librarian");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Register(RegisterViewModel model)
        {
            var errors = await _authService.RegisterAsync(model);

            if (errors != null)
            {
                foreach (var item in errors.Errors)
                {
                    foreach (var message in item.Value)
                    {
                        ModelState.AddModelError(item.Key, message);
                    }
                }

                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var error = await _authService.ForgotPasswordAsync(model.Email);

            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(model);
            }

            ViewBag.Message = "Mật khẩu mới ngẫu nhiên đã được gửi đến email của bạn. Vui lòng kiểm tra email và đăng nhập.";
            return View();
        }



        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("AccessToken");

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }
    }
}
