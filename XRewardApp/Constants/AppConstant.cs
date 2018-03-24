using System;

namespace Spareio.UI.Constants
{
    public class AppConstant
    {
        /// <summary>
        /// Minimization command for system startup
        /// </summary>
        public static readonly string MinimizeCmd = "--minimize";

        /// <summary>
        /// The command line argument used to perform post installation steps.
        /// </summary>
        public static readonly string installCmd = "/install";

        public static readonly string installCmdDefault = "--install";

        /// <summary>
        /// The command line argument used to perform post-uninstallation steps.
        /// </summary>
        public static readonly string unInstallCmd = "/uninstall";

        public static readonly string unInstallCmdDefault = "--uninstall";

        /// <summary>
        /// The command line argument used to perform post-uupdate steps.
        /// </summary>
        public static readonly string updateCmd = "--update";


        /// <summary>
        /// The command line argument used to launch Web Companion's onboarding screen.
        /// </summary>
        public static readonly string SilentCmd = "/silent";

        public static readonly string SilentCmdDefault = "--silent";

        public static readonly string AdBlockingCmd = "/adblock";
        public static readonly string AdBlockingCmdDefault = "--adblock";

        public static readonly string afterinstallCmd = "/afterinstall";

        public static readonly string afterinstallCmdDefault = "--afterinstall";

        public static readonly string afterupdateCmd = "/afterupdate";

        public static readonly string afterupdateCmdDefault = "--afterupdate";

        public static readonly TimeSpan SoftwareUpdateInterval = new TimeSpan(24, 0, 0);

        public static readonly TimeSpan AfterStartupUpdateDelay = new TimeSpan(0, 125, 0);


    }
}
