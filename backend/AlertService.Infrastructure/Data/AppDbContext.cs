using AlertService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Config> Configs { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Config
        modelBuilder.Entity<Config>(entity =>
        {
            entity.ToTable("Configs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TempMax).IsRequired();
            entity.Property(e => e.HumidityMax).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Configuración de Alert
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("Alerts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Threshold).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
        });
    }
}
