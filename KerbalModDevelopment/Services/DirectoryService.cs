using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace KerbalModDevelopment.Services
{
    public class DirectoryService
    {
        private readonly ILogger _logger;

        public DirectoryService(ILogger logger)
        {
            _logger = logger;
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public bool Exists([NotNullWhen(true)] string? path)
        {
            return Directory.Exists(path);
        }

        public void CopyRecursive(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyRecursive(diSource, diTarget, 0);
        }

        public void CopyRecursive(DirectoryInfo source, DirectoryInfo target, int depth)
        {
            string prefix = depth == 0 ? "" : "".PadLeft(depth * 2, ' ');

            _logger.Verbose(@"{0} Creating Directory {1}", prefix, target.FullName);
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            depth++;
            prefix = depth == 0 ? "" : "".PadLeft(depth * 2, ' ');

            foreach (FileInfo fi in source.GetFiles())
            {
                string file = Path.Combine(target.FullName, fi.Name);
                _logger.Verbose(@"{0} Copying {1}", prefix, file);
                fi.CopyTo(file, true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyRecursive(diSourceSubDir, nextTargetSubDir, depth + 1);
            }
        }

        public FileSystemInfo CreateSymbolicLink(string path, string pathToTarget)
        {
            return Directory.CreateSymbolicLink(path, pathToTarget);
        }
    }
}
