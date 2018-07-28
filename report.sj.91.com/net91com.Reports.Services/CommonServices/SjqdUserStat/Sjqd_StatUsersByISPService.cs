using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    public class Sjqd_StatUsersByISPService : BaseService
    {
        private static Sjqd_StatUsersByISPService instance;
        private static readonly object obj = new object();

        public static Sjqd_StatUsersByISPService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_StatUsersByISPService();
                            instance._cachePreviousKey = "Sjqd_StatUsersByISPService";
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
        public List<Sjqd_StatUsersByISP> GetStatUsersByISPCache(DateTime begintime, DateTime endtime, int softid,
                                                                int platform, int period, string netmode, string ispid)
        {
            return CacheHelper.Get<List<Sjqd_StatUsersByISP>>
                (BuildCacheKey("GetStatUsersByISPCache", begintime, endtime, softid, platform, period, netmode, ispid),
                 CacheTimeOption.TenMinutes,
                 () =>
                 Sjqd_StatUsersByISP_DataAccess.Instance.GetStatUsersByISP(begintime, endtime, softid, platform, period,
                                                                           ispid, netmode));
        }

        public DataTable GetStatUsersByISPTableCache(DateTime begintime, DateTime endtime, int softid, int platform,
                                                     int period, List<string> netmode, List<string> ispid)
        {
            var sb1 = new StringBuilder();
            for (int i = 0; i < netmode.Count; i++)
            {
                sb1.Append(netmode[i]);
                sb1.Append(",");
            }
            var sb2 = new StringBuilder();
            for (int i = 0; i < ispid.Count; i++)
            {
                sb2.Append(ispid[i]);
                sb2.Append(",");
            }

            return CacheHelper.Get<DataTable>
                (BuildCacheKey("GetStatUsersByISPTableCache", begintime, endtime, softid, platform, period,
                               sb1.ToString(), sb2.ToString()), CacheTimeOption.TenMinutes,
                 () =>
                 Sjqd_StatUsersByISP_DataAccess.Instance.GetStatUsersByISPTable(begintime, endtime, softid, platform,
                                                                                period, ispid, netmode));
        }
    }
}