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

namespace net91com.Stat.Services.sjqd
{
    public class StatUsersByIntervalService
    {
        private static string connString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static StatUsersByIntervalService service;
        private string _cachePreviousKey;

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

        public static StatUsersByIntervalService GetInstance()
        {
            if (service == null)
            {
                service = new StatUsersByIntervalService();
                service._cachePreviousKey = "StatUsersByIntervalService";
            }
            return service;
        }

        public List<Sjqd_StatUsersByInterval> GetIntervalTimesByCache(DateTime begintime, DateTime endtime, int softid,
                                                                      int platform)
        {
            return CacheHelper.Get<List<Sjqd_StatUsersByInterval>>(BuildCacheKey(begintime, endtime, softid, platform),
                                                                   CacheTimeOption.TenMinutes,
                                                                   () =>
                                                                   GetIntervalTimes(begintime, endtime, softid, platform));
        }

        private List<Sjqd_StatUsersByInterval> GetIntervalTimes(DateTime begintime, DateTime endtime, int softid,
                                                                int platform)
        {
            string sql = @" select A.IntervalDays,AVG(cast(A.UserCount as decimal(18,4))/B.UserCount) [Percent]
		                 from (
			                 select StatDate,IntervalDays,SUM(UserCount) UserCount
			                 from (
				                 select StatDate,
					                case 
						                when IntervalDays>=0 and IntervalDays<=1 then 1
						                when IntervalDays>=2 and IntervalDays<=3 then 2
						                when IntervalDays>=4 and IntervalDays<=7 then 3
						                when IntervalDays>=8 and IntervalDays<=15 then 4
						                when IntervalDays>=16 and IntervalDays<=29 then 5
						                else 6 
					                end IntervalDays,UserCount
				                from Sjqd_StatUsersByInterval with(nolock)
				                where Period=1 and Platform=@platform and SoftID=@SoftID 
					                 and StatDate between @begintime and @endtime  ) A
			                group by StatDate,IntervalDays) A 
			                inner join (
			                select StatDate,SUM(UserCount) UserCount 
			                from Sjqd_StatUsersByInterval with(nolock)
			                where Period=1 and Platform=@platform and SoftID=@SoftID  
				                and StatDate between @begintime and @endtime     
			                group by StatDate) B
			                on A.StatDate=B.StatDate
		                group by A.IntervalDays
		                order by A.IntervalDays";
            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter()
                        {
                            ParameterName = "@begintime",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Size = 4,
                            Value = Convert.ToInt32(begintime.ToString("yyyyMMdd"))
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@endtime",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Size = 4,
                            Value = Convert.ToInt32(endtime.ToString("yyyyMMdd"))
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@SoftID",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Size = 4,
                            Value = softid
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@platform",
                            SqlDbType = System.Data.SqlDbType.TinyInt,
                            Size = 1,
                            Value = platform
                        }
                };

            List<Sjqd_StatUsersByInterval> lists = new List<Sjqd_StatUsersByInterval>();
            using (IDataReader dr = SqlHelper.ExecuteReader(connString, CommandType.Text, sql, param))
            {
                while (dr.Read())
                {
                    Sjqd_StatUsersByInterval userIntervel = new Sjqd_StatUsersByInterval();
                    if (dr["IntervalDays"] != null && dr["IntervalDays"] != DBNull.Value)
                    {
                        userIntervel.IntervalDays = Convert.ToInt32(dr["IntervalDays"]);
                    }

                    if (dr["Percent"] != null && dr["Percent"] != DBNull.Value)
                    {
                        userIntervel.Percent = Convert.ToDouble(dr["Percent"]);
                    }

                    lists.Add(userIntervel);
                }
            }
            var intList = lists.Select(p => p.IntervalDays).ToList();
            for (int i = 1; i <= 6; i++)
            {
                if (!intList.Contains(i))
                    lists.Add(new Sjqd_StatUsersByInterval {IntervalDays = i, Percent = 0});
            }
            return lists.OrderBy(p => p.IntervalDays).ToList();
        }
    }
}