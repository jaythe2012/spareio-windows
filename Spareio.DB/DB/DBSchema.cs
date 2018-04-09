﻿using System.Configuration;

namespace Spareio.Data.DB
{
    public class DBSchema
    {
        public static string DBName = "MineDB.db";
        public static string DBPath = ConfigurationManager.AppSettings["DbPath"];

        public static string MineTable = "Monitor";
        public static string MineConfigurationTable = "MineConfig";
        

    }
}