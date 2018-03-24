using System;

namespace Spareio.Installer.AppCore
{
    internal class ExecuteInstallService : ExecuteCommandStep
    {
        public ExecuteInstallService(string fullPath, string serviceName, string displayName, string serviceArgs) : base("sc.exe",
            String.Format(@"Create ""{0}"" binPath= ""{1}"" DisplayName= ""{2}"" start= auto", serviceName, fullPath, displayName))
        {
            HideWindow = true;
        }
    }

    internal class ExecuteServiceDescription : ExecuteCommandStep
    {
        public ExecuteServiceDescription(string serviceName, string description)
            : base("sc.exe",
                String.Format(@"description ""{0}"" ""{1}""", serviceName, description))
        {
            HideWindow = true;
        }
    }
}
