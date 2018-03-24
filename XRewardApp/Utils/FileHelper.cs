using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;

namespace Spareio.UI.Utils
{
    internal class FileHelper
    {
        internal static void DownloadToLocation(Uri WebLocation, string fileLocation)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileLocation));
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(WebLocation, fileLocation);
            }
        }

        public static Version GetCurrentVersion(Assembly assembly)
        {
            Version currentVersion = null;
            try
            {
                //Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;
                currentVersion = new Version(version);
            }
            catch (Exception)
            {
                throw;
            }
            return currentVersion;
        }


    }
}
