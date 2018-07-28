using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class LinkTagCount
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LinkTagCount()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public LinkTagCount(IDataReader reader)
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
                    case "period":
                        Period = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["Period"]);
                        break;
                    case "statdate":
                        int sDate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["StatDate"]);
                        StatDate = new DateTime(sDate / 10000, sDate / 100 % 100, sDate % 100, 0, 0, 0);
                        break;
                    case "softid":
                        SoftId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["SoftId"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "tagid":
                        TagID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["TagID"]);
                        break;
                    case "tagname":
                        TagName = reader.IsDBNull(i) ? "" : reader["TagName"].ToString();
                        break;
                    case "versionid":
                        VersionID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["VersionID"]);
                        break;
                    case "e_version":
                        E_Version = reader.IsDBNull(i) ? "" : reader["E_Version"].ToString();
                        break;
                    case "statcount":
                        StatCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["StatCount"]);
                        break;
                    case "usercount":
                        UserCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["UserCount"]);
                        break;
                }
            }

        }

        public int Period { get; set; }
        public DateTime StatDate { get; set; }
        public int SoftId { get; set; }
        public int Platform { get; set; }
        public int TagID { get; set; }
        public string TagName { get; set; }
        public int VersionID { get; set; }
        public string E_Version { get; set; }
        public int StatCount { get; set; }
        public int UserCount { get; set; }
    }
}
