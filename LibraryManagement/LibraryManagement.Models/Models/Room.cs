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

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
