using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Core.Web;
using System.Text;
using net91com.Core.Extensions;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using net91com.Reports.UserRights;


namespace net91com.Stat.Services.sjqd
{
    public class StatUsersByVersionService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private string _cachePreviousKey;
        private bool useCache;

        private string BuildCacheKey(params object[] args)
        {
            StringBuilder sbCacheKey = new StringBuilder(_cachePreviousKey);
            foreach (object obj in args)
            {
                sbCacheKey.Append("_");
                sbCacheKey.Append(obj);
            }
            return sbCacheKey.ToString();
        }

        private static StatUsersByVersionService service;

        private StatUsersByVersionService()
        {
        }

        public static StatUsersByVersionService GetInstance()
        {
            if (service == null)
            {
                service = new StatUsersByVersionService();
                service.useCache = true;
                service._cachePreviousKey = "StatUsersByVersionService";
            }
            SqlHelper.CommandTimeout = 120;
            return service;
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platformid"></param>
        /// <param name="cachetime"></param>
        /// <returns></returns>
        public List<SoftVersion> GetVersionCacheStatDB(int softid, int platformid, int versiontype,
                                                       CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetVersionCacheStatDB", softid, platformid, versiontype);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<SoftVersion>>(cacheKey);
                }
                List<SoftVersion> list = GetVersion(softid, platformid, versiontype);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<SoftVersion>>(cacheKey, list, cachetime,
                                                       CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetVersion(softid, platformid, versiontype);
            }
        }

        //根据软件id 和 平台id 获取版本
        public List<SoftVersion> GetVersion(int softid, int platformid, int versiontype)
        {
            string sql = @"select id,Version 
                            from Cfg_Versions 
                            where  softid=?softid and platform=?platform and hidden=0";
            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform", platformid)
                };
            List<SoftVersion> versions = new List<SoftVersion>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(statdbConn, sql, parameters))
            {
                while (dataReader.Read())
                {
                    try
                    {
                        SoftVersion ver = new SoftVersion();
                        ver.VersionID = dataReader["Version"].ToString();
                        ver.VersionCode = dataReader["Version"].ToString();
                        versions.Add(ver);
                    }
                    catch
                    {
                    }
                }
            }
            return versions.OrderByDescending(a => a.Version).ToList();
        }
    }
}