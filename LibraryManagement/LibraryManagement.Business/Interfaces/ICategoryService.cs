using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.CategoryDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}
