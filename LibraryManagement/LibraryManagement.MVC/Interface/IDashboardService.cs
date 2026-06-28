using LibraryManagement.MVC.ViewModels.Dashboard;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Interface
{
    public interface IDashboardService
    {
        Task<DashboardViewModel?> GetDashboardStatsAsync();
    }
}
