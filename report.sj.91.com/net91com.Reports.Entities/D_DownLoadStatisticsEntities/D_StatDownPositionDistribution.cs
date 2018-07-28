using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StatDownPositionDistribution
    {
       #region 属性
        public int SoftId { get; set; }

        public DateTime StatDate { get; set; }

        public int Period { get; set; }

        public int Platform { get; set; }

        public int ResType { get; set; }

        public int ProjectSource { get; set; }

        public int VersionId { get; set; }

        public long DownCount { get; set; }

        public long BrowseCount { get; set; }

        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public string PageName { get; set; }


        public string PageType { get; set; }

        public int ByTag { get; set; }
        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public D_StatDownPositionDistribution(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public D_StatDownPositionDistribution()
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
                        int tempdate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(tempdate/10000, tempdate%10000/100, tempdate%100);
                        break;
                    case "restype":
                        ResType = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["restype"]));
                        break;
                    case "period":
                        Period = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["period"]));
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "projectsource":
                        ProjectSource = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["projectsource"]);
                        break;
                    case "versionid":
                        VersionId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["versionid"]);
                        break;
                    case "positionid":
                        PositionId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["positionid"]);
                        break;
                    case "positionname":
                        PositionName = reader.IsDBNull(i) ? "" : reader["positionname"].ToString();
                        break;
                    case "pagename":
                        PageName = reader.IsDBNull(i) ? "" : reader["pagename"].ToString();
                        break;
                    case "pagetype":
                        PageType = reader.IsDBNull(i) ? "列表下载" : reader["pagetype"].ToString();
                        break;
                    case "downcount":
                        DownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downcount"]);
                        break;
                    case "browsecount":
                        BrowseCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["browsecount"]);
                        break;
                    case "bytag":
                        ByTag = Convert.ToBoolean(reader["bytag"])?1:0;
                        break;
                }
            }
        }
    }
}
