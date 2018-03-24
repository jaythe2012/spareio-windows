using System;
using System.Windows.Forms;
using Spareio.Installer.Service;

namespace Spareio.Installer.Helper
{
    internal class SystemHelper
    {
        static PowerStatus status = SystemInformation.PowerStatus;

        internal static CompleteInstallInfo GetCompleteInstallInfo()
        {
            CompleteInstallInfo sysInfo = new CompleteInstallInfo();
            
            sysInfo.cpuInfo = CpuHelper.GetCpuInfo();
            sysInfo.gpuInfo = GpuHelper.GetGpuInfo();
            sysInfo.uptimePct = CpuHelper.GetUptimePct();
            sysInfo.uptimeCurrent = CpuHelper.GetUptimeCurrent();
            if (status != null)
            {
                sysInfo.batteyStatus = status.BatteryChargeStatus.ToString();
                sysInfo.batteryUsed = status.PowerLineStatus != System.Windows.Forms.PowerLineStatus.Online;
            }
            sysInfo.isAdmin = CpuHelper.IsAdministrator();
            sysInfo.powerProfile = PowerEnumerator.GetCurrentPowerEnumerateVistaAPI();
            sysInfo.screenRes = GpuHelper.GetScreenResolution();
            return sysInfo;
        }

        internal static ErrorInstallInfo GetErrorInstallInfo()
        {
            ErrorInstallInfo errorInstallInfo = new ErrorInstallInfo();
            errorInstallInfo.batteryStatus = status.BatteryChargeStatus.ToString();
            errorInstallInfo.batteryUsed = status.PowerLineStatus != System.Windows.Forms.PowerLineStatus.Online;
            errorInstallInfo.isAdmin = CpuHelper.IsAdministrator();
            errorInstallInfo.screenRes = GpuHelper.GetScreenResolution();
            return errorInstallInfo;
        }

        internal static CompleteUpdateInfo GetCompleteUpdateinfo()
        {
           CompleteUpdateInfo completeUpdateInfo = new CompleteUpdateInfo();
           completeUpdateInfo.isAdmin = CpuHelper.IsAdministrator();
           return completeUpdateInfo;
        }

        internal static ErrorUpdateInfo ErrorUpdateInfo()
        {
            ErrorUpdateInfo errorUpdateInfo = new ErrorUpdateInfo();
            errorUpdateInfo.isAdmin = CpuHelper.IsAdministrator();
            return errorUpdateInfo; ;
        }

        internal static CompleteUninstallInfo CompleteUnninstallInfo()
        {
            CompleteUninstallInfo completeUninstallInfo = new CompleteUninstallInfo();
            completeUninstallInfo.isAdmin = CpuHelper.IsAdministrator();
            return completeUninstallInfo;
        }

        internal static ErrorUninstallInfo ErrorUninstallInfo()
        {
            ErrorUninstallInfo errorUninstallInfo = new ErrorUninstallInfo();
            errorUninstallInfo.isAdmin = CpuHelper.IsAdministrator();
            return errorUninstallInfo;
        }
    }
}
