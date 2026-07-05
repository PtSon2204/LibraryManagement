using LibraryManagement.Business.DTOs.ReservationDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface ISlotTemplateService
    {
        Task<List<SlotTemplateDto>> GetAllActiveTemplatesAsync();
        Task<List<SlotTemplateDto>> GetAllTemplatesAsync();
        Task<SlotTemplateDto> CreateTemplateAsync(CreateSlotTemplateDto dto);
        Task<bool> ToggleTemplateStatusAsync(int id);
        Task<bool> DeleteTemplateAsync(int id);
    }
}
