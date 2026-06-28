using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 10)
        {
            var model = await _categoryService.GetCategoriesAsync(search, pageNumber, pageSize);

            if (model == null)
            {
                TempData["Error"] = "Không thể tải danh sách thể loại. Vui lòng thử lại sau.";
                model = new LibraryManagement.MVC.ViewModels.Category.CategoryListViewModel();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _categoryService.GetCategoryByIdAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCreateForm()
        {
            var model = new CategoryViewModel();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", model);
            }

            var error = await _categoryService.CreateCategoryAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm thể loại thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Edit ─────────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            return PartialView("_Edit", category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", model);
            }

            var error = await _categoryService.UpdateCategoryAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật thể loại thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (success)
                return Json(new { success = true, message = "Xóa thể loại thành công!" });

            return Json(new { success = false, message = "Xóa thể loại thất bại. Có thể do thể loại này đang được liên kết với sách trong hệ thống." });
        }
    }
}
