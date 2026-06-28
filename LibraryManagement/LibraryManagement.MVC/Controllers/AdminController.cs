using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly LibraryManagement.MVC.Interface.IDashboardService _dashboardService;

        public AdminController(LibraryManagement.MVC.Interface.IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            if (stats == null)
            {
                stats = new ViewModels.Dashboard.DashboardViewModel();
            }
            return View(stats);
        }
    }
}
