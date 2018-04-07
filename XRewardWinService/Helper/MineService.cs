using Newtonsoft.Json;
using Spareio.Business;
using Spareio.WinService.Model;
using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace Spareio.WinService.Helper
{
    public class MineService
    {
        static string mineConfigServiceUrl = ConfigurationManager.AppSettings["MineConfigServiceUrl"].ToString();
        static string mineConfigFilePath = ConfigurationManager.AppSettings["MineConfigFilePath"].ToString();
        public static int MineCounter = 0;
        public static double timeToWorkPerDay = 60000D;

        public static void PingMineServer()
        {
            using (var webClient = new WebClient())
            using (var stream = webClient.OpenRead(mineConfigServiceUrl))
            {
                if (stream != null)
                {
                    using (var file = File.OpenWrite(mineConfigFilePath + @"\mineConfiguration.json"))
                    {
                        var buffer = new byte[1024];
                        int bytesReceived = 0;
                        //stream.ReadTimeout = timeout;

                        while ((bytesReceived = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            file.Write(buffer, 0, bytesReceived);
                        }
                    }
                }
            }
        }

        private static MineJSONModel FetchJSONFromMineConfig()
        {
            var jsonResponse = File.ReadAllText(mineConfigFilePath + @"\mineConfiguration.json");
            return JsonConvert.DeserializeObject<MineJSONModel>(jsonResponse);
        }
        public static bool ReadyToMine()
        {
            //TODO : CHeck MineConfig if mining is disabled from user
            bool result = true;
            var mineModel = FetchJSONFromMineConfig();
            try
            {


                if (mineModel.BatteryCharge != null)
                {
                    var batteryCharge = mineModel.BatteryCharge;
                    result = ValidateComparision((System.Windows.Forms.SystemInformation.PowerStatus.BatteryLifePercent * 100).ToString(), batteryCharge.Comparision, batteryCharge.Value);
                    if (result == false) return result;
                }
                if (mineModel.CPUUsage != null)
                {
                    var cpuUsage = mineModel.CPUUsage;
                    result = ValidateComparision(CpuService.GetCurrentCpuUsage(), cpuUsage.Comparision, cpuUsage.Value);
                    if (result == false) return result;
                }
                if (mineModel.FreeMemory != null)
                {
                    var freeMemory = mineModel.FreeMemory;
                    result = ValidateComparision(MemoryService.GetAvailableMemory(), freeMemory.Comparision, freeMemory.Value);
                    if (result == false) return result;
                }
                //if (mineModel.FullScreenApp != null)
                //{
                //Under R&D
                //}
                //if (mineModel.LastActivity != null)
                //{
                //    //Under R&D
                //}

                //if (mineModel.OnActiveDirectory != null)
                //{
                //    var onActiveDirectory = mineModel.OnActiveDirectory;
                //    result = ValidateComparision(ActiveDirectoryService.IsInDomain().ToString(), onActiveDirectory.Comparision, onActiveDirectory.Value);
                //    if (result == false) return result;
                //}

                if (mineModel.OnBattery != null)
                {
                    var onBattery = mineModel.OnBattery;
                    result = ValidateComparision(PowerService.IsOnBattery().ToString(), onBattery.Comparision, onBattery.Value);
                    if (result == false) return result;
                }

                if (mineModel.OnMeteredConnection != null)
                {
                    var meteredConnection = mineModel.OnMeteredConnection;
                    result = ValidateComparision(ConnectionService.IsMeteredConnection().ToString(), meteredConnection.Comparision, meteredConnection.Value);
                    if (result == false) return result;
                }

                if (mineModel.TimeWorkedToday != null)
                {
                    var timeWorkedToday = mineModel.TimeWorkedToday;
                    timeToWorkPerDay = double.Parse(timeWorkedToday.Value);

                    result = ValidateComparision(MineCounter.ToString(), timeWorkedToday.Comparision, timeWorkedToday.Value);
                    if (result == false) return result;
                }

                var mineConfiguration = MineConfigBL.Get();
                if (mineConfiguration != null)
                    result = (mineConfiguration.IsAppOn && mineConfiguration.IsMiningOn);
                else
                    InitMineConfiguration();

            }

            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        private static void InitMineConfiguration()
        {
            //Initialize Mine Configuration
            MineConfigBL.Initialize(new Spareio.Model.MineConfigModel()
            {
                IsAppOn = true,
                IsMiningOn = false
            });
        }
        private static bool ValidateComparision(string valueToCompare, string comparision, string value, string type = "Int")
        {
            bool result = false;

            NCalc.Expression e;

            switch (comparision)
            {
                case "GT":
                    e = new NCalc.Expression(valueToCompare + " > " + value);
                    result = bool.Parse(e.Evaluate().ToString());
                    break;
                case "LT":
                    e = new NCalc.Expression(valueToCompare + " < " + value);
                    result = bool.Parse(e.Evaluate().ToString());
                    break;
                case "EQ":
                    result = (valueToCompare.ToLower() == value.ToLower());
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
