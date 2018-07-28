using System;
using System.Collections.Generic;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.UserRights;
using net91com.Stat.Core;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    public class Sjqd_StatRetainedUsersService : BaseService
    {
        private static Sjqd_StatRetainedUsersService instance;
        private static readonly object obj = new object();

        public static Sjqd_StatRetainedUsersService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_StatRetainedUsersService();
                            instance._cachePreviousKey = "Sjqd_StatRetainedUsersService";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        ///     获取对外的留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="minTime"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetChannelRetainedUsersForOutByCache(int softId, MobileOption platform,
                                                                                 ChannelTypeOptions channelType,
                                                                                 int channelId, int period,
                                                                                 DateTime beginDate, DateTime endDate,
                                                                                 DateTime minTime)
        {
            string key = BuildCacheKey("GetChannelRetainedUsersForOutByCache", softId, platform, channelType, channelId,
                                       period, beginDate, endDate, minTime);
            //传入日期需要特殊处理
            //周
            if (period == (int) PeriodOptions.Weekly)
            {
                while (!(beginDate.DayOfWeek == DayOfWeek.Sunday))
                {
                    beginDate = beginDate.AddDays(1);
                }
            }
            //月
            if (period == (int) PeriodOptions.Monthly)
            {
                while (!(beginDate.Day == 20))
                {
                    beginDate = beginDate.AddDays(1);
                }
            }
            if (CacheHelper.Contains(key))
                return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>(key);
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : new List<int>();
            if (channelIds.Count == 0)
                return new List<Sjqd_StatRetainedUsers>();
            return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 Sjqd_StatChannelRetainedUsers_DataAccess.Instance.GetChannelRetainUsersForOut(softId, platform,
                                                                                               channelIds, period,
                                                                                               beginDate, endDate,
                                                                                               minTime));
        }

        /// <summary>
        ///     获取对外的留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="minTime"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetChannelRetainedUsersForChannelCustomerByCache(int softId,
                                                                                             MobileOption platform,
                                                                                             ChannelTypeOptions
                                                                                                 channelType,
                                                                                             int channelId, int period,
                                                                                             DateTime beginDate,
                                                                                             DateTime endDate,
                                                                                             DateTime minTime)
        {
            string key = BuildCacheKey("GetChannelRetainedUsersForChannelCustomerByCache", softId, platform, channelType,
                                       channelId,
                                       period, beginDate, endDate, minTime);
            //传入日期需要特殊处理
            //周
            if (period == (int) PeriodOptions.Weekly)
            {
                while (!(beginDate.DayOfWeek == DayOfWeek.Sunday))
                {
                    beginDate = beginDate.AddDays(1);
                }
            }

            //月
            if (period == (int) PeriodOptions.NaturalMonth)
            {
                while (!(beginDate.AddDays(1).Day == 1))
                {
                    beginDate = beginDate.AddDays(1);
                }
            }
            if (CacheHelper.Contains(key))
                return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>(key);
            List<int> channelIds = channelId > 0
                                       ? new URChannelsService().GetChannelIds(softId, channelType,
                                                                               new[] {channelId})
                                       : new List<int>();
            if (channelIds.Count == 0)
                return new List<Sjqd_StatRetainedUsers>();
            return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 Sjqd_StatChannelRetainedUsers_DataAccess.Instance.GetChannelRetainUsersForOut(softId, platform,
                                                                                               channelIds, period,
                                                                                               beginDate, endDate,
                                                                                               minTime));
        }
    }
}