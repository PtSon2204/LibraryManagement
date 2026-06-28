using LibraryManagement.Business.DTOs.CategoryDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(string? search, int pageNumber, int pageSize);
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
