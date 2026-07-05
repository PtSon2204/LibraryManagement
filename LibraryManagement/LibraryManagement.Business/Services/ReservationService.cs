using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.ReservationDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;

        private const int SlotDurationHours = 4;

        public ReservationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private static ReservationDto MapToDto(Reservation r) => new ReservationDto
        {
            ReservationId = r.ReservationId,
            ReaderId = r.ReaderId,
            ReaderName = r.Reader?.Profile?.FullName ?? r.Reader?.Email ?? string.Empty,
            RoomId = r.RoomId,
            RoomName = r.Room?.RoomName ?? string.Empty,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            ReservationDate = r.ReservationDate,
            Status = r.Status,
            ActualCheckInTime = r.ActualCheckInTime,
            IsNoShow = r.IsNoShow
        };

        public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(Guid roomId, DateTime date)
        {
            var slots = new List<AvailableSlotDto>();
            var targetDate = date.Date;

            if (targetDate < DateTime.Today || targetDate > DateTime.Today.AddDays(30))
            {
                // Quy định: chỉ được đặt trong vòng 30 ngày tới
                return slots;
            }

            // Lấy các Slot Template đang active
            var activeTemplates = await _unitOfWork.SlotTemplates.Query()
                .Where(t => t.IsActive)
                .OrderBy(t => t.StartTime)
                .ToListAsync();

            var potentialSlots = activeTemplates.Select(t => new
            {
                Start = targetDate.Add(t.StartTime),
                End = targetDate.Add(t.EndTime)
            }).ToList();

            // Lấy các ca bị khóa của phòng này trong ngày
            var lockedSlots = await _unitOfWork.RoomSlotLocks.Query()
                .Where(l => l.RoomId == roomId && l.LockDate == targetDate)
                .ToListAsync();

            // Lấy các đặt phòng đã Confirm/CheckedIn/Pending của phòng này trong ngày
            var existingReservations = await _unitOfWork.Reservations.Query()
                .Where(r => r.RoomId == roomId &&
                            r.StartTime.Date == targetDate &&
                            (r.Status == "Confirmed" || r.Status == "CheckedIn" || r.Status == "Pending"))
                .ToListAsync();

            foreach (var s in potentialSlots)
            {
                var dto = new AvailableSlotDto
                {
                    StartTime = s.Start,
                    EndTime = s.End,
                    IsAvailable = true
                };

                if (s.Start < DateTime.Now)
                {
                    dto.IsAvailable = false;
                    dto.Reason = "Past";
                }
                else
                {
                    // Check xem có bị thủ thư khóa không
                    var lockInfo = lockedSlots.FirstOrDefault(l => l.StartTime == s.Start.TimeOfDay && l.EndTime == s.End.TimeOfDay);
                    if (lockInfo != null)
                    {
                        dto.IsAvailable = false;
                        dto.Reason = lockInfo.Reason ?? "Bảo trì/Sự kiện";
                    }
                    // Check xem có người đặt chưa
                    else if (existingReservations.Any(r => r.StartTime < s.End && r.EndTime > s.Start))
                    {
                        dto.IsAvailable = false;
                        dto.Reason = "Booked";
                    }
                }

                slots.Add(dto);
            }

            return slots;
        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            var reader = await _unitOfWork.Readers.Query().FirstOrDefaultAsync(r => r.ReaderId == dto.ReaderId);
            if (reader == null) throw new Exception("Không tìm thấy độc giả.");

            // 1. Check Locked
            if (reader.RoomBookingLockedUntil.HasValue && reader.RoomBookingLockedUntil.Value > DateTime.Now)
            {
                throw new Exception($"Tài khoản bị khóa quyền đặt phòng đến {reader.RoomBookingLockedUntil.Value.ToString("dd/MM/yyyy HH:mm")}.");
            }

            // 2. Check 5 days max
            if (dto.StartTime.Date > DateTime.Today.AddDays(7) || dto.StartTime < DateTime.Now)
            {
                throw new Exception("Chỉ được đặt trước tối đa 5 ngày và không được đặt trong quá khứ.");
            }

            // 3. Check fixed slots & business hours
            if (dto.StartTime.TimeOfDay != new TimeSpan(8, 0, 0) &&
                dto.StartTime.TimeOfDay != new TimeSpan(12, 0, 0) &&
                dto.StartTime.TimeOfDay != new TimeSpan(16, 0, 0))
            {
                throw new Exception("Giờ bắt đầu không hợp lệ. Vui lòng chọn ca 8h, 12h, hoặc 16h.");
            }
            if ((dto.EndTime - dto.StartTime).TotalHours != SlotDurationHours)
            {
                throw new Exception($"Mỗi slot phòng phải kéo dài chính xác {SlotDurationHours} tiếng.");
            }

            // 4. Check 4 slots per week
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);
            
            var weeklySlotsCount = await _unitOfWork.Reservations.Query()
                .Where(r => r.ReaderId == dto.ReaderId &&
                            r.StartTime >= startOfWeek && r.StartTime < endOfWeek &&
                            r.Status != "Cancelled" && r.Status != "NoShow")
                .CountAsync();

            if (weeklySlotsCount >= 4)
            {
                throw new Exception("Bạn đã đạt giới hạn đặt 4 slot phòng trong tuần này.");
            }

            // 5. Check overlapping for THIS READER
            var isUserOverlapping = await _unitOfWork.Reservations.Query()
                .AnyAsync(r => r.ReaderId == dto.ReaderId &&
                               (r.Status == "Confirmed" || r.Status == "CheckedIn" || r.Status == "Pending") &&
                               r.StartTime < dto.EndTime && r.EndTime > dto.StartTime);
            
            if (isUserOverlapping)
            {
                throw new Exception("Bạn đang có lịch đặt phòng khác trùng với khoảng thời gian này.");
            }

            // 6. Check room availability
            var isRoomBooked = await _unitOfWork.Reservations.Query()
                .AnyAsync(r => r.RoomId == dto.RoomId &&
                               (r.Status == "Confirmed" || r.Status == "CheckedIn" || r.Status == "Pending") &&
                               r.StartTime < dto.EndTime && r.EndTime > dto.StartTime);

            if (isRoomBooked)
            {
                throw new Exception("Phòng này đã được đặt trong khoảng thời gian bạn chọn.");
            }

            var reservation = new Reservation
            {
                ReservationId = Guid.NewGuid(),
                RoomId = dto.RoomId,
                ReaderId = dto.ReaderId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ReservationDate = DateTime.Now,
                Status = "Confirmed", // Auto-confirm for simple flow
                IsNoShow = false
            };

            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            // Reload to get nav props
            var saved = await _unitOfWork.Reservations.Query()
                .Include(r => r.Room)
                .Include(r => r.Reader).ThenInclude(r => r.Profile)
                .FirstAsync(r => r.ReservationId == reservation.ReservationId);

            return MapToDto(saved);
        }

        public async Task<bool> CheckInAsync(Guid reservationId)
        {
            var res = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (res == null) return false;

            if (res.Status != "Confirmed" && res.Status != "Pending")
            {
                throw new Exception($"Không thể check-in phòng đang có trạng thái: {res.Status}.");
            }

            res.Status = "CheckedIn";
            res.ActualCheckInTime = DateTime.Now;

            _unitOfWork.Reservations.Update(res);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<ReservationDto>> GetReservationsAsync(int pageNumber, int pageSize, Guid? readerId = null, string? status = null)
        {
            var query = _unitOfWork.Reservations.Query()
                .Include(r => r.Room)
                .Include(r => r.Reader).ThenInclude(rd => rd.Profile)
                .AsNoTracking();

            if (readerId.HasValue)
                query = query.Where(r => r.ReaderId == readerId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status == status);

            var count = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ReservationDto>
            {
                Data = items.Select(MapToDto).ToList(),
                TotalRecords = count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }

        public async Task<bool> CancelReservationAsync(Guid reservationId, Guid? readerId = null)
        {
            var res = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (res == null) return false;

            if (readerId.HasValue && res.ReaderId != readerId.Value)
                throw new UnauthorizedAccessException("Không có quyền hủy đơn đặt phòng này.");

            if (res.Status == "CheckedIn" || res.Status == "Completed" || res.Status == "NoShow")
                throw new Exception($"Không thể hủy đơn đặt phòng ở trạng thái {res.Status}.");

            res.Status = "Cancelled";
            _unitOfWork.Reservations.Update(res);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
