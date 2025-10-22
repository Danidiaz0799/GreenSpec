using AlertService.Domain.Entities;
using Xunit;

namespace AlertService.Tests.Entities;

public class ConfigEntityTests
{
    [Fact]
    public void Config_CanSetAllProperties()
    {
        // Arrange
        var updatedAt = DateTime.UtcNow;

        // Act
        var config = new Config
        {
            Id = 1,
            TempMax = 50.0,
            HumidityMax = 70.0,
            UpdatedAt = updatedAt
        };

        // Assert
        Assert.Equal(1, config.Id);
        Assert.Equal(50.0, config.TempMax);
        Assert.Equal(70.0, config.HumidityMax);
        Assert.Equal(updatedAt, config.UpdatedAt);
    }

    [Theory]
    [InlineData(30.0, 60.0)]
    [InlineData(50.0, 70.0)]
    [InlineData(100.0, 100.0)]
    public void Config_AcceptsValidThresholds(double tempMax, double humidityMax)
    {
        // Act
        var config = new Config
        {
            TempMax = tempMax,
            HumidityMax = humidityMax
        };

        // Assert
        Assert.Equal(tempMax, config.TempMax);
        Assert.Equal(humidityMax, config.HumidityMax);
    }
}
