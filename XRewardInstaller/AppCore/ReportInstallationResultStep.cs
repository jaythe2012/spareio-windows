using System;
using System.Collections.Generic;
using Spareio.Installer.Helper;
using Spareio.Installer.Service;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    public class ReportInstallationResultStep : IInstallationStep
    {
        private static string _partnerId = String.Empty;
        private static string _campaignId = String.Empty;

        private delegate void ReportState(EventService service);

        private Dictionary<InstallState, ReportState> _reporter = new Dictionary<InstallState, ReportState>
        {
            {InstallState.Ok, service => service.SendCompleteInstallEvent(GetOkInfo())},
            {InstallState.Error, service => service.SendInstallErrorEvent(GetErrorInfo())},
        };

        public enum InstallState
        {
            Ok,
            Error
        }

        private InstallState _state = InstallState.Ok;
        private string _type = "CompleteInstall";

        public ReportInstallationResultStep(InstallState state, string type)
        {
            _state = state;
            _type = type;
        }

        public void Report()
        {
            CommunicationUtils.SendReport("Done sending event");
        }

        public void Perform()
        {
            _reporter[_state](EventService.Instance);
        }

        public void Init()
        {
            CommunicationUtils.SendReport(("sending complete install event"));
        }

        private static CompleteInstallInfo GetOkInfo()
        {
            CompleteInstallInfo completeInstallInfo = SystemHelper.GetCompleteInstallInfo();
            return completeInstallInfo;
        }

        private static ErrorInstallInfo GetErrorInfo()
        {
            var clctr = CommunicationUtils.DataCollector;
            string msg = clctr.ErrorDescription.ToString();
            ErrorInstallInfo erroInstallInfo = SystemHelper.GetErrorInstallInfo();
            erroInstallInfo.ErrorMessage = msg;
            return erroInstallInfo;

        }
    }
}
