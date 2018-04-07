using Spareio.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spareio.Business
{
    public class MineBL
    {
        public static int CurrentRewardId = 0;
        private static Object _thisLock = new Object();
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(MineBL));

        #region Mine

        public static int Initialize(string initTime)
        {
            try
            {
                lock (_thisLock)
                {
                    MineDL mineDL = new MineDL();
                    return mineDL.Initialize(initTime);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while Initialize " + ex.Message);
                return 0;
            }
        }

        public static int InitializeInBulk(Dictionary<string, string> keyValDictionary)
        {

            try
            {
                lock (_thisLock)
                {
                    MineDL mineDL = new MineDL();
                    return mineDL.InitializeInBulk(keyValDictionary);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while inserting in bulk " + ex.Message + "For key " + keyValDictionary.ToString());
                return 0;
            }

        }

        public static int Update(string key, string value)
        {
            try
            {
                lock (_thisLock)
                {
                    MineDL mineDL = new MineDL();
                    return mineDL.Update(CurrentRewardId, key, value);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while Update " + ex.Message + " for key:=" + key + " & value:=" + value);
                return 0;
            }

        }

        public static bool UpdateInBulk(Dictionary<string, string> keyValDictionary)
        {
            try
            {
                lock (_thisLock)
                {
                    MineDL mineDL = new MineDL();
                    return mineDL.UpdateInBulk(CurrentRewardId, keyValDictionary);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while updating in bulk " + ex.Message + "For key " + keyValDictionary.ToString());
                return false;
            }
        }

        public static MineModel Get()
        {
            try
            {
                lock (_thisLock)
                {
                    MineDL mineDL = new MineDL();
                    return mineDL.Get(CurrentRewardId);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while reading table " + ex.Message);
                return null;
            }
        }

        public static string GetValById(string key)
        {
            try
            {
                lock (_thisLock)
                {
                    MineDL mineDL = new MineDL();
                    return mineDL.GetValById(CurrentRewardId, key);
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while reading " + ex.Message + "For key " + key);
                return string.Empty;
            }
        }

        #endregion


    }
}
