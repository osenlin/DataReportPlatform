using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Exception_StatCount
    {
        public Exception_StatCount()
        {
        }

        public Exception_StatCount(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
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
                        int date = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        StatDate = new DateTime(date/10000, date%10000/100, date%100);
                        break;
                    case "period":
                        Period = reader.IsDBNull(i)
                                     ? net91com.Stat.Core.PeriodOptions.All
                                     : (net91com.Stat.Core.PeriodOptions) reader.GetByte(i);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? MobileOption.None : (MobileOption) reader.GetByte(i);
                        break;
                    case "versionid":
                        SoftVersionID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "exceptionmessage":
                        ExceptionMessage = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        break;
                    case "exceptioncount":
                        ExceptionCount = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "useusers":
                        UseUsers = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                }
            }
        }

        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 统计周期
        /// </summary>
        public net91com.Stat.Core.PeriodOptions Period { get; set; }

        /// <summary>
        /// 软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 软件版本ID
        /// </summary>
        public int SoftVersionID { get; set; }

        /// <summary>
        /// 产品所属平台
        /// </summary>
        public MobileOption Platform { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 异常次数
        /// </summary>
        public int ExceptionCount { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public int UseUsers { get; set; }
    }
}