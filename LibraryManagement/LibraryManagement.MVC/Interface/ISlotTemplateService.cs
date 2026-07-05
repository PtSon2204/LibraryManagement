using System.Threading.Tasks;

namespace LibraryManagement.MVC.Interface
{
    public interface ISlotTemplateService
    {
        Task<string> GetAllActiveTemplatesAsync();
        Task<string> GetAllTemplatesAsync();
        Task<bool> CreateTemplateAsync(object payload);
        Task<bool> ToggleTemplateStatusAsync(int id);
        Task<bool> DeleteTemplateAsync(int id);
    }
}
