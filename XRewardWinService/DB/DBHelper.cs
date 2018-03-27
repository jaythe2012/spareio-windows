using LiteDB;
using Spareio.WinService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Spareio.WinService.DB
{
    public class DBHelper
    {

        static string tableName = "xReward";
        static string dbPath = ConfigurationManager.AppSettings["DbPath"];
        private static Object _thisLock = new Object();
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(DBHelper));

        public static int _currentRewardId = 0;

        public static int Initialize(string initTime)
        {
            int result = 0;
            if (CreateDBDirectory() == false) return result;

            // Open database (or create if not exits)
            using (var db = new LiteDatabase(dbPath + "\\xreward.db"))
            {
                var xRewadModelDetails = db.GetCollection<XRewardModel>(tableName);

                XRewardModel xRewardModel = new XRewardModel();
                xRewardModel.InitTime = initTime;
                result = xRewadModelDetails.Insert(xRewardModel);

                return result;
            }
        }
        public static int InitializeInBulk(Dictionary<string, string> keyValDictionary)
        {
            var result = 0;

            try
            {
                var xRewardModel = new XRewardModel();
                lock (_thisLock)
                {
                    keyValDictionary.ToList().ForEach(f =>
                    {
                        xRewardModel = BuildxRewardModel(f.Key, f.Value, xRewardModel);

                    });


                    using (var db = new LiteDatabase(dbPath + "\\xreward.db"))
                    {
                        var xRewardDetails = db.GetCollection<XRewardModel>("xRewadModeldetails");
                        result = xRewardDetails.Insert(xRewardModel);
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
            if (CreateDBDirectory() == false) return result;

            // Open database (or create if not exits)
            using (var db = new LiteDatabase(dbPath + "\\xreward.db"))
            {
                //Get Existing Value
                var existingxRewardModel = Get();

                //Get Table
                var xRewardDetails = db.GetCollection<XRewardModel>("xRewadModeldetails");
                XRewardModel xRewardModel = BuildxRewardModel(key, value, existingxRewardModel);
                xRewardDetails.Update(xRewardModel);
                
                return result;
            }

        }

        public static void UpdateInBulk(Dictionary<string, string> keyValDictionary)
        {
            try
            {
                var existingxRewardModel = Get();
                var xRewardModel = new XRewardModel();

                lock (_thisLock)
                {
                    keyValDictionary.ToList().ForEach(f =>
                    {
                        xRewardModel = BuildxRewardModel(f.Key, f.Value, existingxRewardModel);

                    });


                    using (var db = new LiteDatabase(dbPath + "\\xreward.db"))
                    {
                        var xRewardDetails = db.GetCollection<XRewardModel>("xRewadModeldetails");
                        xRewardDetails.Update(xRewardModel);
                    }

                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while updating in bulk " + ex.Message + "For key " + keyValDictionary.ToString());
            }
        }

        public static XRewardModel Get()
        {
            try
            {
                if (CreateDBDirectory() == false) return null;
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(dbPath + "\\xreward.db"))
                {
                    var xRewadModelDetails = db.GetCollection<XRewardModel>(tableName);
                    return xRewadModelDetails.Find(x => x.Id == _currentRewardId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logWriter.Error("Error while reading table " + ex.Message);
                return null;
            }
        }


        private static XRewardModel BuildxRewardModel(string key, string value, XRewardModel existingModel)
        {

            switch (key)
            {
                case "initTime":
                    existingModel.InitTime = value;
                    break;
                case "xToken":
                    existingModel.XToken = value;
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
                    existingModel.XToken = value;
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

        private static bool CreateDBDirectory()
        {
            try
            {
                System.IO.Directory.CreateDirectory(dbPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static string GetValById(string key)
        {
            string result = String.Empty;

            try
            {
                lock (_thisLock)
                {
                    var existingxRewardModel = Get();
                    if (existingxRewardModel != null)
                    {
                        var t = existingxRewardModel.GetType().GetField(key);
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
    }



}
