namespace Spareio.WinService.Model
{
    class ResponseInfo
    {
        public string SSBattery { get; set; } = "0";
        public string SSPluggedIn { get; set; } = "0";
        public string DOBattery { get; set; } = "0";
        public string DOPluggedIn { get; set; } = "0";
        public string InactivityCount { get; set; } = "0";
    }
}
