using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Core.Web;
using System.Text;
using net91com.Core.Extensions;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd
{
    public class StatUsersByMACService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        //用户计算库连接串
        private static string ComputingDB_Sjqd_ConnString =
            ConfigHelper.GetConnectionString("ComputingDB_Sjqd_ConnString");

        private string _cachePreviousKey;
        private bool useCache = false;

        private string BuildCacheKey(params object[] args)
        {
            StringBuilder sbCacheKey = new StringBuilder(_cachePreviousKey);
            foreach (object obj in args)
            {
                sbCacheKey.Append("_");
                sbCacheKey.Append(obj);
            }
            return sbCacheKey.ToString();
        }

        public StatUsersByMACService(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "StatUsersByLanService";
            SqlHelper.CommandTimeout = 120;
        }

        /// <summary>
        /// 获取MAC地址关联的新增设备数
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="mac"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public Dictionary<MobileOption, int> GetNewUserCountsByMac(int softId, string mac, DateTime startDate,
                                                                   DateTime endDate)
        {
            string cmdText =
                "select Platform,NewUserCount from PCDailyMac with(nolock) where SoftID=@SoftID and MAC=@MAC and StatDate between @StartDate and @EndDate";
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@SoftID", SqlDbType.Int, 4, softId),
                    SqlParamHelper.MakeInParam("@MAC", SqlDbType.VarChar, 50, mac),
                    SqlParamHelper.MakeInParam("@StartDate", SqlDbType.Int, 4, startDate.ToString("yyyyMMdd")),
                    SqlParamHelper.MakeInParam("@EndDate", SqlDbType.Int, 4, endDate.ToString("yyyyMMdd"))
                };
            Dictionary<MobileOption, int> newUserCountsByMac = new Dictionary<MobileOption, int>();
            using (
                SqlDataReader reader = SqlHelper.ExecuteReader(ComputingDB_Sjqd_ConnString, CommandType.Text, cmdText,
                                                               parameters))
            {
                while (reader.Read())
                {
                    MobileOption platform = (MobileOption) Convert.ToInt32(reader["Platform"]);
                    int newUserCount = Convert.ToInt32(reader["NewUserCount"]);
                    if (newUserCountsByMac.ContainsKey(platform))
                        newUserCountsByMac[platform] = newUserCountsByMac[platform] + newUserCount;
                    else
                        newUserCountsByMac.Add(platform, newUserCount);
                }
            }
            return newUserCountsByMac;
        }

        /// <summary>
        /// 获取Moborobo的MAC列表
        /// </summary>
        /// <param name="statDate"></param>
        /// <returns></returns>
        public string GetMoboroboMacs(DateTime statDate)
        {
            string cmdText =
                "select Platform,MAC,NewUserCount from PCDailyMac with(nolock) where SoftID=@SoftID and StatDate=@StatDate order by MAC,Platform";
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@SoftID", SqlDbType.Int, 4, 58),
                    SqlParamHelper.MakeInParam("@StatDate", SqlDbType.Int, 4, statDate.ToString("yyyyMMdd"))
                };
            StringBuilder moboroboMacsJson = new StringBuilder("{\"success\":true,\"data\":[");
            using (
                SqlDataReader reader = SqlHelper.ExecuteReader(ComputingDB_Sjqd_ConnString, CommandType.Text, cmdText,
                                                               parameters))
            {
                while (reader.Read())
                {
                    MobileOption platform = (MobileOption) Convert.ToInt32(reader["Platform"]);
                    string mac = reader["MAC"].ToString();
                    int newUserCount = Convert.ToInt32(reader["NewUserCount"]);
                    moboroboMacsJson.AppendFormat("{{\"MAC\":\"{0}\",\"Platform\":\"{1}\",\"NewUserCount\":{2}}},", mac,
                                                  platform, newUserCount);
                }
            }
            return moboroboMacsJson.ToString().TrimEnd(',') + "]}";
        }

        public List<Sjqd_StatUsersByMAC> GetTop100MacUsers(int softId, MobileOption platform,
                                                           net91com.Stat.Core.PeriodOptions period, DateTime statdate)
        {
            string sql = @"select top 100 softid,PLATFORM,statdate,period,mac ,newusercount
                            from Sjqd_StatUsersByMAC
                            where StatDate=@statdate and Period=@period and SoftID=@softid and Platform=@platform
                            order by NewUserCount desc";
            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter()
                        {
                            ParameterName = "@period",
                            SqlDbType = System.Data.SqlDbType.TinyInt,
                            Size = 1,
                            Value = (int) period
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@softid",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Size = 4,
                            Value = softId
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@statdate",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Size = 4,
                            Value = Convert.ToInt32(statdate.ToString("yyyyMMdd"))
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@platform",
                            SqlDbType = System.Data.SqlDbType.TinyInt,
                            Size = 1,
                            Value = (int) platform
                        }
                };
            List<Sjqd_StatUsersByMAC> results = new List<Sjqd_StatUsersByMAC>();
            using (IDataReader dr = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, param))
            {
                while (dr.Read())
                {
                    results.Add(BindStatUsersByMac(dr));
                }
            }
            return results;
        }

        public Sjqd_StatUsersByMAC BindStatUsersByMac(IDataReader dr)
        {
            Sjqd_StatUsersByMAC macUsers = new Sjqd_StatUsersByMAC();
            if (dr["softid"] != null && dr["softid"] != DBNull.Value)
            {
                macUsers.SoftID = Convert.ToInt32(dr["softid"]);
            }
            if (dr["Period"] != null && dr["Period"] != DBNull.Value)
            {
                macUsers.Period = (net91com.Stat.Core.PeriodOptions) Convert.ToInt32(dr["Period"]);
            }

            if (dr["Platform"] != null && dr["Platform"] != DBNull.Value)
            {
                macUsers.Platform = (MobileOption) Convert.ToInt32(dr["Platform"]);
            }
            if (dr["mac"] != null && dr["mac"] != DBNull.Value)
            {
                macUsers.Mac = dr["mac"].ToString();
            }

            if (dr["newusercount"] != null && dr["newusercount"] != DBNull.Value)
            {
                macUsers.NewUserCount = Convert.ToInt32(dr["newusercount"]);
            }

            if (dr["statdate"] != null && dr["statdate"] != DBNull.Value)
            {
                int statDate = Convert.ToInt32(dr["statdate"]);
                macUsers.StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100);
            }


            return macUsers;
        }

        public List<Sjqd_StatUsersByMAC> GetTop100MacUsersByCache(int softId, MobileOption platform,
                                                                  net91com.Stat.Core.PeriodOptions period,
                                                                  DateTime statdate, CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetTop100MacUsersByCache", softId, platform, period, statdate);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_StatUsersByMAC>>(cacheKey);
                }
                List<Sjqd_StatUsersByMAC> list = GetTop100MacUsers(softId, platform, period, statdate);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_StatUsersByMAC>>(cacheKey, list, cachetime,
                                                               CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetTop100MacUsers(softId, platform, period, statdate);
            }
        }
    }
}