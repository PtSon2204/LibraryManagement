using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models.Models
{
    [Table("RoomSlotLocks")]
    public class RoomSlotLock
    {
        [Key]
        public int RoomSlotLockId { get; set; }

        [Required]
        public Guid RoomId { get; set; }
        public virtual Room Room { get; set; } = null!;

        [Required]
        [Column(TypeName = "date")]
        public DateTime LockDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(255)]
        public string? Reason { get; set; }

        public Guid? LockedByUserId { get; set; }
        
        [ForeignKey(nameof(LockedByUserId))]
        public virtual Account? LockedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
