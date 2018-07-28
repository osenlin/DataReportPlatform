using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Res91com.ResourceDataAccess;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResourceRecommendDetail
    {
          /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public ResourceRecommendDetail(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResourceRecommendDetail()
        {
        }


        #region 属性


        
        /// <summary>
        /// 作为关联下载字段的唯一标识符
        /// </summary>
        public int ID { get; set; }

        public int F_id { get; set; }

        public int Tagid { get; set; }

        public string ResName { get; set; }

        public string PositionName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime StatDate { get; set; } 

        public string Account { get; set; }
 
        public string Memo { get; set; }

        public double TimeSpanHours { get; set; }
        /// <summary>
        /// 是否是合理的数据
        /// </summary>
        public bool IsRightData { get; set; }

        public Res91com.SoftModel.Enums.SoftADTypeOption ProjectType { get; set; }
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

                    case "statdate":
                        int temp = Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(temp / 10000, temp % 10000 / 100, temp % 100);
                        break; 
                    case "account":
                        Account = reader.GetString(i);
                        break;
                    case "f_id":
                        F_id = reader.GetInt32(i);
                        break;
                    case "starttime":
                        StartTime = reader.GetDateTime(i);
                        break;
                    case "endtime":
                        EndTime = reader.GetDateTime(i);
                        break;
                    case "bindtype":
                        int BindType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["bindtype"]);
                        ProjectType = ((Res91com.SoftModel.Enums.SoftADTypeOption)BindType);
                        break;
                    case "positionname":
                        PositionName = reader.IsDBNull(i) ? "" :  reader.GetString(i);
                        break;
                    case "memo":
                        Memo = reader.GetString(i);
                        break; 
                    case  "tagid":
                        Tagid = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["tagid"]);
                        break;
                }
            }
            TimeSpanHours = (GetTime(StatDate, EndTime, false)-(GetTime(StatDate, StartTime, true))).TotalSeconds*1.0/3600;
        }

        /// <summary>
        /// 获取标准显示时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetTime(DateTime standartTime, DateTime time, bool starttime)
        {
            if (standartTime.Year == time.Year && standartTime.Month == time.Month && standartTime.Day == time.Day)
            {
                return time;
            }
            else if (starttime)
            {
                return standartTime;
            }
            else
            {
                return standartTime.AddDays(1).AddSeconds(-1);
            }
        }

        public object GetIndexValue(int index)
        {
            switch (index)
            {
                case 0:
                    return ResName;
                case 1:
                    return ProjectType;
                case 2:
                    return TimeSpanHours;
                case 3:
                    return Memo;
                default:
                    return ResName;
            }
        }
    }
}
