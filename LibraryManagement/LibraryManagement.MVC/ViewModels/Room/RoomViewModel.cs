using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.Room
{
    public class RoomViewModel
    {
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "Tên phòng không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên phòng không được vượt quá 100 ký tự.")]
        public string RoomName { get; set; } = null!;

        [Required(ErrorMessage = "Số chỗ ngồi không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số chỗ ngồi phải lớn hơn 0.")]
        public int Capacity { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public string Status { get; set; } = "Available";

        public DateTime CreatedAt { get; set; }
    }

    public class RoomListViewModel
    {
        public List<RoomViewModel> Data { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string? Search { get; set; }
        public string? Status { get; set; }
    }
}
