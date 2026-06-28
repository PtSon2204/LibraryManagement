using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

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
    public async Task<IActionResult> GetBooks(
        [FromQuery] string? title,
        [FromQuery] string? language,
        [FromQuery] string? publisher,
        [FromQuery] bool availableOnly = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _bookService.GetBooksAsync(title, language, publisher, availableOnly, sortBy, page, pageSize);
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
}
