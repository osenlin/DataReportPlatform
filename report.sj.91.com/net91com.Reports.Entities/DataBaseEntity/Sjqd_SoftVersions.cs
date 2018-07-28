using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_SoftVersions
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_SoftVersions(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_SoftVersions()
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
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["softid"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "version":
                        Version = reader.IsDBNull(i) ? "" : reader["version"].ToString();
                        break;
                        //case "e_version":
                        //    E_Version = reader.IsDBNull(i) ? "" : reader["e_version"].ToString();
                        //    break;
                    case "hidden":
                        Hidden = reader.IsDBNull(i) ? false : Convert.ToInt32(reader["hidden"]) > 0;
                        break;
                        //case "hiddenforul":
                        //    HiddenForUL = reader.IsDBNull(i) ? false : Convert.ToInt32(reader["hiddenforul"]) > 0;
                        //    break;
                }
            }
        }

        /// <summary>
        /// 自增id(用于编辑修改的)
        /// </summary>
        public int ID { get; set; }

        public int SoftID { get; set; }
        public int Platform { get; set; }
        public string Version { get; set; }
        public string E_Version { get; set; }
        public bool Hidden { get; set; }
        public bool HiddenForUL { get; set; }
    }
}