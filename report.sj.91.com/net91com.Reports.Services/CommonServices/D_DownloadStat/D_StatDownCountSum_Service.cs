using System;
using System.Collections.Generic;
using System.Linq;
using net91com.Reports.DataAccess.D_DownloadStat;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.CommonServices.D_DownloadStat
{
    public class D_StatDownCountSum_Service : BaseService
    {
        private D_StatDownCountSum_Service()
        {
            _cachePreviousKey = "D_StatDownBySoft_Service";
        }

        private static readonly object obj = new object();
        private static D_StatDownCountSum_Service instance;

        public static D_StatDownCountSum_Service Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new D_StatDownCountSum_Service();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 统计下载按产品汇总
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="period"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="channelname"></param>
        /// <param name="selectchannelvalue"></param>
        /// <param name="sourceid"></param>
        /// <param name="e_versionid"></param>
        /// <param name="countryid"></param>
        /// <param name="province"></param>
        /// <param name="statypelist"></param>
        /// <param name="loginService"></param>
        /// <returns></returns>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownBySoft_SUMByCache(int restype,
                                                                               int softid,
                                                                               int platform,
                                                                               DateTime begintime,
                                                                               DateTime endtime,
                                                                               int period,
                                                                               ChannelTypeOptions selectchanneltype,
                                                                               string channelname,
                                                                               int selectchannelvalue,
                                                                               int sourceid,
                                                                               List<string> e_versionid, 
                                                                               string countryid,
                                                                               string province, 
                                                                               List<string> statypelist,
                                                                               URLoginService loginService,
                                                                               int areatype=1)
        {
            string key = BuildCacheKey("GetD_StatDownBySoft_SUMByCache", 
                                       restype, 
                                       softid, 
                                       platform, 
                                       begintime, 
                                       endtime,
                                       period,
                                       selectchanneltype, 
                                       channelname, 
                                       selectchannelvalue, 
                                       sourceid,
                                       string.Join(",", e_versionid.Select(p => p.ToString()).ToArray()), 
                                       countryid,
                                       province,
                                       string.Join(",", statypelist.ToArray()),
                                       loginService == null
                                           ? ""
                                           : ((loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.Channel ||
                                               loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.ChannelPartner)
                                                  ? loginService.LoginUser.ID.ToString()
                                                  : ""), 
                                       areatype
                );
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>(key
                 ,Core.CacheTimeOption.TenMinutes
                 ,() =>new D_StatDownCount_DataAccess().GetD_StatDownBySoft_SUM(restype, 
                                                                          softid, 
                                                                          platform, 
                                                                          begintime,
                                                                          endtime, 
                                                                          selectchanneltype, 
                                                                          channelname,
                                                                          selectchannelvalue, 
                                                                          sourceid,
                                                                          e_versionid, 
                                                                          period, 
                                                                          countryid,
                                                                          province, 
                                                                          statypelist,
                                                                          loginService, 
                                                                          areatype));
        }

        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownByChannel_SUMByCache(int restype,
                                                                       int softid,
                                                                       int platform,
                                                                       DateTime begintime,
                                                                       DateTime endtime,
                                                                       int period,
                                                                       ChannelTypeOptions selectchanneltype,
                                                                       string channelname,
                                                                       int selectchannelvalue,
                                                                       List<string> e_versionid,
                                                                       URLoginService loginService)
        {
            string key = BuildCacheKey("GetD_StatDownByChannel_SUMByCache",
                                       restype,
                                       softid,
                                       platform,
                                       begintime,
                                       endtime,
                                       period,
                                       selectchanneltype,
                                       channelname,
                                       selectchannelvalue,
                                       string.Join(",", e_versionid.Select(p => p.ToString()).ToArray()),
                                       loginService == null
                                           ? ""
                                           : ((loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.Channel ||
                                               loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.ChannelPartner)
                                                  ? loginService.LoginUser.ID.ToString()
                                                  : "")
                );
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>(key
                 , Core.CacheTimeOption.TenMinutes
                 , () => new D_StatDownCount_DataAccess().GetD_StatDownByChannel_SUM(restype,
                                                                          softid,
                                                                          platform,
                                                                          begintime,
                                                                          endtime,
                                                                          selectchanneltype,
                                                                          channelname,
                                                                          selectchannelvalue,
                                                                          e_versionid,
                                                                          period,
                                                                          loginService));
        }

        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownBySoft_SUMByAreaCache(int restype,
                                                                               int softid,
                                                                               int platform,
                                                                               DateTime begintime,
                                                                               DateTime endtime,
                                                                               int period,
                                                                               ChannelTypeOptions selectchanneltype,
                                                                               string channelname,
                                                                               int selectchannelvalue,
                                                                               int sourceid,
                                                                               List<string> e_versionid, 
                                                                               int countryid,
                                                                               int province, 
                                                                               URLoginService loginService)
        {
            string key = BuildCacheKey("GetD_StatDownBySoft_SUMByAreaCache", 
                                       restype, 
                                       softid, 
                                       platform, 
                                       begintime, 
                                       endtime,
                                       period,
                                       selectchanneltype, 
                                       channelname, 
                                       selectchannelvalue, 
                                       sourceid,
                                       string.Join(",", e_versionid.Select(p => p.ToString()).ToArray()), 
                                       countryid,
                                       province,
                                       loginService == null
                                           ? ""
                                           : ((loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.Channel ||
                                               loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.ChannelPartner)
                                                  ? loginService.LoginUser.ID.ToString()
                                                  : "")
                );
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>(key
                 ,Core.CacheTimeOption.TenMinutes
                 , () => new D_StatDownCount_DataAccess().GetD_StatDownBySoft_SUMByArea(restype, 
                                                                          softid, 
                                                                          platform, 
                                                                          begintime,
                                                                          endtime, 
                                                                          selectchanneltype, 
                                                                          channelname,
                                                                          selectchannelvalue, 
                                                                          sourceid,
                                                                          e_versionid, 
                                                                          period, 
                                                                          countryid,
                                                                          province, 
                                                                          loginService));
        }

        

        /// <summary>
        /// 获取平均下载
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="period"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="channelname"></param>
        /// <param name="selectchannelvalue"></param>
        /// <param name="sourceid"></param>
        /// <param name="e_versionid"></param>
        /// <param name="stattype"></param>
        /// <param name="loginService"></param>
        /// <returns></returns>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownAvgCountByCache(int restype, 
                                                                             int softid, 
                                                                             int platform,
                                                                             DateTime begintime, 
                                                                             DateTime endtime,
                                                                             int period,
                                                                             ChannelTypeOptions selectchanneltype,
                                                                             string channelname,
                                                                             int selectchannelvalue, 
                                                                             int sourceid,
                                                                             List<int> e_versionid, 
                                                                             int stattype,
                                                                             int areaid,
                                                                             int modetype,
                                                                             URLoginService loginService,
                                                                             int areatype=1)
        {
            string key = BuildCacheKey("GetD_StatDownAvgCountByCache", 
                                        restype, 
                                        softid, 
                                        platform, 
                                        begintime, 
                                        endtime,
                                        period,
                                        selectchanneltype, 
                                        channelname, 
                                        selectchannelvalue, 
                                        sourceid,
                                        string.Join(",", e_versionid.Select(p => p.ToString()).ToArray()), 
                                        stattype,
                                        areaid,
                                        modetype,
                                        loginService == null
                                           ? ""
                                           : ((loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.Channel ||
                                               loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.ChannelPartner)
                                                  ? loginService.LoginUser.ID.ToString()
                                                  : ""),
                                        areatype
                );

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>
                (key, Core.CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownCount_DataAccess().GetD_StatDownAvgCount(restype, 
                                                                        softid, 
                                                                        platform, 
                                                                        begintime,
                                                                        endtime, 
                                                                        selectchanneltype, 
                                                                        channelname,
                                                                        selectchannelvalue, 
                                                                        sourceid, 
                                                                        e_versionid,
                                                                        period, 
                                                                        stattype,
                                                                        areaid,
                                                                        modetype,
                                                                        loginService, 
                                                                        areatype));
        }
        

        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownAvgCountByChannelTypeCache(int restype, 
                                                                             int softid, 
                                                                             int platform,
                                                                             DateTime begintime, 
                                                                             DateTime endtime,
                                                                             int channeltype)
        {
            string key = BuildCacheKey("GetD_StatDownAvgCountByChannelTypeCache", 
                                        restype, 
                                        softid, 
                                        platform, 
                                        begintime, 
                                        endtime,
                                        channeltype
                );
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>
                (key, Core.CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownCount_DataAccess().GetD_StatDownAvgCountByChannelType(
                                                                        softid, 
                                                                        platform,
                                                                        restype, 
                                                                        begintime,
                                                                        endtime,
                                                                        channeltype));
        }

        /// <summary>
        /// 获取下载统计按分类
        /// </summary>
        /// <returns></returns>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownCountCateSumByCache(int restype, 
                                                                                 int softid, 
                                                                                 int platform,
                                                                                 DateTime begintime, 
                                                                                 DateTime endtime,
                                                                                 int period, 
                                                                                 int pcid, 
                                                                                 int cid,
                                                                                 int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownCountCateSumByCache", 
                                        restype, 
                                        softid, 
                                        platform, 
                                        begintime, 
                                        endtime,
                                        period,
                                        pcid, 
                                        cid, 
                                        downtype);

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>
                (key
                ,Core.CacheTimeOption.TenMinutes
                ,() =>new D_StatDownCount_DataAccess().GetD_StatDownCountCateSum(restype, 
                                                                            softid, 
                                                                            platform, 
                                                                            begintime,
                                                                            endtime, 
                                                                            period, 
                                                                            pcid, cid, 
                                                                            downtype));
        }

        /// 资源下载按ID的统计
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="resid"></param>
        /// <param name="restype"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="lstposition"></param>
        /// <param name="version"></param>
        /// <param name="sourceid"></param>
        /// <param name="areatype"></param>
        /// <returns></returns>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownCountSumByResIDCache(int softid,
                                                                                  int platform,
                                                                                  List<int> resid,
                                                                                  int restype,
                                                                                  DateTime begintime,
                                                                                  DateTime endtime,
                                                                                  List<int> lstposition,
                                                                                  int version,
                                                                                  int sourceid,
                                                                                  int areaid,
                                                                                  int areatype)
        {
            string key = BuildCacheKey("GetD_StatDownCountSumByResIDCache",
                                       softid,
                                       restype,
                                       string.Join(",", resid.Select(p => p.ToString()).ToArray()),
                                       platform,
                                       begintime,
                                       endtime,
                                       string.Join(",", lstposition.Select(p => p.ToString()).ToArray()),
                                       version,
                                       sourceid,
                                       areaid,
                                       areatype);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownCount_DataAccess().GetD_StatDownCountSumByResID(softid,
                                                                               platform,
                                                                               resid,
                                                                               restype,
                                                                               begintime,
                                                                               endtime,
                                                                               lstposition,
                                                                               version,
                                                                               sourceid,
                                                                               areaid,
                                                                               areatype));
        }



        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownByVersionDistribution_SUMByCache(int restype,
                                                                       int softid,
                                                                       int platform,
                                                                       DateTime begintime,
                                                                       DateTime endtime,
                                                                       int period,
                                                                       ChannelTypeOptions selectchanneltype,
                                                                       string channelname,
                                                                       int selectchannelvalue,
                                                                       List<int> e_versionid,
                                                                       URLoginService loginService)
        {
            string key = BuildCacheKey("GetD_StatDownByVersionDistribution_SUMByCache",
                                       restype,
                                       softid,
                                       platform,
                                       begintime,
                                       endtime,
                                       period,
                                       selectchanneltype,
                                       channelname,
                                       selectchannelvalue,
                                       string.Join(",", e_versionid.Select(p => p.ToString()).ToArray()),
                                       loginService == null
                                           ? ""
                                           : ((loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.Channel ||
                                               loginService.LoginUser.AccountType ==
                                               Reports.UserRights.UserTypeOptions.ChannelPartner)
                                                  ? loginService.LoginUser.ID.ToString()
                                                  : "")
                );
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>(key
                 , Core.CacheTimeOption.TenMinutes
                 , () => new D_StatDownCount_DataAccess().GetD_StatDownByVersionDistribution_SUMByCache(restype,
                                                                          softid,
                                                                          platform,
                                                                          begintime,
                                                                          endtime,
                                                                          selectchanneltype,
                                                                          channelname,
                                                                          selectchannelvalue,
                                                                          e_versionid,
                                                                          period,
                                                                          loginService));
        }

        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownByAreaDistribution_SUMByCache(int restype, int softid,
                                                                                           int platform,
                                                                                           DateTime begintime,
                                                                                           DateTime endtime,
                                                                                           ChannelTypeOptions
                                                                                               selectchanneltype,
                                                                                           string channelname,
                                                                                           int selectchannelvalue,
                                                                                           int period,
                                                                                           int areatype,
                                                                                           URLoginService loginService)
        {
            string key = BuildCacheKey("GetD_StatDownByAreaDistribution_SUMByCache",
                           restype,
                           softid,
                           platform,
                           begintime,
                           endtime,
                           period,
                           selectchanneltype,
                           channelname,
                           selectchannelvalue,
                          areatype,
                           loginService == null
                               ? ""
                               : ((loginService.LoginUser.AccountType ==
                                   Reports.UserRights.UserTypeOptions.Channel ||
                                   loginService.LoginUser.AccountType ==
                                   Reports.UserRights.UserTypeOptions.ChannelPartner)
                                      ? loginService.LoginUser.ID.ToString()
                                      : ""));

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsBySoft_SUM>>(key
                 , Core.CacheTimeOption.TenMinutes
                 , () => new D_StatDownCount_DataAccess().GetD_StatDownByAreaDistribution_SUM(restype,
                                                                          softid,
                                                                          platform,
                                                                          begintime,
                                                                          endtime,
                                                                          selectchanneltype,
                                                                          channelname,
                                                                          selectchannelvalue,
                                                                          period,
                                                                          areatype,
                                                                          loginService));
        }

        public List<D_StatDownCountsByExtendAttrLst_SUM> GetD_StatDownByExtendAttrLst_SUMByCache(
                                                                        int restype, int softid, int platform,
                                                                        DateTime begintime, DateTime endtime, int period, 
                                                                        int extendAttrLst,int stattype)
        {
            string key = BuildCacheKey("GetD_StatDownByExtendAttrLst_SUMByCache",
               restype,
               softid,
               platform,
               begintime,
               endtime,
               period,
               extendAttrLst,
               stattype
               );

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsByExtendAttrLst_SUM>>(key
                 , Core.CacheTimeOption.TenMinutes
                 , () => new D_StatDownCount_DataAccess().GetD_StatDownByExtendAttrLst_SUM(
                                                                           restype,
                                                                           softid,
                                                                           platform,
                                                                           begintime,
                                                                           endtime,
                                                                           period,
                                                                           extendAttrLst,
                                                                           stattype));
        }
    }
}
