using System;
using CoreLib;

namespace Spareio.WinService.Helper
{
   
    public class EventService
    {
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

                LogWriter.Info("Sending Event on " +url);

                RestService.SendPostRequest(url, String.Format(@"{{""Data"": {0}}}", progRequest));
            }
            catch (Exception ex)
            {
                LogWriter.Error("failed in sending event" + ex);
            }
        }
    }
}
