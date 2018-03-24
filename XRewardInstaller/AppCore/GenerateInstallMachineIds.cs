using System;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class GenerateInstallMachineIds : IInstallationStep
    {
        private string _machineId = CommunicationUtils.DataCollector.MachineId;
        private string _installId = CommunicationUtils.DataCollector.InstallId;

        public void Report()
        {
            //CommunicationUtils.SendReportIds(_machineId, _installId, "Machine Id and Install Id has been generated");
        }

        public void Perform()
        {
            try
            {
                if (String.IsNullOrEmpty(_machineId))
                    _machineId = InstallUtils.GenerateMachineIdGuid();
                if (String.IsNullOrEmpty(_installId))
                    _installId = InstallUtils.GenerateInstallIdGuid();
            }
            catch (System.Exception ex)
            {
                throw new MachineInstallIdException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport("Generating Machine and Install Id ...", false);
        }
    }
}
