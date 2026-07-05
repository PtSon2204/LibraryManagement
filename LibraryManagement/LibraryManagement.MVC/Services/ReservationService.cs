using LibraryManagement.MVC.Interface;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Services
{
    public class ReservationService : IReservationService
    {
        private readonly HttpClient _httpClient;

        public ReservationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAvailableSlotsAsync(Guid roomId, DateTime date)
        {
            var response = await _httpClient.GetAsync($"/api/Reservations/available-slots?roomId={roomId}&date={date:yyyy-MM-dd}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> CreateReservationAsync(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Reservations", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetReservationsAsync(int pageNumber, int pageSize, string status, Guid? readerId = null)
        {
            var url = $"/api/Reservations?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
            if (readerId.HasValue) url += $"&readerId={readerId.Value}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> CheckInAsync(Guid id)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/Reservations/{id}/checkin", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelReservationAsync(Guid id, Guid? readerId = null)
        {
            var url = $"/api/Reservations/{id}";
            if (readerId.HasValue) url += $"?readerId={readerId.Value}";
            
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
    }
}
