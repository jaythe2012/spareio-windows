using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spareio.WinService.Helper
{
   public class ConnectionService
    {
        public static bool IsMeteredConnection()
        {
           // Windows.Networking.Connectivity
            return false;
            //var connectionCost = NetworkInformation.GetInternetConnectionProfile().GetConnectionCost();
            //if (connectionCost.NetworkCostType == NetworkCostType.Unknown
            //        || connectionCost.NetworkCostType == NetworkCostType.Unrestricted)
            //{
            //    //Connection cost is unknown/unrestricted
            //}
            //else
            //{
            //    //Metered Network
            //}
        }
    }
}
