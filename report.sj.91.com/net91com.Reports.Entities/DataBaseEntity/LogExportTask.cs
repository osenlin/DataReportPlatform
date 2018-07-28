using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Res91com.ResourceDataAccess;
using net91com.Core.Extensions;
using net91com.Core;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class LogExportTask
    {
        #region 构造函数

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public LogExportTask(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LogExportTask()
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
                    case "autoid":
                        AutoID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["autoid"]);
                        break;
                    case "appid":
                        AppID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["appid"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "appversion":
                        AppVersion = reader.IsDBNull(i) ? "" : reader["appversion"].ToString();
                        break;
                    case "begintime":
                        BeginTime = reader.IsDBNull(i) ? DateTime.MinValue : Convert.ToDateTime(reader["begintime"]);
                        break;
                    case "endtime":
                        EndTime = reader.IsDBNull(i) ? DateTime.MaxValue : Convert.ToDateTime(reader["endtime"]);
                        break;
                    case "logtype":
                        LogType = reader.IsDBNull(i) ? "" : reader["logtype"].ToString();
                        break;
                    case "newtablename":
                        NewTableName = reader.IsDBNull(i) ? "" : reader["newtablename"].ToString();
                        break;
                    case "status":
                        Status = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["status"]);
                        break;
                }
            }
        }

        #endregion

        #region 属性

        public int AutoID { get; set; }

        public int AppID { get; set; }

        public int Platform { get; set; }

        public string AppVersion { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public string LogType { get; set; }

        public string NewTableName { get; set; }

        public int Status { get; set; }

        #endregion
    }
}