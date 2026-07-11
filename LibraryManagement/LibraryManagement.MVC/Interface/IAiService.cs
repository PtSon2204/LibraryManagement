using System.Threading.Tasks;
using LibraryManagement.MVC.ViewModels.Ai;

namespace LibraryManagement.MVC.Interface
{
    public interface IAiService
    {
        Task<string?> ChatAsync(ChatRequestViewModel request);
    }
}
