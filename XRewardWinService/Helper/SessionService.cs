using System;
using Spareio.WinService.DB;

namespace Spareio.WinService.Helper
{
    public class SessionService
    {
        public static void HandleLogOff()
        {
            DateTime now = DateTime.Now;
            string lastloginTime = DBHelper.GetValById(VariableConstants.LastLoggedInTime);
            DateTime dateValue;
            if (DateTime.TryParse(lastloginTime, out dateValue))
            {
                double diffInSeconds = (now - dateValue).TotalSeconds;
                string totalLoggedInSeconds = DBHelper.GetValById(VariableConstants.TotalLoggedInSeconds);
                int LoggedInSeconds = 0;
                Int32.TryParse(totalLoggedInSeconds, out LoggedInSeconds);
                LoggedInSeconds = LoggedInSeconds + Convert.ToInt32(diffInSeconds);
                DBHelper.Update(VariableConstants.TotalLoggedInSeconds,LoggedInSeconds.ToString());
                DBHelper.Update(VariableConstants.IsLoggedIn, "False");
            }

        }

        internal static void HandleLogIn()
        {
            DBHelper.Update(VariableConstants.LastLoggedInTime, DateTime.Now.ToString());
            DBHelper.Update(VariableConstants.IsLoggedIn, "True");
        }
    }
}
