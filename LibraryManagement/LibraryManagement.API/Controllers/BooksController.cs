using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers
{
    [Route("api/books")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] BookQuery query)
        {
            if (!query.IncludeHidden.HasValue)
            {
                query.IncludeHidden = User.IsInRole("Admin");
            }
            var result = await _bookService.GetBooksAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var book = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, book);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "ISBN đã tồn tại trong hệ thống. Vui lòng kiểm tra lại mã ISBN." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm sách. Vui lòng thử lại." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != updateBookDto.BookId) return BadRequest(new { message = "Book ID không khớp." });

            try
            {
                var success = await _bookService.UpdateBookAsync(updateBookDto);
                if (!success) return NotFound(new { message = "Không tìm thấy sách cần cập nhật." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "ISBN đã tồn tại trong hệ thống. Vui lòng kiểm tra lại mã ISBN." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật sách. Vui lòng thử lại." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var success = await _bookService.DeleteBookAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/toggle-hide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleHide(Guid id)
        {
            var success = await _bookService.ToggleHideAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy sách." });

            return Ok(new { message = "Cập nhật trạng thái sách thành công." });
        }
    }
}
