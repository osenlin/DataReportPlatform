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
    public class StatUsersByLanService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        private static string MySql_StatDbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        private string _cachePreviousKey;
        private static StatUsersByLanService service;

        private StatUsersByLanService()
        {
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

        public static StatUsersByLanService GetInstance()
        {
            if (service == null)
            {
                service = new StatUsersByLanService();
                service._cachePreviousKey = "StatUsersByLanService";
            }
            SqlHelper.CommandTimeout = 120;
            return service;
        }

        /// <summary>
        /// 获取每一天
        /// </summary>
        /// <returns></returns>
        public List<Sjqd_StatUsersByLan> GetLanByDates(DateTime begintime, DateTime endtime, int softid, int platform,
                                                       string lanname)
        {
            if (lanname == "未知语言")
            {
                lanname = "";
            }
            string key = BuildCacheKey("GetLanByDates", begintime, endtime, softid, platform, lanname);


            List<Sjqd_StatUsersByLan> list = CacheHelper.Get<List<Sjqd_StatUsersByLan>>(key);
            if (list == null)
            {
//                string cmdText = @"
//                                select @lanname E_Lan,sum(userscount) userCount,StatDate from
//                                 (
//	                                SELECT case when b.E_Lan='' or b.E_Lan is null then 0 else LanID end LanID,userscount,StatDate,
//                                    isnull(b.E_Lan,'') E_Lan 
//	                                FROM 
//	                                (
//		                                SELECT LanID,SUM(NewUserCount+ActiveUserCount) userscount ,StatDate
//		                                FROM Sjqd_StatUsersByLan WITH(NOLOCK)
//		                                WHERE Period=@period AND StatDate  between @begintime and @endtime  AND SoftID=@SoftID AND [Platform]=@Platform
//		                                GROUP BY LanID,StatDate
//	                                ) A 
//	                                LEFT JOIN 
//	                                Sjqd_Lan b WITH(NOLOCK) ON A.LanID=b.ID
//                                ) A
//                                where E_Lan=@lanname
//                                group by StatDate
//                                ORDER BY  StatDate asc 
//                                ";

//                SqlParameter[] param = new SqlParameter[] {
//                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = 1},
//                    new SqlParameter(){ ParameterName = "@begintime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = begintime.ToString("yyyyMMdd")},
//                    new SqlParameter(){ ParameterName = "@endtime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = endtime.ToString("yyyyMMdd")},
//                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softid},
//                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform},
//                    new SqlParameter(){ ParameterName = "@lanname", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = lanname}
//                };

                var langlst = Sjqd_LanService.GetSjqd_LanList(lanname).Select(p => p.ID.ToString());

                if (langlst.Count() <= 0)
                {
                    return list;
                }
                string sql = string.Format(@"
                             select StatDate,SUM(NewUserCount+ActiveUserCount) userscount,'{6}' E_Lan
                             from U_StatUsersByLan
                             WHERE Period={0} AND StatDate  between {1} and {2}  AND SoftID={3} AND Platform={4} and LangId in({5})
                             group by StatDate", 1, begintime.ToString("yyyyMMdd"),
                                           endtime.ToString("yyyyMMdd"),
                                           softid,
                                           platform,
                                           String.Join(",", langlst.ToArray()), lanname
                    );

                list = new List<Sjqd_StatUsersByLan>();

                using (var read = MySqlHelper.ExecuteReader((MySql_StatDbConn), sql))
                {
                    while (read.Read())
                    {
                        int timedate = Convert.ToInt32(read["StatDate"]);

                        list.Add(new Sjqd_StatUsersByLan()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                Lan = read["E_Lan"].ToString(),
                                StatDate = new DateTime(timedate/10000, timedate%10000/100, timedate%100)
                            });
                    }
                }

                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByLan>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }

        /// <summary>
        /// 获取所有语言分布
        /// </summary>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByLan> GetSoftLanTransverse(net91com.Stat.Core.PeriodOptions period, int statDate,
                                                              int softId, MobileOption platform)
        {
            string key = BuildCacheKey("GetSoftLanTransverse", period, statDate, softId, platform);


            List<Sjqd_StatUsersByLan> list = CacheHelper.Get<List<Sjqd_StatUsersByLan>>(key);
            if (list == null)
            {
//                string cmdText = @"
//                                select E_Lan,sum(userscount) userCount from
//                                 (
//	                                SELECT case when b.E_Lan='' or b.E_Lan is null then 0 else LanID end LanID,userscount,isnull(b.E_Lan,'') E_Lan 
//	                                FROM 
//	                                (
//		                                SELECT LanID,SUM(NewUserCount+ActiveUserCount) userscount 
//		                                FROM Sjqd_StatUsersByLan WITH(NOLOCK)
//		                                WHERE Period=@period AND StatDate =  @StatDate  AND SoftID=@SoftID AND [Platform]=@Platform
//		                                GROUP BY LanID
//	                                ) A 
//	                                LEFT JOIN 
//	                                Sjqd_Lan b WITH(NOLOCK) ON A.LanID=b.ID
//                                ) A
//                                group by E_Lan
//                                ORDER BY sum(userscount) DESC
//                                ";

//                SqlParameter[] param = new SqlParameter[] {
//                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
//                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
//                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
//                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
//                };

                string sql = string.Format(@"
                             select coalesce(LangId,0) langid,SUM(NewUserCount+ActiveUserCount) userscount 
                             from U_StatUsersByLan
                             where Period={0} and statdate={1} and softid={2} and platform={3}
                             group by LangId
                             Order by userscount desc", (int) period, statDate, softId, (int) platform);

                list = new List<Sjqd_StatUsersByLan>();

                using (IDataReader read = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql))
                {
                    while (read.Read())
                    {
                        list.Add(new Sjqd_StatUsersByLan()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                LanID = Convert.ToInt32(read["langid"])
                            });
                    }
                }

                int count = 0;
                var listsjqd = Sjqd_LanService.GetSjqd_LanList("", "", 0, int.MaxValue, out count);

                var reslst = (from itemStatUsersByLan in list
                              join itemlan in listsjqd on itemStatUsersByLan.LanID equals itemlan.ID into temp
                              from tt in temp.DefaultIfEmpty()
                              select new Sjqd_StatUsersByLan()
                                  {
                                      UseCount = itemStatUsersByLan.UseCount,
                                      Lan = tt == null ? "" : tt.E_Lan
                                  }).ToList();

                var realreslst = (from item in reslst
                                  group item by item.Lan
                                  into g
                                  select new Sjqd_StatUsersByLan()
                                      {
                                          Lan = g.Key,
                                          UseCount = g.Sum(p => p.UseCount)
                                      }).ToList();

                if (realreslst.Count > 0)
                {
                    CacheHelper.Set<List<Sjqd_StatUsersByLan>>(key, realreslst, CacheTimeOption.TenMinutes);
                    list = realreslst;
                }
            }
            return list;
        }
    }
}