using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.B_Other
{
    public class B_VersionEntity
    {
        public int ID { get; set; }

        public int SoftID { get; set; }

        public int Platform { get; set; }

        public string Version { get; set; }

        public string Alias { get; set; }

        public bool Hidden { get; set; }


        public bool IsStatisticsVersion { get; set; }


        public B_VersionEntity()
        {
        }

        public B_VersionEntity(IDataReader reader) : this()
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
                    case "id":
                        ID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ID"]);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["SoftID"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["Platform"]);
                        break;
                    case "version":
                        Version = reader.IsDBNull(i) ? "" : reader["Version"].ToString();
                        break;
                    case "alias":
                        Alias = reader.IsDBNull(i) ? "" : reader["Alias"].ToString();
                        break;
                }
            }
        }
    }
}