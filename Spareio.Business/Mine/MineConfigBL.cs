using System;
using System.Linq;
using Spareio.Data.Mine;
using Spareio.Model;

namespace Spareio.Business
{
    public class MineConfigBL
    {
        private static Object _thisLock = new Object();
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(MineConfigBL));

        #region Mine Configuration

        public static int Initialize(MineConfigModel mineConfigModel)
        {
            try
            {
                lock (_thisLock)
                {
                    MineConfigDL mineConfigDL = new MineConfigDL();
                    return mineConfigDL.Initialize(mineConfigModel);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while Initialize " + ex.Message);
                return 0;
            }
        }

        public static int Update(bool? isAppOn, bool? isMiningOn)
        {
            try
            {
                lock (_thisLock)
                {
                    MineConfigDL mineConfigDL = new MineConfigDL();
                    return mineConfigDL.Update(isAppOn, isMiningOn);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while Update " + ex.Message);
                return 0;
            }
        }

        public static MineConfigModel Get()
        {
            try
            {
                lock (_thisLock)
                {
                    MineConfigDL mineConfigDL = new MineConfigDL();
                    return mineConfigDL.Get();
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while reading table " + ex.Message);
                return null;
            }
        }


        #endregion

    }
}
