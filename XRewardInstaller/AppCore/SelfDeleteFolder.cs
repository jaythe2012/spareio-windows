using System;
using System.Diagnostics;
using System.IO;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class SelfDeleteFolder : IInstallationStep
    {
        private bool _bExecute = false;
        private string _path = String.Empty;
        private string _template = @"
:Repeat
rmdir ""{0}"" /S /Q
if exist ""{0}"" goto Repeat
del ""{1}""";

        internal SelfDeleteFolder(string path, bool bExecute)
        {
            _path = path;
            _bExecute = bExecute;
        }

        public void Report()
        {
            CommunicationUtils.SendReport(String.Format("The following folder has been marked for removal: {0}", _path));
        }

        public void Perform()
        {
            // create batch file in the temporary file
            var tmpFile = Path.Combine(Path.GetTempPath(), "spareio_uninstall.bat");
            //string cultureDLL = InstallUtils.GetCultureWcRunFolder();
            //File.WriteAllText(tmpFile, String.Format(_template, _path, cultureDLL, System.Reflection.Assembly.GetEntryAssembly().Location, tmpFile));
            File.WriteAllText(tmpFile, String.Format(_template, _path, tmpFile));

            //File.SetAttributes(tmpFile, FileAttributes.System);
            // execute in cmd
            if (_bExecute)
            {
                Process.Start(new ProcessStartInfo(@"cmd.exe", String.Format(@"/c ""{0}""", tmpFile))
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport(String.Format("Marking the following folder for removal: {0}", _path), false);
        }
    }
}
