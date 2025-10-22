using AlertService.Api.DTOs;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlertService.Api.Controllers;

[ApiController]
[Route("config")]
[Authorize] // Todo el controlador requiere autenticación
public class ConfigController : ControllerBase
{
    private readonly IConfigRepository _configRepository;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IConfigRepository configRepository, ILogger<ConfigController> logger)
    {
        _configRepository = configRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la configuración actual de umbrales
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetConfig()
    {
        try
        {
            var config = await _configRepository.GetConfigAsync();

            if (config == null)
            {
                _logger.LogWarning("No se encontró configuración en la base de datos");
                return NotFound(new { message = "No se encontró configuración. Ejecute las migraciones primero." });
            }

            var configDto = new ConfigDto
            {
                Id = config.Id,
                TempMax = config.TempMax,
                HumidityMax = config.HumidityMax,
                UpdatedAt = config.UpdatedAt
            };

            _logger.LogInformation("Configuración obtenida: TempMax={TempMax}, HumidityMax={HumidityMax}", 
                config.TempMax, config.HumidityMax);

            return Ok(configDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la configuración");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza los umbrales de temperatura y humedad
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateConfig([FromBody] UpdateConfigDto updateDto)
    {
        try
        {
            // Validaciones básicas
            if (updateDto.TempMax <= 0)
            {
                return BadRequest(new { message = "TempMax debe ser mayor a 0" });
            }

            if (updateDto.HumidityMax <= 0 || updateDto.HumidityMax > 100)
            {
                return BadRequest(new { message = "HumidityMax debe estar entre 0 y 100" });
            }

            // Obtener la configuración existente
            var existingConfig = await _configRepository.GetConfigAsync();

            if (existingConfig == null)
            {
                _logger.LogWarning("No se encontró configuración para actualizar");
                return NotFound(new { message = "No se encontró configuración. Ejecute las migraciones primero." });
            }

            // Crear objeto Config con los nuevos valores
            var configToUpdate = new Config
            {
                Id = existingConfig.Id,
                TempMax = updateDto.TempMax,
                HumidityMax = updateDto.HumidityMax
            };

            // Actualizar
            await _configRepository.UpdateConfigAsync(configToUpdate);

            _logger.LogInformation("Configuración actualizada por usuario {User}: TempMax={TempMax}, HumidityMax={HumidityMax}",
                User.Identity?.Name, updateDto.TempMax, updateDto.HumidityMax);

            // Obtener la configuración actualizada para devolverla
            var updatedConfig = await _configRepository.GetConfigAsync();

            var configDto = new ConfigDto
            {
                Id = updatedConfig!.Id,
                TempMax = updatedConfig.TempMax,
                HumidityMax = updatedConfig.HumidityMax,
                UpdatedAt = updatedConfig.UpdatedAt
            };

            return Ok(configDto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error de operación al actualizar configuración");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la configuración");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
