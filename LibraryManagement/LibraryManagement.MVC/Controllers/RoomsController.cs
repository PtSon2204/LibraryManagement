using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;
        private readonly IUserProfileService _userProfileService;

        public RoomsController(IRoomService roomService, IReservationService reservationService, IUserProfileService userProfileService)
        {
            _roomService = roomService;
            _reservationService = reservationService;
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int pageNumber = 1, int pageSize = 12)
        {
            var model = await _roomService.GetRoomsAsync(search, status, pageNumber, pageSize);
            
            if (model == null)
            {
                TempData["Error"] = "Không thể tải danh sách phòng. Vui lòng thử lại sau.";
                model = new LibraryManagement.MVC.ViewModels.Room.RoomListViewModel();
            }

            model.Search = search;
            model.Status = status;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _roomService.GetRoomByIdAsync(id);
            if (model == null) return NotFound();

            model.SuggestedRooms = await _roomService.GetAvailableRoomsExceptAsync(id, maxCount: 4);

            // Kiểm tra số điện thoại - truyền cờ vào View
            if (User.IsInRole("Reader"))
            {
                var profile = await _userProfileService.GetProfile();
                ViewBag.HasPhone = profile != null && !string.IsNullOrWhiteSpace(profile.Phone);
            }
            else
            {
                ViewBag.HasPhone = true;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult MyReservations()
        {
            return View();
        }

        [HttpGet("Rooms/GetMyReservations")]
        public async Task<IActionResult> GetMyReservations(int pageNumber = 1, int pageSize = 10, string status = "")
        {
            try
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out Guid readerId))
                {
                    return Unauthorized();
                }

                var resultJson = await _reservationService.GetReservationsAsync(pageNumber, pageSize, status, readerId);
                return Content(resultJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("Rooms/CancelReservation/{id}")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            try
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out Guid readerId))
                {
                    return Unauthorized();
                }

                var success = await _reservationService.CancelReservationAsync(id, readerId);
                if (success) return Ok();
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "Lỗi server");
            }
        }
    }
}
