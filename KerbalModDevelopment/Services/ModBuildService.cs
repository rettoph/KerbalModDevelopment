using KerbalModDevelopment.Models;
using Serilog;
using System.Diagnostics;

namespace KerbalModDevelopment.Services
{
    internal class ModBuildService
    {
        private readonly SettingService _settings;
        private readonly EnvironmentService _environment;
        private readonly ILogger _logger;
        private readonly DirectoryService _directory;

        public readonly ModConfiguration[] Mods;

        public ModBuildService(SettingService settings, EnvironmentService environment, DirectoryService directory, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
            _environment = environment;
            _directory = directory;
            this.Mods = settings.Get<ModConfiguration[]>(nameof(Mods));
        }

        public bool BuildAll()
        {
            try
            {
                _environment.Apply();

                foreach (ModConfiguration mod in this.Mods)
                {
                    if (mod.Build == false)
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

            try
            {
                if (this.TryBuild(mod, out string bin) == false)
                {
                    return false;
                }

                if (mod.Deploy && this.TryDeploy(bin, mod) == false)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error building {ModName}", mod.Name);
                return false;
            }
        }

        private bool TryBuild(ModConfiguration mod, out string output)
        {
            string cmd = this.BuildBuildCommand(mod, out output);

            try
            {
                _logger.Verbose("Running {cmd}", cmd);

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
                _logger.Error(ex, "Unexpected error building {input}", mod.Name);
                return false;
            }
        }

        private bool TryDeploy(string bin, ModConfiguration mod)
        {
            string target = Path.Combine(_directory.DevelopmentDirectory, Constants.GameData, mod.Name);

            try
            {
                if (Directory.Exists(target))
                {
                    Directory.Delete(target);
                }

                Directory.CreateSymbolicLink(target, bin);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error creating symlink to {target}", target);
                return false;
            }
        }

        private string BuildBuildCommand(ModConfiguration mod, out string output)
        {
            string input = Path.Combine(_directory.SolutionDirectory, mod.Project);
            output = Path.Combine(_directory.SolutionDirectory, "out", mod.Name);

            string cmd = $"build \"{input}\" -o \"{output}\" -r any -c Debug /p:Platform=AnyCPU";

            foreach ((string key, string value) in mod.BuildProperties)
            {
                cmd += $" /p:{key}=\"{_settings.Resolve(value)}\"";
            }

            if (string.IsNullOrEmpty(mod.Solution) == false)
            {
                cmd += $" /p:SolutionDir=\"{Path.GetDirectoryName(_directory.GetFullPath(mod.Solution))}\"";
            }


            return cmd;
        }
    }
}
