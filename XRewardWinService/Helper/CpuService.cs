using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Spareio.WinService.Business;
using Spareio.WinService.DB;

namespace Spareio.WinService.Helper
{
    class CpuService
    {
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(CpuService));

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

            //   RegisterForCpuTimer();

            cpuDictionary = new Dictionary<string, string>();
            cpuDictionary.Add(VariableConstants.CpuTotal, "0.0");
            cpuDictionary.Add(VariableConstants.CpuCount, "0");
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


        ///Commented by Neha Shah
        #region Commented as CPU timer will be off

        /// <summary>
        /// Cpu usgae monitor is managed by timer
        /// </summary>
        //private static void RegisterForCpuTimer()
        //{
        //    try
        //    {



        //        cpuReadTimer = new System.Timers.Timer();
        //        cpuReadTimer.Interval = 5000;
        //        cpuReadTimer.Elapsed += CpuReadTimerOnElapsed;
        //        cpuReadTimer.Enabled = true;
        //        _logWriter.Info("Timer Enabled");
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private static void CpuReadTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        //{
        //    cpuTotal = MineBL.GetValById(VariableConstants.CpuTotal);
        //    cpuTotal = string.IsNullOrEmpty(cpuTotal) ? cpuDictionary[VariableConstants.CpuTotal] : cpuTotal;

        //    cpuCount = MineBL.GetValById(VariableConstants.CpuCount);
        //    cpuCount = string.IsNullOrEmpty(cpuCount) ? cpuDictionary[VariableConstants.CpuCount] : cpuCount;
        //}

        //private static void CpuWriteTimerElapsed(object sender, ElapsedEventArgs e)
        //{
        //    UpdateCpuAverage();
        //}

        #endregion

        private static void GetCpuParamsFromDB()
        {
            cpuTotal = MineBL.GetValById(VariableConstants.CpuTotal);
            cpuTotal = string.IsNullOrEmpty(cpuTotal) ? cpuDictionary[VariableConstants.CpuTotal] : cpuTotal;

            cpuCount = MineBL.GetValById(VariableConstants.CpuCount);
            cpuCount = string.IsNullOrEmpty(cpuCount) ? cpuDictionary[VariableConstants.CpuCount] : cpuCount;
        }

        public static void UpdateCpuAverage()
        {
            try
            {
                GetCpuParamsFromDB();

                string cpuUsage = GetCurrentCpuUsage();
                double cpuTotalNumber = Convert.ToDouble(cpuTotal) + Convert.ToDouble(cpuUsage);
                int cpuCountNumber = Convert.ToInt32(cpuCount) + 1;
                cpuDictionary[VariableConstants.CpuTotal] = cpuTotalNumber.ToString();
                cpuDictionary[VariableConstants.CpuCount] = cpuCountNumber.ToString();
                MineBL.UpdateInBulk(cpuDictionary);
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while updating CPU Info" + ex.Message);
            }

        }
    }
}
