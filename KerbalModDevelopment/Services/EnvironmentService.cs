namespace KerbalModDevelopment.Services
{
    internal class EnvironmentService
    {
        private readonly SettingService _settings;
        public readonly Dictionary<string, string> Environment;

        public EnvironmentService(SettingService settings)
        {
            _settings = settings;

            this.Environment = settings.Get<Dictionary<string, string>>(nameof(this.Environment));
        }

        public void Apply()
        {
            foreach ((string key, string value) in this.Environment)
            {
                System.Environment.SetEnvironmentVariable(key, _settings.Resolve(value), EnvironmentVariableTarget.User);
            }
        }
    }
}
