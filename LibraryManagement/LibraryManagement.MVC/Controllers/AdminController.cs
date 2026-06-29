using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IUserService _userService;

        public AdminController(IDashboardService dashboardService, IUserService userService)
        {
            _dashboardService = dashboardService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            if (stats == null)
            {
                stats = new ViewModels.Dashboard.DashboardViewModel();
            }
            return View(stats);
        }

        // ─── User Management List ───────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Users(string? searchLibrarians, string? searchReaders, int pageLibrarians = 1, int pageReaders = 1)
        {
            var librarians = await _userService.GetLibrariansAsync(searchLibrarians, pageLibrarians, 10);
            var readers = await _userService.GetReadersAsync(searchReaders, pageReaders, 10);

            var model = new AdminUsersViewModel
            {
                Librarians = librarians ?? new PagedResult<LibrarianListItemDto>
                {
                    Data = new List<LibrarianListItemDto>(),
                    PageNumber = pageLibrarians,
                    PageSize = 10,
                    TotalRecords = 0,
                    TotalPages = 0
                },
                Readers = readers ?? new PagedResult<ReaderListItemDto>
                {
                    Data = new List<ReaderListItemDto>(),
                    PageNumber = pageReaders,
                    PageSize = 10,
                    TotalRecords = 0,
                    TotalPages = 0
                },
                SearchLibrarians = searchLibrarians,
                SearchReaders = searchReaders
            };

            return View(model);
        }

        // ─── Librarian Create Form & Post ────────────────────────────────────────

        [HttpGet]
        public IActionResult GetCreateLibrarianForm()
        {
            var model = new CreateLibrarianDto();
            return PartialView("_CreateLibrarian", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLibrarian(CreateLibrarianDto model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateLibrarian", model);
            }

            try
            {
                var result = await _userService.CreateLibrarianAsync(model);
                if (result != null)
                {
                    return Json(new { success = true, message = $"Thêm thủ thư thành công! Mật khẩu khởi tạo gửi qua email: {result.Password}" });
                }
                return Json(new { success = false, message = "Không thể thêm thủ thư. Vui lòng thử lại sau." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ─── Reader Create Form & Post ──────────────────────────────────────────

        [HttpGet]
        public IActionResult GetCreateReaderForm()
        {
            var model = new CreateReaderDto();
            return PartialView("_CreateReader", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReader(CreateReaderDto model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateReader", model);
            }

            try
            {
                var result = await _userService.CreateReaderAsync(model);
                if (result != null)
                {
                    return Json(new { success = true, message = $"Thêm độc giả thành công! Mật khẩu khởi tạo: {result.Password}" });
                }
                return Json(new { success = false, message = "Không thể thêm độc giả. Vui lòng thử lại sau." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ─── Toggle Lock Status ────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> ToggleLibrarianStatus(Guid id)
        {
            var success = await _userService.ToggleLibrarianStatusAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Cập nhật trạng thái thủ thư thành công!" });
            }
            return Json(new { success = false, message = "Cập nhật trạng thái thất bại." });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleReaderStatus(Guid id)
        {
            var success = await _userService.ToggleReaderStatusAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Cập nhật trạng thái độc giả thành công!" });
            }
            return Json(new { success = false, message = "Cập nhật trạng thái thất bại." });
        }
    }
}
