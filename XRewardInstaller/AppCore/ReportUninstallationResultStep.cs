using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spareio.Installer.Helper;
using Spareio.Installer.Service;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class ReportUninstallationResultStep : IInstallationStep
    {
        private static string _partnerId = String.Empty;
        private static string _campaignId = String.Empty;

        private delegate void ReportState(EventService service);

        private Dictionary<ReportUninstallationResultStep.InstallState, ReportUninstallationResultStep.ReportState>
            _reporter =
                new Dictionary<ReportUninstallationResultStep.InstallState, ReportUninstallationResultStep.ReportState>
                {
                    {
                        ReportUninstallationResultStep.InstallState.Ok,
                        service => service.SendCompleteUninstallEvent(GetOkInfo())
                    },
                    {
                        ReportUninstallationResultStep.InstallState.Error,
                        service => service.SendUninstallErrorEvent(GetErrorInfo())
                    },
                };

        public enum InstallState
        {
            Ok,
            Error
        }

        private ReportUninstallationResultStep.InstallState _state = ReportUninstallationResultStep.InstallState.Ok;
        private string _type = "CompleteUninstall";

        public ReportUninstallationResultStep(ReportUninstallationResultStep.InstallState state, string type)
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

        private static CompleteUninstallInfo GetOkInfo()
        {
            CompleteUninstallInfo completeUninstallInfo = SystemHelper.CompleteUnninstallInfo();
            return completeUninstallInfo;
        }

        private static ErrorUninstallInfo GetErrorInfo()
        {
            var clctr = CommunicationUtils.DataCollector;
            string msg = clctr.ErrorDescription.ToString();
            ErrorUninstallInfo errorUninstallInfo = SystemHelper.ErrorUninstallInfo();
            errorUninstallInfo.ErrorMessage = msg;
            return errorUninstallInfo;
        }
    }
}
