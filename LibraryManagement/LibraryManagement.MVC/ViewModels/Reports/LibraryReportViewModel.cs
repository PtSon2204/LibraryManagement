namespace LibraryManagement.MVC.ViewModels.Reports;

public class LibraryReportViewModel
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

    public List<LoanStatusReportViewModel> LoansByStatus { get; set; } = new();

    public List<TopBorrowedBookViewModel> TopBorrowedBooks { get; set; } = new();

    public ReportFilterViewModel Filter { get; set; } = new();
}
