using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_ChannelFromClient
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_ChannelFromClient(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_ChannelFromClient()
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
                        AuID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["autoid"]);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["softid"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["name"].ToString();
                        break;
                    case "bound":
                        Bound = reader.IsDBNull(i) ? false : (Convert.ToInt32(reader["bound"]) > 0 ? true : false);
                        break;
                }
            }
        }


        public int AuID { get; set; }

        public int SoftID { get; set; }

        public int Platform { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 是否绑定
        /// </summary>
        public bool Bound { get; set; }
    }
}