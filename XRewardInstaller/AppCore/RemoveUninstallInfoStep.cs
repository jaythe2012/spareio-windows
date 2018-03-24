using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class RemoveUninstallInfoStep : IInstallationStep
    {
        public delegate void InstalledDate(string installedDate);
        public InstalledDate Installed { set; get; }
        public void Report()
        {
            CommunicationUtils.SendReport("UnInstallation information has been removed");
        }

        public void Perform()
        {
            var iDate = InstallUtils.RemoveUnInstaller();
            if (Installed != null)
            {
                Installed(iDate);
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport("Removing uninstall information ...", false);
        }
    }
}
