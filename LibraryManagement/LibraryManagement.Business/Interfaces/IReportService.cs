using LibraryManagement.Business.DTOs.ReportDTOs;

namespace LibraryManagement.Business.Interfaces;

public interface IReportService
{
    Task<LibraryReportDto> GetLibraryReportAsync(ReportQueryDto query);
}
