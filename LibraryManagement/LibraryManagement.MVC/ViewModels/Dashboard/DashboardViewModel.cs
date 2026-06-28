namespace LibraryManagement.MVC.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalReaders { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
    }
}
