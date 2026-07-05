using System;

namespace LibraryManagement.Models.Models;

/// <summary>
/// Đặt phòng đọc sách tại thư viện.
/// Mỗi Reader chỉ được có 1 lượt đặt phòng đang active (Pending/Confirmed).
/// </summary>
public partial class Reservation
{
    public Guid ReservationId { get; set; }

    /// <summary>FK → Readers. Độc giả đặt phòng.</summary>
    public Guid ReaderId { get; set; }

    /// <summary>FK → Rooms. Phòng được đặt.</summary>
    public Guid RoomId { get; set; }

    /// <summary>Ngày bắt đầu sử dụng phòng.</summary>
    public DateTime StartTime { get; set; }

    /// <summary>Ngày kết thúc sử dụng phòng.</summary>
    public DateTime EndTime { get; set; }

    public DateTime ReservationDate { get; set; }

    /// <summary>Pending | Confirmed | CheckedIn | Completed | Cancelled | NoShow</summary>
    public string Status { get; set; } = null!;

    public DateTime? ActualCheckInTime { get; set; }

    public bool IsNoShow { get; set; }

    // Navigation
    public virtual Reader Reader { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
