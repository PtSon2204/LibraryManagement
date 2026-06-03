using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Business.DTOs.BookDTOs;

public class CreateBookDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? ISBN { get; set; }

    public int? PublicationYear { get; set; }

    [MaxLength(50)]
    public string? Language { get; set; }

    [MaxLength(100)]
    public string? Edition { get; set; }

    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? CoverImageUrl { get; set; }
}
