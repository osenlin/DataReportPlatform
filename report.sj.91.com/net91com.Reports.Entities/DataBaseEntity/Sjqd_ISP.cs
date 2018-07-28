using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_ISP
    {
        public int ID { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_ISP(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_ISP()
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
                        ID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["name"].ToString();
                        break;
                }
            }
        }
    }
}