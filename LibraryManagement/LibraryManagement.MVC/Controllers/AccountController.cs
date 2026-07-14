using System.Security.Claims;
using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryManagement.MVC.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication.Google;

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
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback), "Account")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Không đặt route attribute - để middleware xử lý /signin-google trước,
        // sau đó redirect về đây tại /Account/GoogleCallback
        [HttpGet]
        public async Task<IActionResult> GoogleCallback()
        {
            // Middleware đã xử lý OAuth callback và sign in vào Cookie scheme với Google claims
            var email = User.FindFirstValue(ClaimTypes.Email);
            var fullName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Không lấy được email từ Google.";
                return RedirectToAction(nameof(Login));
            }

            // Sign out cookie tạm do Google middleware tạo
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Gọi API để tìm/tạo tài khoản và nhận JWT
            var user = await _authService.GoogleLoginAsync(email, fullName);

            if (user == null)
            {
                TempData["Error"] = "Đăng nhập Google thất bại. Vui lòng thử lại.";
                return RedirectToAction(nameof(Login));
            }

            // Tạo cookie session với JWT — giống hệt luồng login thường
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

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

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

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var model = new ChangePasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Email is read-only, ensure it is set for the view if validation fails
            model.Email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var error = await _authService.ChangePasswordAsync(model);

            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index", "Home");
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
