using System;
using System.Data;
using net91com.Core;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class AppUninstallEntity
    {
        public DateTime StatDate { get; set; }
        public int NewUserCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int UninstallCount { get; set; }


        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public AppUninstallEntity(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AppUninstallEntity()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "statdate":
                        int tempdate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(tempdate/10000, tempdate%10000/100, tempdate%100);
                        break;
                    case "newusercount":
                        NewUserCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["newusercount"]);
                        break;
                    case "activeusercount":
                        ActiveUserCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["activeusercount"]);
                        break;
                    case "uninstallcount":
                        UninstallCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["uninstallcount"]);
                        break;
                }
            }
        }
    }
}