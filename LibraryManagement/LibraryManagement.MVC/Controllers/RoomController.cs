using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IShelfService _shelfService;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public RoomController(IRoomService roomService, IShelfService shelfService, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _roomService = roomService;
            _shelfService = shelfService;
            _env = env;
        }

        private async Task PopulateFloorsAsync(RoomViewModel model)
        {
            var floors = await _shelfService.GetAllFloorsAsync();
            model.Floors = floors.Select(f => new SelectListItem
            {
                Value = f.FloorId.ToString(),
                Text = f.FloorName,
                Selected = model.FloorId.HasValue && model.FloorId.Value == f.FloorId
            }).ToList();
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
        public async Task<IActionResult> GetCreateForm()
        {
            var model = new RoomViewModel();
            await PopulateFloorsAsync(model);
            return PartialView("_Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFloorsAsync(model);
                return PartialView("_Create", model);
            }

            var uploadError = await HandleImageUpload(model.ImageFile, result => model.Image = result);
            if (uploadError != null)
                return Json(new { success = false, message = uploadError });

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

            await PopulateFloorsAsync(room);
            return PartialView("_Edit", room);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFloorsAsync(model);
                return PartialView("_Edit", model);
            }

            var uploadError = await HandleImageUpload(model.ImageFile, result => model.Image = result);
            if (uploadError != null)
                return Json(new { success = false, message = uploadError });

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

        private async Task<string?> HandleImageUpload(Microsoft.AspNetCore.Http.IFormFile? file, Action<string> setUrl)
        {
            if (file == null || file.Length == 0) return null;

            if (file.Length > MaxFileSizeBytes)
                return "Hình ảnh vượt quá 5 MB. Vui lòng chọn ảnh nhỏ hơn.";

            var ext = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return $"Định dạng không hỗ trợ ({ext}). Chỉ chấp nhận: jpg, jpeg, png, webp, gif.";

            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder = System.IO.Path.Combine(_env.WebRootPath, "images", "rooms");
            System.IO.Directory.CreateDirectory(folder);

            using (var stream = new System.IO.FileStream(System.IO.Path.Combine(folder, fileName), System.IO.FileMode.Create))
                await file.CopyToAsync(stream);

            setUrl($"/images/rooms/{fileName}");
            return null;
        }
    }
}
