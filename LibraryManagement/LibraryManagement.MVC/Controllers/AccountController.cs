using System.Security.Claims;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
            var result = await _authService.RegisterAsync(model);

            if (!result)
            {
                ViewBag.Error = "đăng kí thất bại";
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
