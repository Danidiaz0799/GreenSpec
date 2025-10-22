using AlertService.Domain.Entities;

namespace AlertService.Domain.Interfaces;

public interface IConfigRepository
{
    Task<Config?> GetConfigAsync();
    Task UpdateConfigAsync(Config config);
}
