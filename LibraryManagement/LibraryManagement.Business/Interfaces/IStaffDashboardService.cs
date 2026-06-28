using LibraryManagement.Business.DTOs.DashboardDTOs;

namespace LibraryManagement.Business.Interfaces;

public interface IStaffDashboardService
{
    Task<StaffDashboardDto> GetDashboardAsync();
}
