using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlertService.Infrastructure.Services;

public class SensorSimulationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SensorSimulationService> _logger;
    private readonly IAlertNotificationService? _notificationService;
    private readonly Random _random;

    private const int SimulationIntervalMs = 4000;
    private const double TempMin = 30.0;
    private const double TempMax = 70.0;
    private const double HumidityMin = 50.0;
    private const double HumidityMax = 100.0;

    public SensorSimulationService(
        IServiceProvider serviceProvider,
        ILogger<SensorSimulationService> logger,
        IAlertNotificationService? notificationService = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _notificationService = notificationService;
        _random = new Random();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Servicio de Simulación de Sensores iniciado");

        await Task.Delay(2000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SimulateSensorReadingsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en el ciclo de simulación de sensores");
            }

            await Task.Delay(SimulationIntervalMs, stoppingToken);
        }

        _logger.LogInformation("🛑 Servicio de Simulación de Sensores detenido");
    }

    private async Task SimulateSensorReadingsAsync(CancellationToken cancellationToken)
    {
        // BackgroundService es Singleton, pero repositorios son Scoped
        // Creamos un scope manual para acceder a servicios Scoped
        using var scope = _serviceProvider.CreateScope();
        
        var configRepository = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();

        var config = await configRepository.GetConfigAsync();

        if (config == null)
        {
            _logger.LogWarning("⚠️ No se encontró configuración. Omitiendo simulación.");
            return;
        }

        var temperature = GenerateRandomValue(TempMin, TempMax);
        var humidity = GenerateRandomValue(HumidityMin, HumidityMax);

        _logger.LogInformation(
            "📊 Lecturas de sensores - Temperatura: {Temperature:F2}°C (Umbral: {TempThreshold:F2}°C), Humedad: {Humidity:F2}% (Umbral: {HumidityThreshold:F2}%)",
            temperature, config.TempMax, humidity, config.HumidityMax);

        if (temperature > config.TempMax)
        {
            await CreateAndSaveAlertAsync(
                alertRepository,
                AlertType.Temperature,
                temperature,
                config.TempMax,
                $"Temperatura excedió el umbral: {temperature:F2}°C > {config.TempMax:F2}°C"
            );
        }

        if (humidity > config.HumidityMax)
        {
            await CreateAndSaveAlertAsync(
                alertRepository,
                AlertType.Humidity,
                humidity,
                config.HumidityMax,
                $"Humedad excedió el umbral: {humidity:F2}% > {config.HumidityMax:F2}%"
            );
        }
    }

    private async Task CreateAndSaveAlertAsync(
        IAlertRepository alertRepository,
        string alertType,
        double value,
        double threshold,
        string logMessage)
    {
        var alert = new Alert
        {
            Type = alertType,
            Value = value,
            Threshold = threshold,
            CreatedAt = DateTime.UtcNow,
            Status = AlertStatus.Open
        };

        await alertRepository.AddAlertAsync(alert);

        _logger.LogWarning(
            "🚨 ALERTA GENERADA - Tipo: {AlertType}, Valor: {Value:F2}, Umbral: {Threshold:F2}, Estado: {Status}",
            alertType, value, threshold, AlertStatus.Open);

        _logger.LogInformation("💾 Alerta guardada en la base de datos: {Message}", logMessage);

        if (_notificationService != null)
        {
            await _notificationService.NotifyNewAlertAsync(alert);
        }
    }

    private double GenerateRandomValue(double min, double max)
    {
        return min + (_random.NextDouble() * (max - min));
    }
}
