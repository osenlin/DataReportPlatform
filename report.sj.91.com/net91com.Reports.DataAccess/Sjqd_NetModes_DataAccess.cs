using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess
{
    public class Sjqd_NetModes_DataAccess : BaseDataAccess
    {
        private static Sjqd_NetModes_DataAccess instance = null;
        private static readonly object obj = new object();

        public static Sjqd_NetModes_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_NetModes_DataAccess();
                            instance._cachePreviousKey = "Sjqd_NetModes_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Sjqd_NetModes> GetSjqd_NetModes()
        {
            string sql = "select ID,Name,E_Name from sjqd_netmodes with(nolock)";
            List<Sjqd_NetModes> result = new List<Sjqd_NetModes>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    result.Add(new Sjqd_NetModes(reader));
                }
            }
            return result;
        }
    }
}