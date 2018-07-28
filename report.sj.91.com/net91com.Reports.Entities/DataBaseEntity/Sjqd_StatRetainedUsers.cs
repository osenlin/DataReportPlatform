using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    /// <summary>
    /// 用户留存量相关实体
    /// </summary>
    public class Sjqd_StatRetainedUsers
    {
        /// <summary>
        /// 新增日期
        /// </summary>
        public DateTime OriginalDate { get; set; }

        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 新增用户
        /// </summary>
        public int OriginalNewUserCount { get; set; }

        /// <summary>
        /// 留存用户
        /// </summary>
        public int RetainedUserCount { get; set; }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_StatRetainedUsers(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_StatRetainedUsers()
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
                        int temp = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        StatDate = new DateTime(temp/10000, temp%10000/100, temp%100);
                        break;
                    case "originaldate":
                        int temp2 = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        OriginalDate = new DateTime(temp2/10000, temp2%10000/100, temp2%100);
                        break;
                    case "originalnewusercount":
                        OriginalNewUserCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["originalnewusercount"]);
                        break;
                    case "retainedusercount":
                        RetainedUserCount = reader.IsDBNull(i)
                                                ? 0
                                                : (int) Math.Truncate(Convert.ToDouble(reader["retainedusercount"]));
                        break;
                }
            }
        }
    }
}