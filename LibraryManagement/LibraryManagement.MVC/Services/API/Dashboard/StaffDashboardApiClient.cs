using System.Net.Http.Json;
using LibraryManagement.MVC.Interface.API.Dashboard;
using LibraryManagement.MVC.ViewModels.Dashboard;

namespace LibraryManagement.MVC.Services.API.Dashboard
{
    public class StaffDashboardApiClient : IStaffDashboardApiClient
    {
        private readonly HttpClient _httpClient;

        public StaffDashboardApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StaffDashboardViewModel?> GetDashboardAsync()
        {
            return await _httpClient.GetFromJsonAsync<StaffDashboardViewModel>("/staff/dashboard");
        }
    }
}
