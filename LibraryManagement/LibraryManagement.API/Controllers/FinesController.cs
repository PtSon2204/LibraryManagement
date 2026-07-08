using System.Security.Claims;
using LibraryManagement.Business.DTOs.FineDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[Route("api/fines")]
[ApiController]
[Authorize]
public class FinesController : ControllerBase
{
    private readonly IFineService _fineService;
    private readonly IConfiguration _configuration;

    public FinesController(IFineService fineService, IConfiguration configuration)
    {
        _fineService = fineService;
        _configuration = configuration;
    }

    // ─── Templates ───────────────────────────────────────────────────────────

    /// <summary>Lấy danh sách template đang kích hoạt (dùng cho popup chọn khoản phạt)</summary>
    [HttpGet("templates")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> GetActiveTemplates()
    {
        var result = await _fineService.GetActiveTemplatesAsync();
        return Ok(result);
    }

    /// <summary>Lấy tất cả template kể cả ẩn (Admin CRUD)</summary>
    [HttpGet("templates/all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllTemplates()
    {
        var result = await _fineService.GetAllTemplatesAsync();
        return Ok(result);
    }

    [HttpPost("templates")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTemplate([FromBody] UpsertFineTemplateDto dto)
    {
        try
        {
            await _fineService.CreateTemplateAsync(dto.Name, dto.Amount, dto.FineType);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("templates/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpsertFineTemplateDto dto)
    {
        try
        {
            await _fineService.UpdateTemplateAsync(id, dto.Name, dto.Amount, dto.FineType, dto.IsActive);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("templates/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        try
        {
            await _fineService.DeleteTemplateAsync(id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ─── Fine Operations ─────────────────────────────────────────────────────

    /// <summary>Tạo khoản phạt + ghi nhận trả sách</summary>
    [HttpPost]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> CreateFine([FromBody] CreateFineRequest request)
    {
        try
        {
            await _fineService.CreateFinesAndReturnAsync(GetCurrentUserId(), request);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Danh sách khoản phạt (Admin)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetFines(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        var result = await _fineService.GetFinesAsync(search, status, page, pageSize);
        return Ok(result);
    }

    /// <summary>Tạo thông tin QR thanh toán (Backend tự cấu hình Bank)</summary>
    [HttpGet("generate-qr")]
    [Authorize(Roles = "Librarian,Admin")]
    public IActionResult GenerateQr([FromQuery] decimal amount, [FromQuery] Guid loanDetailId)
    {
        var bankId = _configuration["SePay:BankId"] ?? "MB";
        var accountNo = _configuration["SePay:AccountNumber"] ?? "";
        var accountName = _configuration["SePay:AccountName"] ?? "";

        var transferContent = "PHAT" + loanDetailId.ToString().Substring(0, 8).ToUpper();
        
        // Tạo URL VietQR
        var qrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-compact2.png?amount={amount}&addInfo={transferContent}&accountName={accountName}";

        return Ok(new
        {
            QrUrl = qrUrl,
            TransferContent = transferContent,
            Amount = amount
        });
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
    }
}

/// <summary>DTO upsert cho FineTemplate</summary>
public class UpsertFineTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string FineType { get; set; } = "Fixed";
    public bool IsActive { get; set; } = true;
}
