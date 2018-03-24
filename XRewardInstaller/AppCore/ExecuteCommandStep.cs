using System;
using System.Diagnostics;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class ExecuteCommandStep : IInstallationStep
    {
        protected string _fileName;
        protected string[] _args;
        private bool _waitForExit = true;
        private bool _hideWindow = false;
        private bool _asAdmin = false;
        private bool _asUser = false;

        public string InitMessage { set; get; }
        public ExecuteCommandStep(string fileName, params string[] args)
        {
            _fileName = fileName;
            _args = args;
        }

        public bool WaitForExit { set { _waitForExit = value; } get { return _waitForExit; } }
        public bool HideWindow { set { _hideWindow = value; } get { return _hideWindow; } }
        public bool AsAdmin { set { _asAdmin = value; } get { return _asAdmin; } }
        public bool AsUser { set { _asUser = value; } get { return _asUser; } }
        public string WorkingFolder { set; get; }

        public void Report()
        {
            CommunicationUtils.SendReport(String.Format("Shell command has been successfuly executed ({0})", GetType().Name));
        }

        public void Perform()
        {
            var strtInfo = new ProcessStartInfo
            {
                FileName = _fileName,
                Arguments = String.Join(" ", _args)
            };

            if (AsUser)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
                ProcessAsUser.Launch(String.Format("{0} {1}", _fileName, strtInfo.Arguments));
                return;
            }

            if (HideWindow)
            {
                strtInfo.CreateNoWindow = true;
                strtInfo.WindowStyle = ProcessWindowStyle.Hidden;

            }

            if (AsAdmin)
            {
                strtInfo.UseShellExecute = true;
                strtInfo.RedirectStandardOutput = false;
                strtInfo.Verb = @"runas";
            }
            else if (_waitForExit)
            {
                strtInfo.UseShellExecute = false;
                strtInfo.RedirectStandardOutput = true;
            }

            if (!String.IsNullOrEmpty(WorkingFolder))
            {
                strtInfo.WorkingDirectory = WorkingFolder;
            }

            var pr = Process.Start(strtInfo);
            if (pr != null)
            {
                if (_waitForExit)
                {
                    string output = pr.StandardOutput.ReadToEnd();
                    Trace.WriteLine(output);
                    pr.WaitForExit();
                }
            }
            else
            {
                throw new ExecuteCommandException { Step = this };
            }
        }

        public void Init()
        {
            if (String.IsNullOrEmpty(InitMessage))
            {
                CommunicationUtils.SendReport(String.Format("Executing command  {0} {1}...", _fileName, String.Join(" ", _args)));
            }
            else
            {
                CommunicationUtils.SendReport(InitMessage);
            }
        }
    }
}
