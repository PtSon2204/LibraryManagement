namespace LibraryManagement.MVC.ViewModels.Fines;

public class FineTemplateViewModel
{
    public Guid FineTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    /// <summary>"Fixed" hoặc "PerDay"</summary>
    public string FineType { get; set; } = "Fixed";
    public bool IsActive { get; set; }
}

public class CreateFineViewModel
{
    public Guid LoanDetailId { get; set; }
    public List<SelectedFineItemViewModel> SelectedItems { get; set; } = new();
    public string PaymentMethod { get; set; } = "Cash";
    public string? Note { get; set; }
    public bool IsLostBook { get; set; }
    public Guid ReaderId { get; set; }
}

public class SelectedFineItemViewModel
{
    public Guid FineTemplateId { get; set; }
}

public class FineListItemViewModel
{
    public Guid FineId { get; set; }
    public Guid LoanDetailId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string ReaderName { get; set; } = string.Empty;
    public string ReaderEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class FineListPageViewModel
{
    public List<FineListItemViewModel> Fines { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

public class UpsertFineTemplateViewModel
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string FineType { get; set; } = "Fixed";
    public bool IsActive { get; set; } = true;
}
