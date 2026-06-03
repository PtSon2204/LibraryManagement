using LibraryManagement.MVC.Interface.API.Books;
using LibraryManagement.MVC.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookApiClient _bookApiClient;

        public BooksController(IBookApiClient bookApiClient)
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

        public async Task<IActionResult> Details(Guid id)
        {
            var book = await _bookApiClient.GetBookDetailAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
    }
}
