using System.Linq;

namespace Spareio.Installer.Utils
{
    class ServiceUtils
    {
        internal static bool isServiceExist(string _name)
        {
            System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == _name);
            bool flag = service != null;
            return service != null;

        }
    }
}
