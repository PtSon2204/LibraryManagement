using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[Route("api/staff-dashboard")]
[ApiController]
[Authorize(Roles = "Librarian,Admin")]
public class StaffDashboardController : ControllerBase
{
    private readonly IStaffDashboardService _staffDashboardService;

    public StaffDashboardController(IStaffDashboardService staffDashboardService)
    {
        _staffDashboardService = staffDashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _staffDashboardService.GetDashboardAsync();
        return Ok(result);
    }
}
