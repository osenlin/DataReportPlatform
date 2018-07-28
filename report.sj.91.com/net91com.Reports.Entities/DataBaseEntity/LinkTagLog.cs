using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class LinkTagLog
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LinkTagLog()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public LinkTagLog(IDataReader reader)
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
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["name"].ToString();
                        break;
                    case "id":
                        ID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["id"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "pid":
                        PID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["pid"]);
                        break;
                    case "type":
                        IsCategory = reader.IsDBNull(i) ? false : Convert.ToInt32(reader["type"]) == 1;
                        break;
                }
            }

        }

        public int ID { get; set; }

        public string Name { get; set; }

        public int PID { get; set; }

        public int Platform { get; set; }

        public bool IsCategory { get; set; }

    }
}
