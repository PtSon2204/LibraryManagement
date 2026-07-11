using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AiDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IGeminiService
    {
        Task<string> ChatWithLibraryContextAsync(string prompt, List<ChatMessageDto> history);
    }
}
