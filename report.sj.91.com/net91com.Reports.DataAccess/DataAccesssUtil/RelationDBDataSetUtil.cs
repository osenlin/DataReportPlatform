using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace net91com.Reports.DataAccess.DataAccesssUtil
{
    public class RelationDBDataSetUtil
    {
        public static List<Dictionary<string, string>> ParseDataSet(IDataReader read)
        {
            var lstMap = new List<Dictionary<string, string>>();
            Dictionary<string, string> map;
            while (read.Read())
            {
                map = new Dictionary<string, string>();
                for (int i = 0; i < read.FieldCount; i++)
                {
                    map.Add(read.GetName(i).ToLower(), read.IsDBNull(i) ? "" : read[i].ToString());
                }
                lstMap.Add(map);
            }
            return lstMap;
        }
    }
}