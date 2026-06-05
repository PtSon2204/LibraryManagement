using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Account
{
    public Guid AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    /// <summary>Admin | Librarian</summary>
    public string Role { get; set; } = null!;

    /// <summary>Active | Inactive</summary>
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual UserProfile? Profile { get; set; }

    /// <summary>Các phiếu mượn mà account này đã xử lý (duyệt)</summary>
    public virtual ICollection<Loan> ProcessedLoans { get; set; } = new List<Loan>();

    /// <summary>Các phiên thanh toán mà account này đã xác nhận nhận tiền</summary>
    public virtual ICollection<Payment> ProcessedPayments { get; set; } = new List<Payment>();
}
