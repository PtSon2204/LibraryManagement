using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.ShelfDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class ShelfService : IShelfService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShelfService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static FloorDto MapFloor(Floor f) => new()
        {
            FloorId         = f.FloorId,
            FloorNumber     = f.FloorNumber,
            FloorName       = f.FloorName,
            Description     = f.Description,
            TotalBookshelves = f.Bookshelves.Count
        };

        private static BookshelfDto MapBookshelf(Bookshelf b) => new()
        {
            BookshelfId  = b.BookshelfId,
            FloorId      = b.FloorId,
            FloorName    = b.Floor?.FloorName ?? string.Empty,
            ShelfCode    = b.ShelfCode,
            Name         = b.Name,
            Description  = b.Description,
            TotalShelves = b.Shelves.Count,
            Categories   = b.BookshelfCategories.Select(bc => new CategoryBriefDto
            {
                CategoryId   = bc.Category.CategoryId,
                CategoryName = bc.Category.CategoryName
            }).ToList()
        };

        private static ShelfDto MapShelf(Shelf s) => new()
        {
            ShelfId       = s.ShelfId,
            BookshelfId   = s.BookshelfId,
            BookshelfName = s.Bookshelf?.Name ?? string.Empty,
            ShelfNumber   = s.ShelfNumber,
            Name          = s.Name,
            TotalSlots    = s.ShelfSlots.Count
        };

        private static ShelfSlotDto MapSlot(ShelfSlot sl) => new()
        {
            SlotId          = sl.SlotId,
            ShelfId         = sl.ShelfId,
            ShelfName       = sl.Shelf?.Name ?? string.Empty,
            SlotCode        = sl.SlotCode,
            Capacity        = sl.Capacity,
            CurrentQuantity = sl.BookCopies.Count(bc => bc.Status != "Hidden"),
            Description     = sl.Description
        };

        // ── Tree ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<ShelfTreeDto>> GetShelfTreeAsync()
        {
            var floors = await _unitOfWork.Floors.Query()
                .Include(f => f.Bookshelves)
                    .ThenInclude(b => b.BookshelfCategories)
                        .ThenInclude(bc => bc.Category)
                .Include(f => f.Bookshelves)
                    .ThenInclude(b => b.Shelves)
                        .ThenInclude(s => s.ShelfSlots)
                            .ThenInclude(sl => sl.BookCopies)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();

            return floors.Select(f => new ShelfTreeDto
            {
                FloorId     = f.FloorId,
                FloorNumber = f.FloorNumber,
                FloorName   = f.FloorName,
                Description = f.Description,
                Bookshelves = f.Bookshelves.OrderBy(b => b.ShelfCode).Select(b => new BookshelfTreeDto
                {
                    BookshelfId = b.BookshelfId,
                    ShelfCode   = b.ShelfCode,
                    Name        = b.Name,
                    Description = b.Description,
                    Categories  = b.BookshelfCategories.Select(bc => new CategoryBriefDto
                    {
                        CategoryId   = bc.Category.CategoryId,
                        CategoryName = bc.Category.CategoryName
                    }).ToList(),
                    Shelves = b.Shelves.OrderBy(s => s.ShelfNumber).Select(s => new ShelfTreeNodeDto
                    {
                        ShelfId     = s.ShelfId,
                        ShelfNumber = s.ShelfNumber,
                        Name        = s.Name,
                        Slots       = s.ShelfSlots.OrderBy(sl => sl.SlotCode).Select(sl => new ShelfSlotDto
                        {
                            SlotId          = sl.SlotId,
                            ShelfId         = sl.ShelfId,
                            ShelfName       = s.Name,
                            SlotCode        = sl.SlotCode,
                            Capacity        = sl.Capacity,
                            CurrentQuantity = sl.BookCopies.Count(bc => bc.Status != "Hidden"),
                            Description     = sl.Description
                        }).ToList()
                    }).ToList()
                }).ToList()
            });
        }

        // ── Floors ────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<FloorDto>> GetFloorsAsync()
        {
            var floors = await _unitOfWork.Floors.Query()
                .Include(f => f.Bookshelves)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
            return floors.Select(MapFloor);
        }

        public async Task<FloorDto?> GetFloorByIdAsync(Guid id)
        {
            var floor = await _unitOfWork.Floors.Query()
                .Include(f => f.Bookshelves)
                .FirstOrDefaultAsync(f => f.FloorId == id);
            return floor == null ? null : MapFloor(floor);
        }

        public async Task<FloorDto> CreateFloorAsync(CreateFloorDto dto)
        {
            var floor = new Floor
            {
                FloorId     = Guid.NewGuid(),
                FloorNumber = dto.FloorNumber,
                FloorName   = dto.FloorName,
                Description = dto.Description
            };
            await _unitOfWork.Floors.AddAsync(floor);
            await _unitOfWork.SaveChangesAsync();
            return MapFloor(floor);
        }

        public async Task<bool> UpdateFloorAsync(UpdateFloorDto dto)
        {
            var floor = await _unitOfWork.Floors.GetByIdAsync(dto.FloorId);
            if (floor == null) return false;
            floor.FloorNumber = dto.FloorNumber;
            floor.FloorName   = dto.FloorName;
            floor.Description = dto.Description;
            _unitOfWork.Floors.Update(floor);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFloorAsync(Guid id)
        {
            var floor = await _unitOfWork.Floors.GetByIdAsync(id);
            if (floor == null) return false;
            _unitOfWork.Floors.Delete(floor);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── Bookshelves ───────────────────────────────────────────────────────────

        public async Task<IEnumerable<BookshelfDto>> GetBookshelvesAsync(Guid? floorId = null)
        {
            var query = _unitOfWork.Bookshelves.Query()
                .Include(b => b.Floor)
                .Include(b => b.Shelves)
                .Include(b => b.BookshelfCategories)
                    .ThenInclude(bc => bc.Category)
                .AsQueryable();

            if (floorId.HasValue)
                query = query.Where(b => b.FloorId == floorId.Value);

            var list = await query.OrderBy(b => b.ShelfCode).ToListAsync();
            return list.Select(MapBookshelf);
        }

        public async Task<BookshelfDto?> GetBookshelfByIdAsync(Guid id)
        {
            var b = await _unitOfWork.Bookshelves.Query()
                .Include(b => b.Floor)
                .Include(b => b.Shelves)
                .Include(b => b.BookshelfCategories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.BookshelfId == id);
            return b == null ? null : MapBookshelf(b);
        }

        public async Task<BookshelfDto> CreateBookshelfAsync(CreateBookshelfDto dto)
        {
            var bookshelf = new Bookshelf
            {
                BookshelfId = Guid.NewGuid(),
                FloorId     = dto.FloorId,
                ShelfCode   = dto.ShelfCode,
                Name        = dto.Name,
                Description = dto.Description
            };
            await _unitOfWork.Bookshelves.AddAsync(bookshelf);

            // Gán categories
            foreach (var catId in dto.CategoryIds.Distinct())
            {
                await _unitOfWork.BookshelfCategories.AddAsync(new BookshelfCategory
                {
                    BookshelfId = bookshelf.BookshelfId,
                    CategoryId  = catId
                });
            }

            await _unitOfWork.SaveChangesAsync();

            // Reload để trả về đầy đủ
            return (await GetBookshelfByIdAsync(bookshelf.BookshelfId))!;
        }

        public async Task<bool> UpdateBookshelfAsync(UpdateBookshelfDto dto)
        {
            var bookshelf = await _unitOfWork.Bookshelves.GetByIdAsync(dto.BookshelfId);
            if (bookshelf == null) return false;

            bookshelf.ShelfCode   = dto.ShelfCode;
            bookshelf.Name        = dto.Name;
            bookshelf.Description = dto.Description;
            _unitOfWork.Bookshelves.Update(bookshelf);

            // Ghi đè categories: xóa cũ, thêm mới
            var existingCats = await _unitOfWork.BookshelfCategories.Query()
                .Where(bc => bc.BookshelfId == dto.BookshelfId)
                .ToListAsync();
            foreach (var old in existingCats)
                _unitOfWork.BookshelfCategories.Delete(old);

            foreach (var catId in dto.CategoryIds.Distinct())
            {
                await _unitOfWork.BookshelfCategories.AddAsync(new BookshelfCategory
                {
                    BookshelfId = dto.BookshelfId,
                    CategoryId  = catId
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookshelfAsync(Guid id)
        {
            var bookshelf = await _unitOfWork.Bookshelves.GetByIdAsync(id);
            if (bookshelf == null) return false;
            _unitOfWork.Bookshelves.Delete(bookshelf);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── Shelves ───────────────────────────────────────────────────────────────

        public async Task<IEnumerable<ShelfDto>> GetShelvesAsync(Guid? bookshelfId = null)
        {
            var query = _unitOfWork.Shelves.Query()
                .Include(s => s.Bookshelf)
                .Include(s => s.ShelfSlots)
                .AsQueryable();

            if (bookshelfId.HasValue)
                query = query.Where(s => s.BookshelfId == bookshelfId.Value);

            var list = await query.OrderBy(s => s.ShelfNumber).ToListAsync();
            return list.Select(MapShelf);
        }

        public async Task<ShelfDto?> GetShelfByIdAsync(Guid id)
        {
            var s = await _unitOfWork.Shelves.Query()
                .Include(s => s.Bookshelf)
                .Include(s => s.ShelfSlots)
                .FirstOrDefaultAsync(s => s.ShelfId == id);
            return s == null ? null : MapShelf(s);
        }

        public async Task<ShelfDto> CreateShelfAsync(CreateShelfDto dto)
        {
            var shelf = new Shelf
            {
                ShelfId     = Guid.NewGuid(),
                BookshelfId = dto.BookshelfId,
                ShelfNumber = dto.ShelfNumber,
                Name        = dto.Name
            };
            await _unitOfWork.Shelves.AddAsync(shelf);
            await _unitOfWork.SaveChangesAsync();
            return (await GetShelfByIdAsync(shelf.ShelfId))!;
        }

        public async Task<bool> UpdateShelfAsync(UpdateShelfDto dto)
        {
            var shelf = await _unitOfWork.Shelves.GetByIdAsync(dto.ShelfId);
            if (shelf == null) return false;
            shelf.ShelfNumber = dto.ShelfNumber;
            shelf.Name        = dto.Name;
            _unitOfWork.Shelves.Update(shelf);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShelfAsync(Guid id)
        {
            var shelf = await _unitOfWork.Shelves.GetByIdAsync(id);
            if (shelf == null) return false;
            _unitOfWork.Shelves.Delete(shelf);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── ShelfSlots ────────────────────────────────────────────────────────────

        public async Task<IEnumerable<ShelfSlotDto>> GetSlotsAsync(Guid? shelfId = null)
        {
            var query = _unitOfWork.ShelfSlots.Query()
                .Include(sl => sl.Shelf)
                .Include(sl => sl.BookCopies)
                .AsQueryable();

            if (shelfId.HasValue)
                query = query.Where(sl => sl.ShelfId == shelfId.Value);

            var list = await query.OrderBy(sl => sl.SlotCode).ToListAsync();
            return list.Select(MapSlot);
        }

        public async Task<ShelfSlotDto?> GetSlotByIdAsync(Guid id)
        {
            var sl = await _unitOfWork.ShelfSlots.Query()
                .Include(sl => sl.Shelf)
                .Include(sl => sl.BookCopies)
                .FirstOrDefaultAsync(sl => sl.SlotId == id);
            return sl == null ? null : MapSlot(sl);
        }

        public async Task<ShelfSlotDto> CreateSlotAsync(CreateShelfSlotDto dto)
        {
            var slot = new ShelfSlot
            {
                SlotId      = Guid.NewGuid(),
                ShelfId     = dto.ShelfId,
                SlotCode    = dto.SlotCode,
                Capacity    = dto.Capacity,
                Description = dto.Description
            };
            await _unitOfWork.ShelfSlots.AddAsync(slot);
            await _unitOfWork.SaveChangesAsync();
            return (await GetSlotByIdAsync(slot.SlotId))!;
        }

        public async Task<bool> UpdateSlotAsync(UpdateShelfSlotDto dto)
        {
            var slot = await _unitOfWork.ShelfSlots.GetByIdAsync(dto.SlotId);
            if (slot == null) return false;
            slot.SlotCode    = dto.SlotCode;
            slot.Capacity    = dto.Capacity;
            slot.Description = dto.Description;
            _unitOfWork.ShelfSlots.Update(slot);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSlotAsync(Guid id)
        {
            var slot = await _unitOfWork.ShelfSlots.GetByIdAsync(id);
            if (slot == null) return false;
            _unitOfWork.ShelfSlots.Delete(slot);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
