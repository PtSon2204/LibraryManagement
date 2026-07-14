using System.Security.Claims;
using LibraryManagement.Business.DTOs.LoanDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[Route("api/loans")]
[ApiController]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> GetStaffLoans(
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _loanService.GetStaffLoansAsync(status, search, page, pageSize);
        return Ok(result);
    }

    [HttpGet("readers")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> GetReaderLoanSummaries(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _loanService.GetStaffReaderLoanSummariesAsync(search, page, pageSize);
        return Ok(result);
    }

    [HttpGet("readers/{readerId:guid}")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> GetReaderLoanWorkspace(Guid readerId)
    {
        var result = await _loanService.GetStaffReaderLoanWorkspaceAsync(readerId);
        return result == null ? NotFound("Không tìm thấy độc giả.") : Ok(result);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetMyLoans([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _loanService.GetReaderLoansAsync(GetCurrentUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpPost("borrow")]
    [Authorize(Roles = "Reader")]
    public async Task<IActionResult> BorrowBook(BorrowBookRequestDto request)
    {
        try
        {
            var result = await _loanService.BorrowBookAsync(GetCurrentUserId(), request.BookId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{loanDetailId:guid}/confirm")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> ConfirmBorrowRequest(Guid loanDetailId, ConfirmBorrowRequestDto request)
    {
        try
        {
            await _loanService.ConfirmLoanDetailAsync(GetCurrentUserId(), loanDetailId, request.CopyId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("readers/{readerId:guid}/confirm-batch")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> ConfirmBorrowRequests(Guid readerId, BatchConfirmBorrowRequestDto request)
    {
        try
        {
            await _loanService.ConfirmLoanDetailsAsync(GetCurrentUserId(), readerId, request.Items);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{loanDetailId:guid}/reject")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> RejectBorrowRequest(Guid loanDetailId, RejectLoanDetailDto request)
    {
        try
        {
            await _loanService.RejectLoanDetailAsync(GetCurrentUserId(), loanDetailId, request.Reason);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{loanDetailId:guid}/return")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> ReturnBook(Guid loanDetailId)
    {
        try
        {
            await _loanService.ReturnLoanDetailAsync(GetCurrentUserId(), User.FindFirstValue(ClaimTypes.Role) ?? string.Empty, loanDetailId);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("history")]
    [Authorize(Roles = "Reader")]
    public async Task<IActionResult> GetHistory([FromQuery] Models.Queries.LoanQuery query)
    {
        var result = await _loanService.GetReaderLoanHistoryAsync(GetCurrentUserId(), query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var result = await _loanService.GetLoanDetailByIdAsync(id);
        if (result == null)
            return NotFound("Không tìm thấy phiếu mượn.");

        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
    }
}
