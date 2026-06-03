using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("/staff/dashboard")]
    [ApiController]
    public class StaffDashboardController : ControllerBase
    {
        private readonly IStaffDashboardService _dashboardService;

        public StaffDashboardController(IStaffDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet] 
        public async Task<IActionResult> GetStaffDashboard()
        {
            var dashboard = await _dashboardService.GetDashboardAsync();

            if (dashboard == null)
            {
                return NotFound();
            }

            return Ok(dashboard);
        }
    }
}
