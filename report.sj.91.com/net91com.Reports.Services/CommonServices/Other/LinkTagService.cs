using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.OtherDataAccess;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Stat.Services.Entity;
using System.Data;

namespace net91com.Reports.Services.CommonServices.Other
{
    public class LinkTagService : BaseService
    {
        private static LinkTagService instance = null;
        private static readonly object obj = new object();
        public static LinkTagService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new LinkTagService();
                            instance._cachePreviousKey = "LinkTagService";
                        }
                    }
                }
                return instance;
            }
        }

        public List<LinkTagLog> GetTagTree(int softId, MobileOption[] platforms, bool includeTagIds)
        {
            List<LinkTagLog> tags;
            string cacheKey = BuildCacheKey("GetTagTree", softId, includeTagIds);
            if (CacheHelper.Contains(cacheKey))
            {
                tags = CacheHelper.Get<List<LinkTagLog>>(cacheKey);
            }
            else
            {
                tags = LinkTagLog_DataAccess.Instance.GetTags(softId, includeTagIds);
                CacheHelper.Set<List<LinkTagLog>>(cacheKey, tags, CacheTimeOption.TenMinutes, CacheExpirationOption.AbsoluteExpiration);
            }
            if (!includeTagIds || platforms == null || platforms.Length == 0 || platforms.Contains(MobileOption.None))
                return tags;

            List<LinkTagLog> result = new List<LinkTagLog>();
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i].Platform == 0 || platforms.Contains((MobileOption)tags[i].Platform))
                    result.Add(tags[i]);
            }
            return result;
        }

        public List<LinkTagCount> GetTagCountCache(
            DateTime begin, DateTime end, int softid, int platform, net91com.Stat.Core.PeriodOptions period,
            int version, int tagid, string tagText, bool isCategory, CacheTimeOption cachetime)
        {
            string cacheKey = BuildCacheKey("GetTagCountCache", begin, end, softid, platform, period, version, tagid, isCategory);
            if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
            {
                return CacheHelper.Get<List<LinkTagCount>>(cacheKey).ToList();
            }
            List<LinkTagCount> list = LinkTagLog_DataAccess.Instance.GetTagCountList(
                begin, end, (int)period, softid, platform, version, tagid, tagText, isCategory);
            if (list != null && cachetime != CacheTimeOption.None)
            {
                CacheHelper.Set<List<LinkTagCount>>(cacheKey, list, cachetime, CacheExpirationOption.AbsoluteExpiration);
            }
            return list;
        }

        public Dictionary<string, Dictionary<string, string>> GetLinkTagInfoDicCache(int softid, int platform)
        {
            string cacheKey = BuildCacheKey("GetLinkTagInfoDicCache", softid, platform);
            if (CacheHelper.Contains(cacheKey))
            {
                return CacheHelper.Get<Dictionary<string, Dictionary<string, string>>>(cacheKey);
            }
            Dictionary<string, Dictionary<string, string>> list = LinkTagLog_DataAccess.Instance.GetLinkTagInfoDic(softid, platform);

            if (list != null)
            {
                CacheHelper.Set<Dictionary<string, Dictionary<string, string>>>(cacheKey, list, CacheTimeOption.TenMinutes, CacheExpirationOption.AbsoluteExpiration);
            }
            return list;

        }

        public DataTable GetTagCountTable(DateTime statDate, int softid, int platform)
        {
            return LinkTagLog_DataAccess.Instance.GetTagCountTable(statDate, softid, platform);
        }
        public DataTable GetTagCountMonthTable(int softid, int platform, string tag)
        {
            return LinkTagLog_DataAccess.Instance.GetTagCountMonthTable(softid, platform, tag);
        }
        public DataTable GetTagCountDailyTable(DateTime statDate, int softid, int platform, string tag)
        {
            return LinkTagLog_DataAccess.Instance.GetTagCountDailyTable(statDate, softid, platform, tag);
        }
    }
}
