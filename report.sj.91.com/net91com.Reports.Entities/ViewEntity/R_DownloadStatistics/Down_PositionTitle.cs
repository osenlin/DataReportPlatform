using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    /// <summary>
    /// 位置栏目
    /// </summary>
    public class Down_PositionTitle
    {
        public int ID;

        public int ResType;

        public int ProjectSource;

        public int Platform;

        public int CateID;

        public string CateName;

        public string TitleName;

        public int SortIndex;

        public List<int> PositionList;

        public int TagID;
        public List<int> TagList;
        /// <summary>
        /// 临时对应的一个position
        /// </summary>
        public int TempPosition;

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Down_PositionTitle(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Down_PositionTitle()
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
                    case "id":
                        ID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["id"]);
                        break;
                    case "restype":
                        ResType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["restype"]);
                        break;
                    case "projectsource":
                        ProjectSource = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["projectsource"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "cateid":
                        CateID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["cateid"]);
                        break;
                    case "catename":
                        CateName = reader.IsDBNull(i) ? "" : reader["catename"].ToString();
                        break;
                    case "titlename":
                        TitleName = reader.IsDBNull(i) ? string.Empty : reader["titlename"].ToString();
                        break;
                    case "sortindex":
                        SortIndex = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["sortindex"]);
                        break;
                    case "position":
                        TempPosition = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["position"]);
                        break;
                    case "tagid":
                        TagID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["tagid"]);
                        break;

                }
            }
        }
    }
}
