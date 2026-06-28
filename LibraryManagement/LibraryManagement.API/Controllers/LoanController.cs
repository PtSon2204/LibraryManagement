using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly Business.Services.ILoanService _loanService;

        public LoanController(Business.Services.ILoanService loanService)
        {
            _loanService = loanService;
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Reader")]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] Models.Queries.LoanQuery query)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid readerId))
            {
                return Unauthorized("Không thể xác thực người dùng.");
            }

            var result = await _loanService.GetReaderLoanHistoryAsync(readerId, query);
            return Ok(result);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _loanService.GetLoanDetailByIdAsync(id);
            if (result == null)
                return NotFound("Không tìm thấy phiếu mượn.");

            return Ok(result);
        }
    }
}
