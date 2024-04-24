using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KerbalModDevelopment.Services
{
    internal sealed class SettingService
    {
        private IConfiguration _configuration;

        public string SteamDirectory

        public SettingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Get<T>(string key)
        {
            T? result = _configuration.GetSection(key).Get<T>();
            result ??= JsonSerializer.Deserialize<T>(Environment.GetEnvironmentVariable(key) ?? string.Empty);


            return result ?? throw new KeyNotFoundException(key);
        }
    }
}
