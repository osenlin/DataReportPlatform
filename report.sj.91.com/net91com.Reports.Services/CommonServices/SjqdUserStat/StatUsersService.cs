using System;
using System.Collections.Generic;
using System.Linq;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.UserRights;
using net91com.Stat.Core;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    /// <summary>
    ///     用户统计相关的数据接口类
    /// </summary>
    public class StatUsersService : BaseService
    {
        //数据访问类实例
        private readonly StatUsers_DataAccess suDA = new StatUsers_DataAccess();

        #region 获取渠道排行数据(GetRankOfChannels)

        /// <summary>
        ///     获取渠道排行
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfChannels(int softId, int platform, int channelId, int period,
                                                      ref DateTime beginDate, ref DateTime endDate)
        {
            GetDateOfRank(softId, period, ref beginDate, ref endDate, "GetRankOfChannels");

            DateTime statDate = endDate;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetRankOfChannels", softId, platform, channelId, period, statDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetRankOfChannels(softId, platform, channelId, period, statDate));
        }

        #endregion

        #region 获取版本排行数据(GetRankOfVersions)

        /// <summary>
        ///     获取版本排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfVersions(int softId, int platform, int channelId,
                                                      ChannelTypeOptions channelType, int period, ref DateTime beginDate,
                                                      ref DateTime endDate)
        {
            GetDateOfRank(softId, period, ref beginDate, ref endDate, "GetRankOfVersions");

            DateTime statDate = endDate;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetRankOfVersions", softId, platform, channelId, channelType, period, statDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetRankOfVersions(softId, platform, channelId, channelType, period, statDate));
        }

        #endregion

        #region 获取地区排行数据(GetRankOfAreas)

        /// <summary>
        ///     获取国家平均排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetSumRankOfCountries(int softId, int platform, ChannelTypeOptions channelType,
                                                          int channelId, int period, DateTime startDate,
                                                          DateTime endDate)
        {
            return suDA.GetRankOfCountries(softId, platform, channelType, channelId, period, startDate, endDate);
        }

        /// <summary>
        ///     获取国家排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfCountries(int softId, int platform, ChannelTypeOptions channelType,
                                                       int channelId, int period, ref DateTime startDate,
                                                       ref DateTime endDate)
        {
            GetDateOfRank(softId, period, ref startDate, ref endDate, "GetRankOfCountries");
            DateTime startDate2 = startDate, endDate2 = endDate;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetRankOfCountries", softId, platform, channelType, channelId, period, startDate2,
                              endDate2)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetRankOfCountries(softId, platform, channelType, channelId, period, startDate2, endDate2));
        }

        /// <summary>
        ///     获取省排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfProvinces(int softId, int platform, ChannelTypeOptions channelType,
                                                       int channelId, int period, ref DateTime startDate,
                                                       ref DateTime endDate)
        {
            GetDateOfRank(softId, period, ref startDate, ref endDate, "GetRankOfProvinces");
            DateTime startDate2 = startDate, endDate2 = endDate;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetRankOfProvinces", softId, platform, channelType, channelId, period, startDate2,
                              endDate2)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetRankOfProvinces(softId, platform, channelType, channelId, period, startDate2, endDate2));
        }

        /// <summary>
        ///     获取市排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfCities(int softId, int platform, ChannelTypeOptions channelType,
                                                    int channelId, int period, ref DateTime startDate,
                                                    ref DateTime endDate)
        {
            GetDateOfRank(softId, period, ref startDate, ref endDate, "GetRankOfCities");
            DateTime startDate2 = startDate, endDate2 = endDate;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetRankOfCities", softId, platform, channelType, channelId, period, startDate2, endDate2)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetRankOfCities(softId, platform, channelType, channelId, period, startDate2, endDate2));
        }

        #endregion

        #region 获取分渠道用户数据(GetStatUsersByChannel)

        /// <summary>
        ///     获取分渠道用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="mustLogin"></param>
        /// <param name="forPartner">是否提供给合作方</param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByChannel(int softId, int platform, ChannelTypeOptions channelType,
                                                          int channelId, int period, DateTime beginDate,
                                                          DateTime endDate, bool mustLogin = true,
                                                          bool forPartner = false)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId}, mustLogin)
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetStatUsersByChannel", softId, platform, channelType, channelId, period, beginDate,
                              endDate, mustLogin, forPartner)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatUsersByChannel(softId, platform, channelIds, period, beginDate, endDate, forPartner));
        }

        #endregion

        #region 获取分小时用户数据(GetStatUsersByHour)

        /// <summary>
        ///     获取分小时用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByHour(int softId, int platform, ChannelTypeOptions channelType,
                                                       int channelId, int period, DateTime beginDate, DateTime endDate)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetStatUsersByHour", softId, platform, channelType, channelId, period, beginDate, endDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatUsersByHour(softId, platform, channelIds, period, beginDate, endDate));
        }

        #endregion

        #region 获取分地区用户数据(GetStatUsersByArea)

        /// <summary>
        ///     获取分地区用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="areaId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="mustLogin"></param>
        /// <param name="forPartner"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByArea(int softId, int platform, ChannelTypeOptions channelType,
                                                       int channelId, string areaId, int period, DateTime beginDate,
                                                       DateTime endDate, bool mustLogin = true, bool forPartner = false)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId}, mustLogin)
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetStatUsersByArea", softId, platform, channelType, channelId, areaId, period, beginDate,
                              endDate, mustLogin, forPartner)
                , CacheTimeOption.TenMinutes
                ,
                () =>
                suDA.GetStatUsersByArea(softId, platform, channelIds, areaId, period, beginDate, endDate, forPartner));
        }

        #endregion

        #region 获取分版本用户数据(GetStatUsersByVersion)

        /// <summary>
        ///     获取分版本用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="versionId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByVersion(int softId, int platform, ChannelTypeOptions channelType,
                                                          int channelId, string versionId, int period, DateTime beginDate,
                                                          DateTime endDate)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatUsers>>(
                BuildCacheKey("GetStatUsersByVersion", softId, platform, channelType, channelId, versionId, period,
                              beginDate, endDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatUsersByVersion(softId, platform, channelIds, versionId, period, beginDate, endDate));
        }

        #endregion

        #region 获取分国家分版本(海外)用户数据(GetStatUsersByCountryByVersionEn)

        /// <summary>
        ///     获取分国家分版本(海外)用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="versionId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public Dictionary<string, List<Sjqd_StatUsers>> GetStatUsersByCountryByVersionEn(int softId, int platform,
                                                                                         List<string> versionIds,
                                                                                         List<string> countryIds,
                                                                                         DateTime beginDate,
                                                                                         DateTime endDate)
        {
            string versionIdsString = string.Join(",", versionIds.Select(a => "'" + a + "'").ToArray());
            string countryIdsString = string.Join(",", countryIds.Select(a => "'" + a + "'").ToArray());
            
            return CacheHelper.Get<Dictionary<string, List<Sjqd_StatUsers>>>(
                BuildCacheKey("GetStatUsersByCountryByVersionEn", softId, platform, versionIdsString, countryIdsString, beginDate, endDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatUsersByCountryByVersionEn(softId, platform, versionIdsString, countryIdsString, beginDate, endDate));
        }

        #endregion

        #region 获取新增用户留存率数据(GetStatRetainedUsers)

        /// <summary>
        ///     获取新增用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedUsers(int softId, int platform, int channelId,
                                                                 ChannelTypeOptions channelType, int period,
                                                                 DateTime fromDate, DateTime toDate)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>(
                BuildCacheKey("GetStatRetainedUsers", softId, platform, channelType, channelId, period, fromDate, toDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatRetainedUsers(softId, platform, channelIds, period, fromDate, toDate));
        }

        #endregion

        #region 获取活跃用户留存率数据(GetStatRetainedActiveUsers)

        /// <summary>
        ///     获取活跃用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedActiveUsers(int softId, int platform, int channelId,
                                                                       ChannelTypeOptions channelType, DateTime fromDate,
                                                                       DateTime toDate)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>(
                BuildCacheKey("GetStatRetainedActiveUsers", softId, platform, channelType, channelId, fromDate, toDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatRetainedActiveUsers(softId, platform, channelIds, fromDate, toDate));
        }

        #endregion

        #region 获取分地区新增用户留存率数据(GetStatRetainedUsersByArea)

        /// <summary>
        ///     获取分地区新增用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="areaId"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedUsersByArea(int softId, int platform, string areaId,
                                                                       int channelId, ChannelTypeOptions channelType,
                                                                       int period, DateTime fromDate, DateTime toDate)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>(
                BuildCacheKey("GetStatRetainedUsersByArea", softId, platform, areaId, channelType, channelId, period,
                              fromDate, toDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatRetainedUsersByArea(softId, platform, areaId, channelIds, period, fromDate, toDate));
        }

        #endregion

        #region 获取分地区活跃用户留存率数据(GetStatRetainedActiveUsersByArea)

        /// <summary>
        ///     获取分地区活跃用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="areaId"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedActiveUsersByArea(int softId, int platform, string areaId,
                                                                             int channelId,
                                                                             ChannelTypeOptions channelType,
                                                                             DateTime fromDate, DateTime toDate)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>(
                BuildCacheKey("GetStatRetainedActiveUsersByArea", softId, platform, areaId, channelType, channelId,
                              fromDate, toDate)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetStatRetainedActiveUsersByArea(softId, platform, areaId, channelIds, fromDate, toDate));
        }

        #endregion

        #region 获取分版本新增用户留存率数据(GetStatRetainedUsersByVersion)

        /// <summary>
        ///     获取分版本新增用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="versionId"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedUsersByVersion(int softId, int platform, string versionId,
                                                                          int channelId, ChannelTypeOptions channelType,
                                                                          int period, DateTime fromDate, DateTime toDate)
        {
            List<int> channelIds = channelId > 0
                                       ? new URLoginService().GetAvailableChannelIds(softId, channelType,
                                                                                     new[] {channelId})
                                       : null;
            return CacheHelper.Get<List<Sjqd_StatRetainedUsers>>(
                BuildCacheKey("GetStatRetainedUsersByVersion", softId, platform, versionId, channelType, channelId,
                              period, fromDate, toDate)
                , CacheTimeOption.TenMinutes
                ,
                () =>
                suDA.GetStatRetainedUsersByVersion(softId, platform, versionId, channelIds, period, fromDate, toDate));
        }

        #endregion

        #region 获取排行最适配日期(GetDateOfRank)

        /// <summary>
        ///     获取排行最适配日期
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="method"></param>
        private void GetDateOfRank(int softId, int period, ref DateTime beginDate, ref DateTime endDate, string method)
        {
            DateTime statDate = endDate;
            var dateString = CacheHelper.Get<string>(
                BuildCacheKey("GetDateOfRank", softId, period, statDate, method)
                , CacheTimeOption.TenMinutes
                , () => suDA.GetDateOfRank(softId, period, statDate, method).ToString("yyyy-MM-dd"));
            endDate = Convert.ToDateTime(dateString);

            switch ((PeriodOptions) period)
            {
                case PeriodOptions.Daily:
                    beginDate = endDate;
                    break;
                case PeriodOptions.Weekly:
                case PeriodOptions.LatestOneWeek:
                    beginDate = endDate.AddDays(-6);
                    break;
                case PeriodOptions.Of2Weeks:
                case PeriodOptions.LatestTwoWeeks:
                    beginDate = endDate.AddDays(-13);
                    break;
                case PeriodOptions.Monthly:
                    beginDate = endDate.AddMonths(-1).AddDays(1);
                    break;
                case PeriodOptions.NaturalMonth:
                    beginDate = endDate.AddDays(1).AddMonths(-1);
                    break;
                case PeriodOptions.LatestOneMonth:
                    beginDate = endDate.AddDays(-30);
                    break;
                case PeriodOptions.LatestThreeMonths:
                    beginDate = endDate.AddDays(-90);
                    break;
            }
        }

        #endregion

        /// <summary>
        ///     构造函数
        /// </summary>
        public StatUsersService()
        {
            _cachePreviousKey = "net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService";
        }
    }
}