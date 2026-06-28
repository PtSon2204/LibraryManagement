using LibraryManagement.MVC.ViewModels.Reports;

namespace LibraryManagement.MVC.Interface;

public interface IReportService
{
    Task<LibraryReportViewModel?> GetLibraryReportAsync(ReportFilterViewModel filter);
}
