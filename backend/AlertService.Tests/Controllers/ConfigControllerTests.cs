using AlertService.Api.Controllers;
using AlertService.Api.DTOs;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AlertService.Tests.Controllers;

public class ConfigControllerTests
{
    private readonly Mock<IConfigRepository> _mockConfigRepository;
    private readonly Mock<ILogger<ConfigController>> _mockLogger;
    private readonly ConfigController _controller;

    public ConfigControllerTests()
    {
        _mockConfigRepository = new Mock<IConfigRepository>();
        _mockLogger = new Mock<ILogger<ConfigController>>();
        _controller = new ConfigController(_mockConfigRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetConfig_ReturnsOkWithConfig()
    {
        // Arrange
        var config = new Config
        {
            Id = 1,
            TempMax = 50.0,
            HumidityMax = 70.0,
            UpdatedAt = DateTime.UtcNow
        };
        _mockConfigRepository.Setup(x => x.GetConfigAsync()).ReturnsAsync(config);

        // Act
        var result = await _controller.GetConfig();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var configDto = Assert.IsType<ConfigDto>(okResult.Value);
        Assert.Equal(50.0, configDto.TempMax);
        Assert.Equal(70.0, configDto.HumidityMax);
    }

    [Fact]
    public async Task GetConfig_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockConfigRepository.Setup(x => x.GetConfigAsync()).ReturnsAsync((Config?)null);

        // Act
        var result = await _controller.GetConfig();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateConfig_WithNegativeTempMax_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateConfigDto
        {
            TempMax = -10.0,
            HumidityMax = 70.0
        };

        // Act
        var result = await _controller.UpdateConfig(updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateConfig_WithInvalidHumidity_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateConfigDto
        {
            TempMax = 50.0,
            HumidityMax = 150.0 // Mayor a 100
        };

        // Act
        var result = await _controller.UpdateConfig(updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateConfig_WithZeroHumidity_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateConfigDto
        {
            TempMax = 50.0,
            HumidityMax = 0.0
        };

        // Act
        var result = await _controller.UpdateConfig(updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
