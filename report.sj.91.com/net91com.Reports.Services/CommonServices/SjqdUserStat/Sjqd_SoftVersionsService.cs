using System.Collections.Generic;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    public class Sjqd_SoftVersionsService : BaseService
    {
        private static Sjqd_SoftVersionsService instance;
        private static readonly object obj = new object();

        public static Sjqd_SoftVersionsService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_SoftVersionsService();
                            instance._cachePreviousKey = "Sjqd_SoftVersionsService";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        ///     获取版本类型信息
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platformid"></param>
        /// <param name="versiontype"></param>
        /// <returns></returns>
        public List<Sjqd_SoftVersions> GetVersionByVersionType(int softid, int platformid, int versiontype)
        {
            return CacheHelper.Get<List<Sjqd_SoftVersions>>
                (BuildCacheKey("GetVersionByVersionType", softid, platformid, versiontype),
                 CacheTimeOption.TenMinutes,
                 () => Sjqd_SoftVersion_DataAccess.Instance.GetVersionByVersionType(softid, platformid, versiontype));
        }
    }
}