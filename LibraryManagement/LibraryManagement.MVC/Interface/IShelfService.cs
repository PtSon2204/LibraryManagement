using LibraryManagement.MVC.ViewModels.Shelf;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Interface
{
    public interface IShelfService
    {
        Task<ShelfIndexViewModel?> GetIndexViewModelAsync(Guid? floorId, string? availability);
        Task<BookshelfFormViewModel?> GetCreateFormDataAsync();
        Task<BookshelfFormViewModel?> GetEditFormDataAsync(Guid bookshelfId);
        Task<string?> CreateBookshelfAsync(BookshelfFormViewModel model);
        Task<string?> UpdateBookshelfAsync(BookshelfFormViewModel model);
        Task<bool> DeleteBookshelfAsync(Guid id);

        Task<List<FloorViewModel>> GetAllFloorsAsync();
        Task<FloorFormViewModel?> GetFloorEditFormDataAsync(Guid id);
        Task<string?> CreateFloorAsync(FloorFormViewModel model);
        Task<string?> UpdateFloorAsync(FloorFormViewModel model);
        Task<bool> DeleteFloorAsync(Guid id);

        Task<BookshelfTreeDto?> GetBookshelfDetailsAsync(Guid id);
        
        Task<string?> CreateShelfAsync(ShelfFormViewModel model);
        Task<string?> UpdateShelfAsync(ShelfFormViewModel model);
        Task<bool> DeleteShelfAsync(Guid id);

        Task<List<ShelfSlotViewModel>> GetAllSlotsAsync();
        Task<string?> CreateSlotAsync(ShelfSlotFormViewModel model);
        Task<string?> UpdateSlotAsync(ShelfSlotFormViewModel model);
        Task<bool> DeleteSlotAsync(Guid id);
    }
}
