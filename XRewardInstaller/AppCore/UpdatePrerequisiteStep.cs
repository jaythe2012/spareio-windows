using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    class UpdatePrerequisiteStep : IInstallationStep
    {
        bool PlatformRequired { set; get; }
        public UpdatePrerequisiteStep(string[] args)
        {
            var cmd = new CmdLineArgs(args);
            PlatformRequired = !(cmd.CheckArg("internal") || cmd.CheckArg("prod"));

        }

        public void Report()
        {
            CommunicationUtils.SendReport("Update prerequisites test has been successfully passed");
        }

        public void Perform()
        {
            // verify necessary parameters for update

            if (PlatformRequired)
            {
                CommunicationUtils.SendReport("The platform information is missing during update. Update will be performed against prod");
                //throw new InstallationException(new System.NotSupportedException("The platform information is missing during update. Update is not supported.")) {Step = this};
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport("Checking update prerequisites ...", false);
        }
    }
}
