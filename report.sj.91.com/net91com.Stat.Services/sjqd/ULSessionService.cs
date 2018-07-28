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
using MySql.Data.MySqlClient;

namespace net91com.Stat.Services.sjqd
{
    public class ULSessionService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static string StatDB_MySQL_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
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

        public ULSessionService(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "SessionService";
            SqlHelper.CommandTimeout = 120;
        }

        /// <summary>
        /// 在表sjqd_statusers 获取平均时长和平均使用次数
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platformid"></param>
        /// <param name="period"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public List<Sjqd_ULSessionAvgUsers> GetULSessionAvgUsers(int softid, int platformid, int period,
                                                                 DateTime begintime, DateTime endtime)
        {
            string sql = @"
    select 
	    a.StatDate,a.Period,a.SoftID,a.Platform,
	    NewUserCountFromCache+NewUserCount NewUserCount,
	    ActiveUserCountFromCache+ActiveUserCount ActiveUserCount,
	    #temp.AvgSessions, #temp.AvgSessionLength
    from  Sjqd_StatUsers a left join #temp 
    on a.StatDate = #temp.StatDate and a.Period = #temp.period and a.SoftID = #temp.softid and a.Platform = #temp.platform
    where a.SoftID=@softid and a.platform=@platform
    and a.StatDate between @begintime and @endtime and a.Period=@period";

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platformid),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@begintime", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd")))
                    ,
                    SqlParamHelper.MakeInParam("@endtime", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };

            var sql_session = @"
    select 
	    StatDate,Period,SoftID,Platform, 
	    AvgSessions,AvgSessionLength  
    from  UL_SessionLengthAvg
    where SoftID=?softid and platform=?platform
    and StatDate between ?begintime and ?endtime and Period=?period";

            var mysql_params = new MySqlParameter[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform", platformid),
                    new MySqlParameter("?period", period),
                    new MySqlParameter("?begintime", int.Parse(begintime.ToString("yyyyMMdd"))),
                    new MySqlParameter("?endtime", int.Parse(endtime.ToString("yyyyMMdd")))
                };

            var ds = MySqlHelper.ExecuteDataset(StatDB_MySQL_ConnString, sql_session, mysql_params);

            List<Sjqd_ULSessionAvgUsers> ulSessionsAvg = new List<Sjqd_ULSessionAvgUsers>();

            using (var conn = new SqlConnection(statdbConn))
            {
                conn.Open();

                var sql_temp =
                    "create table #temp (statDate int, period int, softid int, platform int, avgsessions decimal(10,2),AvgSessionLength decimal(10,2));";
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql_temp);

                using (var sqlBulkCopy = new SqlBulkCopy(conn))
                {
                    sqlBulkCopy.DestinationTableName = "#temp";
                    sqlBulkCopy.WriteToServer(ds.Tables[0]);
                }

                using (var dataReader = SqlHelper.ExecuteReader(conn, CommandType.Text, sql, parameters))
                {
                    while (dataReader.Read())
                    {
                        Sjqd_ULSessionAvgUsers ulAvgUser = new Sjqd_ULSessionAvgUsers();
                        if (dataReader["StatDate"] != null && dataReader["StatDate"] != DBNull.Value)
                        {
                            int date = Convert.ToInt32(dataReader["StatDate"]);
                            ulAvgUser.StatDate = new DateTime(date/10000, date%10000/100, date%100);
                        }
                        if (dataReader["Period"] != null && dataReader["Period"] != DBNull.Value)
                        {
                            ulAvgUser.Period = (net91com.Stat.Core.PeriodOptions) Convert.ToInt32(dataReader["Period"]);
                        }
                        if (dataReader["SoftID"] != null && dataReader["SoftID"] != DBNull.Value)
                        {
                            ulAvgUser.SoftID = Convert.ToInt32(dataReader["SoftID"]);
                        }
                        if (dataReader["Platform"] != null && dataReader["Platform"] != DBNull.Value)
                        {
                            ulAvgUser.Platform = (MobileOption) Convert.ToInt32(dataReader["Platform"]);
                        }
                        if (dataReader["NewUserCount"] != DBNull.Value && dataReader["ActiveUserCount"] != DBNull.Value)
                        {
                            ulAvgUser.UseUsers = Convert.ToInt32(dataReader["NewUserCount"]) +
                                                 Convert.ToInt32(dataReader["ActiveUserCount"]);
                        }
                        if (dataReader["AvgSessions"] != null && dataReader["AvgSessions"] != DBNull.Value)
                        {
                            ulAvgUser.AvgSessions = Convert.ToDecimal(dataReader["AvgSessions"]);
                        }
                        if (dataReader["AvgSessionLength"] != null && dataReader["AvgSessionLength"] != DBNull.Value)
                        {
                            ulAvgUser.AvgSessionLength = Convert.ToDecimal(dataReader["AvgSessionLength"]);
                        }
                        ulAvgUser.AllSessionLength = (long) (ulAvgUser.UseUsers*ulAvgUser.AvgSessionLength);
                        ulAvgUser.AllSessions = (int) (ulAvgUser.UseUsers*ulAvgUser.AvgSessions);
                        ulAvgUser.AvgLengthPerSession = ulAvgUser.AvgSessions == 0
                                                            ? 0
                                                            : Math.Round(
                                                                ulAvgUser.AvgSessionLength/ulAvgUser.AvgSessions, 2);
                        ulSessionsAvg.Add(ulAvgUser);
                    }
                }
            }
            return ulSessionsAvg;
        }

        public List<Sjqd_ULSessionAvgUsers> GetULSessionAvgUsersCache(int softid, int platformid, int period,
                                                                      DateTime begintime, DateTime endtime,
                                                                      CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetULSessionAvgUsersCache", softid, platformid, period, begintime,
                                                endtime);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_ULSessionAvgUsers>>(cacheKey).ToList();
                }
                List<Sjqd_ULSessionAvgUsers> list = GetULSessionAvgUsers(softid, platformid, period, begintime, endtime);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_ULSessionAvgUsers>>(cacheKey, list, cachetime,
                                                                  CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetULSessionAvgUsers(softid, platformid, period, begintime, endtime);
            }
        }

        /// <summary>
        /// 单次对应时长的人数和次数
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="versionid"></param>
        /// <param name="period"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public List<Sjqd_ULSessionsSingle> GetSingleULSessionsLength(int softid, int platform, string version,
                                                                     int period, DateTime begintime, DateTime endtime)
        {
            string tablename = "UL_SessionSingle";
            string where = @" where Period=?period and Platform=?platform and SoftID=?softid  
					                 and StatDate between ?begintime and ?endtime ";

            if (version != "不区分版本")
            {
                tablename = "UL_SessionSingleByVersion";
                where = @" where Period=?period and Platform=?platform and SoftID=?softid  
					                 and StatDate between ?begintime and ?endtime  and Version='" + version + "' ";
            }

            string sql = @"select A.SessionLengthLevel,AVG(cast(A.Users as decimal(18,4))/B.Users) Percent
		                 from (
			                 select StatDate,SessionLengthLevel,SUM(Users) Users
			                 from (
				                 select StatDate,
					                case 
						                when SessionLengthLevel>=1 and SessionLengthLevel<=3 then 1
						                when SessionLengthLevel>3 and SessionLengthLevel<=60 then 2
						                when SessionLengthLevel>60 and SessionLengthLevel<=300 then 3
						                when SessionLengthLevel>300 and SessionLengthLevel<=600 then 4
						                when SessionLengthLevel>600 and SessionLengthLevel<=1800  then 5
						                when SessionLengthLevel>1800 and SessionLengthLevel<=3600 then 6
						                when SessionLengthLevel>3600 and SessionLengthLevel<=7200 then 7 
						                when SessionLengthLevel>7200  then 8
					                end SessionLengthLevel,Users
				                from " + tablename + where + @") A
			                group by StatDate,SessionLengthLevel) A 
			                inner join (
			                select StatDate,SUM(Users) Users 
			                from  " + tablename + where + @"
			                group by StatDate) B
			                on A.StatDate=B.StatDate
		                group by A.SessionLengthLevel
		                order by A.SessionLengthLevel;";


            var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform", platform),
                    new MySqlParameter("?period", period),
                    new MySqlParameter("?begintime", int.Parse(begintime.ToString("yyyyMMdd"))),
                    new MySqlParameter("?endtime", int.Parse(endtime.ToString("yyyyMMdd")))
                };

            List<Sjqd_ULSessionsSingle> ulSessionsAvg = new List<Sjqd_ULSessionsSingle>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, sql, parameters))
            {
                while (dataReader.Read())
                {
                    Sjqd_ULSessionsSingle ulSS = new Sjqd_ULSessionsSingle();
                    if (dataReader["SessionLengthLevel"] != null && dataReader["SessionLengthLevel"] != DBNull.Value)
                    {
                        ulSS.SessionLengthLevel = Convert.ToInt32(dataReader["SessionLengthLevel"]);
                    }
                    if (dataReader["Percent"] != null && dataReader["Percent"] != DBNull.Value)
                    {
                        ulSS.Percent = Math.Round(Convert.ToDecimal(dataReader["Percent"]), 4);
                    }
                    ulSessionsAvg.Add(ulSS);
                }
                var list = ulSessionsAvg.Select(p => p.SessionLengthLevel).ToList();
                for (int i = 1; i <= 8; i++)
                {
                    if (!list.Contains(i))
                        ulSessionsAvg.Add(new Sjqd_ULSessionsSingle {SessionLengthLevel = i, Percent = 0});
                }
            }
            return ulSessionsAvg;
        }

        public List<Sjqd_ULSessionsSingle> GetSingleULSessionsLengthCache(int softid, int platform, string version,
                                                                          int period, DateTime begintime,
                                                                          DateTime endtime, CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetSingleULSessionsLengthCache", softid, platform, version, period,
                                                begintime, endtime);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_ULSessionsSingle>>(cacheKey).ToList();
                }
                List<Sjqd_ULSessionsSingle> list = GetSingleULSessionsLength(softid, platform, version, period,
                                                                             begintime, endtime);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_ULSessionsSingle>>(cacheKey, list, cachetime,
                                                                 CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetSingleULSessionsLength(softid, platform, version, period, begintime, endtime);
            }
        }

        /// <summary>
        /// 获取时间范围内的时长分布
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="versionid"></param>
        /// <param name="period"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public List<Sjqd_ULSessionLength> GetULSessionsLength(int softid, int platform, string version, int period,
                                                              DateTime begintime, DateTime endtime)
        {
            string tablename = "UL_SessionLength";
            string where = @" where Period=?period and Platform=?platform and SoftID=?softid  
					                 and StatDate between ?begintime and ?endtime ";

            if (version != "不区分版本")
            {
                tablename = "UL_SessionLengthByVersion";
                where = @" where Period=?period and Platform=?platform and SoftID=?softid  
					                 and StatDate between ?begintime and ?endtime and Version='" + version + "' ";
            }
            string sql = @"select A.SessionLengthLevel,AVG(cast(A.Users as decimal(18,4))/B.Users) Percent
							 from (
								 select StatDate,SessionLengthLevel,SUM(Users) Users
								 from (
									 select StatDate,
										case 
											when SessionLengthLevel>=1 and SessionLengthLevel<=10 then 1
											when SessionLengthLevel>10 and SessionLengthLevel<=60 then 2
											when SessionLengthLevel>60 and SessionLengthLevel<=240 then 3
											when SessionLengthLevel>240 and SessionLengthLevel<=1200 then 4
											when SessionLengthLevel>1200 and SessionLengthLevel<=3600  then 5
											when SessionLengthLevel>3600 and SessionLengthLevel<=7200 then 6
											when SessionLengthLevel>7200 and SessionLengthLevel<=18000 then 7 
											when SessionLengthLevel>18000  then 8
										end SessionLengthLevel,Users
									from " + tablename + where + @") A
								group by StatDate,SessionLengthLevel) A 
								inner join (
								select StatDate,SUM(Users) Users 
								from " + tablename + where + @"
								group by StatDate) B
								on A.StatDate=B.StatDate
							group by A.SessionLengthLevel
							order by A.SessionLengthLevel";

            if (period == 3)
            {
                sql = @"select A.SessionLengthLevel,AVG(cast(A.Users as decimal(18,4))/B.Users) Percent
							 from (
								 select StatDate,SessionLengthLevel,SUM(Users) Users
								 from (
									 select StatDate,
										case 
											when SessionLengthLevel>=1 and SessionLengthLevel<=20 then 1
											when SessionLengthLevel>20 and SessionLengthLevel<=60 then 2
											when SessionLengthLevel>60 and SessionLengthLevel<=3600 then 3
											when SessionLengthLevel>3600 and SessionLengthLevel<=10800 then 4
											when SessionLengthLevel>10800 and SessionLengthLevel<=28800  then 5
											when SessionLengthLevel>28800 and SessionLengthLevel<=57600 then 6
											when SessionLengthLevel>57600 and SessionLengthLevel<=86400 then 7 
											when SessionLengthLevel>86400  then 8
										end SessionLengthLevel,Users
									from " + tablename + where + @") A
								group by StatDate,SessionLengthLevel) A 
								inner join (
								select StatDate,SUM(Users) Users 
								from  " + tablename + where + @"
								group by StatDate) B
								on A.StatDate=B.StatDate
							group by A.SessionLengthLevel
							order by A.SessionLengthLevel";
            }

            if (period != 1 && period != 3)
            {
                sql = @"select A.SessionLengthLevel,AVG(cast(A.Users as decimal(18,4))/B.Users) Percent
							 from (
								 select StatDate,SessionLengthLevel,SUM(Users) Users
								 from (
									 select StatDate,
										case 
											when SessionLengthLevel>=1 and SessionLengthLevel<=60 then 1
											when SessionLengthLevel>60 and SessionLengthLevel<=3600 then 2
											when SessionLengthLevel>3600 and SessionLengthLevel<=28800 then 3
											when SessionLengthLevel>28800 and SessionLengthLevel<=86400 then 4
											when SessionLengthLevel>86400 and SessionLengthLevel<=172800  then 5
											when SessionLengthLevel>172800 and SessionLengthLevel<=259200 then 6
											when SessionLengthLevel>259200 and SessionLengthLevel<=345600 then 7 
											when SessionLengthLevel>345600  then 8
										end SessionLengthLevel,Users
									from " + tablename + where + @") A
								group by StatDate,SessionLengthLevel) A 
								inner join (
								select StatDate,SUM(Users) Users 
								from  " + tablename + where + @"
								group by StatDate) B
								on A.StatDate=B.StatDate
							group by A.SessionLengthLevel
							order by A.SessionLengthLevel";
            }


            var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform", platform),
                    new MySqlParameter("?period", period),
                    new MySqlParameter("?begintime", int.Parse(begintime.ToString("yyyyMMdd"))),
                    new MySqlParameter("?endtime", int.Parse(endtime.ToString("yyyyMMdd")))
                };
            List<Sjqd_ULSessionLength> ulSessionL = new List<Sjqd_ULSessionLength>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, sql, parameters))
            {
                while (dataReader.Read())
                {
                    Sjqd_ULSessionLength ulSL = new Sjqd_ULSessionLength();
                    if (dataReader["SessionLengthLevel"] != null && dataReader["SessionLengthLevel"] != DBNull.Value)
                    {
                        ulSL.SessionLengthLevel = Convert.ToInt32(dataReader["SessionLengthLevel"]);
                    }
                    if (dataReader["Percent"] != null && dataReader["Percent"] != DBNull.Value)
                    {
                        ulSL.Percent = Math.Round(Convert.ToDecimal(dataReader["Percent"]), 4);
                    }

                    ulSessionL.Add(ulSL);
                }
                var list = ulSessionL.Select(p => p.SessionLengthLevel).ToList();
                for (int i = 1; i <= 8; i++)
                {
                    if (!list.Contains(i))
                        ulSessionL.Add(new Sjqd_ULSessionLength {SessionLengthLevel = i, Percent = 0});
                }
            }
            return ulSessionL;
        }

        public List<Sjqd_ULSessionLength> GetULSessionsLengthCache(int softid, int platform, string version, int period,
                                                                   DateTime begintime, DateTime endtime,
                                                                   CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetULSessionsLengthCache", softid, platform, version, period, begintime,
                                                endtime);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_ULSessionLength>>(cacheKey).ToList();
                }
                List<Sjqd_ULSessionLength> list = GetULSessionsLength(softid, platform, version, period, begintime,
                                                                      endtime);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_ULSessionLength>>(cacheKey, list, cachetime,
                                                                CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetULSessionsLength(softid, platform, version, period, begintime, endtime);
            }
        }

        /// <summary>
        /// 获取启动次数分布
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="versionid"></param>
        /// <param name="period"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public List<Sjqd_ULSessionsCount> GetULSessionsCount(int softid, int platform, string version, int period,
                                                             DateTime begintime, DateTime endtime)
        {
            var tablename = "UL_SessionsCount";
            var where = @" where Period=?period and Platform=?platform and SoftID=?softid  
					                 and StatDate between ?begintime and ?endtime ";

            if (version != "不区分版本")
            {
                tablename = "UL_SessionsCountByVersion";
                where = @" where Period=?period and Platform=?platform and SoftID=?softid  
					                 and StatDate between ?begintime and ?endtime and Version='" + version + "' ";
            }

            var sql = @"select A.SessionsLevel,AVG(cast(A.Users as decimal(18,4))/B.Users) Percent
		                 from (
			                 select StatDate,SessionsLevel,SUM(Users) Users
			                 from (
				                 select StatDate,
					                case 
						                when SessionsLevel>=0 and SessionsLevel<=1 then 1
						                when SessionsLevel>1 and SessionsLevel<=3 then 2
						                when SessionsLevel>3 and SessionsLevel<=6 then 3
						                when SessionsLevel>6 and SessionsLevel<=9 then 4
						                when SessionsLevel>9 and SessionsLevel<=12  then 5
						                when SessionsLevel>12 and SessionsLevel<=17 then 6
						                when SessionsLevel>17 and SessionsLevel<=22 then 7 
						                when SessionsLevel>22  then 8
					                end SessionsLevel,Users
				                from " + tablename + where + @") A
			                group by StatDate,SessionsLevel) A 
			                inner join (
			                select StatDate,SUM(Users) Users 
			                from  " + tablename + where + @"
			                group by StatDate) B
			                on A.StatDate=B.StatDate
		                group by A.SessionsLevel
		                order by A.SessionsLevel";

            if (period == 3)
            {
                sql = @"select A.SessionsLevel,AVG(cast(A.Users as decimal(18,4))/B.Users) Percent
		                 from (
			                 select StatDate,SessionsLevel,SUM(Users) Users
			                 from (
				                 select StatDate, SessionsLevel,Users
				                from  " + tablename + where + @") A
			                group by StatDate,SessionsLevel) A 
			                inner join (
			                select StatDate,SUM(Users) Users 
			                from  " + tablename + where + @"
			                group by StatDate) B
			                on A.StatDate=B.StatDate
		                group by A.SessionsLevel
		                order by A.SessionsLevel";
            }
            if (period != 1 && period != 3)
            {
                sql = @"select A.SessionsLevel,AVG(cast(A.Users as decimal(18,4))/B.Users) Percent
		                 from (
			                 select StatDate,SessionsLevel,SUM(Users) Users
			                 from (
				                 select StatDate,
					                case 
						                when SessionsLevel>=0 and SessionsLevel<=2 then 1
						                when SessionsLevel>2 and SessionsLevel<=4 then 2
						                when SessionsLevel>4 and SessionsLevel<=8 then 3
						                when SessionsLevel>8 and SessionsLevel<=12 then 4
						                when SessionsLevel>12 and SessionsLevel<=16  then 5
						                when SessionsLevel>16 and SessionsLevel<=20 then 6
						                when SessionsLevel>20 and SessionsLevel<=25 then 7 
						                when SessionsLevel>25  then 8
					                end SessionsLevel,Users
				                from  " + tablename + where + @") A
			                group by StatDate,SessionsLevel) A 
			                inner join (
			                select StatDate,SUM(Users) Users 
			                from  " + tablename + where + @"
			                group by StatDate) B
			                on A.StatDate=B.StatDate
		                group by A.SessionsLevel
		                order by A.SessionsLevel";
            }
            var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform", platform),
                    new MySqlParameter("?period", period),
                    new MySqlParameter("?begintime", int.Parse(begintime.ToString("yyyyMMdd"))),
                    new MySqlParameter("?endtime", int.Parse(endtime.ToString("yyyyMMdd")))
                };
            List<Sjqd_ULSessionsCount> ulSessionC = new List<Sjqd_ULSessionsCount>();

            using (IDataReader dataReader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, sql, parameters))
            {
                while (dataReader.Read())
                {
                    Sjqd_ULSessionsCount ulSC = new Sjqd_ULSessionsCount();
                    if (dataReader["SessionsLevel"] != null && dataReader["SessionsLevel"] != DBNull.Value)
                    {
                        ulSC.SessionsLevel = Convert.ToInt32(dataReader["SessionsLevel"]);
                    }
                    if (dataReader["Percent"] != null && dataReader["Percent"] != DBNull.Value)
                    {
                        ulSC.Percent = Math.Round(Convert.ToDecimal(dataReader["Percent"]), 4);
                    }
                    ulSessionC.Add(ulSC);
                }
                var list = ulSessionC.Select(p => p.SessionsLevel).ToList();
                for (int i = 1; i <= 8; i++)
                {
                    if (!list.Contains(i))
                        ulSessionC.Add(new Sjqd_ULSessionsCount {SessionsLevel = i, Percent = 0});
                }
            }
            return ulSessionC;
        }

        public List<Sjqd_ULSessionsCount> GetULSessionsCountCache(int softid, int platform, string version, int period,
                                                                  DateTime begintime, DateTime endtime,
                                                                  CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetULSessionsCountCache", softid, platform, version, period, begintime,
                                                endtime);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_ULSessionsCount>>(cacheKey).ToList();
                }
                List<Sjqd_ULSessionsCount> list = GetULSessionsCount(softid, platform, version, period, begintime,
                                                                     endtime);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_ULSessionsCount>>(cacheKey, list, cachetime,
                                                                CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetULSessionsCount(softid, platform, version, period, begintime, endtime);
            }
        }
    }
}