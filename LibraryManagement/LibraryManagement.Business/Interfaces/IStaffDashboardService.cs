using LibraryManagement.Business.DTOs.DashboardDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface IStaffDashboardService
    {
        Task<StaffDashboardDto> GetDashboardAsync();
    }
}
