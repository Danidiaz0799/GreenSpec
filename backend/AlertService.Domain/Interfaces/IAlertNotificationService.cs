using AlertService.Domain.Entities;

namespace AlertService.Domain.Interfaces;

public interface IAlertNotificationService
{
    Task NotifyNewAlertAsync(Alert alert);
}
