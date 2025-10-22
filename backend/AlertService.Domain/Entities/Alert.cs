namespace AlertService.Domain.Entities;

public class Alert
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public double Value { get; set; }
    public double Threshold { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "open"; // "open" o "ack"
}

// Constantes para los estados y tipos de alerta
public static class AlertStatus
{
    public const string Open = "open";
    public const string Acknowledged = "ack";
}

public static class AlertType
{
    public const string Temperature = "Temperature";
    public const string Humidity = "Humidity";
}
