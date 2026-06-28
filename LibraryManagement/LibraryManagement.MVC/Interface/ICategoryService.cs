using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Interface
{
    public interface ICategoryService
    {
        Task<List<CategoryOption>?> GetAllCategoriesAsync();
    }
}
