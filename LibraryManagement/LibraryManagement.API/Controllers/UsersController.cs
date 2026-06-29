using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UsersController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet("librarians")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLibrarians(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _userManagementService.GetLibrariansAsync(search, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("readers")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> GetReaders(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _userManagementService.GetReadersAsync(search, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost("librarians")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLibrarian([FromBody] CreateLibrarianDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userManagementService.CreateLibrarianAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("readers")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> CreateReader([FromBody] CreateReaderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userManagementService.CreateReaderAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("librarians/{id:guid}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleLibrarianStatus(Guid id)
        {
            var success = await _userManagementService.ToggleLibrarianStatusAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy thủ thư." });

            return Ok(new { message = "Cập nhật trạng thái thủ thư thành công." });
        }

        [HttpPut("readers/{id:guid}/toggle-status")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> ToggleReaderStatus(Guid id)
        {
            var success = await _userManagementService.ToggleReaderStatusAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy độc giả." });

            return Ok(new { message = "Cập nhật trạng thái độc giả thành công." });
        }
    }
}
