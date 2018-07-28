using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core.Util;
using net91com.Stat.Services.sjqd.Entity;
using System.Data.SqlClient;
using net91com.Core.Data;
using System.Data;
using net91com.Core.Web;
using net91com.Core;

namespace net91com.Stat.Services.sjqd
{
    public class Sjqd_StatUsersByOneTimeService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
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

        public Sjqd_StatUsersByOneTimeService(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "Sjqd_StatUsersByOneTimeService";
            SqlHelper.CommandTimeout = 120;
        }

        /// <summary>
        /// 根据软件ID 平台获取一次性用户
        /// </summary>
        /// <param name="soft"></param>
        /// <param name="platform"></param>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByOneTime> GetSjqd_StatUsersByOneTime(int soft, int platform, int channelid)
        {
            string key = BuildCacheKey("GetSjqd_StatUsersByOneTime", soft, platform, channelid);
            List<Sjqd_StatUsersByOneTime> list = CacheHelper.Get<List<Sjqd_StatUsersByOneTime>>(key);
            if (list == null)
            {
                string cmdText = @" select SoftID,Platform,ChannelID,UserCount from Sjqd_StatUsersByOneTime
                                    where softid=@SoftID and Platform=@Platform and channelid=@channelid ";

                SqlParameter[] param = new SqlParameter[]
                    {
                        new SqlParameter()
                            {
                                ParameterName = "@channelid",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Size = 4,
                                Value = channelid
                            },
                        new SqlParameter()
                            {
                                ParameterName = "@SoftID",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Size = 4,
                                Value = soft
                            },
                        new SqlParameter()
                            {
                                ParameterName = "@Platform",
                                SqlDbType = System.Data.SqlDbType.TinyInt,
                                Size = 1,
                                Value = platform
                            }
                    };

                list = new List<Sjqd_StatUsersByOneTime>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, cmdText, param))
                {
                    while (read.Read())
                    {
                        Sjqd_StatUsersByOneTime onetimeuser = new Sjqd_StatUsersByOneTime();
                        if (read["SoftID"] != null && read["SoftID"] != DBNull.Value)
                        {
                            onetimeuser.SoftID = Convert.ToInt32(read["SoftID"]);
                        }
                        if (read["Platform"] != null && read["Platform"] != DBNull.Value)
                        {
                            onetimeuser.Platform = Convert.ToInt32(read["Platform"]);
                        }
                        if (read["channelid"] != null && read["channelid"] != DBNull.Value)
                        {
                            onetimeuser.ChannelID = Convert.ToInt32(read["channelid"]);
                        }
                        if (read["UserCount"] != null && read["UserCount"] != DBNull.Value)
                        {
                            onetimeuser.UserCount = Convert.ToInt32(read["UserCount"]);
                        }
                        list.Add(onetimeuser);
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByOneTime>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }

        /// <summary>
        /// 一次性获取多个软件的一次性用户
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByOneTime> GetSjqd_AllSoftsOneTimeUsers(string ids)
        {
            string key = BuildCacheKey("GetSjqd_StatUsersByOneTime", ids);
            List<Sjqd_StatUsersByOneTime> list = CacheHelper.Get<List<Sjqd_StatUsersByOneTime>>(key);
            if (list == null)
            {
                if (string.IsNullOrEmpty(ids.Trim()))
                    return new List<Sjqd_StatUsersByOneTime>();
                string cmdText =
                    string.Format(@" select SoftID,Platform,ChannelID,UserCount from Sjqd_StatUsersByOneTime
                                    where softid in({0}) and channelid=0 ", ids);

                list = new List<Sjqd_StatUsersByOneTime>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, cmdText))
                {
                    while (read.Read())
                    {
                        Sjqd_StatUsersByOneTime onetimeuser = new Sjqd_StatUsersByOneTime();
                        if (read["SoftID"] != null && read["SoftID"] != DBNull.Value)
                        {
                            onetimeuser.SoftID = Convert.ToInt32(read["SoftID"]);
                        }
                        if (read["Platform"] != null && read["Platform"] != DBNull.Value)
                        {
                            onetimeuser.Platform = Convert.ToInt32(read["Platform"]);
                        }
                        if (read["channelid"] != null && read["channelid"] != DBNull.Value)
                        {
                            onetimeuser.ChannelID = Convert.ToInt32(read["channelid"]);
                        }
                        if (read["UserCount"] != null && read["UserCount"] != DBNull.Value)
                        {
                            onetimeuser.UserCount = Convert.ToInt32(read["UserCount"]);
                        }
                        list.Add(onetimeuser);
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByOneTime>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }
    }
}