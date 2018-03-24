using System;
using System.Collections.Generic;
using CoreLib;
using Spareio.WinService.Helper;

namespace Spareio.WinService
{
    public class MonitorService
    {
        public static void Initialize(bool isLoggedIn=false)
        {
            
            InitializeVariablesBulk(isLoggedIn);
            CpuService.Initialize();

            //TODO: Initialize miner engine which returns true/false to determine miner should be running or not
        }

        private static void InitializeVariablesBulk(bool isLoggedIn)
        {
            Dictionary<string,string> dictionary = new Dictionary<string, string>();
            dictionary.Add(VariableConstants.MonitorStartTime, DateTime.Now.ToString());
            dictionary.Add(VariableConstants.LastLoggedInTime, DateTime.Now.ToString());
            dictionary.Add(VariableConstants.TotalLoggedInSeconds, "0");
            dictionary.Add(VariableConstants.IsLoggedIn, isLoggedIn ? "True" : "False");
            dictionary.Add(VariableConstants.CpuTotal, "0.0");  
            dictionary.Add(VariableConstants.CpuCount, "0");
            dictionary.Add(VariableConstants.IsOnBattery, PowerService.IsOnBattery().ToString());
            dictionary.Add(VariableConstants.TotalBatteryTime, "0");
            if (PowerService.IsOnBattery())
                dictionary.Add(VariableConstants.LastBatteryOnTime, DateTime.Now.ToString());
            else
                dictionary.Add(VariableConstants.LastBatteryOnTime, "0");

            XmlHelper.UpdateSettingInBulk(dictionary);

        }

        internal static void Stop(string trigger)
        {
            LogWriter.Info("Sending event for trigger "+trigger);
            TelemetryService.SendInfo(trigger);
        }
    }
}
