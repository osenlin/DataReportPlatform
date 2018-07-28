using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
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
    public class StatUsersByAgeService
    {
        private static string connString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        private static string MySql_StatDbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        private static StatUsersByAgeService service;
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

        public static StatUsersByAgeService GetInstance()
        {
            if (service == null)
            {
                service = new StatUsersByAgeService();
                service._cachePreviousKey = "StatUsersByAgeService";
            }
            return service;
        }

        public List<Sjqd_StatUsersByAge> GetGetUsersAgesByCache(DateTime begintime, DateTime endtime, int softid,
                                                                int platform)
        {
            return CacheHelper.Get<List<Sjqd_StatUsersByAge>>(BuildCacheKey(begintime, endtime, softid, platform),
                                                              CacheTimeOption.TenMinutes,
                                                              () => GetUsersAges(begintime, endtime, softid, platform));
        }

        private List<Sjqd_StatUsersByAge> GetUsersAges(DateTime begintime, DateTime endtime, int softid, int platform)
        {
            string sql = @" select A.AgeDays,AVG(cast(A.UserCount as decimal(18,4))/B.UserCount) Percent
		                 from (
			                 select StatDate,AgeDays,SUM(UserCount) UserCount
			                 from (
				                 select StatDate,
					                case 
						                when AgeDays>=0 and AgeDays<=7 then 1
						                when AgeDays>=8 and AgeDays<=16 then 2
						                when AgeDays>=17 and AgeDays<=31 then 3
						                when AgeDays>=31 and AgeDays<=60 then 4
						                when AgeDays>=61 and AgeDays<=180  then 5
						                when AgeDays>=181 and AgeDays<=365 then 6
						                when AgeDays>=366 and AgeDays<=730 then 7 
						                when AgeDays>=731  then 8
					                end AgeDays,UserCount
				                from U_StatUsersByAge 
				                where Period=1 and Platform=?platform and SoftID=?SoftID  
					                 and StatDate between ?begintime and ?endtime  ) A
			                group by StatDate,AgeDays) A 
			                inner join (
			                select StatDate,SUM(UserCount) UserCount 
			                from U_StatUsersByAge 
			                where Period=1 and Platform=?platform and SoftID=?SoftID  
				                and StatDate between ?begintime and ?endtime   
			                group by StatDate) B
			                on A.StatDate=B.StatDate
		                group by A.AgeDays
		                order by A.AgeDays";
            MySqlParameter[] param = new MySqlParameter[]
                {
                    new MySqlParameter()
                        {
                            ParameterName = "?begintime",
                            Value = Convert.ToInt32(begintime.ToString("yyyyMMdd"))
                        },
                    new MySqlParameter()
                        {
                            ParameterName = "?endtime",
                            Value = Convert.ToInt32(endtime.ToString("yyyyMMdd"))
                        },
                    new MySqlParameter() {ParameterName = "?SoftID", Value = softid},
                    new MySqlParameter() {ParameterName = "?platform", Value = platform}
                };

            List<Sjqd_StatUsersByAge> lists = new List<Sjqd_StatUsersByAge>();
            using (var dr = MySqlHelper.ExecuteReader(MySql_StatDbConn, sql, param))
            {
                while (dr.Read())
                {
                    Sjqd_StatUsersByAge userAge = new Sjqd_StatUsersByAge();
                    if (dr["AgeDays"] != null && dr["AgeDays"] != DBNull.Value)
                    {
                        userAge.AgeDays = Convert.ToInt32(dr["AgeDays"]);
                    }

                    if (dr["Percent"] != null && dr["Percent"] != DBNull.Value)
                    {
                        userAge.Percent = Convert.ToDouble(dr["Percent"]);
                    }

                    lists.Add(userAge);
                }
            }
            var intList = lists.Select(p => p.AgeDays).ToList();
            for (int i = 1; i <= 8; i++)
            {
                if (!intList.Contains(i))
                    lists.Add(new Sjqd_StatUsersByAge {AgeDays = i, Percent = 0});
            }
            return lists.OrderBy(p => p.AgeDays).ToList();
        }
    }
}