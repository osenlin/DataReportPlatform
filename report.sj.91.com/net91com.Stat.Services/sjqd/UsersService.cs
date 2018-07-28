using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Util;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using System.Data;
using System.Data.SqlClient;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Web;

namespace net91com.Stat.Services.sjqd
{
    public class UsersService
    {
        private static string connString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        public string _cachePreviousKey;
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

        public UsersService(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "UsersService";
            SqlHelper.CommandTimeout = 120;
        }

        /// <summary>
        /// 获取 时间范围内的所有软件平台，statuser对象
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="softsid"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetSimpleSoftUserList(DateTime begin, net91com.Stat.Core.PeriodOptions period)
        {
            List<Sjqd_StatUsers> lists = new List<Sjqd_StatUsers>();
            string sqlstr =
                @"select SoftID,Platform,?periodid period,?begindate statdate,SUM(TotalUserCount) TotalUserCount,
                            SUM(NewUserCount+NewUserCountFromCache-ifnull(NewUserCount_Shualiang,0)) NewUserCount,SUM(ActiveUserCount+ActiveUserCountFromCache) ActiveUserCount
                            
                            from U_StatUsers
                            where  StatDate=?begindate and Period=?periodid
                            group by SoftID,Platform";
            var parameters = new []
                {
                    new MySqlParameter("?periodid",(int) period),
                    new MySqlParameter("?begindate", int.Parse(begin.ToString("yyyyMMdd"))),
                };

            using (IDataReader dataReader = MySqlHelper.ExecuteReader(connString,sqlstr, parameters))
            {

                while (dataReader.Read())
                {
                    lists.Add(SimpleSoftUserBind(dataReader));
                }
            }
            return lists;
        }


        /// <summary>
        /// 获取部分user 字段 的对象list
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public Sjqd_StatUsers SimpleSoftUserBind(IDataReader dr)
        {
            Sjqd_StatUsers user = new Sjqd_StatUsers();
            object obj;
            obj = dr["softid"];
            if (obj != DBNull.Value && obj != null)
            {
                user.SoftID = Convert.ToInt32(obj);
            }
            obj = dr["platform"];
            if (obj != DBNull.Value && obj != null)
            {
                user.Platform = (MobileOption) Convert.ToInt32(obj);
            }
            obj = dr["period"];
            if (obj != DBNull.Value && obj != null)
            {
                user.Period = (net91com.Stat.Core.PeriodOptions) Convert.ToInt32(obj);
            }
            obj = dr["statdate"];
            if (obj != DBNull.Value && obj != null)
            {
                int tempdate = Convert.ToInt32(obj);
                user.StatDate = new DateTime(tempdate/10000, tempdate%10000/100, tempdate%10000%100);
            }
            obj = dr["NewUserCount"];
            if (obj != DBNull.Value && obj != null)
            {
                user.NewUserCount = Convert.ToInt32(obj);
            }
            obj = dr["ActiveUserCount"];
            if (obj != DBNull.Value && obj != null)
            {
                user.ActiveUserCount = Convert.ToInt32(obj);
            }
            obj = dr["TotalUserCount"];
            if (obj != DBNull.Value && obj != null)
            {
                user.TotalUserCount = Convert.ToInt32(obj);
            }
            return user;
        }

        public List<Sjqd_StatUsers> GetSimpleSoftUserListCache(DateTime begin, net91com.Stat.Core.PeriodOptions period,
                                                               CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetSimpleSoftUserListCache", begin, period);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_StatUsers>>(cacheKey).ToList();
                }
                List<Sjqd_StatUsers> list = GetSimpleSoftUserList(begin, period);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_StatUsers>>(cacheKey, list, cachetime,
                                                          CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetSimpleSoftUserList(begin, period);
            }
        }
    }
}