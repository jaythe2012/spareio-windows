using System.Collections.Generic;

namespace Spareio.WinService.Model
{
    public class HourlyActivity
    {
        public string xToken { get; set; }
        public string lastTrigger { get; set; }
        public double cpuAvg { get; set; }
        public string systemTime { get; set; }
        public bool batteryStatus { get; set; }
        public double sampleSeconds { get; set; }
        public double miningSeconds { get; set; }
        public double inactivityCounterAvg { get; set; }
        public DisplayCount countScreenSaver { get; set; }
        public DisplayCount countDisplayOff { get; set; }
        public double onBattery { get; set; }
        public double loggedIn { get; set; }
        public Dictionary<string,string> currPowerProfile { get; set; }
        public double batteryTempAvg { get; set; }
        public double cpuTempAvg { get; set; }
        public string machineId { get; set; }
        public string partnerId { get; set; }
        public string campaignId { get; set; }



    }

    public class DisplayCount
    {
        public int onBattery { get; set; }
        public int plugged { get; set; }
    }


}
