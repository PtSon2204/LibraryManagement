using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(BookSearchViewModel search)
    {
        search.Page = Math.Max(search.Page, 1);
        search.PageSize = search.PageSize <= 0 ? 10 : search.PageSize;

        var model = await _bookService.GetBooksAsync(search) ?? new BookListPageViewModel
        {
            Search = search
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var model = await _bookService.GetBookDetailAsync(id);
        if (model == null) return NotFound();

        return View(model);
    }
}
