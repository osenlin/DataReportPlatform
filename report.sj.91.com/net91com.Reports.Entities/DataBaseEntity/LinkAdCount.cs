using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class LinkAdCount
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LinkAdCount()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public LinkAdCount(IDataReader reader)
            : this()
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
                        int sDate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["StatDate"]);
                        StatDate = new DateTime(sDate / 10000, sDate / 100 % 100, sDate % 100);
                        break;
                    case "softid":
                        SoftId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["SoftId"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "adid":
                        AdID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["AdID"]);
                        break;
                    case "planname":
                        PlanName = reader.IsDBNull(i) ? "" : reader["PlanName"].ToString();
                        break;
                    case "unionname":
                        UnionName = reader.IsDBNull(i) ? "" : reader["UnionName"].ToString();
                        break;
                    case "keyword":
                        Keyword = reader.IsDBNull(i) ? "" : reader["Keyword"].ToString();
                        break;
                    case "mapmode":
                        MapMode = reader.IsDBNull(i) ? "" : reader["MapMode"].ToString();
                        break;
                    case "hours":
                        Hours = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["hours"]);
                        break;
                    case "area":
                        Area = reader.IsDBNull(i) ? "" : reader["area"].ToString();
                        break;
                    case "searchword":
                        SearchWord = reader.IsDBNull(i) ? "" : reader["searchword"].ToString();
                        break;
                    case "linkcount":
                        LinkCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["linkcount"]);
                        break;
                    case "statcount_15":
                        StatCount_15 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["StatCount_15"]);
                        break;
                    case "statcount_30":
                        StatCount_30 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["StatCount_30"]);
                        break;
                    case "statcount_new_15":
                        StatCount_New_15 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["StatCount_New_15"]);
                        break;
                    case "statcount_new_30":
                        StatCount_New_30 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["StatCount_New_30"]);
                        break;
                }
            }

        }

        public DateTime StatDate { get; set; }
        public int SoftId { get; set; }
        public int Platform { get; set; }
        public int AdID { get; set; }
        public string PlanName { get; set; }
        public string UnionName { get; set; }
        public string Keyword { get; set; }
        public string MapMode { get; set; }
        public int Hours { get; set; }
        public string Area { get; set; }
        public string SearchWord { get; set; }
        public int LinkCount { get; set; }
        public int StatCount_15 { get; set; }
        public int StatCount_30 { get; set; }
        public int StatCount_New_15 { get; set; }
        public int StatCount_New_30 { get; set; }
    }
}
