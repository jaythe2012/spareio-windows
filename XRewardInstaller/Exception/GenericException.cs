using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Spareio.Installer.AppCore;
using Spareio.Installer.Utils;

namespace Spareio.Installer.Exception
{
    public class GenericException : System.Exception
    {
        private delegate void ExceptionReporter();

        private Dictionary<CommunicationUtils.Scenarion, ExceptionReporter> _reporter = new Dictionary
            <CommunicationUtils.Scenarion, ExceptionReporter>()
            {
                {CommunicationUtils.Scenarion.Install, () => new ReportInstallationResultStep(ReportInstallationResultStep.InstallState.Error, "InstallError").Perform() },
                {CommunicationUtils.Scenarion.Uninstall, () => new ReportUninstallationResultStep(ReportUninstallationResultStep.InstallState.Error, "UninstallError").Perform() },
                {CommunicationUtils.Scenarion.Update, () => new ReportUpdateResultStep(ReportUpdateResultStep.InstallState.Error, "UpdateError").Perform() },
            };

        public GenericException()
        {

        }

        public GenericException(System.Exception ex) : base(String.Empty, ex)
        {

        }

        internal IInstallationStep Step { set; get; }

        internal virtual void Report()
        {
            var exc = this.GetType().Name;
            var step = Step != null ? Step.GetType().Name : "Unknown step";

            var msg = String.Format("{0} failed {1} Exception {2}", exc, step, this.InnerException);
            Trace.WriteLine(msg);

            Utils.CommunicationUtils.DataCollector.ErrorDescription = msg;
            if (CommunicationUtils.CurrentScenario != CommunicationUtils.Scenarion.Undefined)
                _reporter[CommunicationUtils.CurrentScenario]();
        }
    }
}
