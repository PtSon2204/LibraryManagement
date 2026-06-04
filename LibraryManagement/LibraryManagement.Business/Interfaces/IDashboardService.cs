using System.Threading.Tasks;
using LibraryManagement.Business.DTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
