using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.EmailDTOs;

namespace LibraryManagement.Business.Interfaces;

public interface IOverdueNotificationService
{
    Task<OverdueNotificationResultDto> SendOverdueNotificationsAsync();
}
