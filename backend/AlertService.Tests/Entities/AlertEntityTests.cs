using AlertService.Domain.Entities;
using Xunit;

namespace AlertService.Tests.Entities;

public class AlertEntityTests
{
    [Fact]
    public void Alert_DefaultStatus_IsOpen()
    {
        // Arrange & Act
        var alert = new Alert();

        // Assert
        Assert.Equal("open", alert.Status);
    }

    [Fact]
    public void Alert_CanSetAllProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;

        // Act
        var alert = new Alert
        {
            Id = 1,
            Type = AlertType.Temperature,
            Value = 55.5,
            Threshold = 50.0,
            CreatedAt = createdAt,
            Status = AlertStatus.Acknowledged
        };

        // Assert
        Assert.Equal(1, alert.Id);
        Assert.Equal(AlertType.Temperature, alert.Type);
        Assert.Equal(55.5, alert.Value);
        Assert.Equal(50.0, alert.Threshold);
        Assert.Equal(createdAt, alert.CreatedAt);
        Assert.Equal(AlertStatus.Acknowledged, alert.Status);
    }

    [Theory]
    [InlineData("Temperature")]
    [InlineData("Humidity")]
    public void AlertType_HasValidConstants(string expectedType)
    {
        // Assert
        Assert.True(expectedType == AlertType.Temperature || expectedType == AlertType.Humidity);
    }

    [Theory]
    [InlineData("open")]
    [InlineData("ack")]
    public void AlertStatus_HasValidConstants(string expectedStatus)
    {
        // Assert
        Assert.True(expectedStatus == AlertStatus.Open || expectedStatus == AlertStatus.Acknowledged);
    }
}
