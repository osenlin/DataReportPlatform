using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class Down_UsersSumDownCount
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Down_UsersSumDownCount(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Down_UsersSumDownCount()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public   void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "statdate":
                        int m = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(m / 10000, m % 10000 / 100, m % 100);
                        break;
                    case "totaldowncount":
                        TotalDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["totaldowncount"]);
                        break;  
                    case "newuserdowncount":
                        NewUserDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["newuserdowncount"]);
                        break;
                    case "totalusercount":
                        TotalUserCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["totalusercount"]);
                        break;
                    case "newusercount":
                        NewUserCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["newusercount"]);
                        break;
                    case "appversion":
                        AppVersion = reader.IsDBNull(i) ? "" : reader["appversion"].ToString();
                        break; 

                        
                }
                ActiveUserCount = TotalUserCount - NewUserCount;
                ActiveUserDownCount = TotalDownCount - NewUserDownCount;

                AvgTotalUserDown = TotalUserCount == 0 ? 0 : TotalDownCount*1.0/TotalUserCount;
                AvgNewUserDown = NewUserCount == 0 ? 0 : NewUserDownCount * 1.0 / NewUserCount;
                AvgActUserDown = ActiveUserCount == 0 ? 0 : ActiveUserDownCount * 1.0 / ActiveUserCount;
            }
        }

        public DateTime StatDate;

        public long TotalDownCount;

        public long NewUserDownCount;

        public long TotalUserCount;

        public long NewUserCount;

        public long ActiveUserDownCount;
        
        public long ActiveUserCount;

        public double AvgTotalUserDown;

        public double AvgNewUserDown;

        public double AvgActUserDown;

        public string AppVersion;
    }
}
