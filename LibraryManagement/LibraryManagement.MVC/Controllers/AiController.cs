using System.Threading.Tasks;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Ai;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    public class AiController : Controller
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestViewModel request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Yêu cầu không hợp lệ hoặc câu hỏi trống." });
            }

            var responseText = await _aiService.ChatAsync(request);
            if (string.IsNullOrEmpty(responseText))
            {
                return Json(new { response = "Rất tiếc, hệ thống không nhận được câu trả lời từ Trợ lý AI. Vui lòng thử lại sau ít phút!" });
            }

            return Json(new { response = responseText });
        }
    }
}
