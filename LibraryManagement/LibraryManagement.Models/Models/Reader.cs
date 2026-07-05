using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Reader
{
    public Guid ReaderId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    /// <summary>Active | Suspended | Inactive</summary>
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>Thời gian bị khóa quyền đặt phòng (nếu bị NoShow 3 lần)</summary>
    public DateTime? RoomBookingLockedUntil { get; set; }

    // Navigation
    public virtual UserProfile? Profile { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    /// <summary>Lịch sử các phiên thanh toán phạt của độc giả này.</summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
