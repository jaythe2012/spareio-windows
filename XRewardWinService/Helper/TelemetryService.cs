using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;
using Spareio.WinService.DB;
using Spareio.WinService.Model;

namespace Spareio.WinService.Helper
{
    class TelemetryService
    {
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(TelemetryService));


        #region Static Fields
        private static string HourlyType = "HourlyActivity";
        private static string ProductId = "HVstage";
        private static int APIVersion = 1;
        private static int EventVersion = 0;
        #endregion

        #region DynamicFields
        private static string _xToken = String.Empty;
        private static string _lastTrigger = "interval";
        private static double _cpuAvg = 0.0;
        private static string _systemTime = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");
        private static double _sampleSeconds = 3600;
        private static double _miningSeconds = 0;
        private static double _inactivityCounterAvg = 0;
        private static double _onBattery = 0;
        private static double _loggedIn = 0;
        private static Dictionary<string,string> _powerProfile = new Dictionary<string, string>();
        private static ResponseInfo info = new ResponseInfo();
        private static DisplayCount _screenSaver = new DisplayCount();
        private static DisplayCount _displayOff = new DisplayCount();
        private static string _partnerId = "";
        private static string _campaignId = "";
        private static string _machineId = "";



        #endregion

        internal static string Company { get { return "Spareio"; } }
        internal static string Product { get { return "SpareioApp"; } }

        internal static void SendInfo(string trigger)
        {
            PrepareData();
            SendData(trigger);
        }

        private static void PrepareData()
        {
            GetInfoFromApp();
            CountSampleSeconds();
            CountCpuAverage();
            CountloggedInSeconds();
            CountBatterySeconds();
            GetInactivitySeconds();
            GetRegKeys();
            GetPowerProfile();
        }

        private static void GetInfoFromApp()
        {
            try
            {
                HttpWebRequest request =
                        (HttpWebRequest)WebRequest.Create("http://localhost:9007/spareio/getInfo/");
                request.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string str = string.Empty;
                    if (response.StatusCode != HttpStatusCode.OK)
                        _logWriter.InfoFormat(string.Format("Request failed. Received HTTP {0}", (object)response.StatusCode));
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream))
                                str = streamReader.ReadToEnd();
                        }
                    }
                    object abc = new JavaScriptSerializer().Deserialize<object>(str);
                    ResponseInfo jsonInfo = new JavaScriptSerializer().Deserialize<ResponseInfo>(abc.ToString());
                    info = jsonInfo;
                    _screenSaver.onBattery = Convert.ToInt32(info.SSBattery);
                    _screenSaver.plugged = Convert.ToInt32(info.SSPluggedIn);
                    _displayOff.onBattery = Convert.ToInt32(info.DOBattery);
                    _displayOff.plugged = Convert.ToInt32(info.DOPluggedIn);
                    _inactivityCounterAvg = Convert.ToDouble(info.InactivityCount);
                    _logWriter.Info("_inactivityCounterAvg" + jsonInfo.InactivityCount);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while getting info from App" +ex.Message);
                _screenSaver.onBattery = 0;
                _screenSaver.plugged = 0;
                _displayOff.onBattery = 0;
                _displayOff.plugged = 0;
                _inactivityCounterAvg = 0;
            }

        }

        private static void GetPowerProfile()
        {
            try
            {
                _powerProfile = PowerEnumerator.GetCurrentPowerEnumerateVistaAPI();
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while getting power profile " + ex.Message);
            }
        }

        private static void GetRegKeys()
        {
            try
            {
                // REST call to app to get data
                var key_name = String.Format(@"SOFTWARE\{0}\{1}", Company, Product);

                using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(key_name))
                {
                    if (parent != null)
                    {
                        var obj = parent.GetValue("PartnerId");
                        if (obj != null)
                            _partnerId = (string)obj;
                        var obj2 = parent.GetValue("CampaignId");
                        if (obj2 != null)
                            _campaignId = (string)obj2;
                        var obj3 = parent.GetValue("MachineId");
                        if (obj3 != null)
                            _machineId = (string)obj3;
                    }
                }

            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while getting partner, campaign, machineId " + ex.Message);
            }
        }

        private static void CountBatterySeconds()
        {
            try
            {
                if (PowerService.IsOnBattery())
                {
                    DateTime now = DateTime.Now;
                    string lastBatteryOnTime = DBHelper.GetValById(VariableConstants.LastBatteryOnTime);
                    if (!String.IsNullOrEmpty(lastBatteryOnTime))
                    {
                        DateTime dateValue;
                        if (DateTime.TryParse(lastBatteryOnTime, out dateValue))
                        {
                            double diffInSeconds = (now - dateValue).TotalSeconds;
                            string totalBatterySeconds = DBHelper.GetValById(VariableConstants.TotalBatteryTime);
                            int onBatterySeconds = 0;
                            if (!String.IsNullOrEmpty(totalBatterySeconds))
                                Int32.TryParse(totalBatterySeconds, out onBatterySeconds);
                            onBatterySeconds = onBatterySeconds + Convert.ToInt32(diffInSeconds);
                            _onBattery = onBatterySeconds;
                        }
                        else
                        {
                            _onBattery = 0.0;
                        }
                    }
                }
                else
                {
                    string totalBatterySeconds = DBHelper.GetValById(VariableConstants.TotalBatteryTime);
                    int BatterySeconds = 0;
                    Int32.TryParse(totalBatterySeconds, out BatterySeconds);
                    _onBattery = BatterySeconds;
                }

            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while counting battery seconds " + ex.Message);
            }
        }

        private static void GetInactivitySeconds()
        {
            try
            {
                // Rest Call to app to get data
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while counting Inactivity seconds " + ex.Message);
            }
        }

        private static void CountloggedInSeconds()
        {
            try
            {
                if (bool.Parse(DBHelper.GetValById(VariableConstants.IsLoggedIn)))
                {
                    DateTime now = DateTime.Now;
                    string lastloginTime = DBHelper.GetValById(VariableConstants.LastLoggedInTime);
                    DateTime dateValue;
                    if (DateTime.TryParse(lastloginTime, out dateValue))
                    {
                        double diffInSeconds = (now - dateValue).TotalSeconds;
                        string totalLoggedInSeconds = DBHelper.GetValById(VariableConstants.TotalLoggedInSeconds);
                        int LoggedInSeconds = 0;
                        Int32.TryParse(totalLoggedInSeconds, out LoggedInSeconds);
                        LoggedInSeconds = LoggedInSeconds + Convert.ToInt32(diffInSeconds);
                        _loggedIn = LoggedInSeconds;
                    }
                }
                else
                {
                    string totalLoggedInSeconds = DBHelper.GetValById(VariableConstants.TotalLoggedInSeconds);
                    int LoggedInSeconds = 0;
                    Int32.TryParse(totalLoggedInSeconds, out LoggedInSeconds);
                    _loggedIn = LoggedInSeconds;
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while counting loggedIn seconds " + ex.Message);
            }
        }

        private static void CountSampleSeconds()
        {
            try
            {
                _systemTime = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");
                DateTime stopTime = DateTime.Now;
                string stratTime = DBHelper.GetValById(VariableConstants.MonitorStartTime);
                DateTime startTime = Convert.ToDateTime(stratTime);
                double diffInSeconds = (stopTime - startTime).TotalSeconds;

                if (diffInSeconds < 0 || diffInSeconds > 600)
                    _sampleSeconds = 600;
                else
                    _sampleSeconds = diffInSeconds;
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while counting sample seconds " + ex.Message);
            }
        }

        private static void CountCpuAverage()
        {
            try
            {
                string cpuTotal = DBHelper.GetValById(VariableConstants.CpuTotal);
                if (string.IsNullOrEmpty(cpuTotal))
                    cpuTotal = "0.0";
                string cpuCount = DBHelper.GetValById(VariableConstants.CpuCount);
                if (string.IsNullOrEmpty(cpuCount))
                    cpuCount = "0";
                if (cpuCount == "0" || cpuTotal == "0.0")
                {
                    _logWriter.Info("Cpu average has zero value count -- " + cpuCount.ToString() + "toatl --" +
                                   cpuTotal.ToString());
                }
                else
                {
                    double cpuTotalNumber = Convert.ToDouble(cpuTotal);
                    double cpuCountNumber = Convert.ToDouble(cpuCount);
                    _cpuAvg = cpuTotalNumber / cpuCountNumber;
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while counting cpu average " +ex.Message);
            }
        }

        private static void SendData(string trigger)
        {
           
            try
            {
                var progRequest = JsonConvert.SerializeObject(new HourlyActivity()
                {
                    xToken = DBHelper.GetValById(VariableConstants.xToken),
                    lastTrigger = trigger,
                    cpuAvg = _cpuAvg,
                    systemTime = _systemTime,
                    batteryStatus = PowerService.IsOnBattery(),
                    sampleSeconds = _sampleSeconds,
                    miningSeconds = 0,
                    inactivityCounterAvg = _inactivityCounterAvg,
                    countScreenSaver = _screenSaver,
                    countDisplayOff = _displayOff,
                    onBattery = _onBattery,
                    currPowerProfile = _powerProfile,
                    loggedIn = _loggedIn,
                    batteryTempAvg = 0.0,
                    cpuTempAvg = 0.0,
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    machineId = _machineId
                });


                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                _logWriter.Info("Data to be sent -- " + body);
                EventService.SendReport(HourlyType, ProductId, progRequest, APIVersion);
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while preparing Data " + ex.Message);
            }

        }
    }
}
