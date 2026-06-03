using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace LibraryManagement.API.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookQueryService _bookQueryService;

        public BooksController(IBookQueryService bookQueryService)
        {
            _bookQueryService = bookQueryService;
        }

        [EnableQuery]
        public IQueryable<BookOdataDto> Get()
        {
            return _bookQueryService.GetBooksOdataQuery();
        }
    }
}
