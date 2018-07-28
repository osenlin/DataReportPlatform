using System.Collections.Generic;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    public class Sjqd_NetModesService : BaseService
    {
        private static Sjqd_NetModesService instance;
        private static readonly object obj = new object();

        public static Sjqd_NetModesService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_NetModesService();
                            instance._cachePreviousKey = "Sjqd_NetModesService";
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
        public List<Sjqd_NetModes> GetSjqd_NetModesCache()
        {
            return CacheHelper.Get<List<Sjqd_NetModes>>
                (BuildCacheKey("GetSjqd_NetModesCache"), CacheTimeOption.TenMinutes,
                 () => Sjqd_NetModes_DataAccess.Instance.GetSjqd_NetModes());
        }
    }
}