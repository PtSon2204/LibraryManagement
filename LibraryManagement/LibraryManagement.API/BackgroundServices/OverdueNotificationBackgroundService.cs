using LibraryManagement.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.API.BackgroundServices
{
    public class OverdueNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OverdueNotificationBackgroundService> _logger;

        public OverdueNotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<OverdueNotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Overdue Notification Background Service is starting.");

            // Nhường luồng cho ứng dụng khởi động xong
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Background Job: Đang kiểm tra danh sách sách quá hạn để gửi email...");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var overdueService = scope.ServiceProvider.GetRequiredService<IOverdueNotificationService>();

                        // 1. Gửi email quá hạn
                        var overdueResult = await overdueService.SendOverdueNotificationsAsync();
                        _logger.LogInformation($"Background Job: Đã gửi email thông báo cho {overdueResult.TotalOverdueLoans} phiếu mượn quá hạn.");

                        // 2. Gửi email sắp đến hạn (1 ngày nữa)
                        var dueSoonResult = await overdueService.SendDueSoonRemindersAsync(1);
                        _logger.LogInformation($"Background Job: Đã gửi email nhắc nhở cho {dueSoonResult.TotalOverdueLoans} phiếu mượn sắp đến hạn.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi xảy ra trong quá trình chạy OverdueNotificationBackgroundService.");
                }

                // Cài đặt lặp lại mỗi 1 phút để test
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
