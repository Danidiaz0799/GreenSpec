namespace AlertService.Domain.Entities;

public class Config
{
    public int Id { get; set; }
    public double TempMax { get; set; }
    public double HumidityMax { get; set; }
    public DateTime UpdatedAt { get; set; }
}
