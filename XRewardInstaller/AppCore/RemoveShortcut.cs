using System.IO;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class RemoveShortcut : IInstallationStep
    {
        public void Report()
        {
            CommunicationUtils.SendReport("Start Menu shortcut has been removed");
        }

        public void Perform()
        {
            try
            {
                if (Directory.Exists(InstallUtils.GetStartMenuPath()))
                {
                    Directory.Delete(InstallUtils.GetStartMenuPath(), true);
                }
                //InstallUtils.RestartWindowsExplorer();
            }
            catch (System.Exception ex)
            {
                throw new RemoveFolderException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport("Removing Start Menu shortcut ...", false);
        }
    }
}
