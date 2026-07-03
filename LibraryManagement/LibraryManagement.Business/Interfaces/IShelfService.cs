using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.ShelfDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IShelfService
    {
        // ── Tree ──────────────────────────────────────────────────────────────────
        Task<IEnumerable<ShelfTreeDto>> GetShelfTreeAsync();

        // ── Floors ────────────────────────────────────────────────────────────────
        Task<IEnumerable<FloorDto>> GetFloorsAsync();
        Task<FloorDto?> GetFloorByIdAsync(Guid id);
        Task<FloorDto> CreateFloorAsync(CreateFloorDto dto);
        Task<bool> UpdateFloorAsync(UpdateFloorDto dto);
        Task<bool> DeleteFloorAsync(Guid id);

        // ── Bookshelves ───────────────────────────────────────────────────────────
        Task<IEnumerable<BookshelfDto>> GetBookshelvesAsync(Guid? floorId = null);
        Task<BookshelfDto?> GetBookshelfByIdAsync(Guid id);
        Task<BookshelfDto> CreateBookshelfAsync(CreateBookshelfDto dto);
        Task<bool> UpdateBookshelfAsync(UpdateBookshelfDto dto);
        Task<bool> DeleteBookshelfAsync(Guid id);

        // ── Shelves ───────────────────────────────────────────────────────────────
        Task<IEnumerable<ShelfDto>> GetShelvesAsync(Guid? bookshelfId = null);
        Task<ShelfDto?> GetShelfByIdAsync(Guid id);
        Task<ShelfDto> CreateShelfAsync(CreateShelfDto dto);
        Task<bool> UpdateShelfAsync(UpdateShelfDto dto);
        Task<bool> DeleteShelfAsync(Guid id);

        // ── ShelfSlots ────────────────────────────────────────────────────────────
        Task<IEnumerable<ShelfSlotDto>> GetSlotsAsync(Guid? shelfId = null);
        Task<ShelfSlotDto?> GetSlotByIdAsync(Guid id);
        Task<ShelfSlotDto> CreateSlotAsync(CreateShelfSlotDto dto);
        Task<bool> UpdateSlotAsync(UpdateShelfSlotDto dto);
        Task<bool> DeleteSlotAsync(Guid id);
    }
}
