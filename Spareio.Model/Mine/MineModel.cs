

namespace Spareio.Model
{
    public class MineModel
    {
        public int Id { get; set; }

        public string InitTime { get; set; }

        public string xToken { get; set; }

        public string MonitorStartTime { get; set; }

        public string LastLoggedInTime { get; set; }

        public string TotalLoggedInSeconds { get; set; }

        public string IsLoggedIn { get; set; }

        public string CpuTotal { get; set; }

        public string CpuCount { get; set; }

        public string IsOnBattery { get; set; }

        public string TotalBatteryTime { get; set; }

        public string LastBatteryOnTime { get; set; }

    }
}
