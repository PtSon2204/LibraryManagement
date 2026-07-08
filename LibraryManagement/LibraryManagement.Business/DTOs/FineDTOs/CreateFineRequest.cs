namespace LibraryManagement.Business.DTOs.FineDTOs;

/// <summary>Request tạo khoản phạt và ghi nhận trả sách</summary>
public class CreateFineRequest
{
    public Guid LoanDetailId { get; set; }

    /// <summary>Danh sách template được chọn</summary>
    public List<SelectedFineItem> SelectedItems { get; set; } = new();

    /// <summary>"Cash" | "Transfer"</summary>
    public string PaymentMethod { get; set; } = "Cash";

    public string? Note { get; set; }

    /// <summary>Số ngày quá hạn (từ client gửi lên để validate)</summary>
    public int OverdueDays { get; set; }
}

public class SelectedFineItem
{
    public Guid FineTemplateId { get; set; }

    /// <summary>Số tiền thực tế (client đã tính: Fixed = Amount, PerDay = Amount × OverdueDays)</summary>
    public decimal Amount { get; set; }
}
