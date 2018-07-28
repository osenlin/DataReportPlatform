using System.Collections.Generic;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    public class Sjqd_ISPService : BaseService
    {
        private static Sjqd_ISPService instance;
        private static readonly object obj = new object();

        public static Sjqd_ISPService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_ISPService();
                            instance._cachePreviousKey = "Sjqd_ISPService";
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
        public List<Sjqd_ISP> GetSjqd_ISPCache()
        {
            return CacheHelper.Get<List<Sjqd_ISP>>
                (BuildCacheKey("GetSjqd_ISPCache"), CacheTimeOption.TenMinutes,
                 () => Sjqd_ISP_DataAccess.Instance.GetSjqd_ISP());
        }
    }
}