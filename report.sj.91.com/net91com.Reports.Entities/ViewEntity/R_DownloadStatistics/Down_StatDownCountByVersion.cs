using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class Down_StatDownCountByVersion
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Down_StatDownCountByVersion(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Down_StatDownCountByVersion()
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
                        int m = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(m / 10000, m % 10000 / 100, m % 100);
                        break;
                    case "period":
                        Period = reader.IsDBNull(i) ? net91com.Stat.Core.PeriodOptions.All : (net91com.Stat.Core.PeriodOptions)Convert.ToInt32(reader["period"]);
                        break;
                    case "downcount":
                        DownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downcount"]);
                        break;
                    case "downsuccesscount":
                        DownSuccessCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downsuccesscount"]);
                        break;
                    case "downfailcount":
                        DownFailCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downfailcount"]);
                        break;
                    case "setupsuccesscount":
                        SetUpSuccessCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["setupsuccesscount"]) ;
                        break;
                    case "setupfailcount":
                        SetUpFailCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["setupfailcount"]);
                        break;


                }

                 
            }
            long totalDownCount = DownSuccessCount + DownFailCount;
            long totalSetupCount = SetUpFailCount + SetUpSuccessCount;
            SetUpSuccessPercent = totalDownCount == 0 ? 0 : DownSuccessCount*1.0/totalDownCount;
            DownSuccessPercent = totalSetupCount == 0 ? 0 : SetUpSuccessCount * 1.0 / totalSetupCount;
             
        }

        public DateTime StatDate { get; set; }

        public net91com.Stat.Core.PeriodOptions Period { get; set; }

        public long DownCount { get; set; }
        /// <summary>
        /// 下载成功数
        /// </summary>
        public long DownSuccessCount { get; set; }
        /// <summary>
        /// 下载失败数
        /// </summary>
        public long DownFailCount { get; set; }
        /// <summary>
        /// 安装成功数
        /// </summary>
        public long SetUpSuccessCount { get; set; }
        /// <summary>
        /// 安装失败数
        /// </summary>
        public long SetUpFailCount { get; set; }
        /// <summary>
        /// 安装成功率
        /// </summary>
        public double SetUpSuccessPercent { get; set; }
        /// <summary>
        /// 下载成功率
        /// </summary>
        public double DownSuccessPercent { get; set; }




    }
}
