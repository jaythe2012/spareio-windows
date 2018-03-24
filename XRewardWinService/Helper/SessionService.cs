using System;
using CoreLib;

namespace Spareio.WinService.Helper
{
    public class SessionService
    {
        public static void HandleLogOff()
        {
            DateTime now = DateTime.Now;
            string lastloginTime = XmlHelper.ReadSetting(VariableConstants.LastLoggedInTime);
            DateTime dateValue;
            if (DateTime.TryParse(lastloginTime, out dateValue))
            {
                double diffInSeconds = (now - dateValue).TotalSeconds;
                string totalLoggedInSeconds = XmlHelper.ReadSetting(VariableConstants.TotalLoggedInSeconds);
                int LoggedInSeconds = 0;
                Int32.TryParse(totalLoggedInSeconds, out LoggedInSeconds);
                LoggedInSeconds = LoggedInSeconds + Convert.ToInt32(diffInSeconds);
                XmlHelper.UpdateSetting(VariableConstants.TotalLoggedInSeconds,LoggedInSeconds.ToString());
                XmlHelper.UpdateSetting(VariableConstants.IsLoggedIn, "False");
            }

        }

        internal static void HandleLogIn()
        {
            XmlHelper.UpdateSetting(VariableConstants.LastLoggedInTime, DateTime.Now.ToString());
            XmlHelper.UpdateSetting(VariableConstants.IsLoggedIn, "True");
        }
    }
}
