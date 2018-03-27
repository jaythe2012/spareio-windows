using System;
using System.Windows.Forms;
using CoreLib;
using Spareio.WinService.DB;

namespace Spareio.WinService.Helper
{
    class PowerService
    {
        static PowerStatus status = SystemInformation.PowerStatus;

        internal static bool IsOnBattery()
        {
            return status.PowerLineStatus == PowerLineStatus.Offline;
        }

        internal static void HandlePlugOut()
        {
            try
            {
                LogWriter.Info("Handling plugOut in PowerService");
                DBHelper.Update(VariableConstants.LastBatteryOnTime, DateTime.Now.ToString());
                DBHelper.Update(VariableConstants.IsOnBattery, "True");
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
                LogWriter.Info("Handling plugIn in PowerService");
                DBHelper.Update(VariableConstants.IsOnBattery, "False");
                DateTime now = DateTime.Now;
                string lastBatteryOnTime = DBHelper.GetValById(VariableConstants.LastBatteryOnTime);
                if (!String.IsNullOrEmpty(lastBatteryOnTime))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(lastBatteryOnTime, out dateValue))
                    {
                        double diffInSeconds = (now - dateValue).TotalSeconds;
                        string totalBatterySeconds = DBHelper.GetValById(VariableConstants.TotalBatteryTime);
                        int onBatterySeconds = 0;
                        if (!String.IsNullOrEmpty(totalBatterySeconds))
                            Int32.TryParse(totalBatterySeconds, out onBatterySeconds);
                        onBatterySeconds = onBatterySeconds + Convert.ToInt32(diffInSeconds);
                        DBHelper.Update(VariableConstants.TotalBatteryTime, onBatterySeconds.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while handling PlugIn" +ex.Message);
            }
        }
    }
}
