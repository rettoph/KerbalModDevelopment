using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KerbalModDevelopment.Services
{
    internal class GameInstallationService
    {
        private SettingService _settings;

        private readonly string _developmentDirectory;
        private readonly string _steamDirectory;

        public GameInstallationService(SettingService settings)
        {
            _settings = settings;
        }

        public bool Verify(bool force)
        {
            if(force)
            {
                this.DeleteDirectoryIfExists();

            }
            bool result = false;

            return result;
        }

        private void DeleteDirectoryIfExists(string directory)
        {
            if (Directory.Exists(directory))
            {
                Console.WriteLine("Deleting Existing KSP Dev Install...");
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Warning: " + e.Message);
                    return false;
                }
            }
        }
    }
}
