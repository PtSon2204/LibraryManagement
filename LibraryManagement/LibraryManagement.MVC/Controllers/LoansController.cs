using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

[Authorize(Roles = "Reader")]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(LoanSearchViewModel criteria)
    {
        criteria.Page = Math.Max(criteria.Page, 1);
        criteria.PageSize = criteria.PageSize <= 0 ? 10 : criteria.PageSize;

        var model = await _loanService.GetMyLoansAsync(criteria) ?? new LoanListPageViewModel { Search = criteria };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(Guid bookId)
    {
        var result = await _loanService.BorrowBookAsync(bookId);
        if (result == null)
        {
            TempData["Error"] = "Không thể mượn sách này. Vui lòng kiểm tra tình trạng bản sao.";
            return RedirectToAction("Index", "Books");
        }

        TempData["Success"] = $"Đã mượn {result.BookTitle}. Hạn trả: {result.DueAt:dd/MM/yyyy}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(Guid loanDetailId)
    {
        var error = await _loanService.ReturnBookAsync(loanDetailId);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã ghi nhận trả sách.";
        return RedirectToAction(nameof(Index));
    }
}
