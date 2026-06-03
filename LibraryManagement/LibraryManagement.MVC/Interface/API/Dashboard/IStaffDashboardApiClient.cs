using LibraryManagement.MVC.ViewModels.Dashboard;

namespace LibraryManagement.MVC.Interface.API.Dashboard
{
    public interface IStaffDashboardApiClient
    {
        Task<StaffDashboardViewModel?> GetDashboardAsync();
    }
}
