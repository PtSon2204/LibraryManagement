using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/admin/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors([FromQuery] string? search)
        {
            var authors = await _authorService.GetAuthorsAsync(search);
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound(new { message = "Không tìm thấy tác giả." });
            }
            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto)
        {
            try
            {
                var author = await _authorService.CreateAuthorAsync(createAuthorDto);
                return CreatedAtAction(nameof(GetAuthorById), new { id = author.AuthorId }, author);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            try
            {
                var success = await _authorService.UpdateAuthorAsync(id, updateAuthorDto);
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy tác giả." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var success = await _authorService.DeleteAuthorAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy tác giả." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
