using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Business.DTOs.ReservationDTOs;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] Guid roomId, [FromQuery] DateTime date)
        {
            var slots = await _reservationService.GetAvailableSlotsAsync(roomId, date);
            return Ok(slots);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            try
            {
                var result = await _reservationService.CreateReservationAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/checkin")]
        public async Task<IActionResult> CheckIn(Guid id)
        {
            try
            {
                var success = await _reservationService.CheckInAsync(id);
                if (success) return Ok(new { message = "Check-in thành công." });
                return NotFound(new { message = "Không tìm thấy đơn đặt phòng." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? readerId = null, [FromQuery] string? status = null)
        {
            var result = await _reservationService.GetReservationsAsync(pageNumber, pageSize, readerId, status);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(Guid id, [FromQuery] Guid? readerId = null)
        {
            try
            {
                var success = await _reservationService.CancelReservationAsync(id, readerId);
                if (success) return Ok(new { message = "Đã hủy đơn đặt phòng." });
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
