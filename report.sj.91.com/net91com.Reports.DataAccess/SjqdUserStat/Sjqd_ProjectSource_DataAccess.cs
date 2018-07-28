using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class Sjqd_ProjectSource_DataAccess
    {
        private static Sjqd_ProjectSource_DataAccess instance = null;
        private static readonly object obj = new object();
        private static readonly string _cachePreviousKey = "Sjqd_ProjectSource_DataAccess";
        protected static string StatConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        public static Sjqd_ProjectSource_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_ProjectSource_DataAccess();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取项目List
        /// </summary>
        /// <returns></returns>
        private List<Config_ProjectSource> GetConfig_ProjectSourceByID()
        {
            string sql =
                @"SELECT  ProjectSource, SoftID, Name, ProjectSourceType, OnlyInternal, SortIndex, Status, AddTime
                         FROM dbo.R_ProjectSourcesBySoft with(nolock) ";
            List<Config_ProjectSource> projectsource = new List<Config_ProjectSource>();
            using (IDataReader dr = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                while (dr.Read())
                {
                    projectsource.Add(new Config_ProjectSource(dr));
                }
            }
            return projectsource;
        }

        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <param name="timeOption"></param>
        /// <returns></returns>
        public List<Config_ProjectSource> GetConfig_ProjectSourceByIDCache(Core.CacheTimeOption timeOption)
        {
            return
                net91com.Core.Web.CacheHelper.Get<List<Config_ProjectSource>>(
                    BuildCacheKey("GetConfig_ProjectSourceByIDCache"),
                    timeOption,
                    () => GetConfig_ProjectSourceByID());
        }

        protected string BuildCacheKey(params object[] args)
        {
            StringBuilder sbCacheKey = new StringBuilder(_cachePreviousKey);
            foreach (object obj in args)
            {
                sbCacheKey.Append("_");
                sbCacheKey.Append(obj);
            }
            return sbCacheKey.ToString();
        }
    }
}