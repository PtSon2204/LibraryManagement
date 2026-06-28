using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Publisher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class PublisherController : Controller
    {
        private readonly IPublisherService _publisherService;

        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 10)
        {
            var model = await _publisherService.GetPublishersAsync(search, pageNumber, pageSize);

            if (model == null)
                return RedirectToAction("Login", "Account");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _publisherService.GetPublisherByIdAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCreateForm()
        {
            var model = new PublisherViewModel();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PublisherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", model);
            }

            var error = await _publisherService.CreatePublisherAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm nhà xuất bản thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Edit ─────────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null) return NotFound();

            return PartialView("_Edit", publisher);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(PublisherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", model);
            }

            var error = await _publisherService.UpdatePublisherAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật nhà xuất bản thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _publisherService.DeletePublisherAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa nhà xuất bản thành công!" });

            return Json(new { success = false, message = "Xóa nhà xuất bản thất bại. Có thể do nhà xuất bản đang liên kết với sách trong hệ thống." });
        }
    }
}
