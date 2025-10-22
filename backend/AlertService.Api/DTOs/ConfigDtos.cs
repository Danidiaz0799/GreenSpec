namespace AlertService.Api.DTOs;

public class ConfigDto
{
    public int Id { get; set; }
    public double TempMax { get; set; }
    public double HumidityMax { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateConfigDto
{
    public double TempMax { get; set; }
    public double HumidityMax { get; set; }
}
