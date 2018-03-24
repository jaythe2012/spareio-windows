using System;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class CreatePartnerInfoStep : IInstallationStep
    {
        private static string _partnerId = String.Empty;
        private static string _campaignId = String.Empty;
        private static string _xToken = string.Empty;


        internal CreatePartnerInfoStep(params string[] args)
        {
            var argumentProcessor = new Utils.CmdLineArgs(args);
            if (argumentProcessor.CheckArg("partner"))
                _partnerId = argumentProcessor.GetArgValue("partner");
            if (argumentProcessor.CheckArg("campaign"))
                _campaignId = argumentProcessor.GetArgValue("campaign");
            if (argumentProcessor.CheckArg("xToken"))
                _xToken = argumentProcessor.GetArgValue("xToken");
        }

        public void Report()
        {
            //CommunicationUtils.SendReport(String.Format("The latest stable version of Web Companion has been downloaded: {0}", _version));
            CommunicationUtils.SendReport(String.Format("Done Creating parterInfo registry"));
        }

        public void Perform()
        {
            try
            {
                InstallUtils.UpdatePartnerInfo(_partnerId, _campaignId, _xToken);
            }
            catch (System.Exception ex)
            {
                throw new CreatePartnerException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport(String.Format("Creating parterInfo registry"));

        }
    }
}
