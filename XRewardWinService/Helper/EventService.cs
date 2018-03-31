using CoreLib;
using System;
 

namespace Spareio.WinService.Helper
{
   
    public class EventService
    {
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(EventService));

        public static string EventUrl
        {
            get
            {
                return "https://flow.lavasoft.com/v1/event-stat";
                //return "https://falanu.lavasoft11.com/v1/event-stat";

            }
        }

        public static void SendReport(string type, string id, object progRequest, int version)
        {
            try
            {
                var url = String.Format("{0}?Type={1}&ProductID={2}&EventVersion={3}",
                    EventUrl, type, id, version);

                _logWriter.Info("Sending Event on " +url);

                RestService.SendPostRequest(url, String.Format(@"{{""Data"": {0}}}", progRequest));
            }
            catch (Exception ex)
            {
                _logWriter.Error("failed in sending event" + ex);
            }
        }
    }
}
