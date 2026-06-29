using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Librarian;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Librarian,Admin")]
    public class LibrarianController : Controller
    {
        private readonly IStaffDashboardService _staffDashboardService;
        private readonly IUserService _userService;

        public LibrarianController(IStaffDashboardService staffDashboardService, IUserService userService)
        {
            _staffDashboardService = staffDashboardService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _staffDashboardService.GetDashboardAsync();
            if (model == null)
            {
                ViewBag.DashboardError = "Không thể tải dữ liệu dashboard. Vui lòng thử lại sau.";
                model = new ViewModels.Dashboard.StaffDashboardViewModel();
            }

            return View(model);
        }

        // ─── Reader Management for Librarian ───────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Users(string? search, int page = 1)
        {
            var readers = await _userService.GetReadersAsync(search, page, 10);

            var model = new LibrarianUsersViewModel
            {
                Readers = readers ?? new PagedResult<ReaderListItemDto>
                {
                    Data = new List<ReaderListItemDto>(),
                    PageNumber = page,
                    PageSize = 10,
                    TotalRecords = 0,
                    TotalPages = 0
                },
                Search = search
            };

            return View(model);
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
