namespace LibraryManagement.Business.DTOs.DashboardDTOs;

public class RecentReservationDto
{
    public Guid ReservationId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string BookTitle { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; }

    public string Status { get; set; } = string.Empty;
}
