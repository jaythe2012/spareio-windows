using System;
using System.IO;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class InstallService : IInstallationStep
    {
        internal static string ServiceName { get { return "SpareioService"; } }
        internal static string ExeName { get { return "SpareioWinService.exe"; } }
        internal static string ServiceDescription { get { return "SpareioApp Assistant Service"; } }

        private string _outputFolder = String.Empty;
        private string DisplayName { get { return "Spareio Service"; } }
        private string ExeFile { get { return Path.Combine(_outputFolder, ExeName); } }

        public InstallService(string installFolder)
        {
            _outputFolder = installFolder;
        }

        public void Report()
        {
            CommunicationUtils.SendReport("SpareioService has been successfuly installed");
        }

        public void Perform()
        {
            try
            {
                new ExecuteInstallService(ExeFile, ServiceName, DisplayName, null).Perform();
                new ExecuteCommandStep("sc.exe", new[] { "failure", ServiceName, "reset= 30", "actions= restart/60000" }) { HideWindow = true }.Perform();
                new ExecuteServiceDescription(ServiceName, ServiceDescription).Perform();
            }
            catch (System.Exception ex)
            {
                throw new ExecuteCommandException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport("Installing XRewardService ...", false);
        }
    }
}
