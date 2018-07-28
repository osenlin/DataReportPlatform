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
    public class StatUsersByGjbbService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static string MySql_StatDbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        private string _cachePreviousKey;
        private static StatUsersByGjbbService service;

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

        private StatUsersByGjbbService()
        {
        }

        public static StatUsersByGjbbService GetInstance()
        {
            if (service == null)
            {
                service = new StatUsersByGjbbService();
                service._cachePreviousKey = "StatUsersByGjbbService";
            }
            SqlHelper.CommandTimeout = 120;
            return service;
        }

        public List<Sjqd_StatUsersByGjbb> GetSoftGJBBTransverse(net91com.Stat.Core.PeriodOptions period, int statDate,
                                                                int softId, MobileOption platform)
        {
            string key = BuildCacheKey("GetSoftGJBBTransverse", period, statDate, softId, platform);

            List<Sjqd_StatUsersByGjbb> list = CacheHelper.Get<List<Sjqd_StatUsersByGjbb>>(key);

            if (list == null)
            {
//                string cmdText = @"select E_GJBB,sum(userscount) userCount from
//                                 (
//	                                SELECT case when b.E_GJBB='' or b.E_GJBB is null then 0 else GjbbID end GjbbID,userscount,isnull(b.E_GJBB,'') E_GJBB 
//	                                FROM 
//	                                (
//		                                SELECT GjbbID,SUM(NewUserCount+ActiveUserCount) userscount 
//		                                FROM Sjqd_StatUsersByGjbb WITH(NOLOCK)
//		                                WHERE Period=@period AND StatDate =  @StatDate  AND SoftID=@SoftID AND [Platform]=@Platform
//		                                GROUP BY GjbbID
//	                                ) A 
//	                                LEFT JOIN 
//	                                Sjqd_GJBB b WITH(NOLOCK) ON A.GjbbID=b.ID
//                                ) A
//                                group by E_GJBB
//                                ORDER BY sum(userscount) DESC ";

//                SqlParameter[] param = new SqlParameter[] {
//                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
//                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
//                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
//                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
//                };

                String sql = string.Format(@"SELECT GjbbID,SUM(NewUserCount+ActiveUserCount) userscount 
		                                FROM U_StatUsersByGjbb 
		                                WHERE Period={0} AND StatDate ={1}  AND SoftID={2} AND Platform={3}
		                                GROUP BY GjbbID
                                        Order by userscount desc", (int) period, statDate, softId, (int) platform);

                list = new List<Sjqd_StatUsersByGjbb>();

                using (IDataReader read = MySqlHelper.ExecuteReader((MySql_StatDbConn), sql))
                {
                    while (read.Read())
                    {
                        list.Add(new Sjqd_StatUsersByGjbb()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                GjbbID = Convert.ToInt32(read["GjbbID"])
                            });
                    }
                }

                int count = 0;
                var gjbblst = Sjqd_GJBBService.GetSjqd_GJBBList((int) platform, "", "", 0, int.MaxValue, out count);
                var reslst = (from sjqdStatUsersByGjbb in list
                              join sjqdGjbb in gjbblst on sjqdStatUsersByGjbb.GjbbID equals sjqdGjbb.ID
                                  into os
                              from tt in os.DefaultIfEmpty()
                              select new Sjqd_StatUsersByGjbb()
                                  {
                                      UseCount = sjqdStatUsersByGjbb.UseCount,
                                      Gjbb = tt == null ? "" : tt.E_GJBB
                                  }).ToList();

                var realreslst = (from item in reslst
                                  group item by item.Gjbb
                                  into g
                                  select new Sjqd_StatUsersByGjbb()
                                      {
                                          Gjbb = g.Key,
                                          UseCount = g.Sum(p => p.UseCount)
                                      }).OrderByDescending(p => p.UseCount).ToList();

                if (realreslst.Count > 0)
                {
                    CacheHelper.Set<List<Sjqd_StatUsersByGjbb>>(key, realreslst, CacheTimeOption.TenMinutes);
                    list = realreslst;
                }
            }
            return list;
        }

        /// <summary>
        /// 获取每一天
        /// </summary>
        /// <returns></returns>
        public List<Sjqd_StatUsersByGjbb> GetGjbbByDates(DateTime begintime, DateTime endtime, int softid, int platform,
                                                         string gjbb)
        {
            if (gjbb == "未适配固件")
            {
                gjbb = "";
            }
            string key = BuildCacheKey("GetGjbbByDates", begintime, endtime, softid, platform, gjbb);


            List<Sjqd_StatUsersByGjbb> list = CacheHelper.Get<List<Sjqd_StatUsersByGjbb>>(key);
            if (list == null)
            {

                var gjbblst = Sjqd_GJBBService.GetSjqd_GJBBList(platform, gjbb).Select(p => p.ID.ToString());
                if (gjbblst.Count() <= 0)
                {
                    return list;
                }
                String sql = string.Format(@"
                                        SELECT StatDate,SUM(NewUserCount+ActiveUserCount) userscount,'{6}' E_GJBB
		                                FROM U_StatUsersByGjbb 
		                                WHERE Period={0} AND StatDate  between {1} and {2}  AND SoftID={3} AND Platform={4} and gjbbid in({5})
		                                GROUP BY StatDate", 1, begintime.ToString("yyyyMMdd"),
                                           endtime.ToString("yyyyMMdd"),
                                           softid,
                                           platform,
                                           String.Join(",", gjbblst.ToArray()),
                                           gjbb);

                list = new List<Sjqd_StatUsersByGjbb>();

                using (var read = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql))
                {
                    while (read.Read())
                    {
                        int timedate = Convert.ToInt32(read["StatDate"]);

                        list.Add(new Sjqd_StatUsersByGjbb()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                Gjbb = read["E_GJBB"].ToString(),
                                StatDate = new DateTime(timedate/10000, timedate%10000/100, timedate%100)
                            });
                    }
                }

                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByGjbb>>(key, list, CacheTimeOption.TenMinutes,
                                                                CacheExpirationOption.AbsoluteExpiration);
            }
            return list;
        }
    }
}