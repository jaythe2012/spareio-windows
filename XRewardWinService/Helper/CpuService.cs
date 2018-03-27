using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using CoreLib;
using Spareio.WinService.DB;

namespace Spareio.WinService.Helper
{
    class CpuService
    {
        protected static PerformanceCounter cpuCounter;
        private static System.Timers.Timer cpuWriteTimer = null;
        private static System.Timers.Timer cpuReadTimer = null;
        private static string cpuTotal = "0.0";
        private static string cpuCount = "0";
        private static Dictionary<string, string> cpuDictionary;

        public static void Initialize()
        {
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            RegisterForCpuTimer();
            cpuDictionary = new Dictionary<string, string>();
            cpuDictionary.Add(VariableConstants.CpuTotal, "0.0");
            cpuDictionary.Add(VariableConstants.CpuCount,"0");
        }

        public static string GetCurrentCpuUsage()
        {
            try
            {
                return cpuCounter.NextValue().ToString();
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        /// <summary>
        /// Cpu usgae monitor is managed by timer
        /// </summary>
        private static void RegisterForCpuTimer()
        {
            try
            {
                cpuWriteTimer = new System.Timers.Timer();
                cpuWriteTimer.Interval = 10000;
                cpuWriteTimer.Elapsed += CpuWriteTimerElapsed;
                cpuWriteTimer.Enabled = true;
                cpuReadTimer = new System.Timers.Timer();
                cpuReadTimer.Interval = 5000;
                cpuReadTimer.Elapsed += CpuReadTimerOnElapsed;
                cpuReadTimer.Enabled = true;
                LogWriter.Info("Timer Enabled");
            }
            catch (Exception ex)
            {
            }
        }

        private static void CpuReadTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            cpuTotal = DBHelper.GetValById(VariableConstants.CpuTotal);
            cpuCount = DBHelper.GetValById(VariableConstants.CpuCount);
        }

        private static void CpuWriteTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateCpuAverage();
        }

        internal static void UpdateCpuAverage()
        {
            try
            {
                string cpuUsage = GetCurrentCpuUsage();
                double cpuTotalNumber = Convert.ToDouble(cpuTotal) + Convert.ToDouble(cpuUsage);
                int cpuCountNumber = Convert.ToInt32(cpuCount) + 1;
                cpuDictionary[VariableConstants.CpuTotal]= cpuTotalNumber.ToString();
                cpuDictionary[VariableConstants.CpuCount] =  cpuCountNumber.ToString();
                DBHelper.UpdateInBulk(cpuDictionary);
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while updating CPU Info" + ex.Message);
            }

        }
    }
}
