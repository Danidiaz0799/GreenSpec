using AlertService.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AlertService.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username y Password son requeridos" });
        }

        if (request.Username != "demo" || request.Password != "demo")
        {
            _logger.LogWarning("Intento de login fallido para usuario: {Username}", request.Username);
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        var token = GenerateJwtToken(request.Username);
        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes");
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        _logger.LogInformation("Usuario {Username} autenticado exitosamente", request.Username);

        return Ok(new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt
        });
    }

    private string GenerateJwtToken(string username)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]!;
        var issuer = _configuration["JwtSettings:Issuer"]!;
        var audience = _configuration["JwtSettings:Audience"]!;
        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserId", "1"), // ID de usuario de demostración
            new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
