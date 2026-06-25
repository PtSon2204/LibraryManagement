using LibraryManagement.MVC.Interface;
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
    }
}
