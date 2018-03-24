using System;
using System.IO;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class DownloadStep : IInstallationStep
    {
        public delegate bool FileValidater(string fileName);

        internal FileValidater Validator { set; get; }
        internal const int SelfUpdateTimeout = 1;
        internal const int InstallTimeout = 2;
        internal const int UpdateTimeout = 10;
        internal const int SelfUpdateRetry = 2;
        internal const int InstallRetry = 4;
        internal const int UpdateRetry = 10;

        protected string _tmpFolder;
        protected string _version = String.Empty;
        protected string _platform = "internal"; // "prod" is another option
        protected string _useZipFile = String.Empty;
        protected TimeSpan _timeout = TimeSpan.Zero;
        protected int _iMaxDownloadRetry = 3;

        internal DownloadStep(string tmpFolder, TimeSpan timeout, int maxRetries, params string[] args)
        {
            _tmpFolder = tmpFolder;
            _timeout = timeout;
            _iMaxDownloadRetry = maxRetries;

            var argumentProcessor = new Utils.CmdLineArgs(args);
            if (argumentProcessor.CheckArg("version"))
                _version = argumentProcessor.GetArgValue("version");
            if (argumentProcessor.CheckArg("zip"))
                _useZipFile = argumentProcessor.GetArgValue("zip");

            // check platform base on the name of installer
            //_platform = InstallUtils.GetPlatform(args);
        }

        public virtual void Report()
        {
            CommunicationUtils.SendReport(String.Format("The latest stable version of Web Companion has been downloaded: {0}", _version));
        }

        public virtual void Perform()
        {
            try
            {

                String DownloadUrl = "http://voila.webcompanion.com/notifications/download/rt/Spareio.zip";


                //String DownloadUrl = "http://rt.webcompanion.com/notifications/download/rt/Spareio.zip";


                if (!String.IsNullOrEmpty(_version))
                    DownloadUrl = String.Format("http://rt.webcompanion.com/notifications/download/rt/SpareioBuild/{0}/Spareio.zip", _version);

                CommunicationUtils.SendReport(String.Format("Downloading the latest stable version builf: {0}", DownloadUrl), false);

                if (!String.IsNullOrEmpty(DownloadUrl))
                {
                    var dest = _tmpFolder + InstallUtils.WcPackageName;
                    bool bValid = true;
                    int iRetryCount = 0;
                    System.Exception _lastException = null;
                    do
                    {
                        try
                        {
                            // download latest version from ftp/http
                            CommunicationUtils.DownloadToLocation(new Uri(DownloadUrl),
                                dest, (int)_timeout.TotalMilliseconds);
                            // validate zip file and retry the download in case of corruption
                            if (Validator != null)
                                bValid = Validator(dest);
                        }
                        catch (System.Exception ex)
                        {
                            bValid = false;
                            _lastException = ex;
                            CommunicationUtils.SendReport(String.Format("Downloading the latest stable version exception: {0}", ex), false);

                            // switch download url to cloudflare and back
                           // DownloadUrl = CommunicationUtils.SwitchUrls(DownloadUrl, new[] { "wcdownloader", "wcdownloadercdn" });
                        }
                    } while (!bValid && ++iRetryCount < _iMaxDownloadRetry);
                    // rethrow exception to reflect the correct situation
                    if (!bValid)
                    {
                        //throw _lastException ?? new DownloadException(new System.Exception("Something fishi is going on"));
                        throw _lastException ?? new SystemException();
                    }
                }
                else
                {
                    File.Copy(_useZipFile, _tmpFolder + InstallUtils.WcPackageName, true);
                }
            }
            catch (System.Exception ex)
            {
                throw new DownloadException(ex) { Step = this };
            }
        }

        public virtual void Init()
        {
            CommunicationUtils.SendReport(String.Format("Downloading the latest stable version {0}...", _version), false);
        }
    }
}
