namespace LibraryManagement.Business.DTOs.DashboardDTOs
{
    public class DashboardDto
    {
        public int TotalBooks { get; set; }
        public int TotalReaders { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
    }
}
