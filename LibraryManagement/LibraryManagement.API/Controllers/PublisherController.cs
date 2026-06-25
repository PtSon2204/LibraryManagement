using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
