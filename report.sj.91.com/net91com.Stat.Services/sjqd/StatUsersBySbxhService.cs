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
    public class StatUsersBySbxhService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static string MySql_StatDbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private string _cachePreviousKey;
        private static StatUsersBySbxhService service;

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

        private StatUsersBySbxhService()
        {
        }

        public static StatUsersBySbxhService GetInStance()
        {
            if (service == null)
            {
                service = new StatUsersBySbxhService();
                service._cachePreviousKey = "StatUsersBySbxhService";
                SqlHelper.CommandTimeout = 120;
            }
            return service;
        }

        public List<Sjqd_StatUsersBySbxh> GetSoftSBXHTransverse(net91com.Stat.Core.PeriodOptions period, int statDate,
                                                                int softId, MobileOption platform)
        {
            string key = BuildCacheKey("GetSoftSBXHTransverse", period, statDate, softId, platform);
            List<Sjqd_StatUsersBySbxh> list = CacheHelper.Get<List<Sjqd_StatUsersBySbxh>>(key);
            if (list == null)
            {
                //string cmdText = @" select E_SBXH,sum(userscount) userCount from
//                                 (
//	                                SELECT case when b.E_SBXH='' or b.E_SBXH is null then 0 else SbxhID end SbxhID,userscount,isnull(b.E_SBXH,'') E_SBXH 
//	                                FROM 
//	                                (
//		                                SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount 
//		                                FROM Sjqd_StatUsersBySbxh WITH(NOLOCK)
//		                                WHERE Period=@period AND StatDate =  @StatDate  AND SoftID=@SoftID AND [Platform]=@Platform
//		                                GROUP BY SbxhID
//	                                ) A 
//	                                LEFT JOIN 
//	                                Sjqd_SBXH b WITH(NOLOCK) ON A.SbxhID=b.ID
//                                ) A
//                                group by E_SBXH
//                                ORDER BY sum(userscount) DESC";

//                SqlParameter[] param = new SqlParameter[] {
//                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
//                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
//                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
//                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
//                };
                int count = 0;

                String sql = string.Format(@"
                                    SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount 
                                    FROM U_StatUsersBySbxh 
                                    WHERE Period={0} AND StatDate={1} AND SoftID={2} AND Platform={3}
                                    GROUP BY SbxhID", (int) period, statDate, softId, (int) platform);

                list = new List<Sjqd_StatUsersBySbxh>();
                using (IDataReader read = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql))
                {
                    while (read.Read())
                    {
                        list.Add(new Sjqd_StatUsersBySbxh()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                SbxhID = Convert.ToInt32(read["SbxhID"].ToString())
                            });
                    }
                }

                var sbxhlst = Sjqd_SBXHService.GetSjqd_SBXHList((int) platform,
                                                                list.Select(p => p.SbxhID.ToString()).ToList());
                var reslst = from sjqdStatUsersBySbxh in list
                             join sjqdSbxh in sbxhlst on sjqdStatUsersBySbxh.SbxhID equals sjqdSbxh.ID into os
                             from tt in os.DefaultIfEmpty()
                             select new Sjqd_StatUsersBySbxh()
                                 {
                                     Sbxh = tt == null ? "" : tt.E_SBXH,
                                     UseCount = sjqdStatUsersBySbxh.UseCount
                                 };

                var realreslst = (from item in reslst
                                  group item by item.Sbxh
                                  into g
                                  select new Sjqd_StatUsersBySbxh()
                                      {
                                          Sbxh = g.Key,
                                          UseCount = g.Sum(p => p.UseCount)
                                      }).ToList();

                if (realreslst.Count > 0)
                {
                    CacheHelper.Set<List<Sjqd_StatUsersBySbxh>>(key, realreslst, CacheTimeOption.TenMinutes);
                    list = realreslst.OrderByDescending(p => p.UseCount).ToList();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取每一天
        /// </summary>
        /// <returns></returns>
        public List<Sjqd_StatUsersBySbxh> GetSBXHByDates(DateTime begintime, DateTime endtime, int softid, int platform,
                                                         string sbxh)
        {
            if (sbxh.Equals("未适配机型"))
            {
                sbxh = "";
            }
            string key = BuildCacheKey("GetSBXHByDates", begintime, endtime, softid, platform, sbxh);


            List<Sjqd_StatUsersBySbxh> list = CacheHelper.Get<List<Sjqd_StatUsersBySbxh>>(key);
            if (list == null)
            {
//                string cmdText = @"
//                                    select @sbxh E_SBXH,sum(userscount) userCount,StatDate from
//                                 (
//	                                SELECT case when b.E_SBXH='' or b.E_SBXH is null then 0 else SbxhID end SbxhID,userscount,StatDate,
//                                    isnull(b.E_SBXH,'') E_SBXH 
//	                                FROM 
//	                                (
//		                                SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount ,StatDate
//		                                FROM Sjqd_StatUsersBySbxh WITH(NOLOCK)
//		                                WHERE Period=@period AND StatDate  between @begintime and @endtime  AND SoftID=@SoftID AND [Platform]=@Platform
//		                                GROUP BY SbxhID,StatDate
//	                                ) A 
//	                                LEFT JOIN 
//	                                Sjqd_SBXH b WITH(NOLOCK) ON A.SbxhID=b.ID
//                                ) A
//                                where E_SBXH=@sbxh
//                                group by StatDate
//                                ORDER BY  StatDate asc 
//                                
//                                ";

//                SqlParameter[] param = new SqlParameter[] {
//                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = 1},
//                    new SqlParameter(){ ParameterName = "@begintime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = begintime.ToString("yyyyMMdd")},
//                    new SqlParameter(){ ParameterName = "@endtime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = endtime.ToString("yyyyMMdd")},
//                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softid},
//                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform},
//                    new SqlParameter(){ ParameterName = "@sbxh", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = sbxh}
//                };

                var sbxhlst = Sjqd_SBXHService.GetSjqd_SBXHList(platform, "", sbxh).Select(p => p.ID.ToString());

                String sql = string.Format(@"
                                    SELECT StatDate,SUM(NewUserCount+ActiveUserCount) userscount,'{6}' E_SBXH 
                                    FROM U_StatUsersBySbxh 
                                   WHERE Period={0} AND StatDate  between {1} and {2}  AND SoftID={3} AND Platform={4} and SbxhID in({5})
                                    GROUP BY StatDate", 1, begintime.ToString("yyyyMMdd"),
                                           endtime.ToString("yyyyMMdd"),
                                           softid,
                                           platform,
                                           String.Join(",", sbxhlst.ToArray()),
                                           sbxh);


                list = new List<Sjqd_StatUsersBySbxh>();

                using (var read = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql))
                {
                    while (read.Read())
                    {
                        int timedate = Convert.ToInt32(read["StatDate"]);

                        list.Add(new Sjqd_StatUsersBySbxh()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                Sbxh = read["E_SBXH"].ToString(),
                                StatDate = new DateTime(timedate/10000, timedate%10000/100, timedate%100)
                            });
                    }
                }

                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersBySbxh>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }

        /// <summary>
        /// 获取品牌分布
        /// </summary>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersBySbxh> GetSoftBrandTransverse(net91com.Stat.Core.PeriodOptions period, int statDate,
                                                                 int softId, MobileOption platform)
        {
            string key = BuildCacheKey("GetSoftBrandTransverse", period, statDate, softId, platform);
            List<Sjqd_StatUsersBySbxh> list = CacheHelper.Get<List<Sjqd_StatUsersBySbxh>>(key);
            if (list == null)
            {
//                string cmdText = @" select mobile_name,sum(userscount) userCount from (
//                                    SELECT case when b.mobile_name='' or b.mobile_name is null then '未适配品牌' else b.mobile_name end  mobile_name,userscount 
//                                    FROM (
//                                    SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount 
//                                    FROM Sjqd_StatUsersBySbxh WITH(NOLOCK)
//                                    WHERE Period=@period AND StatDate=@StatDate AND SoftID=@SoftID AND [Platform]=@Platform
//                                    GROUP BY SbxhID) A LEFT JOIN Sjqd_SBXH b WITH(NOLOCK) ON A.SbxhID=b.ID) A
//                                    group by  mobile_name
//                                    ORDER BY userCount DESC";

//                SqlParameter[] param = new SqlParameter[] {
//                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
//                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
//                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
//                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
//                };

                String sql = string.Format(@"
                                    SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount 
                                    FROM U_StatUsersBySbxh 
                                    WHERE Period={0} AND StatDate={1} AND SoftID={2} AND Platform={3}
                                    GROUP BY SbxhID", (int) period, statDate, softId, (int) platform);

                list = new List<Sjqd_StatUsersBySbxh>();
                using (IDataReader read = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql))
                {
                    while (read.Read())
                    {
                        list.Add(new Sjqd_StatUsersBySbxh()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                SbxhID = Convert.ToInt32(read["SbxhID"].ToString())
                            });
                    }
                }

                string strrr = "";
                foreach (string s2 in list.Select(p => p.SbxhID.ToString()).ToList())
                {
                    strrr += s2 + ",";
                    LogHelper.WriteInfo("s2:" + strrr);
                }

                var sbxhlst = Sjqd_SBXHService.GetSjqd_SBXHList((int) platform,
                                                                list.Select(p => p.SbxhID.ToString()).ToList());

                var reslst = from sjqdStatUsersBySbxh in list
                             join sjqdSbxh in sbxhlst on sjqdStatUsersBySbxh.SbxhID equals sjqdSbxh.ID into os
                             from tt in os.DefaultIfEmpty()
                             select new Sjqd_StatUsersBySbxh()
                                 {
                                     Brand = tt == null ? "未适配品牌" : tt.mobile_name,
                                     UseCount = sjqdStatUsersBySbxh.UseCount
                                 };

                var s = reslst.GroupBy(p => p.Brand).Select(p => p.Key).ToList();
                foreach (string sjqdStatUsersBySbxhs in s)
                {
                    LogHelper.WriteInfo("brand:" + sjqdStatUsersBySbxhs + "configlst:" + sbxhlst.Count + "reslst:" +
                                        reslst.Count());
                }
                var realreslst = (from item in reslst
                                  group item by item.Brand
                                  into g
                                  select new Sjqd_StatUsersBySbxh()
                                      {
                                          Brand = g.Key,
                                          UseCount = g.Sum(p => p.UseCount)
                                      }).ToList();

                if (realreslst.Count > 0)
                {
                    CacheHelper.Set<List<Sjqd_StatUsersBySbxh>>(key, realreslst, CacheTimeOption.TenMinutes);
                    list = realreslst.OrderByDescending(p => p.UseCount).ToList();
                }
            }
            return list;
        }


        /// <summary>
        /// 获取每一天
        /// </summary>
        /// <returns></returns>
        public List<Sjqd_StatUsersBySbxh> GetBrandByDates(DateTime begintime, DateTime endtime, int softid, int platform,
                                                          string mobile_name)
        {
            if (mobile_name == "未适配品牌")
            {
                mobile_name = "";
            }
            string key = BuildCacheKey("GetBrandByDates", begintime, endtime, softid, platform, mobile_name);


            List<Sjqd_StatUsersBySbxh> list = CacheHelper.Get<List<Sjqd_StatUsersBySbxh>>(key);
            if (list == null)
            {
//                string cmdText = @"select @mobile_name mobile_name,sum(userscount) userCount,StatDate from
//                                 (
//	                                SELECT case when b.mobile_name='' or b.mobile_name is null then 0 else SbxhID end SbxhID,userscount,StatDate,
//                                    isnull(b.mobile_name,'') mobile_name 
//	                                FROM 
//	                                (
//		                                SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount ,StatDate
//		                                FROM Sjqd_StatUsersBySbxh WITH(NOLOCK)
//		                                WHERE Period=@period AND StatDate  between @begintime and @endtime  AND SoftID=@SoftID AND [Platform]=@Platform
//		                                GROUP BY SbxhID,StatDate
//	                                ) A 
//	                                LEFT JOIN 
//	                                Sjqd_SBXH b WITH(NOLOCK) ON A.SbxhID=b.ID
//                                ) A
//                                where mobile_name=@mobile_name
//                                group by StatDate
//                                ORDER BY  StatDate asc 
//                                ";

//                SqlParameter[] param = new SqlParameter[] {
//                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = 1},
//                    new SqlParameter(){ ParameterName = "@begintime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = begintime.ToString("yyyyMMdd")},
//                    new SqlParameter(){ ParameterName = "@endtime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = endtime.ToString("yyyyMMdd")},
//                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softid},
//                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform},
//                    new SqlParameter(){ ParameterName = "@mobile_name", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = mobile_name}
//                };

                var sbxhlst = Sjqd_SBXHService.GetSjqd_SBXHList(platform, mobile_name, "").Select(p => p.ID.ToString());

                String sql = string.Format(@"
                                    SELECT StatDate,SUM(NewUserCount+ActiveUserCount) userscount,'{6}' mobile_name 
                                    FROM U_StatUsersBySbxh 
                                   WHERE Period={0} AND StatDate  between {1} and {2}  AND SoftID={3} AND Platform={4} and SbxhID in({5})
                                    GROUP BY StatDate", 1, begintime.ToString("yyyyMMdd"),
                                           endtime.ToString("yyyyMMdd"),
                                           softid,
                                           platform,
                                           String.Join(",", sbxhlst.ToArray()),
                                           mobile_name);

                list = new List<Sjqd_StatUsersBySbxh>();

                using (var read = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql))
                {
                    while (read.Read())
                    {
                        int timedate = Convert.ToInt32(read["StatDate"]);

                        list.Add(new Sjqd_StatUsersBySbxh()
                            {
                                UseCount = Convert.ToInt32(read["userscount"]),
                                Brand = read["mobile_name"].ToString(),
                                StatDate = new DateTime(timedate/10000, timedate%10000/100, timedate%100)
                            });
                    }
                }

                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersBySbxh>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }
    }
}