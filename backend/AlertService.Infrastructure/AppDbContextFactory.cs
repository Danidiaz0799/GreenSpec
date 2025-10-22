using AlertService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AlertService.Infrastructure;

// Esta clase se usa solo para las migraciones de EF Core
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Cadena de conexión temporal para desarrollo
        // En producción, esto vendrá de appsettings.json
        optionsBuilder.UseNpgsql("Host=localhost;Database=alertservice;Username=postgres;Password=postgres");

        return new AppDbContext(optionsBuilder.Options);
    }
}
