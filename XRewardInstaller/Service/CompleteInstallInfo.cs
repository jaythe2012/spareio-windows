using System.Collections.Generic;

namespace Spareio.Installer.Service
{
    internal class CompleteInstallInfo
    {
        public CpuInfo cpuInfo { get; set; }
        public List<GpuInfo> gpuInfo { get; set; }
        public Dictionary<string, string> powerProfile { get; set; }

        public string uptimePct { get; set; }
        public string uptimeCurrent { get; set; }
        public string batteyStatus { get; set; }
        public bool batteryUsed { get; set; }
        public bool isAdmin { get; set; }
        public string osVersion { get; set; }
        public List<string> screenRes { get; set; }
        public string timeZone { get; set; }
        public string partnerId { get; set; }
        public string campaignId { get; set; }
        public string machineId { get; set; }
    }

    internal class CpuInfo
    {
        public string numProcessors { get; set; }
        public string numberOfLogicalProcessors { get; set; }
        public string totalPhysicalMemory { get; set; }
        public List<ProcessorInstance> processorInstance { get; set; }
    }

    internal class ProcessorInstance
    {
        public string manufacturer { get; set; }
        public string name { get; set; }
        public string clockSpeed { get; set; }
        public string maxClockSpeed { get; set; }
        public string version { get; set; }
        public string physicalCores { get; set; }
        public string logicalCores { get; set; }
        public string threads { get; set; }
    }

    internal class GpuInfo
    {
        public string bitsPerPel { get; set; }
        public string caption { get; set; }
        public string deviceName { get; set; }
        public string displayFlags { get; set; }
        public string displayFrequency { get; set; }
        public string logPixels { get; set; }
        public string pelsHeight { get; set; }
        public string pelsWidth { get; set; }
        public string settingID { get; set; }
        public string specificationVersion { get; set; }
    }
}
