using System;
using System.Collections.Generic;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    public class Sjqd_StatMac2Imei_Service : BaseService
    {
        private static Sjqd_StatMac2Imei_Service instance;
        private static readonly object obj = new object();

        public static Sjqd_StatMac2Imei_Service Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_StatMac2Imei_Service();
                            instance._cachePreviousKey = "Sjqd_StatMac2Imei_Service";
                        }
                    }
                }
                return instance;
            }
        }

        public List<Sjqd_StatMAC2IMEI> GetSjqd_StatMAC2IMEI(string channels, DateTime begintime, DateTime endtime)
        {
            return CacheHelper.Get<List<Sjqd_StatMAC2IMEI>>
                (BuildCacheKey("GetSjqd_StatMAC2IMEI", channels, begintime, endtime), CacheTimeOption.TenMinutes,
                 () => Sjqd_StatMac2Imei_DataAccess.Instance.GetStatMAC2IMEI(channels, begintime, endtime));
        }
    }
}