using AlertService.Api.Controllers;
using AlertService.Api.DTOs;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AlertService.Tests.Controllers;

public class AuthControllerTests
{
    private AuthController CreateController()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"JwtSettings:SecretKey", "SuperSecretKeyForJWTTokenGeneration12345678"},
            {"JwtSettings:Issuer", "AlertServiceApi"},
            {"JwtSettings:Audience", "AlertServiceClient"},
            {"JwtSettings:ExpirationMinutes", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var mockLogger = new Mock<ILogger<AuthController>>();
        return new AuthController(configuration, mockLogger.Object);
    }

    [Fact]
    public void Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var controller = CreateController();
        var request = new LoginRequestDto
        {
            Username = "demo",
            Password = "demo"
        };

        // Act
        var result = controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponseDto>(okResult.Value);
        Assert.NotNull(response.Token);
        Assert.NotEmpty(response.Token);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var controller = CreateController();
        var request = new LoginRequestDto
        {
            Username = "wrong",
            Password = "wrong"
        };

        // Act
        var result = controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public void Login_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new LoginRequestDto
        {
            Username = "",
            Password = "demo"
        };

        // Act
        var result = controller.Login(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new LoginRequestDto
        {
            Username = "demo",
            Password = ""
        };

        // Act
        var result = controller.Login(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_WithNullCredentials_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var request = new LoginRequestDto
        {
            Username = null!,
            Password = null!
        };

        // Act
        var result = controller.Login(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
