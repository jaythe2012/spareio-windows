using System.ServiceProcess;

namespace Spareio.WinService
{
    static class Program 
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

#if (DEBUG)

            XRewardService xRewardService = new XRewardService();
            xRewardService.OnDebug();

#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new XRewardService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }

      
    }
}
