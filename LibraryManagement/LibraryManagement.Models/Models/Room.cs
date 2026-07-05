using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Room
{
    public Guid RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    /// <summary>Số chỗ ngồi tối đa</summary>
    public int Capacity { get; set; }

    public string? Description { get; set; }

    /// <summary>Available | Occupied | Maintenance</summary>
    public string Status { get; set; } = null!;

    public string? Image { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>Tầng mà phòng này thuộc về (nullable)</summary>
    public Guid? FloorId { get; set; }

    // Navigation
    public virtual Floor? Floor { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

