using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using net91com.Core;
using net91com.Core.Extensions;
using net91com.Core.Util;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.OtherDataAccess
{
    /// <summary>
    /// 工具性功能页面涉及的数据访问类
    /// </summary>
    public class ToolDataAccess : BaseDataAccess
    {
        private static ToolDataAccess instance = null;
        private static readonly object obj = new object();

        public static ToolDataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new ToolDataAccess();
                            instance._cachePreviousKey = "ToolDataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取国家名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetCountryNameList()
        {
            string sql = @"select distinct E_Country from Cfg_Areas";
            List<string> countriesList = new List<string>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(StatConn, sql))
            {
                while (reader.Read())
                {
                    countriesList.Add(reader["E_Country"].ToString());
                }
            }
            return countriesList;
        }
        
        public string getTagByTagId(int id)
        {
            string sql = @"select Tag from Cfg_LinkTags where ID="+id;
            string tagname = "";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(StatConn, sql))
            {
                if (reader.Read())
                {
                    tagname=reader["Tag"].ToString();
                }
            }
            return tagname;
        }
    }
}