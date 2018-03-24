using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class UnInstallService : IInstallationStep
    {
        private string _name = null;
        private string _processName = null;
        private int _waitForExitTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        private bool _uninstall = true;

        public UnInstallService(string name, string processName, bool uninstall = true)
        {
            _name = name;
            _processName = Path.GetFileNameWithoutExtension(processName);
            _uninstall = uninstall;
        }

        public void Report()
        {
            CommunicationUtils.SendReport(String.Format("Service {0} has been removed", _name));
        }

        public void Perform()
        {
            try
            {
                var pr = Process.GetProcessesByName(_processName);

                if (pr.Any())
                {
                    // stop service first
                    new ExecuteCommandStep("sc.exe", String.Format(@"Stop ""{0}"" ", _name)) { HideWindow = true }.Perform();
                    pr.ElementAt(0).WaitForExit(_waitForExitTimeout);
                }

                if (_uninstall && ServiceUtils.isServiceExist(_name))
                {
                    // uninstall the service
                    new ExecuteCommandStep("sc.exe", String.Format(@"Delete ""{0}"" ", _name)) { HideWindow = true }
                        .Perform();
                }
            }
            catch (System.Exception ex)
            {
                throw new ExecuteCommandException(ex) { Step = this };
            }

        }

        public void Init()
        {
            CommunicationUtils.SendReport(String.Format("Removing service {0} ...", _name), false);
        }
    }
}
