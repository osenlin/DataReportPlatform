using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.DataAccess.DataAccesssUtil;

namespace net91com.Reports.DataAccess.ToolDataStat
{
    public class Tool_DataAccess : BaseDataAccess
    {
        private static Tool_DataAccess instance = null;
        private static readonly object obj = new object();

        public static Tool_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Tool_DataAccess();
                            instance._cachePreviousKey = "Tool_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取统计状态信息值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DateTime GetEtlStates(string key, string conString = null)
        {
            string sql = "select value from EtlStates where [key]=@key";

            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter()
                        {
                            ParameterName = "@key",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Size = 100,
                            Value = key
                        }
                };
            DateTime dt = DateTime.MinValue;
            int temp = 0;
            using (
                SqlDataReader reader = SqlHelper.ExecuteReader(string.IsNullOrEmpty(conString) ? StatConn : conString,
                                                               CommandType.Text, sql, param))
            {
                if (reader.Read())
                {
                    if (int.TryParse(reader["value"].ToString(), out temp))
                    {
                        dt = new DateTime(temp/10000, temp%10000/100, temp%100);
                    }
                    else
                    {
                        dt = Convert.ToDateTime(reader["value"]);
                    }
                }
            }
            return dt;
        }


        public List<Dictionary<string, string>> GetClusterStatLog(string startdate, string enddate, int modulutype)
        {
            string sql = string.Format(@" select ID,marktime,taskname,detail,modulutype 
                                         from Msg_Error 
                                        where modulutype={2} and marktime >='{0}' and marktime<'{1}'  ", startdate,
                                       enddate, modulutype);


            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                return RelationDBDataSetUtil.ParseDataSet(dataReader);
            }
        }
    }
}