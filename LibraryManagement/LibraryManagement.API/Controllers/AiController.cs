using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AiDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public AiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Yêu cầu không hợp lệ hoặc câu hỏi trống.");
            }

            var responseText = await _geminiService.ChatWithLibraryContextAsync(request.Prompt, request.History);
            return Ok(new { response = responseText });
        }
    }
}
