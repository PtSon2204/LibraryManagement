using LibraryManagement.Business.DTOs.ReportDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[Route("api/reports")]
[ApiController]
[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetLibraryReport([FromQuery] ReportQueryDto query)
    {
        var report = await _reportService.GetLibraryReportAsync(query);
        return Ok(report);
    }
}
