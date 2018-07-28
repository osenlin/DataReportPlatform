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

using net91com.Reports.UserRights;

namespace net91com.Stat.Services.sjqd
{
    public class StatUsersByAreaService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDBReport_ConnString");
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
        public StatUsersByAreaService(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "StatUsersByAreaService";
            SqlHelper.CommandTimeout = 120;
        }
        /// <summary>
        /// 中国地区查找
        /// </summary>
        /// <param name="period">周期</param>
        /// <param name="statDate">统计日期</param>
        /// <param name="softId">软件ID</param>
        /// <param name="platform">平台</param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaTransverseWithChina(PeriodOptions period, int statDate, int softId, MobileOption platform)
        {

            string key = BuildCacheKey("GetSoftAreaTransverseWithChina", period, statDate, softId, platform);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
            if (list == null)
            {

                string cmdText = @" SELECT B.Province AreaName,sum(userscount) usercount FROM
                                    (
	                                    select AreaID , NewUserCount+ActiveUserCount  userscount 
	                                    from  Sjqd_StatUsersByArea with(nolock)
	                                    where Period=@period and StatDate=@StatDate and  SoftID=@SoftID AND [Platform]=@Platform and ChannelID=0
                                    )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                    ON A.AreaID=B.ID  and B.E_Country='中国'
                                    group by B.Province order by sum(userscount) desc";

                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
                };

                list = new List<Sjqd_StatUsersByArea>();
                int allcount = 0;
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, cmdText, param))
                {
                    while (read.Read())
                    {
                        allcount += Convert.ToInt32(read["usercount"]);
                        list.Add(new Sjqd_StatUsersByArea()
                        {
                            UseCount = Convert.ToInt32(read["usercount"]),
                            AreaName = read["AreaName"].ToString(),
                            
                        });
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }

        /// <summary>
        /// 按照渠道商来查找数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="channelids"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaTransverseWithChinaByChannels(int softId, MobileOption platform, PeriodOptions period, int statDate, ChannelTypeOptions selectchanneltype,string channelids)
        {
            string sql = string.Empty;
            string key = BuildCacheKey("GetSoftAreaTransverseWithChinaByChannels", period, statDate, softId, platform, selectchanneltype, channelids);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
           
            if (list == null)
            {
                int[] channels = channelids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
                List<int> channelIds = new URLoginService().GetAvailableChannelIds(softId, selectchanneltype, channels);

                if (channelIds.Count == 0)
                    return new List<Sjqd_StatUsersByArea>();

                string channelIdsString = string.Join(",", channelIds.Select(a => a.ToString()).ToArray());

                sql = string.Format(@" SELECT B.Province AreaName,sum(userscount) usercount FROM
                                    (
	                                    select AreaID , NewUserCount+ActiveUserCount  userscount 
	                                    from  Sjqd_StatUsersByArea with(nolock)
	                                    where Period=@period and StatDate=@StatDate and  SoftID=@SoftID AND [Platform]=@Platform and ChannelID in ({0})
                                    )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                    ON A.AreaID=B.ID  and B.E_Country='中国'
                                    group by B.Province order by sum(userscount) desc", channelIdsString);
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
                };
                list = new List<Sjqd_StatUsersByArea>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, param))
                {
                    while (read.Read())
                    {
                        list.Add(new Sjqd_StatUsersByArea()
                        {
                            UseCount = Convert.ToInt32(read["usercount"]),
                            AreaName = read["AreaName"].ToString(),

                        });
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }
        /// <summary>
        /// 世界地区查找
        /// </summary>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaTransverseWithWorld(PeriodOptions period, int statDate, int softId, MobileOption platform)
        {

            string key = BuildCacheKey("GetSoftAreaTransverseWithWorld", period, statDate, softId, platform);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
            if (list == null)
            {

                string cmdText = @" SELECT B.E_Country AreaName,sum(userscount) usercount FROM
                                    (
	                                    select AreaID , NewUserCount+ActiveUserCount  userscount 
	                                    from  Sjqd_StatUsersByArea with(nolock)
	                                    where Period=@period and StatDate=@StatDate and  SoftID=@SoftID AND [Platform]=@Platform and ChannelID=0
                                    )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                    ON A.AreaID=B.ID
                                    group by B.E_Country order by sum(userscount) desc";

                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
                };

                list = new List<Sjqd_StatUsersByArea>();
                int allcount = 0;
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, cmdText, param))
                {
                    while (read.Read())
                    {
                        allcount += Convert.ToInt32(read["usercount"]);
                        list.Add(new Sjqd_StatUsersByArea()
                        {
                            UseCount = Convert.ToInt32(read["usercount"]),
                            AreaName = read["AreaName"].ToString(),

                        });
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }

        /// <summary>
        /// 根据渠道商集合查询渠道商用户量（世界的）
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="channelids"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaTransverseWithWorldByChannels(int softId, MobileOption platform, PeriodOptions period, int statDate, ChannelTypeOptions selectchanneltype, string channelids)
        {
            string sql = string.Empty;
            string key = BuildCacheKey("GetSoftAreaTransverseWithWorldByChannels", period, statDate, softId, platform, selectchanneltype, channelids);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
            
            if (list == null)
            {
                int[] channels = channelids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
                List<int> channelIds = new URLoginService().GetAvailableChannelIds(softId, selectchanneltype, channels);

                if (channelIds.Count == 0)
                    return new List<Sjqd_StatUsersByArea>();

                string channelIdsString = string.Join(",", channelIds.Select(a => a.ToString()).ToArray());

                sql = string.Format(@" SELECT  B.E_Country  AreaName,sum(userscount) usercount FROM
                                        (
	                                        select AreaID , NewUserCount+ActiveUserCount  userscount 
	                                        from  Sjqd_StatUsersByArea with(nolock)
	                                        where Period=@period and StatDate=@StatDate and  SoftID=@SoftID AND [Platform]=@Platform and ChannelID in ({0})
                                        )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                        ON A.AreaID=B.ID 
                                        group by  B.E_Country  order by sum(userscount) desc", channelIdsString);
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)period},
                    new SqlParameter(){ ParameterName = "@StatDate", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statDate},
                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
                };
                list = new List<Sjqd_StatUsersByArea>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, param))
                {
                    while (read.Read())
                    {
                        list.Add(new Sjqd_StatUsersByArea()
                        {
                            UseCount = Convert.ToInt32(read["usercount"]),
                            AreaName = read["AreaName"].ToString(),

                        });
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }
        /// <summary>
        /// 每一天的量(世界范围)
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="channelids"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaDaylyWithWorldByChannels(string areaname,int softId, MobileOption platform, int begindate,int enddate, ChannelTypeOptions selectchanneltype, string channelids)
        {
            string sql = string.Empty;
            string key = BuildCacheKey("GetSoftAreaDaylyWithWorldByChannels", areaname,begindate,enddate, softId, platform, selectchanneltype, channelids);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
            
            if (list == null)
            {
                if (channelids != "")
                {
                    int[] channels = channelids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
                    List<int> channelIds = new URLoginService().GetAvailableChannelIds(softId, selectchanneltype, channels);

                    if (channelIds.Count == 0)
                        return new List<Sjqd_StatUsersByArea>();

                    string channelIdsString = string.Join(",", channelIds.Select(a => a.ToString()).ToArray());
                    sql = string.Format(@" SELECT  B.E_Country  AreaName,A.StatDate,sum(userscount) usercount,sum(newcount) newcount FROM
                                                (
	                                                select AreaID , NewUserCount+ActiveUserCount  userscount,NewUserCount newcount,StatDate
	                                                from  Sjqd_StatUsersByArea with(nolock)
	                                                where Period=@period and StatDate between @begintime and @endtime and  SoftID=@SoftID AND [Platform]=@Platform and ChannelID in ({0})
                                                )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                                ON A.AreaID=B.ID and B.E_Country='{1}'
                                                group by  B.E_Country,A.StatDate  order by A.StatDate asc", channelIdsString, areaname);
                }
                else
                {
                        sql =string.Format(@" SELECT B.E_Country AreaName,sum(userscount) usercount,sum(newcount) newcount,StatDate FROM
                                            (
	                                            select AreaID , NewUserCount+ActiveUserCount  userscount,NewUserCount newcount,StatDate
	                                            from  Sjqd_StatUsersByArea with(nolock)
	                                            where Period=@period and StatDate between @begintime and @endtime and SoftID=@SoftID AND [Platform]=@Platform and ChannelID=0
                                            )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                            ON A.AreaID=B.ID and B.E_Country='{0}'
                                            group by B.E_Country,A.StatDate order by A.StatDate asc", areaname);
                }
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = 1},
                    new SqlParameter(){ ParameterName = "@begintime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = begindate},
                    new SqlParameter(){ ParameterName = "@endtime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = enddate},
                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
                };
                list = new List<Sjqd_StatUsersByArea>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, param))
                {
                    while (read.Read())
                    {
                        Sjqd_StatUsersByArea area = new Sjqd_StatUsersByArea();
                        if (read["newcount"] != null && read["newcount"] != DBNull.Value)
                        {
                            area.NewUserCount = Convert.ToInt32(read["newcount"]);
                        }
                        if (read["usercount"] != null && read["usercount"] != DBNull.Value)
                        {
                            area.UseCount = Convert.ToInt32(read["usercount"]);
                        }
                        if (read["AreaName"] != null && read["AreaName"] != DBNull.Value)
                        {
                            area.AreaName = read["AreaName"].ToString();
                        }
                        if (read["StatDate"] != null && read["StatDate"] != DBNull.Value)
                        {
                            int date = Convert.ToInt32(read["StatDate"]);
                            area.StatDate = new DateTime(date / 10000, date % 10000 / 100, date % 100);
                        }
                        list.Add(area);
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }

        /// <summary>
        /// 每一天的量(中国范围)
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="channelids"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaDaylyWithChinaByChannels(string areaname, int softId, MobileOption platform, int begindate, int enddate, ChannelTypeOptions selectchanneltype, string channelids)
        {
            string sql = string.Empty;
            string key = BuildCacheKey("GetSoftAreaDaylyWithChinaByChannels",areaname, begindate, enddate, softId, platform, selectchanneltype, channelids);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
            if (list == null)
            {
                if (channelids != "")
                {

                    int[] channels = channelids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
                    List<int> channelIds = new URLoginService().GetAvailableChannelIds(softId, selectchanneltype, channels);

                    if (channelIds.Count == 0)
                        return new List<Sjqd_StatUsersByArea>();

                    string channelIdsString = string.Join(",", channelIds.Select(a => a.ToString()).ToArray());

                    sql = string.Format(@" SELECT  B.Province  AreaName,A.StatDate,sum(userscount) usercount FROM
                                                (
	                                                select AreaID , NewUserCount+ActiveUserCount  userscount,StatDate 
	                                                from  Sjqd_StatUsersByArea with(nolock)
	                                                where Period=@period and StatDate between @begintime and @endtime and  SoftID=@SoftID AND [Platform]=@Platform and ChannelID in ({0})
                                                )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                                ON A.AreaID=B.ID and  B.Province='{1}' and B.E_Country='中国'
                                                group by  B.Province,A.StatDate  order by A.StatDate asc", channelIdsString, areaname);
                }
                else
                {
                    sql = string.Format(@" SELECT B.Province AreaName,sum(userscount) usercount,StatDate FROM
                                            (
	                                            select AreaID , NewUserCount+ActiveUserCount  userscount,StatDate 
	                                            from  Sjqd_StatUsersByArea with(nolock)
	                                            where Period=@period and StatDate between @begintime and @endtime and SoftID=@SoftID AND [Platform]=@Platform and ChannelID=0
                                            )AS A inner join  Sjqd_Areas as B WITH(NOLOCK)
                                            ON A.AreaID=B.ID and  B.Province='{0}' and B.E_Country='中国'
                                            group by B.Province,A.StatDate order by A.StatDate asc", areaname);
                }
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = 1},
                    new SqlParameter(){ ParameterName = "@begintime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = begindate},
                    new SqlParameter(){ ParameterName = "@endtime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = enddate},
                    new SqlParameter(){ ParameterName = "@SoftID", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softId},
                    new SqlParameter(){ ParameterName = "@Platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)platform}
                };
                list = new List<Sjqd_StatUsersByArea>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, param))
                {
                    while (read.Read())
                    {
                        Sjqd_StatUsersByArea area = new Sjqd_StatUsersByArea();
                        if (read["usercount"] != null && read["usercount"] != DBNull.Value)
                        {
                            area.UseCount = Convert.ToInt32(read["usercount"]);
                        }
                        if (read["AreaName"] != null && read["AreaName"] != DBNull.Value)
                        {
                            area.AreaName = read["AreaName"].ToString();
                        }
                        if (read["StatDate"] != null && read["StatDate"] != DBNull.Value)
                        {
                            int date = Convert.ToInt32(read["StatDate"]);
                            area.StatDate = new DateTime(date/10000,date%10000/100,date%100);
                        }
                        list.Add(area);
                         
                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }


       
        /// <summary>
        /// 按城市获取渠道用户分布
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="mobileOption"></param>
        /// <param name="periodOptions"></param>
        /// <param name="statdate"></param>
        /// <param name="channelTypeOptions"></param>
        /// <param name="channelIds"></param>
        /// <param name="areaName"></param> 
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaTransverseWithChinaByCitys(int softid, MobileOption mobileOption,
            PeriodOptions periodOptions, DateTime statdate, 
            ChannelTypeOptions channelTypeOptions,
            List<int> channelIds, string provinceName)
        {
            string channelids = string.Join(",", channelIds.Select(p => p.ToString()).ToArray());
            string sql = string.Empty;
            string key = BuildCacheKey("GetSoftAreaTransverseWithChinaByCitys", softid, mobileOption,
                periodOptions, statdate, channelTypeOptions, channelids, provinceName);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
            if (list == null)
            {
                int[] channels = channelIds.Select(a => a).ToArray();
                List<int> channelavailableIds = channels.Count() == 0 ? new List<int>() : new URLoginService().GetAvailableChannelIds(softid, channelTypeOptions, channels);
                if (channelIds.Count!=0&&channelavailableIds.Count == 0)
                    return new List<Sjqd_StatUsersByArea>();

                string channelIdsString = string.Join(",", channelavailableIds.Select(a => a.ToString()).ToArray());
                //不区分渠道的 
                if (channelIds.Count == 0)
                {
                    sql = @"select B.City AreaName,SUM(A.NewUserCount+A.ActiveUserCount) usercount from dbo.Sjqd_StatUsersByArea A
                            inner join dbo.Sjqd_Areas B
                            on A.AreaID=B.ID and B.E_Country='中国' 
                            and B.Province=@province and Period=@period and SoftID=@softid and Platform=@platform and ChannelID=0
                            and StatDate between @begintime and @endtime
                            group by B.City";
                }
                else //区分渠道
                {
                    sql =
                        string.Format(@"select B.City AreaName,SUM(A.NewUserCount+A.ActiveUserCount) usercount from dbo.Sjqd_StatUsersByArea A
                            inner join dbo.Sjqd_Areas B
                            on A.AreaID=B.ID and B.E_Country='中国' 
                            and B.Province=@province and Period=@period and SoftID=@softid and Platform=@platform
                            and ChannelID in ({0})
                            and StatDate between @begintime and @endtime
                            group by B.City", channelIdsString);
                } 
                SqlParameter[] param = new SqlParameter[] {
 
                    new SqlParameter(){ ParameterName = "@begintime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statdate.ToString("yyyyMMdd")},
                    new SqlParameter(){ ParameterName = "@endtime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = statdate.ToString("yyyyMMdd")},
                    new SqlParameter(){ ParameterName = "@softid", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softid},
                    new SqlParameter(){ ParameterName = "@platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)mobileOption},
                    new SqlParameter(){ ParameterName = "@province", SqlDbType = System.Data.SqlDbType.VarChar, Size = 100, Value =provinceName.Trim() },
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value =(int)periodOptions}
                };
                list = new List<Sjqd_StatUsersByArea>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, param))
                {
                    while (read.Read())
                    {
                        Sjqd_StatUsersByArea area = new Sjqd_StatUsersByArea();
                        if (read["usercount"] != null && read["usercount"] != DBNull.Value)
                        {
                            area.UseCount = Convert.ToInt32(read["usercount"]);
                        }
                        if (read["AreaName"] != null && read["AreaName"] != DBNull.Value)
                        {
                            area.AreaName = read["AreaName"].ToString();
                        } 
                        list.Add(area);

                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }
        /// <summary>
        /// 渠道分城市每天
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="mobileOption"></param>
        /// <param name="periodOptions"></param>
        /// <param name="beginstatdate"></param>
        /// <param name="endstatdate"></param>
        /// <param name="channelTypeOptions"></param>
        /// <param name="channelIds"></param>
        /// <param name="provinceName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsersByArea> GetSoftAreaTransverseWithChinaByCitysDayly(int softid, MobileOption mobileOption,
           PeriodOptions periodOptions, DateTime beginstatdate, DateTime endstatdate,
           ChannelTypeOptions channelTypeOptions,
           List<int> channelIds, string city, string province)
        {
            string channelids = string.Join(",", channelIds.Select(p => p.ToString()).ToArray());
            string sql = string.Empty;
            string key = BuildCacheKey("GetSoftAreaTransverseWithChinaByCitysDayly", softid, mobileOption,
                periodOptions, beginstatdate,
                endstatdate, channelTypeOptions, channelids, city, province);
            List<Sjqd_StatUsersByArea> list = CacheHelper.Get<List<Sjqd_StatUsersByArea>>(key);
            if (list == null)
            {
                int[] channels = channelIds.Select(a => a).ToArray();
                List<int> channelavailableIds  =channels.Count()==0?  new List<int>(): new URLoginService().GetAvailableChannelIds(softid, channelTypeOptions, channels);
                if (channelIds.Count != 0 && channelavailableIds.Count == 0)
                    return new List<Sjqd_StatUsersByArea>();

                string channelIdsString = string.Join(",", channelavailableIds.Select(a => a.ToString()).ToArray());
                //不区分渠道的 
                if (channelIds.Count == 0)
                {
                    sql = @"select A.StatDate,B.City AreaName,SUM(A.NewUserCount+A.ActiveUserCount) usercount from dbo.Sjqd_StatUsersByArea A
                            inner join dbo.Sjqd_Areas B
                            on A.AreaID=B.ID and B.E_Country='中国' 
                            and B.City=@city and B.Province=@province and Period=@period and SoftID=@softid and Platform=@platform and ChannelID=0
                            and StatDate between @begintime and @endtime
                            group by B.City,A.StatDate";
                }
                else //区分渠道
                {
                    sql =
                        string.Format(@"select A.StatDate, B.City AreaName,SUM(A.NewUserCount+A.ActiveUserCount) usercount from dbo.Sjqd_StatUsersByArea A
                            inner join dbo.Sjqd_Areas B
                            on A.AreaID=B.ID and B.E_Country='中国' 
                            and B.City=@city and B.Province=@province and Period=@period and SoftID=@softid and Platform=@platform
                            and ChannelID in ({0})
                            and StatDate between @begintime and @endtime
                            group by B.City,A.StatDate", channelIdsString);
                }
                SqlParameter[] param = new SqlParameter[] {
                    
                    new SqlParameter(){ ParameterName = "@begintime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = beginstatdate.ToString("yyyyMMdd")},
                    new SqlParameter(){ ParameterName = "@endtime", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = endstatdate.ToString("yyyyMMdd")},
                    new SqlParameter(){ ParameterName = "@softid", SqlDbType = System.Data.SqlDbType.Int, Size = 4, Value = softid},
                    new SqlParameter(){ ParameterName = "@platform", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value = (int)mobileOption},
                    new SqlParameter(){ ParameterName = "@city", SqlDbType = System.Data.SqlDbType.VarChar, Size = 100, Value =city.Trim() },
                    new SqlParameter(){ ParameterName = "@province", SqlDbType = System.Data.SqlDbType.VarChar, Size = 100, Value =province.Trim() },
                    new SqlParameter(){ ParameterName = "@period", SqlDbType = System.Data.SqlDbType.TinyInt, Size = 1, Value =(int)periodOptions}
                };
                list = new List<Sjqd_StatUsersByArea>();
                using (IDataReader read = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, param))
                {
                    while (read.Read())
                    {
                        Sjqd_StatUsersByArea area = new Sjqd_StatUsersByArea();
                        if (read["usercount"] != null && read["usercount"] != DBNull.Value)
                        {
                            area.UseCount = Convert.ToInt32(read["usercount"]);
                        }
                        if (read["AreaName"] != null && read["AreaName"] != DBNull.Value)
                        {
                            area.AreaName = read["AreaName"].ToString();
                        }
                        if (read["StatDate"] != null && read["StatDate"] != DBNull.Value)
                        {
                            int date = Convert.ToInt32(read["StatDate"]);
                            area.StatDate = new DateTime(date / 10000, date % 10000 / 100, date % 100);
                        }
                        list.Add(area);

                    }
                }
                if (list.Count > 0)
                    CacheHelper.Set<List<Sjqd_StatUsersByArea>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }

        
    }
}
