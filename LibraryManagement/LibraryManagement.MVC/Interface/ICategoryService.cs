using LibraryManagement.MVC.ViewModels.Category;

namespace LibraryManagement.MVC.Interface
{
    public interface ICategoryService
    {
        Task<CategoryListViewModel?> GetCategoriesAsync(string? search, int pageNumber, int pageSize);
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
        Task<string?> CreateCategoryAsync(CategoryViewModel model);
        Task<string?> UpdateCategoryAsync(CategoryViewModel model);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
