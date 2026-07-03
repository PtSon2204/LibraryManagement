using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Shelf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FloorController : Controller
    {
        private readonly IShelfService _shelfService;

        public FloorController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        // ── Index ─────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var floors = await _shelfService.GetAllFloorsAsync();
            return View(floors);
        }

        // ── Create ────────────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult GetCreateForm()
        {
            return PartialView("_Create", new FloorFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(FloorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", model);

            var error = await _shelfService.CreateFloorAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm tầng thành công!" });

            return Json(new { success = false, message = error });
        }

        // ── Edit ──────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetEditForm(Guid id)
        {
            var model = await _shelfService.GetFloorEditFormDataAsync(id);
            if (model == null) return NotFound();
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FloorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Edit", model);

            var error = await _shelfService.UpdateFloorAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật tầng thành công!" });

            return Json(new { success = false, message = error });
        }

        // ── Delete ────────────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _shelfService.DeleteFloorAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa tầng thành công!" });

            return Json(new
            {
                success = false,
                message = "Xóa thất bại. Tầng có thể đang chứa giá sách bên trong."
            });
        }
    }
}
