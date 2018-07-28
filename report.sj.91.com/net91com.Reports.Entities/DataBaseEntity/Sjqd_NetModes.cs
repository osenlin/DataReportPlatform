using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_NetModes
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string E_Name { get; set; }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_NetModes(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_NetModes()
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
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["name"].ToString();
                        break;
                    case "e_name":
                        E_Name = reader.IsDBNull(i) ? "" : reader["e_name"].ToString();
                        break;
                }
            }
        }
    }
}