using LiteDB;
using Spareio.WinService.DB;
using Spareio.WinService.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spareio.WinService.Business
{
    public class MineBL
    {
        public static int CurrentRewardId = 0;
        private static Object _thisLock = new Object();
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(MineBL));

        #region Mine

        public static int Initialize(string initTime)
        {
            int result = 0;
            if (DBHelper.CreateDBDirectory() == false) return result;

            // Open database (or create if not exits)
            using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
            {
                var xRewadModelDetails = db.GetCollection<MineModel>(DBSchema.MineTable);

                MineModel MineModel = new MineModel();
                MineModel.InitTime = initTime;

                xRewadModelDetails.EnsureIndex(x => x.Id, true);
                result = xRewadModelDetails.Insert(MineModel);

                return result;
            }
        }

        public static int InitializeInBulk(Dictionary<string, string> keyValDictionary)
        {
            var result = 0;

            try
            {
                var MineModel = new MineModel();
                lock (_thisLock)
                {
                    keyValDictionary.ToList().ForEach(f =>
                    {
                        MineModel = BuildMineModel(f.Key, f.Value, MineModel);

                    });

                    using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
                    {
                        var xRewardDetails = db.GetCollection<MineModel>(DBSchema.MineTable);
                        xRewardDetails.EnsureIndex(x => x.Id, true);

                        result = xRewardDetails.Insert(MineModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while inserting in bulk " + ex.Message + "For key " + keyValDictionary.ToString());
            }

            return result;
        }

        public static int Update(string key, string value)
        {
            int result = 0;
            if (DBHelper.CreateDBDirectory() == false) return result;

            // Open database (or create if not exits)
            using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
            {
                //Get Existing Value
                var existingMineModel = Get();

                //Get Table
                var xRewardDetails = db.GetCollection<MineModel>(DBSchema.MineTable);
                MineModel mineModel = BuildMineModel(key, value, existingMineModel);
                xRewardDetails.Update(mineModel);

                return result;
            }

        }

        public static void UpdateInBulk(Dictionary<string, string> keyValDictionary)
        {
            try
            {
                var existingMineModel = Get();
                var mineModel = new MineModel();

                lock (_thisLock)
                {
                    keyValDictionary.ToList().ForEach(f =>
                    {
                        mineModel = BuildMineModel(f.Key, f.Value, existingMineModel);

                    });


                    using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
                    {
                        var xRewardDetails = db.GetCollection<MineModel>(DBSchema.MineTable);
                        xRewardDetails.Update(mineModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while updating in bulk " + ex.Message + "For key " + keyValDictionary.ToString());
            }
        }

        public static MineModel Get()
        {
            try
            {
                if (DBHelper.CreateDBDirectory() == false) return null;
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
                {
                    var xRewardDetails = db.GetCollection<MineModel>(DBSchema.MineTable);
                    return xRewardDetails.Find(x => x.Id == CurrentRewardId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while reading table " + ex.Message);
                return null;
            }
        }

        private static MineModel BuildMineModel(string key, string value, MineModel existingModel)
        {

            switch (key)
            {
                case "initTime":
                    existingModel.InitTime = value;
                    break;
                case "xToken":
                    existingModel.xToken = value;
                    break;
                case "MonitorStartTime":
                    existingModel.MonitorStartTime = value;
                    break;
                case "LastLoggedInTime":
                    existingModel.LastLoggedInTime = value;
                    break;
                case "TotalLoggedInSeconds":
                    existingModel.TotalLoggedInSeconds = value;
                    break;
                case "IsLoggedIn":
                    existingModel.IsLoggedIn = value;
                    break;
                case "CpuTotal":
                    existingModel.CpuTotal = value;
                    break;
                case "CpuCount":
                    existingModel.CpuCount = value;
                    break;
                case "IsOnBattery":
                    existingModel.IsOnBattery = value;
                    break;
                case "TotalBatteryTime":
                    existingModel.TotalBatteryTime = value;
                    break;
                case "LastBatteryOnTime":
                    existingModel.LastBatteryOnTime = value;
                    break;
                default:
                    break;
            }


            return existingModel;
        }

        public static string GetValById(string key)
        {
            string result = String.Empty;

            try
            {
                lock (_thisLock)
                {
                    var existingMineModel = Get();
                    if (existingMineModel != null)
                    {
                        result = existingMineModel.GetType().GetProperty(key).GetValue(existingMineModel, null).ToString(); //existingMineModel.GetType().GetField(key);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while reading " + ex.Message + "For key " + key);
                return result;
            }
        }

        #endregion


    }
}
