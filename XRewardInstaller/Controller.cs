using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Spareio.Installer.AppCore;
using Spareio.Installer.Exception;
using Spareio.Installer.Service;
using Spareio.Installer.Utils;

namespace Spareio.Installer
{
    internal static class Controller
    {
        internal static IEnumerable<IInstallationStep> GetUpdateProcedure(params string[] args)
        {
            // create tmp folder
            string installedDate = null;
            var tmpFolder = Path.GetTempPath();
            var owner = ProcessAsUser.GetProcessOwner(Process.GetCurrentProcess().Id).ToLower();
            bool bAsUser = owner.Contains("system");
            Trace.WriteLine("SpareioInstaller owner=" + owner);
            var cmdArgs = new Utils.CmdLineArgs(args);
            bool bPreProd = cmdArgs.CheckArg("preprod");
            string _token = String.Empty;
            
            _token = cmdArgs.CheckArg("xToken") ? cmdArgs.GetArgValue("xToken") : InstallUtils.ReadValue("xToken");
            

            InitializeEventService(args);


            yield return new GenerateInstallMachineIds();
            yield return new StopProcess(InstallUtils.WcProcessName);
            yield return new DownloadStep(tmpFolder, TimeSpan.FromMinutes(DownloadStep.UpdateTimeout), DownloadStep.UpdateRetry, args) { Validator = x => ZipUtils.IsZipValid(x) };

            yield return new UnInstallService(InstallService.ServiceName, InstallService.ExeName, true);
            yield return new UnzipStep(Path.Combine(tmpFolder, "Spareio.zip"), InstallUtils.GetInstallFolder());
            yield return new RemoveUninstallInfoStep { Installed = iDate => { installedDate = iDate; } };
            yield return new UninstallInfoStep { InstalledDate = installedDate };
            yield return new InstallService(InstallUtils.GetWcRunFolder());
            yield return new ExecuteCommandStep("sc.exe", new[] { "start", InstallService.ServiceName, !String.IsNullOrEmpty(_token) ? _token : "" }) { HideWindow = true };
            //yield return new RunSpareio(Path.Combine(InstallUtils.GetWcRunFolder(), "Spareio.exe")) { silent = true, preprod = bPreProd, update = true, InitMessage = "Applying update for distributed components..." };
            yield return new RunSpareio(Path.Combine(InstallUtils.GetWcRunFolder(), "Spareio.exe"), "") { preprod = bPreProd, AsUser = bAsUser, afterupdate = true, WaitForExit = false };
            yield return new ReportUpdateResultStep(ReportUpdateResultStep.InstallState.Ok, "CompleteUpdate");
        }

        internal static IEnumerable<IInstallationStep> GetUnInstallationProcedure(params string[] args)
        {
            InitializeEventService(args);

            yield return new GenerateInstallMachineIds(); 
            yield return new StopProcess(InstallUtils.WcProcessName);

            yield return new UnInstallService(InstallService.ServiceName, InstallService.ExeName);
            yield return new RemoveUninstallInfoStep();

            yield return new RemoveAppdataFolder(InstallUtils.GetLocalAppDataFolder(), "Spario");
            yield return new RemoveFolder(InstallUtils.GetWCProgramDataFolder());
            yield return new RemoveFolder(InstallUtils.GetRoamingAppDataWCFolder());
            yield return new RemoveShortcut();

            yield return new RemoveLavasoftEmptyDirectories(InstallUtils.GetLavasoftProgramDataFolder(), InstallUtils.GetRoamingAppDataLavasoftFolder(), InstallUtils.GetLocalAppDataFolder());
            yield return new UpdateInstalledRegistry(false); // remove Installed

            yield return new ReportUninstallationResultStep(ReportUninstallationResultStep.InstallState.Ok, "CompleteUninstall");

            yield return new SelfDeleteFolder(InstallUtils.fullPathToBeDeleted(InstallUtils.GetInstallFolder()), CommunicationUtils.DataCollector.Version != "1.0.0.0");
        }

        internal static IEnumerable<IInstallationStep> GetInstallationProcedure(params string[] args)
        {
            // create tmp folder
            var tmpFolder = Path.GetTempPath();

            var cmdArgs = new Utils.CmdLineArgs(args);
            bool bSilent = cmdArgs.CheckArg("silent");
            string xToken = cmdArgs.GetArgValue("xToken");
            InitializeEventService(args);
            yield return new GenerateInstallMachineIds();
            yield return new CreatePartnerInfoStep(args);

            yield return new DownloadStep(tmpFolder, TimeSpan.FromMinutes(DownloadStep.InstallTimeout), DownloadStep.InstallRetry, args) { Validator = x => ZipUtils.IsZipValid(x) };
            yield return new UnzipStep(Path.Combine(tmpFolder, "Spareio.zip"), InstallUtils.GetInstallFolder());
            yield return new InstallService(InstallUtils.GetWcRunFolder());
            yield return new ExecuteCommandStep("sc.exe", new[] { "start", InstallService.ServiceName, xToken }) { HideWindow = true };
            yield return new UninstallInfoStep();
            yield return new UpdateInstalledRegistry(true); 
            yield return new RunSpareio(Path.Combine(InstallUtils.GetWcRunFolder(), "Spareio.exe")) { silent = bSilent, install = true, InitMessage = "Applying Selected Configuration ..." };
            yield return new RunSpareio(Path.Combine(InstallUtils.GetWcRunFolder(), "Spareio.exe")) { silent = bSilent, afterinstall = true, WaitForExit = false };
            yield return new ReportInstallationResultStep(ReportInstallationResultStep.InstallState.Ok, "CompleteInstall");
        }

        private static void InitializeEventService(string[] args)
        {
            string _partnerId = String.Empty;
            string _campaignId = String.Empty;
            string _token = String.Empty;

            if (args != null)
            {
                var argumentProcessor = new Utils.CmdLineArgs(args);
                if (argumentProcessor.CheckArg("partner"))
                    _partnerId = argumentProcessor.GetArgValue("partner");
                else
                {
                    _partnerId = InstallUtils.ReadValue("PartnerId");
                }
                if (argumentProcessor.CheckArg("campaign"))
                    _campaignId = argumentProcessor.GetArgValue("campaign");
                else
                {
                    _campaignId = InstallUtils.ReadValue("CampaignId");
                }
                if (argumentProcessor.CheckArg("xToken"))
                    _token = argumentProcessor.GetArgValue("xToken");
                else
                {
                    _token = InstallUtils.ReadValue("xToken");
                }
            }

            EventService.Initialize(_partnerId, _campaignId, _token);
        }

        internal delegate bool IsCanceledHandler();
        internal static int StepExecuter(IEnumerable<IInstallationStep> procedure, IsCanceledHandler isCanceled, bool bInterruptOnReportError = true)
        {
            try
            {
                foreach (var step in procedure)
                {
                    if (isCanceled())
                    {
                        return 1; // canceled
                    }

                    InstallUtils.SafeCallInterruptable(bInterruptOnReportError, () => step.Init());

                    step.Perform();

                    InstallUtils.SafeCallInterruptable(bInterruptOnReportError, () => step.Report());
                }
            }
            catch (GenericException ex)
            {
                // report installation failure

                InstallUtils.SafeCallInterruptable(false, () => ex.Report());

                return -1;
            }

            // report success

            return 0;
        }

        internal static int RunUninstall(IsCanceledHandler isCanceled, params string[] args)
        {
            CommunicationUtils.CurrentScenario = CommunicationUtils.Scenarion.Uninstall;
            return StepExecuter(GetUnInstallationProcedure(args), isCanceled, false); // uninstall will not fail if the report cannot be sent
        }

        internal static int RunInstallation(IsCanceledHandler isCanceled, params string[] args)
        {
            CommunicationUtils.CurrentScenario = CommunicationUtils.Scenarion.Install;
            return StepExecuter(GetInstallationProcedure(args), isCanceled);
        }

        public static int RunUpdate(IsCanceledHandler isCanceled, params string[] args)
        {
            CommunicationUtils.CurrentScenario = CommunicationUtils.Scenarion.Update;
            return StepExecuter(GetUpdateProcedure(args), isCanceled);
        }
    }
}
