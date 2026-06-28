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

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
    }
}
