using LibraryManagement.Business.DTOs.RoomDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _roomService.GetRoomsAsync(search, status, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound(new { message = "Không tìm thấy phòng." });
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _roomService.CreateRoomAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.RoomId }, result);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true ||
                                                ex.InnerException?.Message.Contains("UQ_Rooms_RoomName") == true)
            {
                return Conflict(new { message = "Tên phòng đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo phòng. Vui lòng thử lại." });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.RoomId) return BadRequest(new { message = "ID không khớp." });

            try
            {
                var result = await _roomService.UpdateRoomAsync(id, dto);
                if (result == null) return NotFound(new { message = "Không tìm thấy phòng cần cập nhật." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true ||
                                                ex.InnerException?.Message.Contains("UQ_Rooms_RoomName") == true)
            {
                return Conflict(new { message = "Tên phòng đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật phòng. Vui lòng thử lại." });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _roomService.DeleteRoomAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy phòng." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCE constraint") == true ||
                                                ex.InnerException?.Message.Contains("foreign key") == true ||
                                                ex.InnerException?.Message.Contains("FK_Reservations_Rooms") == true)
            {
                return BadRequest(new { message = "Không thể xóa phòng này vì đang có đặt phòng được liên kết trong hệ thống." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa phòng. Vui lòng thử lại." });
            }
        }
    }
}
