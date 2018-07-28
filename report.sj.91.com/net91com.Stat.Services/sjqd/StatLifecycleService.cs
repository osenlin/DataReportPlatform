using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
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
    public class StatLifecycleService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        private static string MySql_StatDbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

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

        public StatLifecycleService(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "LifecycleService";
            SqlHelper.CommandTimeout = 120;
        }

        /// 绑定LifeCycle对象 生命周期
        public Sjqd_StatLifecycle LifeCycleBind(IDataReader dataReader)
        {
            Sjqd_StatLifecycle model = new Sjqd_StatLifecycle();
            object obj;
            obj = dataReader["StatDate"];
            if (obj != null && obj != DBNull.Value)
            {
                int temp = Convert.ToInt32(obj);
                model.StatDate = new DateTime(temp/10000, temp/100%100, temp%100, 0, 0, 0);
            }
            obj = dataReader["SoftID"];
            if (obj != null && obj != DBNull.Value)
            {
                model.SoftID = Convert.ToInt32(obj);
            }
            obj = dataReader["Platform"];
            if (obj != null && obj != DBNull.Value)
            {
                model.Platform = (MobileOption) Convert.ToInt32(obj);
            }
            obj = dataReader["NewUserCount"];
            if (obj != null && obj != DBNull.Value)
            {
                model.NewUserCount = Convert.ToInt32(obj);
            }
            obj = dataReader["RetainedUserCount"];
            if (obj != null && obj != DBNull.Value)
            {
                model.RetainedUserCount = Convert.ToInt32(obj);
            }
            obj = dataReader["Days"];
            if (obj != null && obj != DBNull.Value)
            {
                model.Days = Convert.ToInt32(obj);
            }
            return model;
        }


        public List<Sjqd_StatLifecycle> GetSoftLifeCycle(int softid, int platformid, DateTime date)
        {
            string sql = string.Empty;
            if (platformid != (int) MobileOption.None)
            {
                sql = @"select StatDate,SoftID,Platform,NewUserCount,RetainedUserCount,Days
                           from U_StatLifecycle 
                        where  statdate=?statdate and softid=?softid and platform=?platform   
                        order by days";
            }
            else
            {
                sql =
                    @"select  StatDate, SoftID,0 Platform,sum(NewUserCount) NewUserCount,sum(RetainedUserCount) RetainedUserCount,days
                        from U_StatLifecycle 
                        where softid=?softid and statdate=?statdate  
                        group by days,softid,statdate
                        order by days";
            }
            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform", platformid),
                    new MySqlParameter("?statdate", date.ToString("yyyyMMdd"))
                };
            List<Sjqd_StatLifecycle> lists = new List<Sjqd_StatLifecycle>();
            using (var dataReader = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(LifeCycleBind(dataReader));
                }
            }
            List<Sjqd_StatLifecycle> listresult = new List<Sjqd_StatLifecycle>();
            ///填充满
            if (lists.Count != 0)
            {
                int begin = lists.Min(p => p.Days);
                int end = lists.Max(p => p.Days);
                var firstone = lists[0];

                while (begin <= end)
                {
                    var temp = lists.Find(p => p.Days == begin);
                    if (temp != null)
                        listresult.Add(temp);
                    else
                        listresult.Add(new Sjqd_StatLifecycle
                            {
                                StatDate = firstone.StatDate,
                                Days = begin,
                                RetainedUserCount = 0,
                                SoftID = firstone.SoftID,
                                Platform = firstone.Platform,
                                NewUserCount = firstone.NewUserCount
                            });
                    begin = begin + 1;
                }
            }
            return listresult;
        }

        public List<Sjqd_StatLifecycle> GetSoftLifeCycleCache(int softid, int platformid, DateTime date,
                                                              CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetSoftLifeCycleCache", softid, platformid, date);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_StatLifecycle>>(cacheKey).Clone().ToList();
                }
                List<Sjqd_StatLifecycle> list = GetSoftLifeCycle(softid, platformid, date);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_StatLifecycle>>(cacheKey, list, cachetime,
                                                              CacheExpirationOption.AbsoluteExpiration);
                }
                return list.Clone().ToList();
            }
            else
            {
                return GetSoftLifeCycle(softid, platformid, date);
            }
        }
    }

    /// <summary>
    /// 实现扩展方法clone 的静态类
    /// </summary>
    public static class CloneClass
    {
        public static IList<T> Clone<T>(this IList<T> source)
            where T : ICloneable
        {
            IList<T> newList = new List<T>(source.Count);
            foreach (var item in source)
            {
                newList.Add((T) ((ICloneable) item.Clone()));
            }
            return newList;
        }
    }
}