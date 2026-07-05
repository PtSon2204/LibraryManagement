using LibraryManagement.MVC.Interface;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Services
{
    public class RoomSlotLockService : IRoomSlotLockService
    {
        private readonly HttpClient _httpClient;

        public RoomSlotLockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetLocksByRoomAndDateAsync(Guid roomId, DateTime date)
        {
            var response = await _httpClient.GetAsync($"/api/RoomSlotLocks?roomId={roomId}&date={date:yyyy-MM-dd}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> LockSlotAsync(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/RoomSlotLocks", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UnlockSlotAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/RoomSlotLocks/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
