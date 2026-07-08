namespace LibraryManagement.Business.DTOs.FineDTOs;

public class FineTemplateDto
{
    public Guid FineTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    /// <summary>"Fixed" hoặc "PerDay"</summary>
    public string FineType { get; set; } = "Fixed";
    public bool IsActive { get; set; }
}
