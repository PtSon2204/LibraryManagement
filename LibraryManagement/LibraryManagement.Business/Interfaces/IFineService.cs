using LibraryManagement.Business.DTOs.FineDTOs;

namespace LibraryManagement.Business.Interfaces;

public interface IFineService
{
    // ─── Fine Templates ──────────────────────────────────────────────────────
    Task<List<FineTemplateDto>> GetActiveTemplatesAsync();
    Task<List<FineTemplateDto>> GetAllTemplatesAsync();
    Task CreateTemplateAsync(string name, decimal amount, string fineType);
    Task UpdateTemplateAsync(Guid id, string name, decimal amount, string fineType, bool isActive);
    Task DeleteTemplateAsync(Guid id);

    // ─── Fine Operations ─────────────────────────────────────────────────────
    /// <summary>Tạo khoản phạt + Payment + ghi nhận trả sách về Available</summary>
    Task CreateFinesAndReturnAsync(Guid actorId, CreateFineRequest request);

    /// <summary>Lấy danh sách khoản phạt (Admin)</summary>
    Task<FineListPageDto> GetFinesAsync(string? search, string? status, int page, int pageSize);
}
