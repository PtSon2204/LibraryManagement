using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthorDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _authorService.GetAuthorsAsync(search, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound(new { message = "Không tìm thấy tác giả." });
            return Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _authorService.CreateAuthorAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.AuthorId }, result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm tác giả. Vui lòng thử lại." });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.AuthorId) return BadRequest(new { message = "ID không khớp." });

            try
            {
                var result = await _authorService.UpdateAuthorAsync(id, dto);
                if (result == null) return NotFound(new { message = "Không tìm thấy tác giả cần cập nhật." });
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật tác giả. Vui lòng thử lại." });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _authorService.DeleteAuthorAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy tác giả." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCE constraint") == true ||
                                                ex.InnerException?.Message.Contains("foreign key") == true ||
                                                ex.InnerException?.Message.Contains("FK_BookAuthors_Authors") == true ||
                                                ex.InnerException?.Message.Contains("BookAuthors") == true)
            {
                return BadRequest(new { message = "Không thể xóa tác giả này vì đang có sách thuộc tác giả này tồn tại trong hệ thống." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa tác giả. Vui lòng thử lại." });
            }
        }
    }
}
