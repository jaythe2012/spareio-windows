using LiteDB;
using Spareio.WinService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using Spareio.WinService.Model;

namespace Spareio.WinService.Business
{
    public class MineConfigBL
    {
        private static Object _thisLock = new Object();
        private static readonly log4net.ILog _logWriter = log4net.LogManager.GetLogger(typeof(MineBL));

        #region Mine Configuration

        public static int Initialize(MineConfigModel minConfigModel)
        {
            int result = 0;
            if (DBHelper.CreateDBDirectory() == false) return result;

            // Open database (or create if not exits)
            using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
            {
                var xRewadModelDetails = db.GetCollection<MineConfigModel>(DBSchema.MineConfigurationTable);

                MineConfigModel MineConfigModel = new MineConfigModel();
                MineConfigModel.IsAppOn = minConfigModel.IsAppOn;
                MineConfigModel.IsMiningOn = minConfigModel.IsMiningOn;

                xRewadModelDetails.EnsureIndex(x => x.Id, true);
                result = xRewadModelDetails.Insert(MineConfigModel);

                return result;
            }
        }

        public static int Update(MineConfigModel mineConfigModel)
        {
            int result = 0;
            if (DBHelper.CreateDBDirectory() == false) return result;

            // Open database (or create if not exits)
            using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
            {
                //Get Existing Value
                var existingMineConfigModel = Get();

                //Get Table
                var xRewardDetails = db.GetCollection<MineConfigModel>(DBSchema.MineConfigurationTable);
                if (existingMineConfigModel.IsMiningOn != mineConfigModel.IsMiningOn)
                    mineConfigModel.IsMiningOn = existingMineConfigModel.IsMiningOn;

                if (existingMineConfigModel.IsAppOn != mineConfigModel.IsAppOn)
                    mineConfigModel.IsAppOn = existingMineConfigModel.IsAppOn;

                mineConfigModel.Id = existingMineConfigModel.Id;
                xRewardDetails.Update(mineConfigModel);

                return result;
            }

        }

        public static MineConfigModel Get()
        {
            try
            {
                if (DBHelper.CreateDBDirectory() == false) return null;
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(DBSchema.DBPath + "\\" + DBSchema.DBName))
                {
                    var xRewardDetails = db.GetCollection<MineConfigModel>(DBSchema.MineConfigurationTable);
                    return xRewardDetails.FindAll().FirstOrDefault();
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
