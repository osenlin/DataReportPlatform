using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using net91com.Core.Data;
using System.Data;
using net91com.Core.Web;
using net91com.Core;

namespace net91com.Reports.DataAccess.OtherDataAccess
{
    public class EtlStates_DataAccess : BaseDataAccess
    {
        private static EtlStates_DataAccess instance = null;
        private static readonly object obj = new object();

        public static EtlStates_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new EtlStates_DataAccess();
                            instance._cachePreviousKey = "EtlStates_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private DateTime GetKeyTime_TimeStr(string key)
        {
            string sql = "select [value] from EtlStates with(nolock) where [key]=@key";
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@key", SqlDbType.VarChar, 100, key)
                };
            return Convert.ToDateTime(SqlHelper.ExecuteScalar(StatConn, CommandType.Text, sql, parameters));
        }

        public DateTime GetKeyTime_IntStrByCache(string key)
        {
            string cacheKey = BuildCacheKey("GetMaxTimeCache", key);
            if (CacheHelper.Contains(cacheKey))
            {
                return new DateTime(CacheHelper.Get<DateTime>(cacheKey).Ticks);
            }
            DateTime dt = GetKeyTime_TimeStr(key);
            if (dt != null)
            {
                CacheHelper.Set<DateTime>(cacheKey, dt, CacheTimeOption.TenMinutes,
                                          CacheExpirationOption.AbsoluteExpiration);
            }
            return new DateTime(dt.Ticks);
        }
    }
}