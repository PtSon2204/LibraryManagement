using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Librarian,Admin")]
    public class LibrarianController : Controller
    {
        private readonly IStaffDashboardService _staffDashboardService;

        public LibrarianController(IStaffDashboardService staffDashboardService)
        {
            _staffDashboardService = staffDashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _staffDashboardService.GetDashboardAsync();
            if (model == null)
            {
                ViewBag.DashboardError = "Không thể tải dữ liệu dashboard. Vui lòng thử lại sau.";
                model = new StaffDashboardViewModel();
            }

            return View(model);
        }
    }
}
