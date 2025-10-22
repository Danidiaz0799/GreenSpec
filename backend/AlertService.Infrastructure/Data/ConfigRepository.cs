using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Infrastructure.Data;

public class ConfigRepository : IConfigRepository
{
    private readonly AppDbContext _context;

    public ConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Config?> GetConfigAsync()
    {
        // Devuelve la única fila de configuración (la primera si hay varias)
        return await _context.Configs.FirstOrDefaultAsync();
    }

    public async Task UpdateConfigAsync(Config config)
    {
        // Obtener la configuración existente
        var existingConfig = await _context.Configs.FirstOrDefaultAsync();

        if (existingConfig == null)
        {
            throw new InvalidOperationException("No existe una configuración en la base de datos. Ejecute las migraciones primero.");
        }

        // Actualizar solo los campos permitidos
        existingConfig.TempMax = config.TempMax;
        existingConfig.HumidityMax = config.HumidityMax;
        existingConfig.UpdatedAt = DateTime.UtcNow;

        // EF Core detectará los cambios automáticamente
        await _context.SaveChangesAsync();
    }
}
