using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Author;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AuthorOption>?> GetAllAuthorsAsync()
        {
            var response = await _httpClient.GetAsync("api/Author/all");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<AuthorOption>>();
            }
            return new List<AuthorOption>();
        }

        public async Task<AuthorListViewModel?> GetAuthorsAsync(string? search, int pageNumber, int pageSize)
        {
            var url = $"api/Author?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthorListViewModel>(json, _jsonOptions);
            if (result != null) result.Search = search;

            return result;
        }

        public async Task<AuthorViewModel?> GetAuthorByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Author/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthorViewModel>(json, _jsonOptions);
        }

        public async Task<string?> CreateAuthorAsync(AuthorViewModel model)
        {
            var payload = new { model.FullName };
            var response = await _httpClient.PostAsJsonAsync("api/Author", payload);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Thêm tác giả thất bại. Vui lòng thử lại.");
        }

        public async Task<string?> UpdateAuthorAsync(AuthorViewModel model)
        {
            var payload = new { model.AuthorId, model.FullName };
            var response = await _httpClient.PutAsJsonAsync($"api/Author/{model.AuthorId}", payload);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Cập nhật tác giả thất bại. Vui lòng thử lại.");
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Author/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<string> ReadErrorMessage(HttpResponseMessage response, string fallback)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                var err = JsonSerializer.Deserialize<AuthorErrorResponse>(body, _jsonOptions);
                return err?.Message ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }

    internal class AuthorErrorResponse
    {
        public string? Message { get; set; }
    }
}
