    using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IPublisherService _publisherService;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public BookController(IBookService bookService, IPublisherService publisherService, IWebHostEnvironment env)
        {
            _bookService = bookService;
            _publisherService = publisherService;
            _env = env;
        }

        // ─── Index ────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, int? publisherId, int? publicationYear,
                                               string? language, int pageNumber = 1, int pageSize = 10)
        {
            var model = await _bookService.GetBooksAsync(searchTerm, publisherId, publicationYear, language, pageNumber, pageSize);
            if (model == null)
                return RedirectToAction("Login", "Account");
            return View(model);
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetCreateForm()
        {
            var model = new CreateBookViewModel();
            await PopulatePublishers(model.Publishers);
            return PartialView("_Create", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePublishers(model.Publishers);
                return PartialView("_Create", model);
            }

            var uploadError = await HandleCoverImageUpload(model.CoverImageFile, result => model.CoverImageUrl = result);
            if (uploadError != null)
                return Json(new { success = false, message = uploadError });

            var error = await _bookService.CreateBookAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm sách thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Edit ─────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetEditForm(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            var model = new UpdateBookViewModel
            {
                BookId       = book.BookId,
                Title        = book.Title,
                ISBN         = book.ISBN,
                PublisherId  = book.PublisherId,
                PublicationYear = book.PublicationYear,
                Language     = book.Language,
                Edition      = book.Edition,
                Description  = book.Description,
                CoverImageUrl = book.CoverImageUrl
            };
            await PopulatePublishers(model.Publishers);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePublishers(model.Publishers);
                return PartialView("_Edit", model);
            }

            var uploadError = await HandleCoverImageUpload(model.CoverImageFile, result => model.CoverImageUrl = result);
            if (uploadError != null)
                return Json(new { success = false, message = uploadError });

            var error = await _bookService.UpdateBookAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật sách thành công!" });

            return Json(new { success = false, message = error });
        }

        // ─── Details ──────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return PartialView("_Details", book);
        }

        // ─── Toggle Hide ──────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleHide(Guid id)
        {
            var success = await _bookService.ToggleHideBookAsync(id);
            if (success)
                return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });

            return Json(new { success = false, message = "Cập nhật trạng thái thất bại. Vui lòng thử lại." });
        }

        // ─── Private Helpers ──────────────────────────────────────────────────────

        private async Task PopulatePublishers(List<PublisherOption> list)
        {
            var publishers = await _publisherService.GetPublishersAsync(null, 1, 1000);
            list.AddRange(publishers?.Data.Select(p => new PublisherOption
            {
                PublisherId  = p.PublisherId,
                PublisherName = p.PublisherName
            }) ?? Enumerable.Empty<PublisherOption>());
        }

        /// <summary>
        /// Upload file ảnh bìa vào wwwroot/images/books.
        /// Nếu thành công: gọi callback với đường dẫn mới, trả về null.
        /// Nếu thất bại: trả về thông báo lỗi.
        /// </summary>
        private async Task<string?> HandleCoverImageUpload(IFormFile? file, Action<string> setUrl)
        {
            if (file == null || file.Length == 0) return null;

            if (file.Length > MaxFileSizeBytes)
                return "Ảnh bìa vượt quá 5 MB. Vui lòng chọn ảnh nhỏ hơn.";

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return $"Định dạng không hỗ trợ ({ext}). Chỉ chấp nhận: jpg, jpeg, png, webp, gif.";

            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder   = Path.Combine(_env.WebRootPath, "images", "books");
            Directory.CreateDirectory(folder);

            using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                await file.CopyToAsync(stream);

            setUrl($"/images/books/{fileName}");
            return null;
        }
    }
}
