using System;
using System.Windows.Forms;
using CoreLib;
using Spareio.WinService.Business;
using Spareio.WinService.DB;

namespace Spareio.WinService.Helper
{
    class PowerService
    {
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(PowerService));

        static PowerStatus status = SystemInformation.PowerStatus;

        internal static bool IsOnBattery()
        {
            return status.PowerLineStatus == PowerLineStatus.Offline;
        }

        internal static void HandlePlugOut()
        {
            try
            {
                _logWriter.Info("Handling plugOut in PowerService");
                MineBL.Update(VariableConstants.LastBatteryOnTime, DateTime.Now.ToString());
                MineBL.Update(VariableConstants.IsOnBattery, "True");
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while handling PlugOut" + ex.Message);
            }
        }

        internal static void HandlePlugIn()
        {
            try
            {
                _logWriter.Info("Handling plugIn in PowerService");
                MineBL.Update(VariableConstants.IsOnBattery, "False");
                DateTime now = DateTime.Now;
                string lastBatteryOnTime = MineBL.GetValById(VariableConstants.LastBatteryOnTime);
                if (!String.IsNullOrEmpty(lastBatteryOnTime))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(lastBatteryOnTime, out dateValue))
                    {
                        double diffInSeconds = (now - dateValue).TotalSeconds;
                        string totalBatterySeconds = MineBL.GetValById(VariableConstants.TotalBatteryTime);
                        int onBatterySeconds = 0;
                        if (!String.IsNullOrEmpty(totalBatterySeconds))
                            Int32.TryParse(totalBatterySeconds, out onBatterySeconds);
                        onBatterySeconds = onBatterySeconds + Convert.ToInt32(diffInSeconds);
                        MineBL.Update(VariableConstants.TotalBatteryTime, onBatterySeconds.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while handling PlugIn" +ex.Message);
            }
        }
    }
}
