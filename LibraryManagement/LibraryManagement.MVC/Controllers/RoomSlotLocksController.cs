using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    public class RoomSlotLocksController : Controller
    {
        private readonly IRoomSlotLockService _roomSlotLockService;
        private readonly IRoomService _roomService;

        public RoomSlotLocksController(IRoomSlotLockService roomSlotLockService, IRoomService roomService)
        {
            _roomSlotLockService = roomSlotLockService;
            _roomService = roomService;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetRoomsAsync(null, null, 1, 100);
            ViewBag.Rooms = rooms?.Data ?? new List<LibraryManagement.MVC.ViewModels.Room.RoomViewModel>();
            return View();
        }

        [HttpGet("RoomSlotLocks/Get")]
        public async Task<IActionResult> Get(Guid roomId, DateTime date)
        {
            try
            {
                var resultJson = await _roomSlotLockService.GetLocksByRoomAndDateAsync(roomId, date);
                return Content(resultJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("RoomSlotLocks/Lock")]
        public async Task<IActionResult> Lock([FromBody] object payload)
        {
            var success = await _roomSlotLockService.LockSlotAsync(payload);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpDelete("RoomSlotLocks/Unlock/{id}")]
        public async Task<IActionResult> Unlock(int id)
        {
            var success = await _roomSlotLockService.UnlockSlotAsync(id);
            if (success) return Ok();
            return BadRequest();
        }
    }
}
