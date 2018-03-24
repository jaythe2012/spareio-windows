using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Spareio.UI.WcfHost
{
    [Serializable]
    class ResponseTemplate
    {
        public Dictionary<string, string> data { get; set; }

    }

    class ResponseObject
    {
        public static string GetValue( Dictionary<string, string> data = null)
        {
            //var response = new JsonSerializer(code, message, data);
            //var template = new ResponseTemplate
            //{
            //    data = data
            //};

            var json = new JavaScriptSerializer().Serialize(data);
            //json = json.Replace(@"\\", string.Empty);
            return json;
        }
    }
}
