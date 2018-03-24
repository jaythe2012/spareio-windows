using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Spareio.Installer.Service
{
    public static class RestService
    {
        public static string SendGetRequest(string url, IList<KeyValuePair<string, string>> parameters = null)
        {
            var strUrl = BuildUri(url, parameters);
            var request = (HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "GET";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }

        private static Uri BuildUri(string url, IList<KeyValuePair<string, string>> parameters = null)
        {
            var urlBuilder = new StringBuilder(url);

            if (parameters != null)
            {
                urlBuilder.Append("?");
                bool bAddSeparator = false;
                foreach (var pair in parameters)
                {
                    if (bAddSeparator)
                    {
                        urlBuilder.Append("&");
                    }
                    else
                    {
                        bAddSeparator = true;
                    }
                    urlBuilder.AppendFormat("{0}={1}", pair.Key, pair.Value);
                }
            }

            return new Uri(urlBuilder.ToString());
        }

        public static string SendPostRequest(string url, string body)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var data = Encoding.ASCII.GetBytes(body);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.Proxy = null;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = request.GetResponse().GetResponseStream();

                if (response != null)
                {
                    return new StreamReader(response).ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
