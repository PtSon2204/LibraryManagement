using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    [Route("[controller]")]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IUserProfileService _userProfileService;

        public ReservationsController(IReservationService reservationService, IUserProfileService userProfileService)
        {
            _reservationService = reservationService;
            _userProfileService = userProfileService;
        }

        [HttpGet("AvailableSlots")]
        public async Task<IActionResult> AvailableSlots(Guid roomId, DateTime date)
        {
            try
            {
                var resultJson = await _reservationService.GetAvailableSlotsAsync(roomId, date);
                return Content(resultJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi kết nối đến API: " + ex.Message });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] object payload)
        {
            try
            {
                // Kiểm tra số điện thoại trước khi đặt phòng
                if (User.IsInRole("Reader"))
                {
                    var profile = await _userProfileService.GetProfile();
                    if (profile == null || string.IsNullOrWhiteSpace(profile.Phone))
                    {
                        return BadRequest(new { message = "Bạn cần cập nhật Số điện thoại trong hồ sơ cá nhân trước khi đặt phòng.", requirePhone = true });
                    }
                }

                var success = await _reservationService.CreateReservationAsync(payload);
                if (success)
                    return Ok();
                
                return BadRequest("Đặt phòng thất bại.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi kết nối đến API: " + ex.Message });
            }
        }
    }
}
