using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Spareio.Installer.Exception;
using Spareio.Installer.Utils;

namespace Spareio.Installer.AppCore
{
    internal class StopProcess : IInstallationStep
    {
        string ProcName { set; get; }

        internal StopProcess(string procName)
        {
            ProcName = Path.GetFileNameWithoutExtension(procName);
        }

        public void Report()
        {
            CommunicationUtils.SendReport(String.Format("Process {0} has been stopped", ProcName));
        }

        public void Perform()
        {
            try
            {
                var prs = Process.GetProcessesByName(ProcName);
                if (prs.Any())
                {
                    var id = Process.GetCurrentProcess().Id;

                    // kill all processes that has this name except myself
                    foreach (var p in prs)
                    {
                        if (p.Id != id)
                        {
                            p.Kill();
                            p.WaitForExit();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new StopProcessException(ex) { Step = this };
            }
        }

        public void Init()
        {
            CommunicationUtils.SendReport(String.Format("Stopping process {0}", ProcName), false);
        }
    }
}
