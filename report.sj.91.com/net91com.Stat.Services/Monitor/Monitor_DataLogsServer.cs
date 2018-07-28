using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core.Util;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using System.Data;
using System.Data.SqlClient;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Web;
using net91com.Stat.Services.Monitor.Entity;

namespace net91com.Stat.Services.Monitor
{
    public class Monitor_DataLogsServer
    {
        private static string _connectionString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
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

        public Monitor_DataLogsServer(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "Monitor_DataLogsServer";
            SqlHelper.CommandTimeout = 120;
        }

        private static Dictionary<string, string> dataLogNameDict = null;

        /// <summary>
        /// 获取日志名称列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetDataLogNameDirt()
        {
            if (dataLogNameDict == null)
            {
                dataLogNameDict = new Dictionary<string, string>();
                dataLogNameDict.Add("sjqd", "渠道");
                dataLogNameDict.Add("cpa", "CPA");
                dataLogNameDict.Add("sjzs", "助手");
                dataLogNameDict.Add("sjqd&ul", "外部渠道与时长");
                dataLogNameDict.Add("search", "搜索");
                dataLogNameDict.Add("down", "下载");
                dataLogNameDict.Add("func", "功能");
                dataLogNameDict.Add("pcfunc", "助手功能");
            }
            return dataLogNameDict;
        }

        /// <summary>
        /// 获取日志名称列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetServerIPList()
        {
            return ConfigHelper.GetSetting("MONITOR_SERVER_IP_LIST")
                               .Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        }

        private List<Monitor_DataLogs> GetLogListByDate(string dataLogName, string ServerIp, DateTime begintime,
                                                        DateTime endtime, net91com.Stat.Core.PeriodOptions period)
        {
            string sql = string.Empty;
            string groupby = string.Empty;
            string select = string.Empty;

            switch (period)
            {
                case net91com.Stat.Core.PeriodOptions.Daily:
                    select = string.Format("select DataLogName,{0}, LogDate,0 LogHour,sum(LogFileSize) LogFileSize",
                                           ServerIp == "0" ? "'不区分服务器' ServerIP" : "ServerIP");
                    groupby = string.Format("group by datalogname,{0} logdate ", ServerIp == "0" ? " " : "ServerIp,");
                    break;
                case net91com.Stat.Core.PeriodOptions.Hours:
                    select = string.Format("select DataLogName,{0},LogDate,LogHour,sum(LogFileSize) LogFileSize",
                                           ServerIp == "0" ? "'不区分服务器' ServerIP" : "ServerIP");
                    groupby = string.Format("group by datalogname,{0} logdate,LogHour ",
                                            ServerIp == "0" ? " " : "ServerIp,");
                    break;
                case net91com.Stat.Core.PeriodOptions.TimeOfDay:
                    select =
                        string.Format("select DataLogName,{0},20120501 LogDate,LogHour,sum(LogFileSize) LogFileSize",
                                      ServerIp == "0" ? "'不区分服务器' ServerIP" : "ServerIP");
                    groupby = string.Format("group by datalogname,{0} loghour", ServerIp == "0" ? " " : "ServerIp,");
                    break;
            }

            sql = select + string.Format(@" from Monitor_DataLogs
                                   where LogDate Between @begintime and @endtime and 
                                   DataLogName=@logname  {0}", ServerIp == "0" ? " " : "and ServerIP=@serverip ") +
                  groupby;
            SqlParameter[] para =
                {
                    new SqlParameter
                        {
                            ParameterName = "@begintime",
                            SqlDbType = SqlDbType.Int,
                            Size = 4,
                            Value = begintime.ToString("yyyyMMdd")
                        },
                    new SqlParameter
                        {
                            ParameterName = "@endtime",
                            SqlDbType = SqlDbType.Int,
                            Size = 4,
                            Value = endtime.ToString("yyyyMMdd")
                        },
                    new SqlParameter
                        {
                            ParameterName = "@logname",
                            SqlDbType = SqlDbType.VarChar,
                            Size = 100,
                            Value = dataLogName
                        },
                    new SqlParameter
                        {
                            ParameterName = "@serverip",
                            SqlDbType = SqlDbType.VarChar,
                            Size = 100,
                            Value = ServerIp
                        }
                };
            List<Monitor_DataLogs> list = new List<Monitor_DataLogs>();
            using (IDataReader dr = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, sql, para))
            {
                while (dr.Read())
                {
                    list.Add(Bind(dr));
                }
            }
            return list;
        }

        private List<Monitor_DataLogs> GetLogTransverse(string dataLogName, DateTime begintime, DateTime endtime)
        {
            string sql = @"select datalogname,serverip,0 logdate,0 loghour,sum(logfilesize) logfilesize
                            from dbo.Monitor_DataLogs
                            where LogDate Between @begintime and @endtime and  datalogname=@logname
                            group by datalogname,serverip";
            SqlParameter[] para =
                {
                    new SqlParameter
                        {
                            ParameterName = "@begintime",
                            SqlDbType = SqlDbType.Int,
                            Size = 4,
                            Value = begintime.ToString("yyyyMMdd")
                        },
                    new SqlParameter
                        {
                            ParameterName = "@endtime",
                            SqlDbType = SqlDbType.Int,
                            Size = 4,
                            Value = endtime.ToString("yyyyMMdd")
                        },
                    new SqlParameter
                        {
                            ParameterName = "@logname",
                            SqlDbType = SqlDbType.VarChar,
                            Size = 100,
                            Value = dataLogName
                        }
                };
            List<Monitor_DataLogs> list = new List<Monitor_DataLogs>();
            using (IDataReader dr = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, sql, para))
            {
                while (dr.Read())
                {
                    list.Add(Bind(dr));
                }
            }
            return list;
        }

        public List<Monitor_DataLogs> GetLogTransverseCache(string dataLogName, DateTime begintime, DateTime endtime,
                                                            CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetLogTransverseCache", dataLogName, begintime, endtime);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Monitor_DataLogs>>(cacheKey).ToList();
                }
                List<Monitor_DataLogs> list = GetLogTransverse(dataLogName, begintime, endtime);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Monitor_DataLogs>>(cacheKey, list, cachetime,
                                                            CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetLogTransverse(dataLogName, begintime, endtime);
            }
        }

        /// <summary>
        /// 日志列表按照日期
        /// </summary>
        /// <param name="dataLogName"></param>
        /// <param name="ServerIp"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="cachetime"></param>
        /// <returns></returns>
        public List<Monitor_DataLogs> GetLogListByDateCache(string dataLogName, string ServerIp, DateTime begintime,
                                                            DateTime endtime, net91com.Stat.Core.PeriodOptions period,
                                                            CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetLogListByDateCache", dataLogName, ServerIp, begintime, endtime,
                                                period);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Monitor_DataLogs>>(cacheKey).ToList();
                }
                List<Monitor_DataLogs> list = GetLogListByDate(dataLogName, ServerIp, begintime, endtime, period);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Monitor_DataLogs>>(cacheKey, list, cachetime,
                                                            CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetLogListByDate(dataLogName, ServerIp, begintime, endtime, period);
            }
        }

        public Monitor_DataLogs Bind(IDataReader dr)
        {
            Monitor_DataLogs log = new Monitor_DataLogs();
            object obj;
            obj = dr["DataLogName"];
            if (obj != DBNull.Value && obj != null)
            {
                log.DataLogName = GetDataLogNameDirt()[obj.ToString()];
            }
            obj = dr["ServerIP"];
            if (obj != DBNull.Value && obj != null)
            {
                log.ServerIp = obj.ToString();
            }
            obj = dr["LogHour"];
            if (obj != DBNull.Value && obj != null)
            {
                log.LogHour = Convert.ToInt32(obj);
            }
            obj = dr["LogFileSize"];
            if (obj != DBNull.Value && obj != null)
            {
                log.LogFileSize = Convert.ToInt32(obj);
            }
            obj = dr["LogDate"];
            if (obj != DBNull.Value && obj != null)
            {
                int tempDate = Convert.ToInt32(obj);
                if (tempDate > 0)
                {
                    log.LogDate = new DateTime(tempDate/10000, tempDate%10000/100, tempDate%100).AddHours(log.LogHour);
                }
            }
            return log;
        }
    }
}