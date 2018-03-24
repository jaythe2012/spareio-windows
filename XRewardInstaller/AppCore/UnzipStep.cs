using System;
using ICSharpCode.SharpZipLib.Zip;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class UnzipStep : IInstallationStep
    {
        private string _zipFileName;
        private string _outputFolder;
        public UnzipStep(string zipFileName, string outputFolder)
        {
            _zipFileName = zipFileName;
            _outputFolder = outputFolder;
        }

        public void Report()
        {
            CommunicationUtils.SendReport(String.Format("Extracting Spareio.zip has been successful"));
        }

        public void Perform()
        {
            try
            {
                ZipUtils.ExtractZipFile(_zipFileName, null, _outputFolder);
            }
            catch (System.Exception ex)
            {
                throw new UnzipException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport(String.Format("Extracting {0} ...", _zipFileName), false);
        }
    }
}
