using LiteDB;
using System;
using System.Linq;
using Spareio.Model;
using Spareio.Data.DB;

namespace Spareio.Data.Mine
{
    public class MineConfigDL
    {

        #region Mine Configuration

        public int Initialize(MineConfigModel minConfigModel)
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

        public int Update(bool? isAppOn, bool? isMiningOn)
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

                if (isMiningOn.HasValue)
                    existingMineConfigModel.IsMiningOn = existingMineConfigModel.IsMiningOn;

                if (isAppOn.HasValue)
                    existingMineConfigModel.IsAppOn = existingMineConfigModel.IsAppOn;

                xRewardDetails.Update(existingMineConfigModel);

                return result;
            }

        }

        public MineConfigModel Get()
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
                return null;
            }
        }


        #endregion

    }
}
