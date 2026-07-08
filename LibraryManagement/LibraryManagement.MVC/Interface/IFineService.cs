using LibraryManagement.MVC.ViewModels.Fines;

namespace LibraryManagement.MVC.Interface;

public interface IFineService
{
    Task<List<FineTemplateViewModel>> GetActiveTemplatesAsync();
    Task<List<FineTemplateViewModel>> GetAllTemplatesAsync();
    Task<string?> CreateFineAsync(CreateFineViewModel model);
    Task<FineListPageViewModel?> GetFinesAsync(string? search, string? status, int page, int pageSize);

    // Admin CRUD templates
    Task<string?> CreateTemplateAsync(UpsertFineTemplateViewModel model);
    Task<string?> UpdateTemplateAsync(Guid id, UpsertFineTemplateViewModel model);
    Task<string?> DeleteTemplateAsync(Guid id);
    Task<PaymentQrViewModel?> GenerateQrAsync(decimal amount, Guid loanDetailId);
}
