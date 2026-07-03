using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Shelf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class ShelfController : Controller
    {
        private readonly IShelfService _shelfService;

        public ShelfController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        // ── Index ─────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index(Guid? floorId, string? availability)
        {
            var model = await _shelfService.GetIndexViewModelAsync(floorId, availability);
            if (model == null)
            {
                TempData["Error"] = "Không thể tải dữ liệu kệ sách. Vui lòng thử lại.";
                model = new ShelfIndexViewModel();
            }

            model.FilterFloorId     = floorId;
            model.FilterAvailability = availability;
            return View(model);
        }

        // ── Create (Admin only) ───────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCreateForm()
        {
            var model = await _shelfService.GetCreateFormDataAsync();
            if (model == null) return StatusCode(500);
            return PartialView("_Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookshelfFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", model);

            var error = await _shelfService.CreateBookshelfAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm giá sách thành công!" });

            return Json(new { success = false, message = error });
        }

        // ── Edit (Admin only) ─────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEditForm(Guid id)
        {
            var model = await _shelfService.GetEditFormDataAsync(id);
            if (model == null) return NotFound();
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(BookshelfFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Edit", model);

            var error = await _shelfService.UpdateBookshelfAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật giá sách thành công!" });

            return Json(new { success = false, message = error });
        }

        // ── Delete (Admin only) ───────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _shelfService.DeleteBookshelfAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa giá sách thành công!" });

            return Json(new
            {
                success = false,
                message = "Xóa thất bại. Giá sách có thể đang chứa bản sao sách trong hệ thống."
            });
        }
        // ── Details (Manage Shelves and Slots) ────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var bookshelf = await _shelfService.GetBookshelfDetailsAsync(id);
            if (bookshelf == null) return NotFound();
            return View(bookshelf);
        }

        // ── Shelves CRUD ──────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShelf(ShelfFormViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
            var error = await _shelfService.CreateShelfAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm kệ thành công!" });
            return Json(new { success = false, message = error });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditShelf(ShelfFormViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
            var error = await _shelfService.UpdateShelfAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật kệ thành công!" });
            return Json(new { success = false, message = error });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShelf(Guid id)
        {
            var success = await _shelfService.DeleteShelfAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa kệ thành công!" });
            return Json(new { success = false, message = "Xóa thất bại. Kệ đang có dữ liệu slot." });
        }

        // ── Slots CRUD ────────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSlot(ShelfSlotFormViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
            var error = await _shelfService.CreateSlotAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm slot thành công!" });
            return Json(new { success = false, message = error });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSlot(ShelfSlotFormViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
            var error = await _shelfService.UpdateSlotAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật slot thành công!" });
            return Json(new { success = false, message = error });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSlot(Guid id)
        {
            var success = await _shelfService.DeleteSlotAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa slot thành công!" });
            return Json(new { success = false, message = "Xóa thất bại. Slot đang chứa sách." });
        }
    }
}
