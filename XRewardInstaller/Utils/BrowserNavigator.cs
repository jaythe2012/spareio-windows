using System.Diagnostics;

namespace Spareio.Installer.Utils
{
    public class BrowserNavigator
    {
        public static void Navigate(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (System.Exception ex)
            {

            }
        }
    }
}
