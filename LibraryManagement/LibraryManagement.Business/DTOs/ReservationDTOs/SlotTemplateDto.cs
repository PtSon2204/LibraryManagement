using System;

namespace LibraryManagement.Business.DTOs.ReservationDTOs
{
    public class SlotTemplateDto
    {
        public int SlotTemplateId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSlotTemplateDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
