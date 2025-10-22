using AlertService.Api.DTOs;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlertService.Api.Controllers;

[ApiController]
[Route("alerts")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IAlertRepository _alertRepository;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(IAlertRepository alertRepository, ILogger<AlertsController> logger)
    {
        _alertRepository = alertRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las alertas ordenadas por fecha de creación (más recientes primero)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllAlerts()
    {
        try
        {
            var alerts = await _alertRepository.GetAllAlertsAsync();

            var alertDtos = alerts.Select(a => new AlertDto
            {
                Id = a.Id,
                Type = a.Type,
                Value = a.Value,
                Threshold = a.Threshold,
                CreatedAt = a.CreatedAt,
                Status = a.Status
            }).ToList();

            _logger.LogInformation("Se obtuvieron {Count} alertas", alertDtos.Count);

            return Ok(alertDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las alertas");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una alerta específica por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlertById(int id)
    {
        try
        {
            var alert = await _alertRepository.GetAlertByIdAsync(id);

            if (alert == null)
            {
                return NotFound(new { message = $"Alerta con ID {id} no encontrada" });
            }

            var alertDto = new AlertDto
            {
                Id = alert.Id,
                Type = alert.Type,
                Value = alert.Value,
                Threshold = alert.Threshold,
                CreatedAt = alert.CreatedAt,
                Status = alert.Status
            };

            return Ok(alertDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la alerta {AlertId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza el estado de una alerta (por ejemplo, de "open" a "ack")
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateAlertStatus(int id, [FromBody] UpdateAlertStatusDto updateDto)
    {
        try
        {
            // Validar que el status es válido
            if (updateDto.Status != AlertStatus.Open && updateDto.Status != AlertStatus.Acknowledged)
            {
                return BadRequest(new 
                { 
                    message = $"Estado inválido. Valores permitidos: '{AlertStatus.Open}' o '{AlertStatus.Acknowledged}'" 
                });
            }

            var alert = await _alertRepository.GetAlertByIdAsync(id);

            if (alert == null)
            {
                return NotFound(new { message = $"Alerta con ID {id} no encontrada" });
            }

            // Actualizar el estado
            alert.Status = updateDto.Status;
            await _alertRepository.UpdateAlertAsync(alert);

            _logger.LogInformation(
                "Usuario {User} actualizó el estado de la alerta {AlertId} a '{Status}'",
                User.Identity?.Name, id, updateDto.Status);

            var alertDto = new AlertDto
            {
                Id = alert.Id,
                Type = alert.Type,
                Value = alert.Value,
                Threshold = alert.Threshold,
                CreatedAt = alert.CreatedAt,
                Status = alert.Status
            };

            return Ok(alertDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado de la alerta {AlertId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Reconoce (acknowledges) una alerta cambiando su estado a "ack"
    /// </summary>
    [HttpPost("{id}/acknowledge")]
    public async Task<IActionResult> AcknowledgeAlert(int id)
    {
        try
        {
            var alert = await _alertRepository.GetAlertByIdAsync(id);

            if (alert == null)
            {
                return NotFound(new { message = $"Alerta con ID {id} no encontrada" });
            }

            if (alert.Status == AlertStatus.Acknowledged)
            {
                return BadRequest(new { message = "La alerta ya ha sido reconocida" });
            }

            alert.Status = AlertStatus.Acknowledged;
            await _alertRepository.UpdateAlertAsync(alert);

            _logger.LogInformation(
                "Usuario {User} reconoció la alerta {AlertId} (Tipo: {Type}, Valor: {Value})",
                User.Identity?.Name, id, alert.Type, alert.Value);

            var alertDto = new AlertDto
            {
                Id = alert.Id,
                Type = alert.Type,
                Value = alert.Value,
                Threshold = alert.Threshold,
                CreatedAt = alert.CreatedAt,
                Status = alert.Status
            };

            return Ok(alertDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reconocer la alerta {AlertId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
