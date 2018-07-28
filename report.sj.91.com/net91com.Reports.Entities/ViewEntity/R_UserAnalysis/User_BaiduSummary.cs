using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_UserAnalysis
{
    public class User_BaiduSummary
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public User_BaiduSummary(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        public User_BaiduSummary()
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
                        int temp = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(temp/10000, temp%10000/100, temp%100);
                        break;

                    case "newusercount":
                        NewUserCount = reader.IsDBNull(i)
                                           ? 0
                                           : Convert.ToInt32(reader["newusercount"]);
                        break;
                    case "actusercount":
                        ActUserCount = reader.IsDBNull(i)
                                           ? 0
                                           : Convert.ToInt32(reader["actusercount"]);
                        break;
                }
            }
        }

        public DateTime StatDate;

        public int NewUserCount;

        public int ActUserCount;
    }
}