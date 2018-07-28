using System;
using System.Collections.Generic;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.D_DownloadStat;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;

namespace net91com.Reports.Services.CommonServices.D_DownloadStat
{
    internal class D_StatDownRank_Service : BaseService
    {
        private D_StatDownRank_Service()
        {
            _cachePreviousKey = "D_StatDownRank_Service";
        }

        private static readonly object obj = new object();
        private static D_StatDownRank_Service instance;

        public static D_StatDownRank_Service Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new D_StatDownRank_Service();
                        }
                    }
                }
                return instance;
            }
        }


        public List<Dictionary<string, string>> GetD_StatDownRankByClassByCacheMap(int restype,
                                                                                   int softid,
                                                                                   int platform,
                                                                                   DateTime begintime,
                                                                                   DateTime endtime,
                                                                                   int period,
                                                                                   int pcid,
                                                                                   int cid,
                                                                                   int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownRankByClassMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        pcid,
                                        cid,
                                        downtype);

            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key
                , CacheTimeOption.TenMinutes
                , () => new D_StatDownRank_DataAccess().GetD_StatDownRankByClassMap(restype,
                                                                                     softid,
                                                                                     platform,
                                                                                     begintime,
                                                                                     endtime,
                                                                                     period,
                                                                                     pcid,
                                                                                     cid,
                                                                                     downtype));
        }

        public List<D_StatDownRank_SUM> GetD_StatDownRankByClassByCache(int restype,
                                                                           int softid,
                                                                           int platform,
                                                                           DateTime begintime,
                                                                           int period,
                                                                           int pcid,
                                                                           int cid,
                                                                           int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownRankByClass",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        period,
                                        pcid,
                                        cid,
                                        downtype);

            return CacheHelper.Get<List<D_StatDownRank_SUM>>
                (key
                , CacheTimeOption.TenMinutes
                , () => new D_StatDownRank_DataAccess().GetD_StatDownRankByClass(restype,
                                                                                     softid,
                                                                                     platform,
                                                                                     begintime,
                                                                                     period,
                                                                                     pcid,
                                                                                     cid,
                                                                                     downtype));
        }


        public List<Dictionary<string, string>> GetD_StatDownRankByAreaByCacheMap(int restype,
                                                                                int softid,
                                                                                int platform,
                                                                                DateTime begintime,
                                                                                DateTime endtime,
                                                                                int period,
                                                                                int countryid,
                                                                                int provinceid,
                                                                                int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownRankByAreaByCacheMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        countryid,
                                        provinceid, downtype);
            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownRankByAreaMap(restype,
                                                                            softid,
                                                                            platform,
                                                                            begintime,
                                                                            endtime,
                                                                            period,
                                                                            countryid,
                                                                            provinceid,
                                                                            downtype));
        }

        public List<D_StatDownRank_SUM> GetD_StatDownRankByAreaByCache(int restype,
                                                                        int softid,
                                                                        int platform,
                                                                        DateTime begintime,
                                                                        int period,
                                                                        string areaid,
                                                                        int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownRankByAreaByCache",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        period,
                                        areaid,
                                        downtype);

            return CacheHelper.Get<List<D_StatDownRank_SUM>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownRankByArea(restype,
                                                                            softid,
                                                                            platform,
                                                                            begintime,
                                                                            period,
                                                                            areaid,
                                                                            downtype));
        }

        public List<Dictionary<string, string>> GetD_StatDownRankByExtendAttrLstMapMap(int restype,
                                                                        int softid,
                                                                        int platform,
                                                                        DateTime begintime,
                                                                        DateTime endtime,
                                                                        int period,
                                                                        int extendAttrlst,
                                                                        int stattype)
        {
            string key = BuildCacheKey("GetD_StatDownRankByAreaByCacheMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        extendAttrlst,
                                        stattype
                                       );
            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownRankByExtendAttrLstMap(restype,
                                                                            softid,
                                                                            platform,
                                                                            begintime,
                                                                            endtime,
                                                                            period,
                                                                            extendAttrlst,
                                                                            stattype
                                                                            ));
        }

        public List<D_StatDownRank_SUM> GetD_StatDownRankByExtendAttrLst_IdentiferCache(
                                                                        int restype,
                                                                        int softid,
                                                                        int platform,
                                                                        DateTime begintime,
                                                                        int period,
                                                                        int extendAttrlst,
                                                                        int stattype)
        {
            string key = BuildCacheKey("GetD_StatDownRankByExtendAttrLst_IdentiferCache",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        period,
                                        extendAttrlst,
                                        stattype
                                       );
            return CacheHelper.Get<List<D_StatDownRank_SUM>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownRankByExtendAttrLst_Identifer(restype,
                                                                            softid,
                                                                            platform,
                                                                            begintime,
                                                                            period,
                                                                            extendAttrlst,
                                                                            stattype
                                                                            ));
        }

        


        public List<Dictionary<string, string>> GetD_StatDownCPAByCacheMap(int restype,
                                                                                int softid,
                                                                                int platform,
                                                                                DateTime begintime,
                                                                                DateTime endtime,
                                                                                int period,
                                                                                int countryid,
                                                                                int provinceid,
                                                                                int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownCPAByCacheMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        countryid,
                                        provinceid, downtype);

            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownCPAByCacheMap(restype,
                                                                              softid,
                                                                              platform,
                                                                              begintime,
                                                                              endtime,
                                                                              period,
                                                                              countryid,
                                                                              provinceid,
                                                                              downtype));
        }

        public List<Dictionary<string, string>> GetD_StatDownCPAAndApiByCacheMap(int restype,
                                                                                int softid,
                                                                                int platform,
                                                                                DateTime begintime,
                                                                                DateTime endtime,
                                                                                int period,
                                                                                int countryid,
                                                                                int provinceid,
                                                                                int downtype, string positions, string adsource)
        {
            string key = BuildCacheKey("GetD_StatDownCPAAndApiByCacheMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        countryid,
                                        provinceid, downtype, positions, adsource);

            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownCPAAndApiByCacheMap(restype,
                                                                              softid,
                                                                              platform,
                                                                              begintime,
                                                                              endtime,
                                                                              period,
                                                                              countryid,
                                                                              provinceid,
                                                                              downtype, positions, adsource));
        }

        public List<Dictionary<string, string>> GetD_StatDownCPAAndApiDailyByCacheMap(int restype,
                                                                                int softid,
                                                                                int platform,
                                                                                DateTime begintime,
                                                                                DateTime endtime,
                                                                                int period,
                                                                                int countryid,
                                                                                int provinceid,
                                                                                int downtype, string positions, string adsource, string version)
        {
            string key = BuildCacheKey("GetD_StatDownCPAAndApiDailyByCacheMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        countryid,
                                        provinceid, downtype, positions, adsource, version);

            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key, CacheTimeOption.TenMinutes,
                 () =>  new D_StatDownRank_DataAccess().GetD_StatDownCPAAndApiDailyByCacheMap(restype,
                                                                              softid,
                                                                              platform,
                                                                              begintime,
                                                                              endtime,
                                                                              period,
                                                                              countryid,
                                                                              provinceid,
                                                                              downtype, positions, adsource, version));

        }
        public List<Dictionary<string, string>> GetD_StatDownCPAAndApiByResIdByCacheMap(int restype,
                                                                                int softid,
                                                                                int platform,
                                                                                DateTime begintime,
                                                                                DateTime endtime,
                                                                                int period,
                                                                                int countryid,
                                                                                int provinceid,
                                                                                int downtype, string positions, string identifier, int compaignId, string adsource)
        {
            string key = BuildCacheKey("GetD_StatDownCPAAndApiByCacheMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        countryid,
                                        provinceid, downtype, positions, identifier, compaignId, adsource);
            
            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownCPAAndApiByResIdByCacheMap(restype,
                                                                              softid,
                                                                              platform,
                                                                              begintime,
                                                                              endtime,
                                                                              period,
                                                                              countryid,
                                                                              provinceid,
                                                                              downtype, positions, identifier, compaignId, adsource));
        }


        public List<Dictionary<string, string>> GetD_StatDownRankBySoftByCacheMap(int restype,
                                                                                    int softid,
                                                                                    int platform,
                                                                                    DateTime begintime,
                                                                                    DateTime endtime,
                                                                                    int period,
                                                                                    int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownRankBySoftByCacheMap",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period,
                                        downtype);

            return CacheHelper.Get<List<Dictionary<string, string>>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownRankBySoftMap(restype,
                                                                            softid,
                                                                            platform,
                                                                            begintime,
                                                                            endtime,
                                                                            period,
                                                                            downtype));
        }

        public List<D_StatDownRank_SUM> GetD_StatDownRankBySoftByCache(int restype,
                                                                            int softid,
                                                                            int platform,
                                                                            DateTime begintime,
                                                                            int period,
                                                                            int downtype)
        {
            string key = BuildCacheKey("GetD_StatDownRankBySoftByCache",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        period,
                                        downtype);

            return CacheHelper.Get<List<D_StatDownRank_SUM>>
                (key, CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownRankBySoft(restype,
                                                                            softid,
                                                                            platform,
                                                                            begintime,
                                                                            period,
                                                                            downtype));
        }

        internal List<D_StatDownCPAEntity> GetD_StatDownCPAByResIDCache(int restype, int softid, int platform, DateTime begintime, DateTime endtime, int resid, int areaid)
        {
            return new D_StatDownRank_DataAccess().GetD_StatDownCPAByResIDCache(restype, softid, platform, begintime, endtime, resid, areaid);
        }

        public List<D_StatDownCountsByResIDEntity> GetD_StatDownCountRankByAuthorIDCache(
                                                                     int period,
                                                                     int restype,
                                                                     int softid,
                                                                     int platform,
                                                                     DateTime begintime,
                                                                     DateTime endtime
                                                                     )
        {
            string key = BuildCacheKey("GetD_StatDownCountRankByAuthorID",
                                        restype,
                                        softid,
                                        platform,
                                        begintime,
                                        endtime,
                                        period
                );
            return net91com.Core.Web.CacheHelper.Get<List<D_StatDownCountsByResIDEntity>>
                (key, Core.CacheTimeOption.TenMinutes,
                 () =>
                 new D_StatDownRank_DataAccess().GetD_StatDownCountRankByAuthorID(
                                                                        period,
                                                                        softid,
                                                                        platform,
                                                                        restype,
                                                                        begintime,
                                                                        endtime));
        }

        
    }
}
