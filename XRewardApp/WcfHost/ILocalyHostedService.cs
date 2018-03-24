using System.ServiceModel;
using System.ServiceModel.Web;

namespace Spareio.UI.WcfHost
{
    [ServiceContract]
    public interface ILocalyHostedService
    {
        [OperationContract]
        [WebGet(UriTemplate = "getInfo/", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        string GetInfo();

        [OperationContract]
        [WebGet(UriTemplate = "monitor/", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        void StartMonitoring();

    }
}
