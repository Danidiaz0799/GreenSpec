namespace AlertService.Api.DTOs;

public class AlertDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public double Value { get; set; }
    public double Threshold { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateAlertStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class AlertsFilterDto
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
