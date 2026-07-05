using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

[Authorize(Roles = "Reader")]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IUserProfileService _userProfileService;

    public LoansController(ILoanService loanService, IUserProfileService userProfileService)
    {
        _loanService = loanService;
        _userProfileService = userProfileService;
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
        // Kiểm tra số điện thoại
        var profile = await _userProfileService.GetProfile();
        if (profile == null || string.IsNullOrWhiteSpace(profile.Phone))
        {
            TempData["RequirePhone"] = "true";
            TempData["Error"] = "Bạn cần cập nhật Số điện thoại trong hồ sơ cá nhân trước khi mượn sách.";
            return RedirectToAction("Index", "UserProfile");
        }

        var result = await _loanService.BorrowBookAsync(bookId);
        if (result == null)
        {
            TempData["Error"] = "Không thể mượn sách này. Vui lòng kiểm tra tình trạng bản sao.";
            return RedirectToAction("Index", "Books");
        }

        TempData["Success"] = $"Đã gửi yêu cầu mượn {result.BookTitle}. Vui lòng chờ thủ thư xác nhận.";
        return RedirectToAction(nameof(Index));
    }
}
