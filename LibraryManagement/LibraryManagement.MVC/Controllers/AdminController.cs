using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs;
using LibraryManagement.Business.DTOs.AuthDTOs;
using LibraryManagement.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly LibraryApiClient _apiClient;

        public AdminController(LibraryApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stats = await _apiClient.GetDashboardStatsAsync();
            return View(stats);
        }

        [HttpGet]
        public async Task<IActionResult> Users(string? search = null)
        {
            ViewData["Search"] = search;
            var users = await _apiClient.GetUsersAsync(search);
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu nhập vào không hợp lệ.";
                return RedirectToAction(nameof(Users));
            }

            var success = await _apiClient.CreateUserAsync(createUserDto);
            if (success)
            {
                TempData["Success"] = "Tạo tài khoản mới thành công.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi tạo tài khoản. Vui lòng kiểm tra lại email.";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var success = await _apiClient.ToggleUserStatusAsync(id);
            if (success)
            {
                TempData["Success"] = "Thay đổi trạng thái tài khoản thành công.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi thay đổi trạng thái tài khoản.";
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> Authors(string? search = null)
        {
            ViewData["Search"] = search;
            var authors = await _apiClient.GetAuthorsAsync(search);
            return View(authors);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDto createAuthorDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu nhập vào không hợp lệ.";
                return RedirectToAction(nameof(Authors));
            }

            var success = await _apiClient.CreateAuthorAsync(createAuthorDto);
            if (success)
            {
                TempData["Success"] = "Thêm tác giả mới thành công.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi thêm tác giả.";
            }
            return RedirectToAction(nameof(Authors));
        }

        [HttpPost]
        public async Task<IActionResult> EditAuthor(int id, UpdateAuthorDto updateAuthorDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu nhập vào không hợp lệ.";
                return RedirectToAction(nameof(Authors));
            }

            var success = await _apiClient.UpdateAuthorAsync(id, updateAuthorDto);
            if (success)
            {
                TempData["Success"] = "Cập nhật tác giả thành công.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật tác giả.";
            }
            return RedirectToAction(nameof(Authors));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var success = await _apiClient.DeleteAuthorAsync(id);
            if (success)
            {
                TempData["Success"] = "Xóa tác giả thành công.";
            }
            else
            {
                TempData["Error"] = "Không thể xóa tác giả này vì tác giả này đã có sách trong thư viện.";
            }
            return RedirectToAction(nameof(Authors));
        }
    }
}
