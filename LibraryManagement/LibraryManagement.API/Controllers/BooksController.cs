using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace LibraryManagement.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)] // OData controller — excluded from Swagger
    public class BooksController : ControllerBase
    {
        private readonly IBookQueryService _bookQueryService;

        public BooksController(IBookQueryService bookQueryService)
        {
            _bookQueryService = bookQueryService;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<BookOdataDto> Get()
        {
            return _bookQueryService.GetBooksOdataQuery();
        }
    }
}
