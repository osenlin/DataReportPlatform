using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class Sjqd_ISP_DataAccess : BaseDataAccess
    {
        private static Sjqd_ISP_DataAccess instance = null;
        private static readonly object obj = new object();

        public static Sjqd_ISP_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_ISP_DataAccess();
                            instance._cachePreviousKey = "Sjqd_ISP_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        public List<Sjqd_ISP> GetSjqd_ISP()
        {
            string sql = "select distinct E_Name name from Sjqd_ISP with(nolock)";
            List<Sjqd_ISP> result = new List<Sjqd_ISP>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    result.Add(new Sjqd_ISP(reader));
                }
            }
            return result;
        }
    }
}