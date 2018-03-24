using System;
using System.IO;

namespace Spareio.UI.Constants
{
    class XRewardPathConstants
    {
        internal static string Company { get { return "Spareio"; } }
        internal static string Product { get { return "SpareioApp"; } }
        internal static string XRewardProcessName { get { return "Spareio.exe"; } }

        public static string GetPathToExe()
        {
            return Path.Combine(GetWcRunFolder(), "Spareio.exe");
        }

        public static string GetPathToInstallerExe()
        {
            return Path.Combine(GetWcRunFolder(), "SpareioInstaller.exe");
        }

        public static string GetPathToIcon()
        {
            return Path.Combine(GetWcRunFolder(), "xRewardIcon.ico");
        }

        public static string GetProgramFilesFolder()
        {
            string strInstallPath = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (String.IsNullOrEmpty(strInstallPath))
            {
                strInstallPath = Environment.GetEnvironmentVariable("ProgramFiles");
            }

            return strInstallPath;
        }
        internal static string GetInstallFolder()
        {
            string[] wc = { Company, Product };

            return Path.Combine(GetProgramFilesFolder(), String.Join(Path.DirectorySeparatorChar.ToString(), wc));
        }
        internal static string GetWcRunFolder()
        {
            //string[] wc = { "Application" };
            //return Path.Combine(GetInstallFolder(), String.Join(Path.DirectorySeparatorChar.ToString(), wc));
            string[] wc = { Company, Product };

            return Path.Combine(GetProgramFilesFolder(), String.Join(Path.DirectorySeparatorChar.ToString(), wc));
        }
    }
}
