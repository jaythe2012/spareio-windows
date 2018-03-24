using System.ServiceModel;

namespace Spareio.UI.WcfHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class LocalyHostedService : ILocalyHostedService
    {
        public string GetInfo()
        {
            return ServiceController.Instance.GetInfo();
            //throw new NotImplementedException();
        }

        public void StartMonitoring()
        {
            ServiceController.Instance.CleanUpData();
        }
    }
}
