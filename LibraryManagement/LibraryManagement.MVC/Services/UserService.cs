using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.MVC.Interface;

namespace LibraryManagement.MVC.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResult<LibrarianListItemDto>?> GetLibrariansAsync(string? search, int pageNumber, int pageSize)
        {
            var url = $"api/users/librarians?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PagedResult<LibrarianListItemDto>>(_jsonOptions);
        }

        public async Task<PagedResult<ReaderListItemDto>?> GetReadersAsync(string? search, int pageNumber, int pageSize)
        {
            var url = $"api/users/readers?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PagedResult<ReaderListItemDto>>(_jsonOptions);
        }

        public async Task<CreateUserResponseDto?> CreateLibrarianAsync(CreateLibrarianDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/librarians", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CreateUserResponseDto>(_jsonOptions);
            }

            await HandleErrorResponse(response);
            return null;
        }

        public async Task<CreateUserResponseDto?> CreateReaderAsync(CreateReaderDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/readers", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CreateUserResponseDto>(_jsonOptions);
            }

            await HandleErrorResponse(response);
            return null;
        }

        public async Task<bool> ToggleLibrarianStatusAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/users/librarians/{id}/toggle-status", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleReaderStatusAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/users/readers/{id}/toggle-status", null);
            return response.IsSuccessStatusCode;
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var err = JsonSerializer.Deserialize<ApiErrorResponse>(content, _jsonOptions);
                if (err != null && !string.IsNullOrEmpty(err.Message))
                {
                    throw new Exception(err.Message);
                }
            }
            catch (JsonException)
            {
                // Not standard API error format, let's look for model state validation errors
            }

            throw new Exception("Thao tác thất bại. Vui lòng kiểm tra lại thông tin.");
        }
    }

    internal class ApiErrorResponse
    {
        public string? Message { get; set; }
    }
}
