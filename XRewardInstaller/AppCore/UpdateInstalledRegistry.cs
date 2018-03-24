using System;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class UpdateInstalledRegistry : IInstallationStep
    {
        private bool _bSetValue = false;

        public UpdateInstalledRegistry(bool bSetValue)
        {
            _bSetValue = bSetValue;
        }

        public void Report()
        {
            CommunicationUtils.SendReport(String.Format("'Installed' mark was {0}.", _bSetValue ? "set" : "removed"));
        }

        public void Perform()
        {
            if (_bSetValue)
                InstallUtils.GenerateSuccessfullInstallMark();
            else
            {
                InstallUtils.RemoveInstalledMark();
                // https://lavasoft.atlassian.net/browse/WC-2019 It is decided to leave machine id always
                //string GUID = InstallUtils.RemoveMachineIdGuid();
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport("Changing 'Installed' mark ...", false);
        }
    }
}
