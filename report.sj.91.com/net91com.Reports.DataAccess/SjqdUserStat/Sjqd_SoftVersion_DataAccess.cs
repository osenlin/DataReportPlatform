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
    public class Sjqd_SoftVersion_DataAccess
    {
        private static Sjqd_SoftVersion_DataAccess instance = null;
        private static readonly object obj = new object();
        private static readonly string _cachePreviousKey = "Sjqd_ProjectSource_DataAccess";
        protected static string StatConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        public static Sjqd_SoftVersion_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_SoftVersion_DataAccess();
                        }
                    }
                }
                return instance;
            }
        }


        //根据软件id 和 平台id 获取版本
        public List<Sjqd_SoftVersions> GetVersionByVersionType(int softid, int platformid, int versiontype)
        {
            string sql = @"select distinct id,Version from  Sjqd_SoftVersions_Edit with(nolock)  
                            where softid=@softid and platform=@platform and hidden=0  order by ID DESC";
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platformid)
                };
            List<Sjqd_SoftVersions> versions = new List<Sjqd_SoftVersions>();
            using (IDataReader dataReader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                while (dataReader.Read())
                {
                    versions.Add(new Sjqd_SoftVersions(dataReader));
                }
            }
            return versions;
        }
    }
}