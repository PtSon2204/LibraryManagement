using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                LatestBooks = await _bookService.GetLatestBooksAsync(5)
            };

            return View(model);
        }
    }
}
