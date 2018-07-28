using System.Collections.Generic;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    public class Sjqd_Areas_Service : BaseService
    {
        private static Sjqd_Areas_Service instance;
        private static readonly object obj = new object();

        public static Sjqd_Areas_Service Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_Areas_Service();
                            instance._cachePreviousKey = "Sjqd_Areas_Service";
                        }
                    }
                }
                return instance;
            }
        }

        public List<Sjqd_Areas> GetAreasList(List<int> areas)
        {
            return Sjqd_Areas_DataAccess.Instance.GetAreasByIds(areas);
        }

        public Dictionary<int, string> GetProvince()
        {
            return CacheHelper.Get<Dictionary<int, string>>
                (BuildCacheKey("GetProvince"), CacheTimeOption.TenMinutes,
                 () => Sjqd_Areas_DataAccess.Instance.GetProvince());
        }
    }
}