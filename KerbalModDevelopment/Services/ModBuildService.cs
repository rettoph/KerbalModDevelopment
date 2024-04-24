using KerbalModDevelopment.Models;
using Serilog;
using System.Diagnostics;

namespace KerbalModDevelopment.Services
{
    internal class ModBuildService
    {
        private readonly EnvironmentService _environment;
        private readonly ILogger _logger;

        public readonly ModConfiguration[] Mods;
        public readonly string DevelopmentDirectory;
        public readonly string SolutionDirectory;

        public ModBuildService(SettingService settings, EnvironmentService environment, ILogger logger)
        {
            _logger = logger;
            _environment = environment;
            this.Mods = settings.Get<ModConfiguration[]>(nameof(Mods));
            this.DevelopmentDirectory = settings.Get(nameof(DevelopmentDirectory));
            this.SolutionDirectory = settings.Get(nameof(SolutionDirectory));
        }

        public bool BuildAll()
        {
            try
            {
                _environment.Apply();

                foreach (ModConfiguration mod in this.Mods)
                {
                    if (mod.Enabled == false)
                    {
                        continue;
                    }

                    if (this.Build(mod) == false)
                    {
                        return false;
                    }
                }

                return true;
            }
            finally
            {

            }
        }

        private bool Build(ModConfiguration mod)
        {
            _logger.Information("Building {ModName}", mod.Name);

            string deployTargetPath = Path.GetFullPath(Path.Combine(this.SolutionDirectory, mod.Source));
            string deployTargetOutput = Path.GetFullPath(Path.Combine(this.DevelopmentDirectory, Constants.GameData, mod.Name));

            string cmd = $"build \"{deployTargetPath}\" -o \"{deployTargetOutput}\" -r any -c Debug /p:Platform=AnyCPU";
            _logger.Verbose("Running {cmd}", cmd);

            try
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "dotnet",
                        Arguments = cmd,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };

                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data is null)
                    {
                        return;
                    }

                    _logger.Verbose(e.Data);
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data is null)
                    {
                        return;
                    }

                    _logger.Error(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error building {ModName}", mod.Name);
                return false;
            }
        }
    }
}
