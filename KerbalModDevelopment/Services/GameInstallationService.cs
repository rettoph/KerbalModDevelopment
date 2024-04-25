using Serilog;

namespace KerbalModDevelopment.Services
{
    internal class GameInstallationService
    {
        private static readonly Dictionary<string, string> UnityFiles = new()
        {
            { "WindowsPlayer.exe", Constants.KSP_x64_Dbg_Exe },
            { "UnityPlayer.dll", "UnityPlayer.dll" },
            { "WinPixEventRuntime.dll", "WinPixEventRuntime.dll" },
        };

        private readonly ILogger _logger;
        private readonly DirectoryService _directory;

        public GameInstallationService(SettingService settings, DirectoryService directory, ILogger logger)
        {
            _logger = logger;
            _directory = directory;
        }

        public bool Verify(bool force)
        {
            try
            {
                if (force == true && this.DeleteExistingInstallationIfExists() == false)
                { // Something went wrong attempting to delete the pre-existing installation
                    return false;
                }

                if (force == false && _directory.Exists(_directory.DevelopmentDirectory))
                { // Install exists and is not forced
                    return true;
                }

                if (this.CopySteamDirectoryToDevelopmentDirectory() == false)
                { // Error copying steam files
                    return false;
                }

                if (this.CopyUnityDebugFilesToDevelopmentDirectory() == false)
                { // Error configuring unity...
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GameInstallationService)}::{Verify} - Error attempting to verify installation.");
                return false;
            }
        }

        private bool DeleteExistingInstallationIfExists()
        {
            if (Directory.Exists(_directory.DevelopmentDirectory) == false)
            {
                return true;
            }

            _logger.Information($"{nameof(GameInstallationService)}::{nameof(DeleteExistingInstallationIfExists)} - Deleting Existing Development Install...");
            try
            {
                Directory.Delete(_directory.DevelopmentDirectory, true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GameInstallationService)}::{nameof(DeleteExistingInstallationIfExists)} - Error attempting to delete directory.");
                return false;
            }
        }

        private bool CopySteamDirectoryToDevelopmentDirectory()
        {
            _logger.Information($"{nameof(GameInstallationService)}::{nameof(CopySteamDirectoryToDevelopmentDirectory)} - Copying Steam files...");

            try
            {
                _directory.CreateDirectory(_directory.DevelopmentDirectory);

                if (_directory.Exists(_directory.SteamDirectory) == false)
                {
                    return false;
                }

                _directory.CopyRecursive(_directory.SteamDirectory, _directory.DevelopmentDirectory);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GameInstallationService)}::{nameof(CopySteamDirectoryToDevelopmentDirectory)} - Error copying steam files.");
                return false;
            }
        }

        private bool CopyUnityDebugFilesToDevelopmentDirectory()
        {
            _logger.Information($"{nameof(GameInstallationService)}::{nameof(CopyUnityDebugFilesToDevelopmentDirectory)} - Configuring Unity for debugging...");

            try
            {
                _logger.Verbose($"{nameof(GameInstallationService)}::{nameof(CopyUnityDebugFilesToDevelopmentDirectory)} - Updating {Constants.Boot_Config}...");
                File.AppendAllText(Path.Combine(_directory.DevelopmentDirectory, Constants.KSP_x64_Data, Constants.Boot_Config), "player-connection-debug=1");

                foreach ((string source, string target) in GameInstallationService.UnityFiles)
                {
                    _logger.Verbose($"{nameof(GameInstallationService)}::{nameof(CopyUnityDebugFilesToDevelopmentDirectory)} - Copying Unity file {source} => {target}...");
                    File.Copy(Path.Combine(_directory.UnityDirectory, source), Path.Combine(_directory.DevelopmentDirectory, target), true);
                }

                _logger.Verbose($"{nameof(GameInstallationService)}::{nameof(CopyUnityDebugFilesToDevelopmentDirectory)} - Creating SymLink...");
                _directory.CreateSymbolicLink(Path.Combine(_directory.DevelopmentDirectory, Constants.KSP_x64_Dbg_Data), Path.Combine(_directory.DevelopmentDirectory, Constants.KSP_x64_Data));

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GameInstallationService)}::{nameof(CopyUnityDebugFilesToDevelopmentDirectory)} - Error configuring Unity.");
                return false;
            }
        }
    }
}
