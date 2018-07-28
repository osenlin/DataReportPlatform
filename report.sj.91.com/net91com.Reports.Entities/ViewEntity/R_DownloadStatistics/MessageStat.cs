using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    /// <summary>
    /// 消息统计实体类
    /// </summary>
    public class MessageStat
    {
        #region 构造函数

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public MessageStat(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MessageStat()
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
                        int m = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatTime = new DateTime(m / 10000, m % 10000 / 100, m % 100);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "projectsource":
                        ProjectSource = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["projectsource"]);
                        break;
                    case "restype":
                        ResType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["restype"]);
                        break;
                    case "resid":
                        ResID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["resid"]);
                        break;
                    case "arrivedcount":
                        ArrivedCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["arrivedcount"]);
                        break;
                    case "readcount":
                        ReadCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["readcount"]);
                        break;
                    case "triggercount":
                        TriggerCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["triggercount"]);
                        break;
                    case "position":
                        Position = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["position"]);
                        break;
                }
            }
        }

        public int GetStatDownByStatType(int i)
        {
            switch (i)
            {
                case 11:
                    return ArrivedCount;
                case 12:
                    return ReadCount;
                case 13:
                    return TriggerCount;
                default:
                    return ArrivedCount;

            }
        }
        /// <summary>
        /// 获取每一列数据
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object GetIndexValue(int i)
        {
            switch (i)
            {
                case 0:
                    return this.StatTime;
                case 1:
                    return this.ArrivedCount;
                case 2:
                    return this.ReadCount;
                case 3:
                    return this.TriggerCount;
                default:
                    return this.StatTime;
            }
        }

        #endregion

        #region 属性


        public DateTime StatTime { get; set; }

        /// <summary>
        /// 位置编号
        /// </summary>
        public int Position { get; set; }

        public int ProjectSource { get; set; }

        public int ResType { get; set; }

        public int ResID { get; set; }

        public int ReadCount { get; set; }

        public int ArrivedCount { get; set; }

        public int TriggerCount { get; set; }

        public int AddDate { get; set; }

        public int Platform { get; set; }

        public string PositionName { get; set; }

        public string ResName { get; set; }


        #endregion
    }
}