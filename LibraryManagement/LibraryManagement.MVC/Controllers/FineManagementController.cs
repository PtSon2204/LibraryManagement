using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Fines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers;

[Authorize(Roles = "Admin")]
public class FineManagementController : Controller
{
    private readonly IFineService _fineService;

    public FineManagementController(IFineService fineService)
    {
        _fineService = fineService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? status, int page = 1)
    {
        var model = await _fineService.GetFinesAsync(search, status, page, 15);
        if (model == null)
        {
            model = new FineListPageViewModel { Page = page, PageSize = 15 };
            TempData["Error"] = "Không thể tải danh sách khoản phạt.";
        }
        model.Search = search;
        model.Status = status;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Templates()
    {
        var templates = await _fineService.GetAllTemplatesAsync();
        return View(templates);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTemplate(UpsertFineTemplateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Thông tin không hợp lệ.";
            return RedirectToAction(nameof(Templates));
        }

        var error = await _fineService.CreateTemplateAsync(model);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã thêm loại khoản phạt mới.";
        return RedirectToAction(nameof(Templates));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTemplate(Guid id, UpsertFineTemplateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Thông tin không hợp lệ.";
            return RedirectToAction(nameof(Templates));
        }

        var error = await _fineService.UpdateTemplateAsync(id, model);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã cập nhật loại khoản phạt.";
        return RedirectToAction(nameof(Templates));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        var error = await _fineService.DeleteTemplateAsync(id);
        TempData[error == null ? "Success" : "Error"] = error ?? "Đã xóa loại khoản phạt.";
        return RedirectToAction(nameof(Templates));
    }
}
