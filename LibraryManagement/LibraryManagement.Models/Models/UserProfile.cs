using System;

namespace LibraryManagement.Models.Models;

/// <summary>
/// Bảng thông tin cá nhân dùng chung cho cả Reader và Account.
/// Đúng 1 trong 2 FK (ReaderId hoặc AccountId) phải có giá trị (check constraint).
/// </summary>
public partial class UserProfile
{
    public Guid UserProfileId { get; set; }

    /// <summary>FK → Readers. Null nếu profile này thuộc về Account.</summary>
    public Guid? ReaderId { get; set; }

    /// <summary>FK → Accounts. Null nếu profile này thuộc về Reader.</summary>
    public Guid? AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    // Navigation
    public virtual Reader? Reader { get; set; }

    public virtual Account? Account { get; set; }
}
