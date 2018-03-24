using System;
using System.IO;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class RemoveLavasoftEmptyDirectories : IInstallationStep
    {
        private string[] _LavasoftDirectoryPaths = { };

        public RemoveLavasoftEmptyDirectories(params string[] LavasoftDirectoryPaths)
        {
            _LavasoftDirectoryPaths = LavasoftDirectoryPaths;
        }

        public void Report()
        {
            CommunicationUtils.SendReport(String.Format("Folder cleanup done"));
        }

        public void Perform()
        {
            try
            {
                foreach (string directory in _LavasoftDirectoryPaths)
                {
                    if (Directory.Exists(directory) && InstallUtils.IsDirectoryEmpty(directory))
                    {
                        Directory.Delete(directory, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new RemoveFolderException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport(String.Format("Cleaning empty folders"), false);
        }
    }
}

