using System;

namespace Spareio.Data.DB
{
    public class DBHelper
    {
        private static Object _thisLock = new Object();
         
        #region General Function

        public static bool CreateDBDirectory()
        {
            try
            {
                System.IO.Directory.CreateDirectory(DBSchema.DBPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        
    }



}
