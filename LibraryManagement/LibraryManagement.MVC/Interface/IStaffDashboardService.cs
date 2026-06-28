using LibraryManagement.MVC.ViewModels.Dashboard;

namespace LibraryManagement.MVC.Interface;

public interface IStaffDashboardService
{
    Task<StaffDashboardViewModel?> GetDashboardAsync();
}
