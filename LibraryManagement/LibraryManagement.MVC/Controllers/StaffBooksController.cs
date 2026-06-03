using LibraryManagement.MVC.Interface.API.Books;
using LibraryManagement.MVC.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

public class StaffBooksController : Controller
{
    private readonly IBookApiClient _bookApiClient;

    public StaffBooksController(IBookApiClient bookApiClient)
    {
        _bookApiClient = bookApiClient;
    }

    public async Task<IActionResult> Index(BookSearchViewModel search)
    {
        if (search.Page <= 0)
        {
            search.Page = 1;
        }

        if (search.PageSize <= 0)
        {
            search.PageSize = 10;
        }

        var response = await _bookApiClient.GetBooksAsync(search);
        var totalCount = response.Count ?? response.Value.Count;

        var viewModel = new BookListPageViewModel
        {
            Search = search,
            Books = response.Value,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)search.PageSize)
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View(new BookCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var book = await _bookApiClient.AddBookAsync(model);

        if (book == null)
        {
            ModelState.AddModelError(string.Empty, "Không thể thêm sách. Vui lòng kiểm tra ISBN hoặc thử lại.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Thêm sách thành công.";
        return RedirectToAction(nameof(Index));
    }
}
