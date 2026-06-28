using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Room;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Services
{
    public class RoomService : IRoomService
    {
        private readonly HttpClient _httpClient;

        public RoomService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RoomListViewModel?> GetRoomsAsync(string? search, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var query = $"api/rooms?search={Uri.EscapeDataString(search ?? "")}&status={Uri.EscapeDataString(status ?? "")}&pageNumber={pageNumber}&pageSize={pageSize}";
                return await _httpClient.GetFromJsonAsync<RoomListViewModel>(query);
            }
            catch
            {
                return null;
            }
        }

        public async Task<RoomViewModel?> GetRoomByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<RoomViewModel>($"api/rooms/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> CreateRoomAsync(RoomViewModel model)
        {
            var payload = new
            {
                model.RoomName,
                model.Capacity,
                model.Description,
                model.Status
            };

            var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/rooms", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Thêm phòng thất bại. Vui lòng thử lại.");
        }

        public async Task<string?> UpdateRoomAsync(RoomViewModel model)
        {
            var payload = new
            {
                model.RoomId,
                model.RoomName,
                model.Capacity,
                model.Description,
                model.Status
            };

            var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/rooms/{model.RoomId}", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Cập nhật thông tin phòng thất bại. Vui lòng thử lại.");
        }

        public async Task<bool> DeleteRoomAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/rooms/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<string> ReadErrorMessage(HttpResponseMessage response, string fallback)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                var err = JsonSerializer.Deserialize<RoomErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return err?.Message ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }

    internal class RoomErrorResponse
    {
        public string? Message { get; set; }
    }
}
