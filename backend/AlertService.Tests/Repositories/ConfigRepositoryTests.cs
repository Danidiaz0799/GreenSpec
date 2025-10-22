using AlertService.Domain.Entities;
using AlertService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlertService.Tests.Repositories;

public class ConfigRepositoryTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetConfigAsync_ReturnsFirstConfig()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ConfigRepository(context);

        var config = new Config
        {
            TempMax = 50.0,
            HumidityMax = 70.0,
            UpdatedAt = DateTime.UtcNow
        };
        context.Configs.Add(config);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetConfigAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(50.0, result.TempMax);
        Assert.Equal(70.0, result.HumidityMax);
    }

    [Fact]
    public async Task UpdateConfigAsync_UpdatesValues()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ConfigRepository(context);

        var config = new Config
        {
            TempMax = 50.0,
            HumidityMax = 70.0,
            UpdatedAt = DateTime.UtcNow
        };
        context.Configs.Add(config);
        await context.SaveChangesAsync();

        // Act
        config.TempMax = 60.0;
        config.HumidityMax = 80.0;
        await repository.UpdateConfigAsync(config);

        // Assert
        var updated = await repository.GetConfigAsync();
        Assert.NotNull(updated);
        Assert.Equal(60.0, updated.TempMax);
        Assert.Equal(80.0, updated.HumidityMax);
    }

    [Fact]
    public async Task UpdateConfigAsync_UpdatesTimestamp()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ConfigRepository(context);

        var originalTime = DateTime.UtcNow.AddHours(-1);
        var config = new Config
        {
            TempMax = 50.0,
            HumidityMax = 70.0,
            UpdatedAt = originalTime
        };
        context.Configs.Add(config);
        await context.SaveChangesAsync();

        // Act
        await Task.Delay(100); // Pequeño delay para asegurar diferencia de tiempo
        config.TempMax = 60.0;
        await repository.UpdateConfigAsync(config);

        // Assert
        var updated = await repository.GetConfigAsync();
        Assert.NotNull(updated);
        Assert.True(updated.UpdatedAt > originalTime);
    }
}
