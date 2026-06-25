using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Librarian,Admin")]
    public class LibrarianController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
