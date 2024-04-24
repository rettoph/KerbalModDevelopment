using System.Text.RegularExpressions;

namespace KerbalModDevelopment.Services
{
    internal class EnvironmentService
    {
        private const string _pattern = @"\[(.*)\]";

        private readonly SettingService _settings;
        public readonly Dictionary<string, string> Environment;

        public EnvironmentService(SettingService settings)
        {
            _settings = settings;

            this.Environment = settings.Get<Dictionary<string, string>>(nameof(this.Environment));
        }

        public void Apply()
        {
            foreach ((string key, string rawValue) in this.Environment)
            {
                string value = rawValue;
                Match match = Regex.Match(value, _pattern);
                if (match.Success)
                {
                    value = match.Groups[1].Value;
                    value = _settings.Get(value);
                }

                System.Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.User);
            }
        }
    }
}
