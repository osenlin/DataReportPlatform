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
using MySql.Data.MySqlClient;

namespace net91com.Stat.Services.sjqd
{
    public class RetainedUsersService
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

        public RetainedUsersService(bool useCache)
        {
            this.useCache = useCache;
            _cachePreviousKey = "RetainedUsersService";
            SqlHelper.CommandTimeout = 120;
        }

        ///绑定StatRetaindeUsers 对象
        public Sjqd_StatChannelRetainedUsers RetainedUserBind(IDataReader dataReader)
        {
            Sjqd_StatChannelRetainedUsers modelretained = new Sjqd_StatChannelRetainedUsers();
            object obj;
            obj = dataReader["ChannelID"];
            if (obj != null && obj != DBNull.Value)
            {
                modelretained.ChannelID = Convert.ToInt32(obj);
            }
            obj = dataReader["SoftID"];
            if (obj != null && obj != DBNull.Value)
            {
                modelretained.SoftID = Convert.ToInt32(obj);
            }
            obj = dataReader["Period"];
            if (obj != null && obj != DBNull.Value)
            {
                modelretained.Period = (net91com.Stat.Core.PeriodOptions) Convert.ToInt32(obj);
            }
            obj = dataReader["Platform"];
            if (obj != null && obj != DBNull.Value)
            {
                modelretained.Platform = (MobileOption) Convert.ToInt32(obj);
            }
            obj = dataReader["OriginalNewUserCount"];
            if (obj != null && obj != DBNull.Value)
            {
                modelretained.OriginalNewUserCount = Convert.ToInt32(obj);
            }
            obj = dataReader["RetainedUserCount"];
            if (obj != null && obj != DBNull.Value)
            {
                modelretained.RetainedUserCount = Convert.ToInt32(obj);
            }
            obj = dataReader["OriginalDate"];
            if (obj != null && obj != DBNull.Value)
            {
                int temp = Convert.ToInt32(obj);
                modelretained.OriginalDate = new DateTime(temp/10000, temp/100%100, temp%100, 0, 0, 0);
            }

            obj = dataReader["StatDate"];
            if (obj != null && obj != DBNull.Value)
            {
                int temp = Convert.ToInt32(obj);
                modelretained.StatDate = new DateTime(temp/10000, temp/100%100, temp%100, 0, 0, 0);
            }

            return modelretained;
        }

        /// <summary>
        /// 共用的用户留存量
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatChannelRetainedUsers> GetStatRetainedUsers(int softId, int platform, int channelId,
                                                                        net91com.Stat.Core.PeriodOptions period,
                                                                        DateTime fromDate, DateTime toDate,
                                                                        ChannelTypeOptions channelType,
                                                                        URLoginService loginService)
        {
            string cmdText;
            if (channelId > 0 || loginService.LoginUser.AccountType == UserTypeOptions.Channel ||
                loginService.LoginUser.AccountType == UserTypeOptions.ChannelPartner)
            {
                List<int> rangeChannelIds = null;
                if (loginService == null)
                {
                    rangeChannelIds = new URChannelsService().GetChannelIds(softId, channelType, new int[] {channelId});
                }
                else
                {
                    rangeChannelIds = channelId > 0
                                          ? loginService.GetAvailableChannelIds(softId, channelType,
                                                                                new int[] {channelId})
                                          : loginService.GetAvailableChannelIds(softId);
                }


                if (rangeChannelIds.Count == 0)
                    return new List<Sjqd_StatChannelRetainedUsers>();

                string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());

                //因为有些质量低的渠道不存在留存用户，造成之前的方法在聚合时原始新增量会少
                cmdText = string.Format(
                    @"select 0 ChannelID,B.StatDate,B.OriginalDate,{0} Period,{1} SoftID,{2} Platform,A.NewUserCount OriginalNewUserCount,B.RetainedUserCount
                            from (select StatDate,SUM(NewUserCount) NewUserCount from U_StatChannelUsers where SoftID={1} and Platform={2} and Period={0} and StatDate between {3} and {4} and ChannelID IN ({5}) group by StatDate) A inner join 
                                 (select StatDate,OriginalDate,SUM(RetainedUserCount) RetainedUserCount from U_StatRetainedUsers where SoftID={1} and Platform={2} and Period={0} and OriginalDate between {3} and {4} and ChannelID IN ({5}) GROUP BY StatDate,OriginalDate) B 
                            on A.StatDate=B.OriginalDate 
                            order by B.OriginalDate desc,B.StatDate desc", (int)period, softId, platform, fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"), channelIdsString);
            }
            else
            {
                if (platform != 0)
                {
                    cmdText = string.Format(
                        "select *,0 ChannelID from U_StatRetainedUsers where SoftID={1} and Platform={2} and Period={0} and OriginalDate between {3} and {4} and ChannelID=-1 order by OriginalDate desc ,StatDate desc"
                            , (int)period, softId, platform, fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"));
                }
                else //不区分平台
                {
                    cmdText = string.Format(
                        @"select * ,0 ChannelID, 0 Platform from (select StatDate,OriginalDate,Period,SoftID,sum(OriginalNewUserCount) OriginalNewUserCount,sum(RetainedUserCount) RetainedUserCount
                            from U_StatRetainedUsers
                            where SoftID={1} and Period={0} and OriginalDate between {2} and {3} and ChannelID=-1
                            group by StatDate,OriginalDate,Period,SoftID) as temp  order by OriginalDate desc ,StatDate desc"
                                ,(int)period, softId, fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"));
                }
            }

            if (period == net91com.Stat.Core.PeriodOptions.Daily)
            {
                if ((toDate - fromDate).Days > 31)
                {
                    fromDate = toDate.AddDays(-31);
                }
            }
            List<Sjqd_StatChannelRetainedUsers> retainedUsers = new List<Sjqd_StatChannelRetainedUsers>();
            using (IDataReader reader = MySqlHelper.ExecuteReader(statdbConn, cmdText))
            {
                while (reader.Read())
                {
                    retainedUsers.Add(RetainedUserBind(reader));
                }
            }
            return retainedUsers;
        }

        /// <summary>
        /// 共用的用户留存量
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="cachetime"></param>
        /// <returns></returns>
        public List<Sjqd_StatChannelRetainedUsers> GetStatRetainedUsersCache(int softId, int platform, int channelId,
                                                                             net91com.Stat.Core.PeriodOptions period,
                                                                             DateTime fromDate, DateTime toDate,
                                                                             CacheTimeOption cachetime,
                                                                             ChannelTypeOptions channelType,
                                                                             URLoginService loginService)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetStatRetainedUsersCache", softId, platform, channelId, period,
                                                fromDate, toDate, channelType,
                                                loginService == null
                                                    ? ""
                                                    : ((loginService.LoginUser.AccountType ==
                                                        Reports.UserRights.UserTypeOptions.Channel ||
                                                        loginService.LoginUser.AccountType ==
                                                        Reports.UserRights.UserTypeOptions.ChannelPartner)
                                                           ? loginService.LoginUser.ID.ToString()
                                                           : ""));
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<Sjqd_StatChannelRetainedUsers>>(cacheKey).ToList();
                }
                List<Sjqd_StatChannelRetainedUsers> list = GetStatRetainedUsers(softId, platform, channelId, period,
                                                                                fromDate, toDate, channelType,
                                                                                loginService);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<Sjqd_StatChannelRetainedUsers>>(cacheKey, list, cachetime,
                                                                         CacheExpirationOption.AbsoluteExpiration);
                }
                return list.ToList();
            }
            else
            {
                return GetStatRetainedUsers(softId, platform, channelId, period, fromDate, toDate, channelType,
                                            loginService);
            }
        }
    }
}