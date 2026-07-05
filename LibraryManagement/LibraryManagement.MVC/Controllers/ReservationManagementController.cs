using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    public class ReservationManagementController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservationManagementController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("ReservationManagement/GetList")]
        public async Task<IActionResult> GetList(int pageNumber = 1, int pageSize = 10, string status = "")
        {
            try
            {
                var resultJson = await _reservationService.GetReservationsAsync(pageNumber, pageSize, status);
                return Content(resultJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("ReservationManagement/CheckIn/{id}")]
        public async Task<IActionResult> CheckIn(Guid id)
        {
            var success = await _reservationService.CheckInAsync(id);
            if (success) return Ok();
            return BadRequest();
        }
    }
}
