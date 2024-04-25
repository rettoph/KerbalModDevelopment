using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace KerbalModDevelopment.Services
{
    public sealed class SettingService
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

        private const string _pattern = @"\[(.*)\]";
        public string Resolve(string value)
        {
            Match match;
            while ((match = Regex.Match(value, _pattern)) is not null)
            {
                if (match.Success == false)
                {
                    break;
                }

                value = value.Replace(match.Value, this.Get(match.Groups[1].Value));
            }

            return value;
        }
    }
}
