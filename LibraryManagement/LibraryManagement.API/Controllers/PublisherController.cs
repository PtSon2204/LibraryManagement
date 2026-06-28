using LibraryManagement.Business.DTOs.PublisherDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers
{
    [Route("api/publishers")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _publisherService.GetPublishersAsync(search, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null) return NotFound(new { message = "Không tìm thấy nhà xuất bản." });
            return Ok(publisher);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePublisherDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _publisherService.CreatePublisherAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.PublisherId }, result);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "Tên nhà xuất bản đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm nhà xuất bản. Vui lòng thử lại." });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePublisherDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.PublisherId) return BadRequest(new { message = "ID không khớp." });

            try
            {
                var result = await _publisherService.UpdatePublisherAsync(id, dto);
                if (result == null) return NotFound(new { message = "Không tìm thấy nhà xuất bản cần cập nhật." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "Tên nhà xuất bản đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật nhà xuất bản. Vui lòng thử lại." });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _publisherService.DeletePublisherAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy nhà xuất bản." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCE constraint") == true ||
                                                ex.InnerException?.Message.Contains("foreign key") == true)
            {
                return BadRequest(new { message = "Không thể xóa nhà xuất bản này vì có sách thuộc nhà xuất bản này đang tồn tại trong hệ thống." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa nhà xuất bản. Vui lòng thử lại." });
            }
        }
    }
}
