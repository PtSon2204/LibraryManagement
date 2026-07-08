using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

[Authorize(Roles = "Librarian,Admin")]
public class LoanManagementController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IFineService _fineService;

    public LoanManagementController(ILoanService loanService, IFineService fineService)
    {
        _loanService = loanService;
        _fineService = fineService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(LoanSearchViewModel criteria)
    {
        criteria.Page = Math.Max(criteria.Page, 1);
        criteria.PageSize = criteria.PageSize <= 0 ? 10 : criteria.PageSize;

        var model = await _loanService.GetStaffReaderLoanSummariesAsync(criteria) ?? new ReaderLoanSummaryPageViewModel { Search = criteria };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Reader(Guid id)
    {
        var model = await _loanService.GetStaffReaderLoanWorkspaceAsync(id);
        if (model == null) return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmSelected(Guid readerId, List<ConfirmLoanDetailItemViewModel> items)
    {
        items = items.Where(item => item.LoanDetailId != Guid.Empty && item.CopyId != Guid.Empty).ToList();
        var error = await _loanService.ConfirmBorrowRequestsAsync(readerId, items);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã xác nhận các yêu cầu mượn được chọn.";

        return RedirectToAction(nameof(Reader), new { id = readerId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid loanDetailId, Guid copyId, LoanSearchViewModel criteria)
    {
        var error = await _loanService.ConfirmBorrowRequestAsync(loanDetailId, copyId);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã xác nhận yêu cầu mượn sách.";

        return RedirectToAction(nameof(Reader), new { id = criteria.ReaderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(Guid loanDetailId, LoanSearchViewModel criteria)
    {
        var error = await _loanService.ReturnBookAsync(loanDetailId);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã ghi nhận trả sách.";

        if (criteria.ReaderId.HasValue)
            return RedirectToAction(nameof(Reader), new { id = criteria.ReaderId.Value });

        return RedirectToAction(nameof(Index), new
        {
            criteria.Status,
            criteria.Search,
            criteria.Page,
            criteria.PageSize
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetFineTemplates()
    {
        var templates = await _fineService.GetActiveTemplatesAsync();
        return Json(templates);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFine(LibraryManagement.MVC.ViewModels.Fines.CreateFineViewModel model)
    {
        var error = await _fineService.CreateFineAsync(model);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã ghi nhận phạt và trả sách thành công.";
        
        return RedirectToAction(nameof(Reader), new { id = model.ReaderId });
    }

    [HttpGet]
    public async Task<IActionResult> GenerateQr(decimal amount, Guid loanDetailId)
    {
        var result = await _fineService.GenerateQrAsync(amount, loanDetailId);
        if (result == null) return BadRequest("Không thể tạo mã QR");
        return Json(result);
    }
}
