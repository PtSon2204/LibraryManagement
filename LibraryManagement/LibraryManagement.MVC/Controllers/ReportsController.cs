using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ReportFilterViewModel filter)
    {
        var model = await _reportService.GetLibraryReportAsync(filter);
        if (model == null)
        {
            ViewBag.ReportError = "Không thể tải dữ liệu báo cáo. Vui lòng thử lại sau.";
            model = new LibraryReportViewModel { Filter = filter };
        }

        return View(model);
    }
}
