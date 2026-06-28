using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int pageNumber = 1, int pageSize = 10)
        {
            var model = await _roomService.GetRoomsAsync(search, status, pageNumber, pageSize);

            if (model == null)
            {
                TempData["Error"] = "Không thể tải danh sách phòng. Vui lòng thử lại sau.";
                model = new RoomListViewModel();
            }

            // Keep query parameter inputs in model to repopulate fields
            model.Search = search;
            model.Status = status;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _roomService.GetRoomByIdAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCreateForm()
        {
            var model = new RoomViewModel();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", model);
            }

            var error = await _roomService.CreateRoomAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm phòng thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Edit ─────────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEditForm(Guid id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();

            return PartialView("_Edit", room);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", model);
            }

            var error = await _roomService.UpdateRoomAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật thông tin phòng thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _roomService.DeleteRoomAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa phòng thành công!" });

            return Json(new { success = false, message = "Xóa phòng thất bại. Có thể do phòng này đang có đặt phòng được liên kết trong hệ thống." });
        }
    }
}
