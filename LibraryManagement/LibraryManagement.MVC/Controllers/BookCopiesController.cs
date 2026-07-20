using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.BookCopies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class BookCopiesController : Controller
    {
        private readonly IBookCopyService _bookCopyService;
        private readonly IBookService _bookService;
        private readonly IShelfService _shelfService;

        public BookCopiesController(IBookCopyService bookCopyService, IBookService bookService, IShelfService shelfService)
        {
            _bookCopyService = bookCopyService;
            _bookService     = bookService;
            _shelfService    = shelfService;
        }

        // ── INDEX ──────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index(Guid bookId, string? searchTerm, string? statusFilter,
                                               string? locationFilter, int pageNumber = 1, int pageSize = 10)
        {
            var model = await _bookCopyService.GetBookCopiesAsync(
                bookId, searchTerm, statusFilter, locationFilter, pageNumber, pageSize);

            if (model == null)
                return RedirectToAction("Index", "Book");

            // Nếu chưa có BookTitle (list rỗng), lấy từ Book API
            if (string.IsNullOrWhiteSpace(model.BookTitle))
            {
                var book = await _bookService.GetBookByIdAsync(bookId);
                if (book != null)
                {
                    model.BookTitle = book.Title;
                    model.BookISBN  = book.ISBN;
                }
            }

            return View(model);
        }

        // ── ADD ONE COPY – load partial ────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetCreateForm(Guid bookId)
        {
            var book = await _bookService.GetBookByIdAsync(bookId);
            var model = new CreateBookCopyViewModel
            {
                BookId    = bookId,
                BookTitle = book?.Title ?? string.Empty,
                Status    = "Available"
            };
            
            ViewBag.Slots = await _shelfService.GetAllSlotsAsync();
            return PartialView("_Create", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookCopyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Slots = await _shelfService.GetAllSlotsAsync();
                return PartialView("_Create", model);
            }

            var error = await _bookCopyService.CreateBookCopyAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Thêm bản sao thành công!" });

            return Json(new { success = false, message = error });
        }

        // ── GENERATE MULTIPLE – load partial ──────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetGenerateForm(Guid bookId)
        {
            var book = await _bookService.GetBookByIdAsync(bookId);
            var model = new GenerateBookCopiesViewModel
            {
                BookId    = bookId,
                BookTitle = book?.Title ?? string.Empty,
                Quantity  = 5,
                Status    = "Available"
            };
            
            ViewBag.Slots = await _shelfService.GetAllSlotsAsync();
            return PartialView("_Generate", model);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(GenerateBookCopiesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Slots = await _shelfService.GetAllSlotsAsync();
                return PartialView("_Generate", model);
            }

            var error = await _bookCopyService.GenerateBookCopiesAsync(model);
            if (error == null)
                return Json(new { success = true, message = $"Đã tạo thành công {model.Quantity} bản sao!" });

            return Json(new { success = false, message = error });
        }

        // ── EDIT ──────────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetEditForm(Guid id)
        {
            var copy = await _bookCopyService.GetBookCopyByIdAsync(id);
            if (copy == null) return NotFound();

            var model = new UpdateBookCopyViewModel
            {
                CopyId    = copy.CopyId,
                BookId    = copy.BookId,
                BookTitle = copy.BookTitle,
                Barcode   = copy.Barcode,
                Status    = copy.Status,
                ReplacementPrice = copy.ReplacementPrice,
                ShelfSlotId = copy.ShelfSlotId
            };
            
            ViewBag.Slots = await _shelfService.GetAllSlotsAsync();
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBookCopyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Slots = await _shelfService.GetAllSlotsAsync();
                return PartialView("_Edit", model);
            }

            var error = await _bookCopyService.UpdateBookCopyAsync(model);
            if (error == null)
                return Json(new { success = true, message = "Cập nhật bản sao thành công!" });

            return Json(new { success = false, message = error });
        }

        // ── TOGGLE HIDE ───────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleHide(Guid id)
        {
            var success = await _bookCopyService.ToggleHideAsync(id);
            return Json(success
                ? new { success = true,  message = "Cập nhật trạng thái thành công!" }
                : new { success = false, message = "Cập nhật trạng thái thất bại." });
        }

        // ── DELETE ────────────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _bookCopyService.DeleteBookCopyAsync(id);
            return Json(success
                ? new { success = true,  message = "Đã xóa bản sao thành công!" }
                : new { success = false, message = "Xóa bản sao thất bại." });
        }
    }
}
