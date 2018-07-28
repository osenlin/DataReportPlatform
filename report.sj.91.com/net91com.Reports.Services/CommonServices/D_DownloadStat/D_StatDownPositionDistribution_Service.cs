using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Reports.DataAccess.D_DownloadStat;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.CommonServices.D_DownloadStat
{
    public class D_StatDownPositionDistribution_Service : BaseService
    {
        private D_StatDownPositionDistribution_Service()
        {
            _cachePreviousKey = "D_StatDownPositionDistribution_Service";
        }

        private static readonly object obj = new object();
        private static D_StatDownPositionDistribution_Service instance;

        public static D_StatDownPositionDistribution_Service Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new D_StatDownPositionDistribution_Service();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取下载位置分布
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="projectsource"></param>
        /// <param name="versionid"></param>
        /// <param name="isupdate"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="period"></param>
        /// <param name="isdiffpagetype"></param>
        /// <param name="stattype"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionByCache(int restype, int softid,
                                                                                             int platform,
                                                                                             int projectsource,
                                                                                             string versionid,
                                                                                             int isupdate,
                                                                                             DateTime begintime,
                                                                                             DateTime endtime,
                                                                                             int period,
                                                                                             int isdiffpagetype,
                                                                                             int stattype,
                                                                                             string areaid)
        {
            string key = BuildCacheKey("GetD_StatDownPositionDistributionByCache",
                                       restype,
                                       softid,
                                       platform,
                                       projectsource,
                                       versionid,
                                       isupdate,
                                       begintime,
                                       endtime,
                                       period,
                                       isdiffpagetype,
                                       stattype,
                                       areaid);

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownPositionDistribution(restype,
                                                                                                   softid,
                                                                                                   platform,
                                                                                                   begintime,
                                                                                                   endtime,
                                                                                                   projectsource,
                                                                                                   versionid,
                                                                                                   isupdate,
                                                                                                   period,
                                                                                                   areaid,
                                                                                                   isdiffpagetype,
                                                                                                   stattype
                                                                                                   ));
        }

        /// <summary>
        /// 获取下肢位置分布按专辑
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="projectsource"></param>
        /// <param name="versionid"></param>
        /// <param name="isupdate"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="period"></param>
        /// <param name="pagename"></param>
        /// <param name="pagetype"></param>
        /// <param name="isdiffpagetype"></param>
        /// <param name="stattype"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionByTagCache(int restype,
                                                                                                int softid,
                                                                                                int platform,
                                                                                                int projectsource,
                                                                                                string versionid,
                                                                                                int isupdate,
                                                                                                DateTime begintime,
                                                                                                DateTime endtime,
                                                                                                int period,
                                                                                                string pagename,
                                                                                                string pagetype,
                                                                                                int isdiffpagetype,
                                                                                                int stattype,
                                                                                                string areaid)
        {
            string key = BuildCacheKey("GetD_StatDownPositionDistributionByTagCache",
                                       restype,
                                       softid,
                                       platform,
                                       begintime,
                                       endtime,
                                       period,
                                       projectsource,
                                       versionid,
                                       isupdate,
                                       pagename,
                                       pagetype,
                                       isdiffpagetype,
                                       stattype,
                                       areaid);

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                 , Core.CacheTimeOption.TenMinutes
                 , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownPositionDistributionByTag(restype, softid,
                                                                                                        platform,
                                                                                                        begintime,
                                                                                                        endtime,
                                                                                                        projectsource,
                                                                                                        versionid,
                                                                                                        isupdate, period,
                                                                                                        pagename,
                                                                                                        pagetype,
                                                                                                        areaid,
                                                                                                        isdiffpagetype,
                                                                                                        stattype));
        }

        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionByTagClassCacheDetail(int restype,
                                                                                                           int softid,
                                                                                                           int platform,
                                                                                                           int projectsource,
                                                                                                           string versionid,
                                                                                                           int isupdate,
                                                                                                           DateTime begintime,
                                                                                                           DateTime endtime,
                                                                                                           int period,
                                                                                                           string pagename,
                                                                                                           string areaid,
                                                                                                           int isdiffpagetype,
                                                                                                           int stattype)
        {
            string key = BuildCacheKey("GetD_StatDownPositionDistributionByTagClassCacheDetail",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        projectsource,
                                        versionid,
                                        isupdate,
                                        areaid,
                                        pagename,
                                        isdiffpagetype,
                                        stattype);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                   , Core.CacheTimeOption.TenMinutes
                   , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownPositionDistributionByTagClassDetail(restype,
                                                                                                                            softid,
                                                                                                                            platform,
                                                                                                                            begintime,
                                                                                                                            endtime,
                                                                                                                            projectsource,
                                                                                                                            versionid,
                                                                                                                            isupdate,
                                                                                                                            period,
                                                                                                                            pagename,
                                                                                                                            areaid,
                                                                                                                            isdiffpagetype,
                                                                                                                            stattype));
        }

        /// <summary>
        /// 获取下载位置按专辑明细
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="projectsource"></param>
        /// <param name="versionid"></param>
        /// <param name="isupdate"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="period"></param>
        /// <param name="positionid"></param>
        /// <param name="isdiffpagetype"></param>
        /// <param name="stattype"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionByTagDetailCache(int restype,
                                                                                                      int softid,
                                                                                                      int platform,
                                                                                                      int projectsource,
                                                                                                      string versionid,
                                                                                                      int isupdate,
                                                                                                      DateTime begintime,
                                                                                                      DateTime endtime,
                                                                                                      int period,
                                                                                                      int positionid,
                                                                                                      string areaid,
                                                                                                      int isdiffpagetype
                                                                                                          = 0,
                                                                                                      int stattype = 1)
        {
            string key = BuildCacheKey("GetD_StatDownPositionDistributionByTagDetailCache",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        projectsource,
                                        versionid,
                                        isupdate,
                                        positionid,
                                        isdiffpagetype,
                                        stattype,
                                        areaid);

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownPositionDistributionByTagDetail(restype,
                                                                                                              softid,
                                                                                                              platform,
                                                                                                              begintime,
                                                                                                              endtime,
                                                                                                              projectsource,
                                                                                                              versionid,
                                                                                                              isupdate,
                                                                                                              period,
                                                                                                              positionid,
                                                                                                              areaid,
                                                                                                              isdiffpagetype,
                                                                                                              stattype));
        }

        /// <summary>
        /// 获取下载位置明细
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="projectsource"></param>
        /// <param name="versionid"></param>
        /// <param name="isupdate"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="period"></param>
        /// <param name="positionid"></param>
        /// <param name="isdiffpagetype"></param>
        /// <param name="stattype"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionDetailCache(int restype,
                                                                                                 int softid,
                                                                                                 int platform,
                                                                                                 int projectsource,
                                                                                                 string versionid,
                                                                                                 int isupdate,
                                                                                                 DateTime begintime,
                                                                                                 DateTime endtime,
                                                                                                 int period,
                                                                                                 int positionid,
                                                                                                 string pagename,
                                                                                                 string areaid,
                                                                                                 int isdiffpagetype = 0,
                                                                                                 int stattype = 1)
        {
            string key = BuildCacheKey("GetD_StatDownPositionDistributionDetailCache",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        projectsource,
                                        versionid,
                                        isupdate,
                                        positionid,
                                        pagename,
                                        areaid,
                                        isdiffpagetype,
                                        stattype);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownPositionDistributionDetail(restype,
                                                                                                         softid,
                                                                                                         platform,
                                                                                                         begintime,
                                                                                                         endtime,
                                                                                                         projectsource,
                                                                                                         versionid,
                                                                                                         isupdate,
                                                                                                         period,
                                                                                                         positionid,
                                                                                                         pagename,
                                                                                                         areaid,
                                                                                                         isdiffpagetype,
                                                                                                         stattype));
        }

        /// <summary>
        /// 获取api日分发
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownApiCache(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownApiCache", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownApi(statdate));
        }
        public List<D_StatDownPositionDistribution> GetD_StatDownApiCacheMonth(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownApiCacheMonth", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownApiMonth(statdate));
        }

        /// <summary>
        /// 获取阿拉丁苹果园日分发
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownAladinCache(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownAladin", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownAladin(statdate));
        }
        /// <summary>
        /// 获取阿拉丁苹果园月分发
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownAladinCacheMonth(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownAladinMonth", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownAladinMonth(statdate));
        }

        /// <summary>
        /// 获取web日分发
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownWebCache(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownWebCache", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownWeb(statdate));
        }
        /// <summary>
        /// 获取web月分发
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownWebCacheMonth(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownWebCacheMonth", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownWebMonth(statdate));
        }

        public List<D_StatDownPositionDistribution> GetD_StatDownWebPgzsCacheMonth(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownWebPgzsCacheMonth", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownWebPgzsMonth(statdate));
        }

        /// <summary>
        /// 获取UV日分发
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownUVCache(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownUVCache", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownUV(statdate));
        }

        /// <summary>
        /// 获取Web+Api月分发
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownMonthWebAndApiCache(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownMonthWebAndApiCache", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownMonthWebAndApi(statdate));
        }

        /// <summary>
        /// 获取资源下载按ID的位置明细
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softs"></param>
        /// <param name="platform"></param>
        /// <param name="projectsource"></param>
        /// <param name="version"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="areatype"></param>
        /// <param name="lstresid"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionByResIDCacheDetailCache(int restype,
                                                                                                 int softs,
                                                                                                 int platform,
                                                                                                 int projectsource,
                                                                                                 int version,
                                                                                                 DateTime begintime,
                                                                                                 DateTime endtime,
                                                                                                 int areatype,
                                                                                                 int areaid,
                                                                                                 List<int> lstresid)
        {
            string key = BuildCacheKey("GetD_StatDownPositionByResIDCacheDetailCache",
                                        restype,
                                        softs,
                                        platform,
                                        begintime,
                                        endtime,
                                        projectsource,
                                        version,
                                        areatype,
                                        areaid,
                                        string.Join(",", lstresid.Select(p => p.ToString()).ToArray()));

            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownPositionByResIDCacheDetail(restype,
                                                                                                         softs,
                                                                                                         platform,
                                                                                                         begintime,
                                                                                                         endtime,
                                                                                                         projectsource,
                                                                                                         version,
                                                                                                         areatype,
                                                                                                         areaid,
                                                                                                         lstresid));
        }

    

        public List<D_StatDownPositionDistribution> GetD_StatDownWebPgzsCache(int statdate)
        {
            string key = BuildCacheKey("GetD_StatDownWebPgzsCache", statdate);
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownPositionDistribution>>(key
                , Core.CacheTimeOption.TenMinutes
                , () => new D_StatDownPositionDistribution_DataAccess().GetD_StatDownWebPgzs(statdate));
        }
    }
}
