using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Reservation
{
    public Guid ReservationId { get; set; }

    public Guid BookId { get; set; }

    public Guid? CopyId { get; set; }

    public Guid UserId { get; set; }

    public DateTime ReservationDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;

    public virtual BookCopy? Copy { get; set; }

    public virtual User User { get; set; } = null!;
}
