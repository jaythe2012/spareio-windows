using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Spareio.Installer.Service;

namespace Spareio.Installer.Helper
{
    class CpuHelper
    {
        public static CpuInfo GetCpuInfo()
        {
            return GetProcessorDetails();
        }

        private static List<ProcessorInstance> GetProcessorinstances()
        {
            List<ProcessorInstance> processorInstanceList = new List<ProcessorInstance>();
            ProcessorInstance processorInstance = new ProcessorInstance();
            try
            {
                ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (ManagementObject obj in win32Proc.Get())
                {
                    processorInstance.manufacturer = obj.TryGetProperty("Manufacturer").ToString();
                    processorInstance.name = obj.TryGetProperty("Name").ToString();
                    processorInstance.clockSpeed = obj.TryGetProperty("CurrentClockSpeed").ToString();
                    processorInstance.maxClockSpeed = obj["MaxClockSpeed"].ToString();
                    processorInstance.version = obj.TryGetProperty("Version").ToString();
                    processorInstance.physicalCores = obj.TryGetProperty("NumberOfCores").ToString();
                    processorInstance.logicalCores = obj.TryGetProperty("NumberOfLogicalProcessors").ToString();
                    processorInstance.threads = obj.TryGetProperty("ThreadCount").ToString();
                    processorInstanceList.Add(processorInstance);
                }
                return processorInstanceList;
            }
            catch (System.Exception ex)
            {
                return processorInstanceList;

            }
        }

        internal static string GetUptimeCurrent()
        {
            string uptimeCurrent = String.Empty;
            int result = Environment.TickCount;
            double minutesFromTs = TimeSpan.FromMilliseconds(result).TotalMinutes;
            uptimeCurrent = minutesFromTs.ToString();
            return uptimeCurrent;
        }

        private static CpuInfo GetProcessorDetails()
        {
            CpuInfo cpuInfo = new CpuInfo();
            try
            {
                ManagementObjectSearcher win32CompSys =
                    new ManagementObjectSearcher("select * from Win32_ComputerSystem");
                foreach (ManagementObject obj in win32CompSys.Get())
                {
                    cpuInfo.numProcessors = obj.TryGetProperty("NumberOfProcessors").ToString(); ;
                    cpuInfo.numberOfLogicalProcessors = obj.TryGetProperty("NumberOfLogicalProcessors").ToString();
                    cpuInfo.totalPhysicalMemory = obj.TryGetProperty("TotalPhysicalMemory").ToString();
                }
                cpuInfo.processorInstance = GetProcessorinstances();
                return cpuInfo;
            }
            catch (System.Exception ex)
            {
                return cpuInfo;
            }
        }

        public static string GetUptimePct()
        {
            Dictionary<string, object> sysFacts = new Dictionary<string, object>();
            string uptimePct = String.Empty;
            try
            {
                var uptime = new PerformanceCounter("System", "System Up Time");
                uptime.NextValue();       //Call this an extra time before reading its value
                var uptimeCount = uptime.NextValue();

                string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                DateTime earliestSoftware = DateTime.Now;
                String earliestSoftwareName = "";
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
                {
                    foreach (string subkey_name in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                        {
                            var installDate = subkey.GetValue("InstallDate");

                            var dateParsed = new DateTime();
                            string[] dateFormat = { "yyyyMMdd", "MM/dd/yyyy", "dd/MM/yyyy" };
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            if (installDate != null)
                            {
                                DateTime.TryParseExact(installDate.ToString(), dateFormat, provider, DateTimeStyles.None, out dateParsed);
                                if (DateTime.Compare(earliestSoftware, dateParsed) > 0) // earliestSoftware is later than dateParsed
                                {
                                    earliestSoftware = dateParsed;
                                    earliestSoftwareName = subkey.GetValue("DisplayName").ToString();
                                }
                            }

                        }
                    }
                }

                var systemAgeDays = (DateTime.Now - earliestSoftware).TotalDays;

                /* check for uptime */
                EventLog log = new EventLog("System");
                var entries = log.Entries.Cast<EventLogEntry>().Where(x => x.EventID == 6013 & x.TimeGenerated > DateTime.Now.AddSeconds(86400 * 30 * -1)).Select(x => new
                {
                    x.MachineName,
                    x.Site,
                    x.TimeGenerated,
                    x.Source,
                    x.Message
                }).ToList();


                UInt32 uptimeTrack = 0;
                UInt32 restartTrack = 0;
                UInt32 uptimeCalc = 0;
                foreach (var uptimeEntry in entries)
                {
                    // check starting entry...
                    Match m = Regex.Match(uptimeEntry.Message, @"(\d+)\s");
                    if (m.Success)
                    {
                        if (uptimeTrack == 0)
                        {
                            //Console.WriteLine("\tGot entry, first, uptime set");
                            uptimeTrack = Convert.ToUInt32(m.Value);
                        }
                        else
                        {
                            if (Convert.ToUInt32(m.Value) >= uptimeTrack) // uptime was greater than the last record, add delta to uptime calculator
                            {
                                uptimeCalc += Convert.ToUInt32(m.Value) - uptimeTrack;
                                var lastUptime = uptimeTrack;
                                uptimeTrack = Convert.ToUInt32(m.Value);
                                //Console.WriteLine("\tGot entry, sequential, uptime is " + uptimeTrack + ", delta was " + lastUptime);
                            }
                            else // uptime was less than last record, restart/reset happened, reset tracker
                            {
                                uptimeTrack = Convert.ToUInt32(m.Value);
                                //Console.WriteLine("\tGot entry, new, uptime is " + uptimeTrack);
                                if (Convert.ToUInt32(m.Value) < 1000)
                                {
                                    // system restarted
                                    restartTrack++;
                                }
                            }
                        }
                    }
                }

                double uptimeRatio = (double)uptimeCalc / (double)(86400 * 29); // calculate rough % of online time.  Will be less accurate for systems restarted many times or used intermittently.



                sysFacts.Add("EstimatedUptime", uptimeCalc);
                sysFacts.Add("EstimatedStarts", restartTrack);
                sysFacts.Add("EstimatedUptimePct", uptimeRatio.ToString("N3"));
                sysFacts.Add("SystemUptime", uptimeCount);
                sysFacts.Add("SystemAge", systemAgeDays);
                sysFacts.Add("SystemFirstSoftware", earliestSoftwareName);


                uptimePct = uptimeRatio.ToString("N3");
            }
            catch (System.Exception ex)
            {
                return uptimePct;
            }

            return uptimePct;

        }

        internal static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
