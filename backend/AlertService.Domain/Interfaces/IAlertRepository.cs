using AlertService.Domain.Entities;

namespace AlertService.Domain.Interfaces;

public interface IAlertRepository
{
    Task AddAlertAsync(Alert alert);
    Task<List<Alert>> GetAllAlertsAsync();
    Task<Alert?> GetAlertByIdAsync(int id);
    Task UpdateAlertAsync(Alert alert);
}
