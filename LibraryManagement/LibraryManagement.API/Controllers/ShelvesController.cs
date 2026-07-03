using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.ShelfDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/shelves")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class ShelvesController : ControllerBase
    {
        private readonly IShelfService _shelfService;

        public ShelvesController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        // ── Tree ─────────────────────────────────────────────────────────────────

        /// <summary>Lấy toàn bộ cây phân cấp: Tầng → Giá → Kệ → Ô</summary>
        [HttpGet("tree")]
        public async Task<IActionResult> GetShelfTree()
        {
            var tree = await _shelfService.GetShelfTreeAsync();
            return Ok(tree);
        }

        // ── Floors ────────────────────────────────────────────────────────────────

        /// <summary>Danh sách tầng</summary>
        [HttpGet("floors")]
        public async Task<IActionResult> GetFloors()
        {
            var floors = await _shelfService.GetFloorsAsync();
            return Ok(floors);
        }

        /// <summary>Chi tiết tầng theo ID</summary>
        [HttpGet("floors/{id:guid}")]
        public async Task<IActionResult> GetFloor(Guid id)
        {
            var floor = await _shelfService.GetFloorByIdAsync(id);
            if (floor == null) return NotFound(new { message = "Không tìm thấy tầng." });
            return Ok(floor);
        }

        /// <summary>Tạo tầng mới — chỉ Admin</summary>
        [HttpPost("floors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFloor([FromBody] CreateFloorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var floor = await _shelfService.CreateFloorAsync(dto);
                return CreatedAtAction(nameof(GetFloor), new { id = floor.FloorId }, floor);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                      ex.InnerException?.Message.Contains("UQ_Floors") == true)
            {
                return Conflict(new { message = "Số tầng đã tồn tại." });
            }
        }

        /// <summary>Cập nhật tầng — chỉ Admin</summary>
        [HttpPut("floors/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFloor(Guid id, [FromBody] UpdateFloorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.FloorId) return BadRequest(new { message = "Floor ID không khớp." });
            var success = await _shelfService.UpdateFloorAsync(dto);
            if (!success) return NotFound(new { message = "Không tìm thấy tầng." });
            return NoContent();
        }

        /// <summary>Xóa tầng (cascade: xóa luôn giá/kệ/ô bên trong) — chỉ Admin</summary>
        [HttpDelete("floors/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFloor(Guid id)
        {
            var success = await _shelfService.DeleteFloorAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy tầng." });
            return NoContent();
        }

        // ── Bookshelves ───────────────────────────────────────────────────────────

        /// <summary>Danh sách giá sách (tùy chọn lọc theo tầng)</summary>
        [HttpGet("bookshelves")]
        public async Task<IActionResult> GetBookshelves([FromQuery] Guid? floorId)
        {
            var list = await _shelfService.GetBookshelvesAsync(floorId);
            return Ok(list);
        }

        /// <summary>Chi tiết giá sách theo ID</summary>
        [HttpGet("bookshelves/{id:guid}")]
        public async Task<IActionResult> GetBookshelf(Guid id)
        {
            var bs = await _shelfService.GetBookshelfByIdAsync(id);
            if (bs == null) return NotFound(new { message = "Không tìm thấy giá sách." });
            return Ok(bs);
        }

        /// <summary>Tạo giá sách mới — chỉ Admin</summary>
        [HttpPost("bookshelves")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBookshelf([FromBody] CreateBookshelfDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var bs = await _shelfService.CreateBookshelfAsync(dto);
                return CreatedAtAction(nameof(GetBookshelf), new { id = bs.BookshelfId }, bs);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                      ex.InnerException?.Message.Contains("UQ_Bookshelves") == true)
            {
                return Conflict(new { message = "Mã giá đã tồn tại trong tầng này." });
            }
        }

        /// <summary>Cập nhật giá sách và danh sách thể loại — chỉ Admin</summary>
        [HttpPut("bookshelves/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookshelf(Guid id, [FromBody] UpdateBookshelfDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.BookshelfId) return BadRequest(new { message = "Bookshelf ID không khớp." });
            var success = await _shelfService.UpdateBookshelfAsync(dto);
            if (!success) return NotFound(new { message = "Không tìm thấy giá sách." });
            return NoContent();
        }

        /// <summary>Xóa giá sách — chỉ Admin</summary>
        [HttpDelete("bookshelves/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookshelf(Guid id)
        {
            var success = await _shelfService.DeleteBookshelfAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy giá sách." });
            return NoContent();
        }

        // ── Shelves ───────────────────────────────────────────────────────────────

        /// <summary>Danh sách kệ (tùy chọn lọc theo giá sách)</summary>
        [HttpGet("racks")]
        public async Task<IActionResult> GetShelves([FromQuery] Guid? bookshelfId)
        {
            var list = await _shelfService.GetShelvesAsync(bookshelfId);
            return Ok(list);
        }

        /// <summary>Chi tiết kệ theo ID</summary>
        [HttpGet("racks/{id:guid}")]
        public async Task<IActionResult> GetShelf(Guid id)
        {
            var s = await _shelfService.GetShelfByIdAsync(id);
            if (s == null) return NotFound(new { message = "Không tìm thấy kệ." });
            return Ok(s);
        }

        /// <summary>Tạo kệ mới — chỉ Admin</summary>
        [HttpPost("racks")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShelf([FromBody] CreateShelfDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var s = await _shelfService.CreateShelfAsync(dto);
                return CreatedAtAction(nameof(GetShelf), new { id = s.ShelfId }, s);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                      ex.InnerException?.Message.Contains("UQ_Shelves") == true)
            {
                return Conflict(new { message = "Số kệ đã tồn tại trong giá sách này." });
            }
        }

        /// <summary>Cập nhật kệ — chỉ Admin</summary>
        [HttpPut("racks/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateShelf(Guid id, [FromBody] UpdateShelfDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.ShelfId) return BadRequest(new { message = "Shelf ID không khớp." });
            var success = await _shelfService.UpdateShelfAsync(dto);
            if (!success) return NotFound(new { message = "Không tìm thấy kệ." });
            return NoContent();
        }

        /// <summary>Xóa kệ — chỉ Admin</summary>
        [HttpDelete("racks/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShelf(Guid id)
        {
            var success = await _shelfService.DeleteShelfAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy kệ." });
            return NoContent();
        }

        // ── ShelfSlots ────────────────────────────────────────────────────────────

        /// <summary>Danh sách ô kệ (tùy chọn lọc theo kệ)</summary>
        [HttpGet("slots")]
        public async Task<IActionResult> GetSlots([FromQuery] Guid? shelfId)
        {
            var list = await _shelfService.GetSlotsAsync(shelfId);
            return Ok(list);
        }

        /// <summary>Chi tiết ô kệ theo ID</summary>
        [HttpGet("slots/{id:guid}")]
        public async Task<IActionResult> GetSlot(Guid id)
        {
            var slot = await _shelfService.GetSlotByIdAsync(id);
            if (slot == null) return NotFound(new { message = "Không tìm thấy ô kệ." });
            return Ok(slot);
        }

        /// <summary>Tạo ô kệ mới — chỉ Admin</summary>
        [HttpPost("slots")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSlot([FromBody] CreateShelfSlotDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var slot = await _shelfService.CreateSlotAsync(dto);
                return CreatedAtAction(nameof(GetSlot), new { id = slot.SlotId }, slot);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                      ex.InnerException?.Message.Contains("UQ_ShelfSlots") == true)
            {
                return Conflict(new { message = "Mã ô đã tồn tại trong kệ này." });
            }
        }

        /// <summary>Cập nhật ô kệ — chỉ Admin</summary>
        [HttpPut("slots/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSlot(Guid id, [FromBody] UpdateShelfSlotDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.SlotId) return BadRequest(new { message = "Slot ID không khớp." });
            var success = await _shelfService.UpdateSlotAsync(dto);
            if (!success) return NotFound(new { message = "Không tìm thấy ô kệ." });
            return NoContent();
        }

        /// <summary>Xóa ô kệ — chỉ Admin</summary>
        [HttpDelete("slots/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSlot(Guid id)
        {
            var success = await _shelfService.DeleteSlotAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy ô kệ." });
            return NoContent();
        }
    }
}
