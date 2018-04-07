using LiteDB;
using Spareio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Spareio.Data.DB;

namespace Spareio.Business
{
    public class MineDL
    {
        private static Object _thisLock = new Object();

        #region Mine

        public int Initialize(string initTime)
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

        public int InitializeInBulk(Dictionary<string, string> keyValDictionary)
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
            catch (Exception ex) { }
            return result;
        }

        public int Update(int id, string key, string value)
        {
            int result = 0;
            if (DBHelper.CreateDBDirectory() == false) return result;

            // Open database (or create if not exits)
            using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
            {
                //Get Existing Value
                var existingMineModel = Get(id);

                //Get Table
                var xRewardDetails = db.GetCollection<MineModel>(DBSchema.MineTable);
                MineModel mineModel = BuildMineModel(key, value, existingMineModel);
                xRewardDetails.Update(mineModel);

                return result;
            }

        }

        public bool UpdateInBulk(int id, Dictionary<string, string> keyValDictionary)
        {
            try
            {
                var existingMineModel = Get(id);
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

                    return true;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public MineModel Get(int id)
        {
            try
            {
                if (DBHelper.CreateDBDirectory() == false) return null;
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
                {
                    var xRewardDetails = db.GetCollection<MineModel>(DBSchema.MineTable);
                    return xRewardDetails.Find(x => x.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private MineModel BuildMineModel(string key, string value, MineModel existingModel)
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

        public string GetValById(int id, string key)
        {
            string result = String.Empty;

            try
            {
                lock (_thisLock)
                {
                    var existingMineModel = Get(id);
                    if (existingMineModel != null)
                    {
                        result = existingMineModel.GetType().GetProperty(key).GetValue(existingMineModel, null).ToString(); //existingMineModel.GetType().GetField(key);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        #endregion


    }
}
