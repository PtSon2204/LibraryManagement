using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.MVC.Controllers
{
    [Authorize(Roles = "Reader")]
    public class LoanController : Controller
    {
        private readonly Interface.ILoanService _loanService;

        public LoanController(Interface.ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? status, DateTime? fromDate, DateTime? toDate, int pageNumber = 1)
        {
            int pageSize = 10;
            var result = await _loanService.GetMyLoanHistoryAsync(searchTerm, status, fromDate, toDate, pageNumber, pageSize);
            
            if (result == null)
            {
                TempData["ErrorMessage"] = "Không thể tải lịch sử mượn sách.";
                result = new ViewModels.Loans.LoanListViewModel();
            }

            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _loanService.GetLoanDetailAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsModal", result);
        }
    }
}
