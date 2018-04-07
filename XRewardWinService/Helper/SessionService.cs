using System;
using Spareio.Business;


namespace Spareio.WinService.Helper
{
    public class SessionService
    {
        public static void HandleLogOff()
        {
            DateTime now = DateTime.Now;
            string lastloginTime = MineBL.GetValById(VariableConstants.LastLoggedInTime);
            DateTime dateValue;
            if (DateTime.TryParse(lastloginTime, out dateValue))
            {
                double diffInSeconds = (now - dateValue).TotalSeconds;
                string totalLoggedInSeconds = MineBL.GetValById(VariableConstants.TotalLoggedInSeconds);
                int LoggedInSeconds = 0;
                Int32.TryParse(totalLoggedInSeconds, out LoggedInSeconds);
                LoggedInSeconds = LoggedInSeconds + Convert.ToInt32(diffInSeconds);
                MineBL.Update(VariableConstants.TotalLoggedInSeconds,LoggedInSeconds.ToString());
                MineBL.Update(VariableConstants.IsLoggedIn, "False");
            }

        }

        internal static void HandleLogIn()
        {
            MineBL.Update(VariableConstants.LastLoggedInTime, DateTime.Now.ToString());
            MineBL.Update(VariableConstants.IsLoggedIn, "True");
        }
    }
}
