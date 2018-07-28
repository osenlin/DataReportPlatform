using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class Down_StatDownCountByTitle
    {
        public DateTime StatDate;

        public string TitleName;

        public int TitleID;

        public string CateName;

        public int CateID;

        public int AvgDownCount;

        public int DownCount;

        public int PositionIndex;

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Down_StatDownCountByTitle(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Down_StatDownCountByTitle()
        {
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
                        int temp = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(temp / 10000, temp % 10000 / 100, temp % 100);
                        break;
                    case "titlename":
                        TitleName = reader.IsDBNull(i) ? "" : reader["titlename"].ToString();
                        break;
                    case "downcount":
                        DownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                        break;
                    case "avgdowncount":
                        AvgDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["avgdowncount"]);
                        break;
                    case "titleid":
                        TitleID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["titleid"]);
                        break;
                    case "cateid":
                        CateID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["cateid"]);
                        break;
                    case "catename":
                        CateName = reader.IsDBNull(i) ? string.Empty : reader["catename"].ToString();
                        break;
                    case "positionindex":
                        PositionIndex = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["positionindex"]);
                        break;
                    

                }
            }
        }
    }
}
