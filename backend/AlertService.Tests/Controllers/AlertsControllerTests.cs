using AlertService.Api.Controllers;
using AlertService.Api.DTOs;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AlertService.Tests.Controllers;

public class AlertsControllerTests
{
    private readonly Mock<IAlertRepository> _mockAlertRepository;
    private readonly Mock<ILogger<AlertsController>> _mockLogger;
    private readonly AlertsController _controller;

    public AlertsControllerTests()
    {
        _mockAlertRepository = new Mock<IAlertRepository>();
        _mockLogger = new Mock<ILogger<AlertsController>>();
        _controller = new AlertsController(_mockAlertRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAlerts_ReturnsOkWithAlerts()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            new Alert { Id = 1, Type = AlertType.Temperature, Value = 55.0, Threshold = 50.0, Status = AlertStatus.Open, CreatedAt = DateTime.UtcNow },
            new Alert { Id = 2, Type = AlertType.Humidity, Value = 80.0, Threshold = 70.0, Status = AlertStatus.Acknowledged, CreatedAt = DateTime.UtcNow }
        };
        _mockAlertRepository.Setup(x => x.GetAllAlertsAsync()).ReturnsAsync(alerts);

        // Act
        var result = await _controller.GetAllAlerts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var alertDtos = Assert.IsType<List<AlertDto>>(okResult.Value);
        Assert.Equal(2, alertDtos.Count);
    }

    [Fact]
    public async Task GetAlertById_WithValidId_ReturnsOk()
    {
        // Arrange
        var alert = new Alert
        {
            Id = 1,
            Type = AlertType.Temperature,
            Value = 55.0,
            Threshold = 50.0,
            Status = AlertStatus.Open,
            CreatedAt = DateTime.UtcNow
        };
        _mockAlertRepository.Setup(x => x.GetAlertByIdAsync(1)).ReturnsAsync(alert);

        // Act
        var result = await _controller.GetAlertById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var alertDto = Assert.IsType<AlertDto>(okResult.Value);
        Assert.Equal(1, alertDto.Id);
        Assert.Equal(AlertType.Temperature, alertDto.Type);
    }

    [Fact]
    public async Task GetAlertById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockAlertRepository.Setup(x => x.GetAlertByIdAsync(999)).ReturnsAsync((Alert?)null);

        // Act
        var result = await _controller.GetAlertById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AcknowledgeAlert_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockAlertRepository.Setup(x => x.GetAlertByIdAsync(999)).ReturnsAsync((Alert?)null);

        // Act
        var result = await _controller.AcknowledgeAlert(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AcknowledgeAlert_AlreadyAcknowledged_ReturnsBadRequest()
    {
        // Arrange
        var alert = new Alert
        {
            Id = 1,
            Type = AlertType.Temperature,
            Value = 55.0,
            Threshold = 50.0,
            Status = AlertStatus.Acknowledged, // Ya reconocida
            CreatedAt = DateTime.UtcNow
        };
        _mockAlertRepository.Setup(x => x.GetAlertByIdAsync(1)).ReturnsAsync(alert);

        // Act
        var result = await _controller.AcknowledgeAlert(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
