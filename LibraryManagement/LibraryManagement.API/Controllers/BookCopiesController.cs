using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.BookCopyDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers
{
    [Route("api/book-copies")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class BookCopiesController : ControllerBase
    {
        private readonly IBookCopyService _bookCopyService;

        public BookCopiesController(IBookCopyService bookCopyService)
        {
            _bookCopyService = bookCopyService;
        }

        /// <summary>
        /// Lấy danh sách bản sao sách có phân trang, filter, search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBookCopies([FromQuery] BookCopyQuery query)
        {
            if (!query.IncludeHidden.HasValue)
            {
                query.IncludeHidden = User.IsInRole("Admin");
            }

            var result = await _bookCopyService.GetBookCopiesAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 bản sao theo ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookCopyById(Guid id)
        {
            var copy = await _bookCopyService.GetBookCopyByIdAsync(id);
            if (copy == null) return NotFound(new { message = "Không tìm thấy bản sao sách." });
            return Ok(copy);
        }

        /// <summary>
        /// Thêm 1 bản sao sách
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBookCopy([FromBody] CreateBookCopyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var copy = await _bookCopyService.CreateBookCopyAsync(dto);
                return CreatedAtAction(nameof(GetBookCopyById), new { id = copy.CopyId }, copy);
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("duplicate key") == true ||
                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "Barcode đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm bản sao sách. Vui lòng thử lại." });
            }
        }

        /// <summary>
        /// Thêm nhiều bản sao sách cùng lúc (batch)
        /// </summary>
        [HttpPost("batch")]
        public async Task<IActionResult> CreateMultipleBookCopies([FromBody] CreateMultipleBookCopiesDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var copies = await _bookCopyService.CreateMultipleBookCopiesAsync(dto);
                return Ok(new
                {
                    message = $"Đã thêm thành công {dto.Copies.Count} bản sao.",
                    data    = copies
                });
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("duplicate key") == true ||
                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "Một hoặc nhiều Barcode đã tồn tại. Vui lòng kiểm tra lại danh sách." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm bản sao sách. Vui lòng thử lại." });
            }
        }

        /// <summary>
        /// Cập nhật thông tin bản sao sách
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBookCopy(Guid id, [FromBody] UpdateBookCopyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.CopyId) return BadRequest(new { message = "Copy ID không khớp." });

            try
            {
                var success = await _bookCopyService.UpdateBookCopyAsync(dto);
                if (!success) return NotFound(new { message = "Không tìm thấy bản sao cần cập nhật." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("duplicate key") == true ||
                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "Barcode đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật bản sao sách. Vui lòng thử lại." });
            }
        }

        /// <summary>
        /// Ẩn / hiện bản sao sách (toggle hide) - chỉ Admin
        /// </summary>
        [HttpPut("{id:guid}/toggle-hide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleHide(Guid id)
        {
            var success = await _bookCopyService.ToggleHideAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy bản sao sách." });

            return Ok(new { message = "Cập nhật trạng thái hiển thị bản sao thành công." });
        }

        /// <summary>
        /// Xóa bản sao sách - chỉ Admin
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookCopy(Guid id)
        {
            var success = await _bookCopyService.DeleteBookCopyAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy bản sao sách." });

            return NoContent();
        }
    }
}
