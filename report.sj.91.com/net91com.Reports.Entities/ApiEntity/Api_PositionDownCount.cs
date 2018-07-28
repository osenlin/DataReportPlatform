using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_PositionDownCount
    {

        public int statdate { get; set; }

        public int position { get; set; }

        public int positionIndex { get; set; }

        public string positionName { get; set; }

        public int downcount { get; set; }

        public int restype { get; set; }

        public Api_PositionDownCount(){}
        public Api_PositionDownCount(IDataReader reader)
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {

                    case "statdate":
                        statdate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        break;
                    case "positionname":
                        positionName = reader.IsDBNull(i) ? "" : reader["positionname"].ToString();
                        break;
                    case "position":
                        position = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["position"]);
                        break;
                    case "positionindex":
                        positionIndex = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["positionindex"]);
                        break;
                    case "downcount":
                        downcount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                        break;
                    case "restype":
                        restype = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["restype"]);
                        break;
                }
            }
        }

    }
}
