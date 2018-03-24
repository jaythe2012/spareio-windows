using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Spareio.Installer.Utils
{
    class CommunicationUtils
    {
        private static ReportDataCollector _collector = new ReportDataCollector();

        internal static void SendReport(string msg, bool bSendReport = true)
        {
            Trace.WriteLine(String.Format("{0} :-> {1}", DateTime.Now, msg));
            Trace.Flush();
        }

        internal static void DownloadToLocation(Uri WebLocation, string fileLocation, int timeout)
        {
            ShFileUtils.DeleteOrRename(fileLocation);
            // create all folders to the file
            Directory.CreateDirectory(Path.GetDirectoryName(fileLocation));

            // This was the old way to download file
            // unfortunatelly it doesn't have an easy way to change the default read timeout
            //using (WebClient client = new WebClient())
            //{
            //    client.DownloadFile(WebLocation, fileLocation);
            //}
            using (var webClient = new WebClient())
            using (var stream = webClient.OpenRead(WebLocation))
            {
                if (stream != null)
                {
                    using (var file = File.OpenWrite(fileLocation))
                    {
                        // If we had use .net 4 framework, all the following lines could be replaced by:
                        // stream.CopyTo(file)

                        var buffer = new byte[1024];
                        int bytesReceived = 0;
                        stream.ReadTimeout = timeout;

                        while ((bytesReceived = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            file.Write(buffer, 0, bytesReceived);
                        }
                    }
                }
            }
        }

        internal enum Scenarion
        {
            Undefined = 0,
            Install,
            Update,
            Uninstall
        };

        internal static Scenarion CurrentScenario { set; get; }

        internal static ReportDataCollector DataCollector { get { return _collector; } }
    }
}
