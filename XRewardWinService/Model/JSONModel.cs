using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.WinService.Model
{
    public class CpuUsage
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class FreeMemory
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class OnBattery
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class BatteryCharge
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class LastActivity
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class FullScreenApp
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class OnActiveDirectory
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class OnMeteredConnection
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class TimeWorkedToday
    {
        [JsonProperty(PropertyName = "comparision")]
        public string Comparision { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class MineModel
    {
        [JsonProperty(PropertyName = "cpuUsage")]
        public CpuUsage CPUUsage { get; set; }

        [JsonProperty(PropertyName = "freeMemory")]
        public FreeMemory FreeMemory { get; set; }

        [JsonProperty(PropertyName = "onBattery")]
        public OnBattery OnBattery { get; set; }

        [JsonProperty(PropertyName = "batteryCharge")]
        public BatteryCharge BatteryCharge { get; set; }

        [JsonProperty(PropertyName = "lastActivity")]
        public LastActivity LastActivity { get; set; }

        [JsonProperty(PropertyName = "fullScreenApp")]
        public FullScreenApp FullScreenApp { get; set; }

        [JsonProperty(PropertyName = "onActiveDirectory")]
        public OnActiveDirectory OnActiveDirectory { get; set; }

        [JsonProperty(PropertyName = "onMeteredConnection")]
        public OnMeteredConnection OnMeteredConnection { get; set; }

        [JsonProperty(PropertyName = "timeWorkedToday")]
        public TimeWorkedToday TimeWorkedToday { get; set; }
    }
}
