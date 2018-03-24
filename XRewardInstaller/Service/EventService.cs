using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Script.Serialization;
using Spareio.Installer.Helper;
using Spareio.Installer.Utils;

namespace Spareio.Installer.Service
{
    public class EventService
    {
        private static string _partnerId;
        private static string _campaignId;
        private static string _machineId;
        private static string _xToken;
        private static string _installDate;

        public static string EventUrl
        {
            get
            {
                return "https://flow.lavasoft.com/v1/event-stat";
            }
        }
        private static string CompleteInstallType = "CompleteInstall";
        private static string ErrorInstallType = "ErrorInstall";
        private static string CompleteUpdateType = "CompleteUpdate";
        private static string ErrorUpdateType = "ErrorUpdate";
        private static string CompleteUninstallType = "CompleteUninstall";
        private static string ErrorUninstallType = "ErrorUninstall";

        private static string ProductId = "HVstage";
        private static int APIVersion = 1;
        internal static void Initialize(string partnerId, string campaignId, string xToken)
        {
            _partnerId = partnerId;
            _campaignId = campaignId;
            _xToken = xToken;
            _machineId = InstallUtils.ReadMachineGuid();
            _installDate = InstallUtils.ReadValue("installDate");
        }

        internal static void SendComplteInstallEvent(CompleteInstallInfo completeInstallInfo)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(completeInstallInfo);

            //Dumping event data into file until we have API ready
            //string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            //string filePath = Path.Combine(folder, @"XRewards\dump.txt");
            //File.WriteAllText(filePath, data);

            try
            {
                var progRequest = js.Serialize(new CompleteInstallInfo()
                {
                    cpuInfo = completeInstallInfo.cpuInfo,
                    gpuInfo = completeInstallInfo.gpuInfo,
                    powerProfile = null,
                    uptimeCurrent = completeInstallInfo.uptimeCurrent,
                    uptimePct = completeInstallInfo.uptimePct,
                    batteyStatus = completeInstallInfo.batteyStatus,
                    batteryUsed = completeInstallInfo.batteryUsed,
                    isAdmin = completeInstallInfo.isAdmin,
                    osVersion = InstallUtils.GetOSVersion(),
                    screenRes = null,
                    machineId = _machineId,
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    timeZone = InstallUtils.GetTimeZone()
                });
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                SendReport(CompleteInstallType, ProductId, progRequest, APIVersion);
            }
            catch (System.Exception ex)
            {
            }
        }

        public static void SendReport(string type, string id, object progRequest, int version)
        {
            try
            {
                var url = String.Format("{0}?Type={1}&ProductID={2}&EventVersion={3}",
                    EventUrl, type, id, version);
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                Trace.WriteLine("Sending Data " + body);
                RestService.SendPostRequest(url, String.Format(@"{{""Data"": {0}}}", progRequest));
            }
            catch (System.Exception ex)
            {
            }
        }

        internal static void VerifyToken()
        {
            string abc = _partnerId + _campaignId + _machineId;
            string xyz = "";
            //throw new NotImplementedException();
        }


        #region NewEventService

        private static EventService _instance = null;
        private static object _sync = new object();
        public static EventService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_sync)
                    {
                        if (_instance == null)
                        {
                            _instance = new EventService();
                        }
                    }
                }

                return _instance;
            }
        }

        internal void SendCompleteInstallEvent(CompleteInstallInfo completeInstallInfo)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(completeInstallInfo);

            try
            {
                var progRequest = js.Serialize(new CompleteInstallInfo()
                {
                    cpuInfo = completeInstallInfo.cpuInfo,
                    gpuInfo = completeInstallInfo.gpuInfo,
                    powerProfile = completeInstallInfo.powerProfile,
                    uptimeCurrent = completeInstallInfo.uptimeCurrent,
                    uptimePct = completeInstallInfo.uptimePct,
                    batteyStatus = completeInstallInfo.batteyStatus,
                    batteryUsed = completeInstallInfo.batteryUsed,
                    isAdmin = completeInstallInfo.isAdmin,
                    osVersion = InstallUtils.GetOSVersion(),
                    screenRes = null,
                    machineId = _machineId,
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    timeZone = InstallUtils.GetTimeZone()
                });
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                SendReport(CompleteInstallType, ProductId, progRequest, APIVersion);
            }
            catch (System.Exception ex)
            {
            }
        }
        internal void SendInstallErrorEvent(ErrorInstallInfo errorInstallInfo)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(errorInstallInfo);

            try
            {
                var progRequest = js.Serialize(new ErrorInstallInfo()
                {
                  
                    ErrorMessage = errorInstallInfo.ErrorMessage,
                    batteryStatus = errorInstallInfo.batteryStatus,
                    batteryUsed = errorInstallInfo.batteryUsed,
                    isAdmin = errorInstallInfo.isAdmin,
                    osVersion = InstallUtils.GetOSVersion(),
                    screenRes = null,
                    machineId = _machineId,
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    sdkVersion = FileHelper.GetCurrentVersion(Assembly.GetExecutingAssembly()),
                    timeZone = InstallUtils.GetTimeZone()
                });
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                SendReport(ErrorInstallType, ProductId, progRequest, APIVersion);
            }
            catch (System.Exception ex)
            {
            }
        }
        internal void SendCompleteUpdateEvent(CompleteUpdateInfo completeUpdateInfo)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(completeUpdateInfo);

            try
            {
                var progRequest = js.Serialize(new CompleteUpdateInfo()
                {
                    
                    isAdmin = completeUpdateInfo.isAdmin,
                    osVersion = InstallUtils.GetOSVersion(),
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    installDate = _installDate,
                    machineId = _machineId,
                    token = _xToken,
                    miners = null,
                    HVVersion = FileHelper.GetCurrentVersion(Assembly.GetExecutingAssembly())
                });
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                SendReport(CompleteUpdateType, ProductId, progRequest, APIVersion);
            }
            catch (System.Exception ex)
            {
            }
        }
        internal void SendUpdateErrorEvent(ErrorUpdateInfo errorUpdateInfo)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(errorUpdateInfo);

            try
            {
                var progRequest = js.Serialize(new ErrorUpdateInfo()
                {
                    isAdmin = errorUpdateInfo.isAdmin,
                    osVersion = InstallUtils.GetOSVersion(),
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    machineId = _machineId,
                    token = _xToken,
                    miners = null,
                    HVVersion = FileHelper.GetCurrentVersion(Assembly.GetExecutingAssembly()),
                    ErrorMessage = errorUpdateInfo.ErrorMessage
                });
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                SendReport(ErrorUpdateType, ProductId, progRequest, APIVersion);
            }
            catch (System.Exception ex)
            {
            }
        }
        internal void SendCompleteUninstallEvent(CompleteUninstallInfo completeUninstallInfo)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(completeUninstallInfo);

            try
            {
                var progRequest = js.Serialize(new CompleteUninstallInfo()
                {
                   
                    isAdmin = completeUninstallInfo.isAdmin,
                    osVersion = InstallUtils.GetOSVersion(),
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    installDate = _installDate,
                    machineId = _machineId,
                    token = _xToken,
                    miners = null,
                    HVVersion = FileHelper.GetCurrentVersion(Assembly.GetExecutingAssembly())
                });
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                SendReport(CompleteUninstallType, ProductId, progRequest, APIVersion);
            }
            catch (System.Exception ex)
            {
            }
        }
        internal void SendUninstallErrorEvent(ErrorUninstallInfo errorUninstallInfo)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(errorUninstallInfo);

            try
            {
                var progRequest = js.Serialize(new ErrorUninstallInfo()
                {
                    isAdmin = errorUninstallInfo.isAdmin,
                    osVersion = InstallUtils.GetOSVersion(),
                    partnerId = _partnerId,
                    campaignId = _campaignId,
                    installDate = _installDate,
                    machineId = _machineId,
                    token = _xToken,
                    miners = null,
                    HVVersion = FileHelper.GetCurrentVersion(Assembly.GetExecutingAssembly()),
                    ErrorMessage = errorUninstallInfo.ErrorMessage
                });
                string body = String.Format(@"{{""Data"": {0}}}", progRequest);
                SendReport(ErrorUninstallType, ProductId, progRequest, APIVersion);
            }
            catch (System.Exception ex)
            {
            }
        }



        #endregion
    }
}
