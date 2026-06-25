using System;

namespace LibraryManagement.Models.Models;
public partial class UserProfile
{
    public Guid UserProfileId { get; set; }

    public Guid? ReaderId { get; set; }

    public Guid? AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public virtual Reader? Reader { get; set; }

    public virtual Account? Account { get; set; }
}
