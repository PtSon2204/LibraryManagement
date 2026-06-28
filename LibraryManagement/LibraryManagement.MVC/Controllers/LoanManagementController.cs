using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

[Authorize(Roles = "Librarian,Admin")]
public class LoanManagementController : Controller
{
    private readonly ILoanService _loanService;

    public LoanManagementController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(LoanSearchViewModel criteria)
    {
        criteria.Page = Math.Max(criteria.Page, 1);
        criteria.PageSize = criteria.PageSize <= 0 ? 10 : criteria.PageSize;

        var model = await _loanService.GetStaffLoansAsync(criteria) ?? new LoanListPageViewModel { Search = criteria };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid loanDetailId, Guid copyId, LoanSearchViewModel criteria)
    {
        var error = await _loanService.ConfirmBorrowRequestAsync(loanDetailId, copyId);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã xác nhận yêu cầu mượn sách.";

        return RedirectToAction(nameof(Index), new
        {
            criteria.Status,
            criteria.Search,
            criteria.Page,
            criteria.PageSize
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(Guid loanDetailId, LoanSearchViewModel criteria)
    {
        var error = await _loanService.ReturnBookAsync(loanDetailId);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã ghi nhận trả sách.";

        return RedirectToAction(nameof(Index), new
        {
            criteria.Status,
            criteria.Search,
            criteria.Page,
            criteria.PageSize
        });
    }
}
