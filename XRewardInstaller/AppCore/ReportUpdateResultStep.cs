using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spareio.Installer.Helper;
using Spareio.Installer.Service;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class ReportUpdateResultStep : IInstallationStep
    {
        private static string _partnerId = String.Empty;
        private static string _campaignId = String.Empty;

        private delegate void ReportState(EventService service);

        private Dictionary<ReportUpdateResultStep.InstallState, ReportUpdateResultStep.ReportState>
            _reporter =
                new Dictionary<ReportUpdateResultStep.InstallState, ReportUpdateResultStep.ReportState>
                {
                    {
                        ReportUpdateResultStep.InstallState.Ok,
                        service => service.SendCompleteUpdateEvent(GetOkInfo())
                    },
                    {
                        ReportUpdateResultStep.InstallState.Error,
                        service => service.SendUpdateErrorEvent(GetErrorInfo())
                    },
                };

        public enum InstallState
        {
            Ok,
            Error
        }

        private ReportUpdateResultStep.InstallState _state = ReportUpdateResultStep.InstallState.Ok;
        private string _type = "CompleteUninstall";

        public ReportUpdateResultStep(ReportUpdateResultStep.InstallState state, string type)
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

        private static CompleteUpdateInfo GetOkInfo()
        {
            CompleteUpdateInfo completeupdateInfo = SystemHelper.GetCompleteUpdateinfo();
            return completeupdateInfo;
        }

        private static ErrorUpdateInfo GetErrorInfo()
        {
            var clctr = CommunicationUtils.DataCollector;
            string msg = clctr.ErrorDescription.ToString();
            ErrorUpdateInfo erroInstallInfo = SystemHelper.ErrorUpdateInfo();
            erroInstallInfo.ErrorMessage = msg;
            return erroInstallInfo;
        }
    }
}
