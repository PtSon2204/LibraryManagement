using LibraryManagement.Business.DTOs.ReservationDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public class RoomSlotLockService : IRoomSlotLockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomSlotLockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RoomSlotLockDto>> GetLocksByRoomAndDateAsync(Guid roomId, DateTime date)
        {
            var targetDate = date.Date;
            var locks = await _unitOfWork.RoomSlotLocks.Query()
                .Include(l => l.LockedByUser)
                .Where(l => l.RoomId == roomId && l.LockDate == targetDate)
                .ToListAsync();

            return locks.Select(l => new RoomSlotLockDto
            {
                RoomSlotLockId = l.RoomSlotLockId,
                RoomId = l.RoomId,
                LockDate = l.LockDate,
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                Reason = l.Reason,
                LockedByUserName = l.LockedByUser != null 
                    ? (l.LockedByUser.Profile != null ? l.LockedByUser.Profile.FullName : l.LockedByUser.Email) 
                    : null
            }).ToList();
        }

        public async Task<RoomSlotLockDto> LockSlotAsync(CreateRoomSlotLockDto dto, Guid lockedByUserId)
        {
            var slotLock = new RoomSlotLock
            {
                RoomId = dto.RoomId,
                LockDate = dto.LockDate.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Reason = dto.Reason,
                LockedByUserId = lockedByUserId
            };

            await _unitOfWork.RoomSlotLocks.AddAsync(slotLock);
            await _unitOfWork.SaveChangesAsync();

            return new RoomSlotLockDto
            {
                RoomSlotLockId = slotLock.RoomSlotLockId,
                RoomId = slotLock.RoomId,
                LockDate = slotLock.LockDate,
                StartTime = slotLock.StartTime,
                EndTime = slotLock.EndTime,
                Reason = slotLock.Reason,
                LockedByUserId = slotLock.LockedByUserId
            };
        }

        public async Task<bool> UnlockSlotAsync(int roomSlotLockId)
        {
            var slotLock = await _unitOfWork.RoomSlotLocks.GetByIdAsync(roomSlotLockId);
            if (slotLock == null) return false;

            _unitOfWork.RoomSlotLocks.Delete(slotLock);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
