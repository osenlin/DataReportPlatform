using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Reports.Entities.DataBaseEntity;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using System.Data;
using net91com.Reports.Entities.ViewEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class EtlStates_DataAccess : BaseDataAccess
    {
        private static EtlStates_DataAccess instance = null;
        private static readonly object obj = new object();

        public static EtlStates_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new EtlStates_DataAccess();
                            instance._cachePreviousKey = "EtlStates_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取当前统计时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timetype">0 是int 型时间,1 为datetime型时间</param>
        /// <returns></returns>
        public DateTime GetNowStatTimeByKey(string key, int timetype)
        {
            string sql = string.Format("select `Value` from EtlStates where `Key`='{0}'", key.Replace("'", "''"));
            DateTime dt = DateTime.MinValue;
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(StatConn, sql))
            {
                if (reader.Read())
                {
                    if (timetype == 0)
                    {
                        int temp = Convert.ToInt32(reader["value"]);
                        dt = new DateTime(temp/10000, temp%10000/100, temp%100);
                    }
                    else
                    {
                        dt = Convert.ToDateTime(reader["value"]);
                    }
                }
            }
            return dt;
        }
    }
}