using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Web;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using System.Data.SqlClient;
using net91com.Core.Util;
using System.Data;
using MySql.Data.MySqlClient;

namespace net91com.Stat.Services
{
    public static class PlatExtend
    {
        public static int GetPlatIndex(this MobileOption plat)
        {
            switch (plat)
            {
                case MobileOption.Android:
                    return 2;
                case MobileOption.AndroidPad:
                    return 5;
                case MobileOption.AndroidTV:
                    return 6;
                case MobileOption.IPAD:
                    return 3;
                case MobileOption.PC:
                    return 4;
                case MobileOption.S60:
                    return 10;
                case MobileOption.WM:
                    return 9;
                case MobileOption.WP7:
                    return 5;
                case MobileOption.WebGame:
                    return 8;
                case MobileOption.Win8:
                    return 6;
                case MobileOption.iPhone:
                    return 1;
                case (MobileOption) 252:
                case (MobileOption) 253:
                case (MobileOption) 254:
                    return 7;
                default:
                    return 20;
            }
        }
    }

    public class UtilityService
    {
        private static string connString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static UtilityService service;
        private string _cachePreviousKey;
        private bool useCache = true;

        private UtilityService()
        {
        }

        /// <summary>
        /// 获取平台列表
        /// </summary>
        /// <param name="filter">是否过滤掉 MobileOption.None和All</param>
        /// <returns></returns>
        public static List<MobileOption> GetMobileList(bool filter)
        {
            List<MobileOption> list = new List<MobileOption>();
            var array = new MobileOption[] { MobileOption.None, MobileOption.iPhone, MobileOption.Android, MobileOption.IPAD, MobileOption.All };
            foreach (MobileOption item in array)
            {
                if (filter)
                {
                    if (item != MobileOption.None && item != MobileOption.All)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    list.Add(item);
                }
            }            
            return list.OrderBy(p => p.GetPlatIndex()).ToList();
        }

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

        public static UtilityService GetInstance()
        {
            if (service == null)
            {
                service = new UtilityService();
                service._cachePreviousKey = "UtilityService";
            }
            return service;
        }

        /// <summary>
        /// 专门为特定渠道商查询能够获取的最大时间
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public DateTime GetMaxChannelUserTime(int softid, MobileOption plat, net91com.Stat.Core.PeriodOptions period)
        {
            ///默认是当前
            DateTime maxTime = DateTime.Now;
            string cmdText = string.Format(@"select max(StatDate) from  U_StatChannelUsers
                                where softid={0}   and period={1}", softid, (int)plat);
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(connString, cmdText))
            {
                while (dataReader.Read())
                {
                    if (dataReader[0] != null && dataReader[0] != DBNull.Value)
                    {
                        int time = Convert.ToInt32(dataReader[0]);
                        maxTime = new DateTime(time/10000, time/100%100, time%100, 0, 0, 0);
                    }
                }
            }
            return maxTime;
        }

        public DateTime GetMaxChannelUserTimeCache(int softid, MobileOption plat,
                                                   net91com.Stat.Core.PeriodOptions period, CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetMaxChannelUserTimeCache", softid, plat, period);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return new DateTime(CacheHelper.Get<DateTime>(cacheKey).Ticks);
                }
                DateTime list = GetMaxChannelUserTime(softid, plat, period);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<DateTime>(cacheKey, list, cachetime, CacheExpirationOption.AbsoluteExpiration);
                }
                return new DateTime(list.Ticks);
            }
            else
            {
                return GetMaxChannelUserTime(softid, plat, period);
            }
        }

        /// 获取能查询数据的最大时间
        public DateTime GetMaxTime(net91com.Stat.Core.PeriodOptions period, ReportType Type, string otherKeyString = "")
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        /// 获取能查询数据的最大时间
        public DateTime GetMaxTimeForDownSpeed(net91com.Stat.Core.PeriodOptions period)
        {
            string sqlstr = "select value from etlstates where [key]=@key ";
            string key = "";
            if (period == net91com.Stat.Core.PeriodOptions.TimeOfDay)
                key = "CurStatDate_DownHourSpeed";
            else
                key = "CurStatDate_DownDaySpeed";
            SqlParameter[] para = new SqlParameter[1]
                {
                    SqlParamHelper.MakeInParam("@key", SqlDbType.VarChar, 100, key)
                };
            DateTime dt = DateTime.MinValue;
            using (IDataReader dataReader = SqlHelper.ExecuteReader(connString, CommandType.Text, sqlstr, para))
            {
                if (dataReader.Read())
                {
                    if (dataReader[0] != null && dataReader[0] != DBNull.Value)
                    {
                        dt = Convert.ToDateTime(dataReader[0]);
                    }
                }
            }
            if (period == net91com.Stat.Core.PeriodOptions.TimeOfDay)
                return dt.AddHours(-1);
            else
                return dt.AddDays(-1);
        }

        public DateTime GetMaxTimeCache(net91com.Stat.Core.PeriodOptions period, ReportType Type,
                                        CacheTimeOption cachetime, string otherKeyString = "")
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetMaxTimeCache", period, Type, otherKeyString);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return new DateTime(CacheHelper.Get<DateTime>(cacheKey).Ticks);
                }
                DateTime list = GetMaxTime(period, Type, otherKeyString);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<DateTime>(cacheKey, list, cachetime, CacheExpirationOption.AbsoluteExpiration);
                }
                return new DateTime(list.Ticks);
            }
            else
            {
                return GetMaxTime(period, Type, otherKeyString);
            }
        }

        // 获取能查的数据的最小时间(给定范围内最接近的时间)
        public DateTime GetMinTime(int softid, int platformid, ReportType Type, DateTime begintime, DateTime endtime)
        {
            ///默认是当前
            DateTime minTime = new DateTime(2009, 9, 1, 0, 0, 0);
            ;
            string sqlstr = string.Empty;
            string field = string.Empty;
            if (Type == ReportType.UserLifecycle)
            {
                if (platformid != (int) MobileOption.None)
                {
                    sqlstr = @"select min(statdate) from Sjqd_StatLifecycle with(nolock)
                           where softid=@softid  and platform=@platform  and  StatDate between @begindate and @enddate ";
                }
                else
                {
                    sqlstr = @"select min(statdate) from Sjqd_StatLifecycle with(nolock)
                           where softid=@softid  and  StatDate between @begindate and @enddate ";
                }
            }
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platformid),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd")))
                    ,
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };

            using (IDataReader dataReader = SqlHelper.ExecuteReader(connString, CommandType.Text, sqlstr, parameters))
            {
                while (dataReader.Read())
                {
                    if (dataReader[0] != null && dataReader[0] != DBNull.Value)
                    {
                        int time = Convert.ToInt32(dataReader[0]);
                        minTime = new DateTime(time/10000, time/100%100, time%100, 0, 0, 0);
                    }
                }
            }
            return minTime;
        }

        public DateTime GetMinTimeCache(int softid, int platformid, ReportType Type, DateTime begintime,
                                        DateTime endtime, CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetMinTimeCache", softid, platformid, Type, begintime, endtime);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return new DateTime(CacheHelper.Get<DateTime>(cacheKey).Ticks);
                }
                DateTime list = GetMinTime(softid, platformid, Type, begintime, endtime);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<DateTime>(cacheKey, list, cachetime, CacheExpirationOption.AbsoluteExpiration);
                }
                return new DateTime(list.Ticks);
            }
            else
            {
                return GetMinTime(softid, platformid, Type, begintime, endtime);
            }
        }
    }
}