using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Res91com.ResourceDataAccess;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResourceRecommendNoPosition
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public ResourceRecommendNoPosition(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResourceRecommendNoPosition()
        {
        }


        #region 属性


        public string FName { get; set; }

        public int Position { get; set; }

        public int F_ID { get; set; }

        public int TagID { get; set; }
        /// <summary>
        /// 作为关联下载字段的唯一标识符
        /// </summary>
        public int ID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime EditTime { get; set; }

        public int ProjectSource { get; set; }

        public string EditReason { get; set; } 

        public string Memo { get; set; }
        /// <summary>
        /// 下载量
        /// </summary>
        public int DownCount { get; set; }

        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {

                    case "fname":
                        FName = reader.GetString(i);
                        break;
                    case "f_id":
                        F_ID = reader.GetInt32(i);
                        break;
                    case "tagid":
                        TagID = reader.GetInt32(i);
                        break;
                    case "starttime":
                        StartTime = reader.GetDateTime(i);
                        break;
                    case "endtime":
                        EndTime = reader.GetDateTime(i);
                        break;
                    case "editreason":
                        EditReason = reader.GetString(i);
                        break;
                    case "edittime":
                        EditTime = reader.GetDateTime(i);
                        break;
                    case "memo":
                        Memo = reader.GetString(i);
                        break; 
                    case "id":
                        ID = reader.GetInt32(i);
                        break;
                    case "downcount":
                        DownCount =reader.IsDBNull(i)?0:  Convert.ToInt32(reader["downcount"]);
                        break;
                    case "projectsource":
                        ProjectSource = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["projectsource"]));
                        break;
                }
            }
            ///不区分位置
            Position = -1;
        }


        public object GetIndexValue(int index)
        {
            switch (index)
            {
                case 0:
                    return FName;
                case 3:
                    return DownCount;
                case 2://这一列是固定值
                    return 1;
                case 1:
                    return (int)ProjectSource;
                case 4:
                    return StartTime;
                case 5:
                    return EndTime;
                case 6:
                    return EditTime;
                case 7:
                    return EditReason;
                case 8:
                    return Memo;

                default:
                    return FName;
            }
        }
        
    }
}
