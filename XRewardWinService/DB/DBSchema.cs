using System.Configuration;

namespace Spareio.WinService.DB
{
    public class DBSchema
    {
        public static string DBName = "MineDB.db";
        public static string DBPath = ConfigurationManager.AppSettings["DbPath"];

        public static string MineTable = "Mine";
        public static string MineConfigurationTable = "MineConfig";
        

    }
}
