using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers;

[Route("api/books")]
[ApiController]
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
        query.PageNumber = Math.Max(query.Page ?? query.PageNumber, 1);

        if (IsManagementQuery())
        {
            query.IncludeHidden ??= User.IsInRole("Admin");

            return Ok(await _bookService.GetBooksAsync(query));
        }

        var result = await _bookService.GetPublicBooksAsync(query);
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata")]
    public IActionResult GetBooksOData()
    {
        return Ok(_bookService.GetBooksQuery());
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] int count = 5)
    {
        var result = await _bookService.GetLatestBooksAsync(count);
        return Ok(result);
    }

    [HttpGet("{bookId:guid}")]
    public async Task<IActionResult> GetBookDetail(Guid bookId)
    {
        var result = await _bookService.GetBookDetailAsync(bookId);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookDto createBookDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var book = await _bookService.CreateBookAsync(createBookDto);
            return CreatedAtAction(nameof(GetBookDetail), new { bookId = book.BookId }, book);
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

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPut("{id:guid}")]
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

    [Authorize(Roles = "Admin,Librarian")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var success = await _bookService.DeleteBookAsync(id);
        return success ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/toggle-hide")]
    public async Task<IActionResult> ToggleHide(Guid id)
    {
        var success = await _bookService.ToggleHideAsync(id);
        if (!success) return NotFound(new { message = "Không tìm thấy sách." });

        return Ok(new { message = "Cập nhật trạng thái sách thành công." });
    }

    private bool IsManagementQuery()
    {
        var query = Request.Query;

        return query.ContainsKey("searchTerm")
            || query.ContainsKey("publisherId")
            || query.ContainsKey("publicationYear")
            || query.ContainsKey("pageNumber")
            || query.ContainsKey("includeHidden");
    }
}
