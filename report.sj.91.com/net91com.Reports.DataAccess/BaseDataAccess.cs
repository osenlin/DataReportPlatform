using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Util;

namespace net91com.Reports.DataAccess
{
    public abstract class BaseDataAccess
    {
        protected static string MySQLConnectionString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        protected static string SJStaticDBConn = ConfigHelper.GetConnectionString("SJStaticDB_ConnString");
        protected static string SJStaticDB2Conn = ConfigHelper.GetConnectionString("SJStaticDB2_ConnString");
        protected static string SJStaticDB3Conn = ConfigHelper.GetConnectionString("SJStaticDB3_ConnString");
        protected static string StatConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        protected static string ComputingDBConn = ConfigHelper.GetConnectionString("ComputingDB_ConnString");
        protected static string ComputingDB_En_ConnString = ConfigHelper.GetConnectionString("ComputingDB_En_ConnString");

        protected static string ComputingDB_CN_ConnString = ConfigHelper.GetConnectionString("ComputingDB_CN_ConnString");

        protected static string ComputingDB_Sjqd_ConnString =
            ConfigHelper.GetConnectionString("ComputingDB_Sjqd_ConnString");

        protected static string NewResourceDB_ConnString = ConfigHelper.GetConnectionString("NewResourceDB_ConnString");
        protected static string MiscDB_ConnString = ConfigHelper.GetConnectionString("MiscDB_ConnString");
        protected static string CommentDB_ConnString = ConfigHelper.GetConnectionString("CommentDB_ConnString");

        protected static string ResourceSearchDB_ConnString =
            ConfigHelper.GetConnectionString("ResourceSearchDB_ConnString");

        protected static string SearchLogDB_ConnString = ConfigHelper.GetConnectionString("SearchLogDB_ConnString");
        protected static string ResourceSTATDB_ConnString = ConfigHelper.GetConnectionString("ResourceSTATDB_ConnString");

        protected static string ResourceSTATDB_En_ConnString =
            ConfigHelper.GetConnectionString("ResourceSTATDB_En_ConnString");

        protected static string ResourceSTATDB_Tw_ConnString =
            ConfigHelper.GetConnectionString("ResourceSTATDB_Tw_ConnString");

        protected static string ResourceLogDB_ConnString = ConfigHelper.GetConnectionString("ResourceLogDB_ConnString");
        protected static string SoftUseLogDB_ConnString = ConfigHelper.GetConnectionString("SoftUseLogDB_ConnString");
        protected static string FunctionLogDB_ConnString = ConfigHelper.GetConnectionString("FunctionLogDB_ConnString");
        protected static string UseLongLogDB_ConnString = ConfigHelper.GetConnectionString("UseLongLogDB_ConnString");
        protected static string ExceptionLogDB_ConnString = ConfigHelper.GetConnectionString("ExceptionLogDB_ConnString");
        protected static string UserSoftware_ConnString = ConfigHelper.GetConnectionString("UserSoftware_ConnString");
        protected static string AdvertiseDB_ConnString = ConfigHelper.GetConnectionString("AdvertiseDB_ConnString");

        protected static string ComputingDB_SoftID51_ConnString =
            ConfigHelper.GetConnectionString("ComputingDB_SoftID51");

        protected static string UserOptionDB_En_ConnString =
            ConfigHelper.GetConnectionString("UserOptionDB_En_ConnString");

        protected static string UserOptionDB_Tw_ConnString =
            ConfigHelper.GetConnectionString("UserOptionDB_Tw_ConnString");

        protected static string ComputingDB_SoftID6_ConnString =
            ConfigHelper.GetConnectionString("ComputingDB_SoftID6_ConnString");

        protected static string Mysql_Statdb_Connstring = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        protected string _cachePreviousKey;


        protected string GetSelectPageSQL(string tableName, string selectClause, string whereClause
                                          , string sortClause, int pageIndex, int pageSize)
        {
            int pageStart = (pageIndex - 1)*pageSize + 1;
            int pageEnd = pageIndex*pageSize;

            string cmdText = string.Empty;
            if (pageSize <= 0)
            {
                cmdText = new StringBuilder()
                    .AppendFormat("select {0} from {1} where {2} {3}",
                                  selectClause, tableName, whereClause, sortClause).ToString();
            }
            else if (pageIndex <= 1)
            {
                cmdText = new StringBuilder()
                    .AppendFormat("select top {4} {0} from {1} where {2} {3}",
                                  selectClause, tableName, whereClause, sortClause, pageSize).ToString();
            }
            else
            {
                cmdText = new StringBuilder()
                    .AppendFormat("select {0} from (", selectClause)
                    .AppendFormat(" select {0},row_number() over({1}) rno ", selectClause, sortClause)
                    .AppendFormat(" from {0}   where {1}", tableName, whereClause)
                    .AppendFormat(") a where rno between {0} and {1}", pageStart, pageEnd).ToString();
            }
            return cmdText;
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

        /// <summary>
        /// 获取项目来源列表
        /// </summary>
        /// <param name="projectSource">0 就返回所有项目来源</param>
        /// <returns></returns>
        protected List<int> GetProjectSources(int projectSource)
        {
            List<int> projectSources = new List<int>();

            string sql = string.Format(@"select distinct ProjectSource from R_ProjectSourcesBySoft with(nolock)
                         {0}",
                                       projectSource == 0
                                           ? ""
                                           : (" where ProjectSource=" + projectSource + " or ParentProjectSource=" +
                                              projectSource));


            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    int project = Convert.ToInt32(reader["ProjectSource"]);
                    projectSources.Add(project);
                }
            }


            return projectSources;
        }
    }
}