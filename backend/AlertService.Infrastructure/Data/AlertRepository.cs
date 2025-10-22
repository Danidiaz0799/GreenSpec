using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Infrastructure.Data;

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _context;

    public AlertRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAlertAsync(Alert alert)
    {
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Alert>> GetAllAlertsAsync()
    {
        return await _context.Alerts
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Alert?> GetAlertByIdAsync(int id)
    {
        return await _context.Alerts.FindAsync(id);
    }

    public async Task UpdateAlertAsync(Alert alert)
    {
        _context.Alerts.Update(alert);
        await _context.SaveChangesAsync();
    }
}
