using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int pageNumber = 1, int pageSize = 12)
        {
            var model = await _roomService.GetRoomsAsync(search, status, pageNumber, pageSize);
            
            if (model == null)
            {
                TempData["Error"] = "Không thể tải danh sách phòng. Vui lòng thử lại sau.";
                model = new LibraryManagement.MVC.ViewModels.Room.RoomListViewModel();
            }

            model.Search = search;
            model.Status = status;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _roomService.GetRoomByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }
    }
}
