using Serilog;
using System.Diagnostics;

namespace KerbalModDevelopment.Services
{
    internal class LaunchService
    {
        private readonly GameInstallationService _install;
        private readonly ModBuildService _builder;
        private readonly DirectoryService _directory;
        private readonly ILogger _logger;

        public LaunchService(GameInstallationService gameInstallation, ModBuildService builder, SettingService settings, DirectoryService directory, ILogger logger)
        {
            _install = gameInstallation;
            _builder = builder;
            _directory = directory;
            _logger = logger;
        }

        public void TryLaunch()
        {
            try
            {
                this.KillRunningInstances();

                if (_install.Verify(false) == false)
                {
                    _logger.Error($"Error verifying game installation...");
                    return;
                }


                if (_builder.BuildAll() == false)
                {
                    _logger.Error($"Error building mods...");
                    return;
                }

                string exe = Path.Combine(_directory.DevelopmentDirectory, Constants.KSP_x64_Dbg_Exe);
                Process.Start(exe);

                Console.WriteLine("KSP has been started. To debug open Debug > Attach Unity Debugger");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected Error launching KSP...");
            }
        }

        private void KillRunningInstances()
        {
            var processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (process.MainModule is null)
                    {
                        continue;
                    }

                    if (process.MainModule.FileName.Contains(Constants.KSP_x64_Dbg_Exe, StringComparison.OrdinalIgnoreCase))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    //
                }
            };
        }
    }
}
