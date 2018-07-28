using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class Sjqd_Areas_DataAccess : BaseDataAccess
    {
        private static Sjqd_Areas_DataAccess instance = null;
        private static readonly object obj = new object();

        public static Sjqd_Areas_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_Areas_DataAccess();
                            instance._cachePreviousKey = "Sjqd_Areas_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        public List<Sjqd_Areas> GetAreasByIds(List<int> areaids)
        {
            List<Sjqd_Areas> result = new List<Sjqd_Areas>();
            if (areaids.Count == 0)
                return result;
            string filters = string.Join(",", areaids.Select(p => p.ToString()).ToArray());
            string sql = string.Format(
                @"select ID, Country, Province, City, E_Country, E_Province
from dbo.Sjqd_Areas with(nolock)
where ID in({0})", filters);
            using (IDataReader dr = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                while (dr.Read())
                {
                    result.Add(new Sjqd_Areas(dr));
                }
            }
            return result;
        }

        public Dictionary<int, string> GetProvince()
        {
            Dictionary<int, string> data = new Dictionary<int, string>();
            string sql = @"select ID,Province from Sjqd_Province_Edit with(nolock)";
            using (IDataReader dr = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                while (dr.Read())
                {
                    data[Convert.ToInt32(dr[0])] = dr[1].ToString();
                }
            }
            return data;
        }
    }
}