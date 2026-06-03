using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BookDetailsController : ControllerBase
    {
        private readonly IBookQueryService _bookQueryService;

        public BookDetailsController(IBookQueryService bookQueryService)
        {
            _bookQueryService = bookQueryService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookDetailAsync(Guid id)
        {
            var book = await _bookQueryService.GetBookDetailAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookAsync([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var book = await _bookQueryService.CreateBookAsync(dto);
                return CreatedAtAction(nameof(GetBookDetailAsync), new { id = book.BookId }, book);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
