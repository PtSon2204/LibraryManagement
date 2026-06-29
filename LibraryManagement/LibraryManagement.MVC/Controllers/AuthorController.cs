using System;
using System.Threading.Tasks;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 10)
        {
            var model = await _authorService.GetAuthorsAsync(search, pageNumber, pageSize);

            if (model == null)
            {
                TempData["Error"] = "Không thể tải danh sách tác giả. Vui lòng thử lại sau.";
                model = new AuthorListViewModel();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _authorService.GetAuthorByIdAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCreateForm()
        {
            var model = new AuthorViewModel();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", model);
            }

            var error = await _authorService.CreateAuthorAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm tác giả thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Edit ─────────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();

            return PartialView("_Edit", author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(AuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", model);
            }

            var error = await _authorService.UpdateAuthorAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật tác giả thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _authorService.DeleteAuthorAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa tác giả thành công!" });

            return Json(new { success = false, message = "Xóa tác giả thất bại. Có thể do tác giả này đang được liên kết với sách trong hệ thống." });
        }
    }
}
