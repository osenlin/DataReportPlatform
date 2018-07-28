using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Res91com.ResourceDataAccess;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResouceRecommendPositionDetail
    {
           /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public ResouceRecommendPositionDetail(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResouceRecommendPositionDetail()
        {
        }


        #region 属性


        public string FName { get; set; }

        public int F_ID { get; set; }
        /// <summary>
        /// 作为关联下载字段的唯一标识符
        /// </summary>
        public int ID { get; set; }

        public int TagID { get; set; }

        public string Account { get; set; }

        public int DownCount { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime EditTime { get; set; }

        public int ProjectSource { get; set; }

        public string EditReason { get; set; }

        public int Position { get; set; }

        public string Memo { get; set; }

        public string UserName { get; set; }
       
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
                    case "id":
                        ID = Convert.ToInt32(reader["id"]);
                        break;
                    case "downcount":
                        DownCount = reader.IsDBNull(i)?0: Convert.ToInt32(reader["downcount"]);
                        break;
                    case "account":
                        Account = reader["account"].ToString();
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
                    case "position":
                        Position = reader.GetInt32(i);
                        break;
                    case "projectsource":
                        ProjectSource = (reader.IsDBNull(i)?0: Convert.ToInt32(reader["projectsource"]));
                        break;
                    case "username":
                        UserName = reader["username"].ToString();
                        break;
                    case "tagid":
                        TagID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["tagid"]);
                        break;
                }
            } 
        }
    }
}
