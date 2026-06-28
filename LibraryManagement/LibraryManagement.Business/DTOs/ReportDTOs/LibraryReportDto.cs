namespace LibraryManagement.Business.DTOs.ReportDTOs;

public class LibraryReportDto
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int TotalBooks { get; set; }

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }

    public int TotalReaders { get; set; }

    public int NewReaders { get; set; }

    public int LoansCreated { get; set; }

    public int BooksReturned { get; set; }

    public int ActiveLoans { get; set; }

    public int OverdueLoans { get; set; }

    public decimal UnpaidFineAmount { get; set; }

    public List<LoanStatusReportDto> LoansByStatus { get; set; } = new();

    public List<TopBorrowedBookDto> TopBorrowedBooks { get; set; } = new();
}
