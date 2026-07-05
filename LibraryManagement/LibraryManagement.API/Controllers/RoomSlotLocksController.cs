using LibraryManagement.Business.DTOs.ReservationDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomSlotLocksController : ControllerBase
    {
        private readonly IRoomSlotLockService _roomSlotLockService;

        public RoomSlotLocksController(IRoomSlotLockService roomSlotLockService)
        {
            _roomSlotLockService = roomSlotLockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocksByRoomAndDate([FromQuery] Guid roomId, [FromQuery] DateTime date)
        {
            var locks = await _roomSlotLockService.GetLocksByRoomAndDateAsync(roomId, date);
            return Ok(locks);
        }

        [HttpPost]
        public async Task<IActionResult> LockSlot([FromBody] CreateRoomSlotLockDto dto)
        {
            // Trong thực tế, lấy ID từ Token, ở đây mock 1 ID hoặc lấy từ JWT claims nếu có
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Empty;
            if (!string.IsNullOrEmpty(userIdStr))
            {
                Guid.TryParse(userIdStr, out userId);
            }

            var result = await _roomSlotLockService.LockSlotAsync(dto, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UnlockSlot(int id)
        {
            var success = await _roomSlotLockService.UnlockSlotAsync(id);
            if (!success) return NotFound();
            return Ok();
        }
    }
}
