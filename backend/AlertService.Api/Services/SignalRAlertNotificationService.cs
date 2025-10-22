using AlertService.Api.Hubs;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AlertService.Api.Services;

public class SignalRAlertNotificationService : IAlertNotificationService
{
    private readonly IHubContext<AlertsHub> _hubContext;
    private readonly ILogger<SignalRAlertNotificationService> _logger;

    public SignalRAlertNotificationService(
        IHubContext<AlertsHub> hubContext,
        ILogger<SignalRAlertNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyNewAlertAsync(Alert alert)
    {
        try
        {
            // Enviar la nueva alerta a todos los clientes conectados
            await _hubContext.Clients.All.SendAsync("ReceiveNewAlert", alert);
            
            _logger.LogInformation(
                "📡 Alerta enviada por SignalR - ID: {AlertId}, Tipo: {AlertType}, Valor: {Value:F2}",
                alert.Id, alert.Type, alert.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar alerta por SignalR");
        }
    }
}
