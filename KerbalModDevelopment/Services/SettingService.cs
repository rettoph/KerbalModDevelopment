using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace KerbalModDevelopment.Services
{
    internal sealed class SettingService
    {
        private IConfiguration _configuration;

        public SettingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Get(string key)
        {
            string? result = _configuration.GetSection(key).Value;
            result ??= Environment.GetEnvironmentVariable(key);

            return result ?? throw new KeyNotFoundException(key);
        }

        public T Get<T>(string key)
        {
            T? result = _configuration.GetSection(key).Get<T>();
            result ??= JsonSerializer.Deserialize<T>(Environment.GetEnvironmentVariable(key) ?? string.Empty);


            return result ?? throw new KeyNotFoundException(key);
        }
    }
}
