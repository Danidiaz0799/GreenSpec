using AlertService.Domain.Entities;
using AlertService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlertService.Tests.Repositories;

public class AlertRepositoryTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAlertAsync_AddsAlertToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new AlertRepository(context);

        var alert = new Alert
        {
            Type = AlertType.Temperature,
            Value = 55.0,
            Threshold = 50.0,
            Status = AlertStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAlertAsync(alert);

        // Assert
        var alerts = await repository.GetAllAlertsAsync();
        Assert.Single(alerts);
        Assert.Equal(AlertType.Temperature, alerts[0].Type);
    }

    [Fact]
    public async Task GetAllAlertsAsync_ReturnsOrderedByDateDescending()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new AlertRepository(context);

        var alert1 = new Alert
        {
            Type = AlertType.Temperature,
            Value = 55.0,
            Threshold = 50.0,
            Status = AlertStatus.Open,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        var alert2 = new Alert
        {
            Type = AlertType.Humidity,
            Value = 80.0,
            Threshold = 70.0,
            Status = AlertStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        context.Alerts.AddRange(alert1, alert2);
        await context.SaveChangesAsync();

        // Act
        var alerts = await repository.GetAllAlertsAsync();

        // Assert
        Assert.Equal(2, alerts.Count);
        Assert.Equal(AlertType.Humidity, alerts[0].Type); // Más reciente primero
        Assert.Equal(AlertType.Temperature, alerts[1].Type);
    }

    [Fact]
    public async Task GetAlertByIdAsync_ReturnsCorrectAlert()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new AlertRepository(context);

        var alert = new Alert
        {
            Type = AlertType.Temperature,
            Value = 55.0,
            Threshold = 50.0,
            Status = AlertStatus.Open,
            CreatedAt = DateTime.UtcNow
        };
        context.Alerts.Add(alert);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAlertByIdAsync(alert.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(alert.Id, result.Id);
        Assert.Equal(AlertType.Temperature, result.Type);
    }

    [Fact]
    public async Task GetAlertByIdAsync_ReturnsNullForInvalidId()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new AlertRepository(context);

        // Act
        var result = await repository.GetAlertByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAlertAsync_UpdatesAlertStatus()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new AlertRepository(context);

        var alert = new Alert
        {
            Type = AlertType.Temperature,
            Value = 55.0,
            Threshold = 50.0,
            Status = AlertStatus.Open,
            CreatedAt = DateTime.UtcNow
        };
        context.Alerts.Add(alert);
        await context.SaveChangesAsync();

        // Act
        alert.Status = AlertStatus.Acknowledged;
        await repository.UpdateAlertAsync(alert);

        // Assert
        var updated = await repository.GetAlertByIdAsync(alert.Id);
        Assert.NotNull(updated);
        Assert.Equal(AlertStatus.Acknowledged, updated.Status);
    }
}
