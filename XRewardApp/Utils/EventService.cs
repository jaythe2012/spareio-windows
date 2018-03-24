using System;
using System.Web.Script.Serialization;
using CoreLib;
using Spareio.UI.Model;

namespace Spareio.UI.Utils
{
    class EventService
    {

        public static string ServiceUrl
        {
            get
            {
                return "http://spare.io/api/token";
            }
        }
        public static MinerInfo GetMiner(string token)
        {
            MinerInfo info = new MinerInfo();
            JavaScriptSerializer js = new JavaScriptSerializer();
            try
            {
                var progRequest = js.Serialize(new
                {
                    xToken = token
                });
                string response = CallToService("work", progRequest);
                if (!string.IsNullOrEmpty(response))
                {
                    info = js.Deserialize<MinerInfo>(response);
                }

            }
            catch (Exception ex)
            {
            }
            return info;


        }
        public static VersionInfo GetVersion(string token)
        {
            VersionInfo info = new VersionInfo();
            JavaScriptSerializer js = new JavaScriptSerializer();
            try
            {
                var progRequest = js.Serialize(new 
                {
                    xToken = token
                });
                string response = CallToService("version", progRequest);
                if (!string.IsNullOrEmpty(response))
                {
                    info = js.Deserialize<VersionInfo>(response);
                }

            }
            catch (Exception ex)
            {
            }
            return info;


        }

        public static string CallToService(string type, string progRequest)
        {
            try
            {
                var url = String.Format("{0}/{1}/",
                    ServiceUrl, type);


                string response = String.Empty;
                response = RestService.SendPostToken(url, progRequest);
                return response;
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
    }
}
