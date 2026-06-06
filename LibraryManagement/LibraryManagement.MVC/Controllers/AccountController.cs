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

            // Lưu JWT
            HttpContext.Session.SetString(
                "AccessToken",
                user.Token);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
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
