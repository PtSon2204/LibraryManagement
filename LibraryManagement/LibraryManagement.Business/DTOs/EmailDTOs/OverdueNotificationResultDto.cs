using System;

namespace LibraryManagement.Business.DTOs.EmailDTOs;

public class OverdueNotificationResultDto
{
    public int TotalOverdueLoans { get; set; }
    public int TotalReadersNotified { get; set; }
    public DateTime NotifiedAt { get; set; }
}
