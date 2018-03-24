using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class UninstallInfoStep : IInstallationStep
    {
        private string _version = null;
        public string InstalledDate { get; set; }

        public void Report()
        {
            CommunicationUtils.SendReport("UnInstallation information has been created");
        }

        public void Perform()
        {
            _version = InstallUtils.CreateUninstaller(InstalledDate);
        }

        public void Init()
        {
            CommunicationUtils.SendReport("Creating Uninstallation information ...", false);
        }
    }
}
