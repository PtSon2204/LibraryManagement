using System;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.API.HostedServices;

public class OverdueNotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OverdueNotificationBackgroundService> _logger;

    public OverdueNotificationBackgroundService(IServiceProvider serviceProvider, ILogger<OverdueNotificationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Overdue Notification Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                // Target time: 8:00 AM ICT, which is 01:00 AM UTC
                // However, we just check if it's the right time and then delay.
                // A simpler approach for the daily task is to calculate the time until next 1:00 AM UTC.
                var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0, DateTimeKind.Utc);
                
                if (now >= nextRunTime)
                {
                    nextRunTime = nextRunTime.AddDays(1);
                }

                var delay = nextRunTime - now;
                _logger.LogInformation($"Next overdue notification run is scheduled at {nextRunTime} UTC (Delay: {delay}).");

                await Task.Delay(delay, stoppingToken);

                // Time to run!
                _logger.LogInformation("Running overdue notifications task...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<IOverdueNotificationService>();
                    var result = await notificationService.SendOverdueNotificationsAsync();

                    _logger.LogInformation($"Overdue notification task completed. Total Loans Overdue: {result.TotalOverdueLoans}, Readers Notified: {result.TotalReadersNotified}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the overdue notification background service.");
                // Prevent tight loop in case of continuous errors
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
