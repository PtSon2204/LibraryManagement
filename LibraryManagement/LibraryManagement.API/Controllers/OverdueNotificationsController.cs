using System.Threading.Tasks;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OverdueNotificationsController : ControllerBase
{
    private readonly IOverdueNotificationService _notificationService;

    public OverdueNotificationsController(IOverdueNotificationService notificationService)
    {
        _notificationService = notificationService; 
    }

    [HttpPost("send")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> SendOverdueNotifications()
    {
        var result = await _notificationService.SendOverdueNotificationsAsync();
        return Ok(new
        {
            Message = "Gửi thông báo sách quá hạn thành công.",
            Data = result
        });
    }
}
