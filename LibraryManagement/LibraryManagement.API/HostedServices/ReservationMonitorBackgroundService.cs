using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.API.HostedServices
{
    public class ReservationMonitorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationMonitorBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public ReservationMonitorBackgroundService(IServiceProvider serviceProvider, ILogger<ReservationMonitorBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Monitor Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessNoShowsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing ProcessNoShowsAsync.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Reservation Monitor Service is stopping.");
        }

        private async Task ProcessNoShowsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // Lấy các Reservation đã qua 15 phút mà vẫn chưa check-in (Status == Confirmed/Pending)
            var thresholdTime = DateTime.Now.AddMinutes(-15);
            
            var noShowReservations = await unitOfWork.Reservations.Query()
                .Where(r => (r.Status == "Confirmed" || r.Status == "Pending") &&
                            r.StartTime <= thresholdTime)
                .ToListAsync(stoppingToken);

            if (!noShowReservations.Any()) return;

            var readerIdsToCheck = noShowReservations.Select(r => r.ReaderId).Distinct().ToList();

            foreach (var res in noShowReservations)
            {
                res.Status = "NoShow";
                res.IsNoShow = true;
                unitOfWork.Reservations.Update(res);
            }

            await unitOfWork.SaveChangesAsync();

            // Áp dụng phạt: tính số lần NoShow trong 30 ngày qua cho các reader có vi phạm lần này
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            
            foreach (var readerId in readerIdsToCheck)
            {
                var noShowCount = await unitOfWork.Reservations.Query()
                    .Where(r => r.ReaderId == readerId &&
                                r.IsNoShow == true &&
                                r.StartTime >= thirtyDaysAgo)
                    .CountAsync(stoppingToken);

                if (noShowCount >= 3)
                {
                    var reader = await unitOfWork.Readers.GetByIdAsync(readerId);
                    if (reader != null)
                    {
                        // Phạt khóa quyền đặt phòng 7 ngày
                        reader.RoomBookingLockedUntil = DateTime.Now.AddDays(7);
                        unitOfWork.Readers.Update(reader);
                        _logger.LogWarning($"Reader {readerId} bị khóa quyền đặt phòng 7 ngày do vi phạm 3 lần No-show.");
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
}
