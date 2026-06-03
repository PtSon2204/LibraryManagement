using LibraryManagement.MVC.Interface.API.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    public class StaffDashboardController : Controller
    {
        private readonly IStaffDashboardApiClient _staffDashboardApiClient;

        public StaffDashboardController(IStaffDashboardApiClient staffDashboardApiClient)
        {
            _staffDashboardApiClient = staffDashboardApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _staffDashboardApiClient.GetDashboardAsync();

            if (dashboard == null)
            {
                return NotFound();
            }

            return View(dashboard);
        }
    }
}
