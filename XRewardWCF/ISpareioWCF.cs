using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Spareio.WCF
{
    [ServiceContract]
    public interface ISpareioWCF
    {
        [OperationContract]
        int RunProcess(string fullPath, string args);
    }
}
