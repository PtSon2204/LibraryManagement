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

    /// <summary>Đánh dấu bản sao bị mất; số tiền được lấy từ BookCopy.ReplacementPrice ở server.</summary>
    public bool IsLostBook { get; set; }

}

public class SelectedFineItem
{
    public Guid FineTemplateId { get; set; }
}
