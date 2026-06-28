using LibraryManagement.Business.DTOs.DashboardDTOs;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardStatsAsync();
    }
}
