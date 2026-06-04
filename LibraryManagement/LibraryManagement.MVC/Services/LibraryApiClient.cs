using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs;
using LibraryManagement.Business.DTOs.AuthDTOs;
using Microsoft.Extensions.Configuration;

namespace LibraryManagement.MVC.Services
{
    public class LibraryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public LibraryApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7229/";
            if (!_baseUrl.EndsWith("/"))
            {
                _baseUrl += "/";
            }
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/auth/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            }
            return null;
        }

        public async Task<DashboardStatsDto?> GetDashboardStatsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/admin/dashboard");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
            }
            return null;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(string? search)
        {
            var url = $"{_baseUrl}api/admin/users";
            if (!string.IsNullOrEmpty(search))
            {
                url += $"?search={Uri.EscapeDataString(search)}";
            }
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>() ?? Array.Empty<UserDto>();
            }
            return Array.Empty<UserDto>();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/admin/users/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            return null;
        }

        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/admin/users", createUserDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}api/admin/users/{id}", updateUserDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleUserStatusAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"{_baseUrl}api/admin/users/{id}/toggle-status", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorsAsync(string? search)
        {
            var url = $"{_baseUrl}api/admin/authors";
            if (!string.IsNullOrEmpty(search))
            {
                url += $"?search={Uri.EscapeDataString(search)}";
            }
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<AuthorDto>>() ?? Array.Empty<AuthorDto>();
            }
            return Array.Empty<AuthorDto>();
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/admin/authors/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthorDto>();
            }
            return null;
        }

        public async Task<bool> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/admin/authors", createAuthorDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAuthorAsync(int id, UpdateAuthorDto updateAuthorDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}api/admin/authors/{id}", updateAuthorDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}api/admin/authors/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
