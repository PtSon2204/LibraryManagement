using System.Diagnostics;
using LibraryManagement.MVC.Interface.API.Books;
using LibraryManagement.MVC.Models;
using LibraryManagement.MVC.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookApiClient _bookApiClient;

        public HomeController(ILogger<HomeController> logger, IBookApiClient bookApiClient)
        {
            _logger = logger;
            _bookApiClient = bookApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var latestBooks = await _bookApiClient.GetLatestBooksAsync(6);

            var viewModel = new HomePageViewModel
            {
                LatestBooks = latestBooks
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
