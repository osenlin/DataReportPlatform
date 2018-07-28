using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Direct_Config
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Direct_Config(IDataReader reader)
            : this()
        {
            LoadFromDB(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Direct_Config()
        {
        }

        public void LoadFromDB(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "id":
                        ID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "urlname":
                        UrlName = reader.IsDBNull(i) ? "" : reader["urlname"].ToString();
                        break;
                    case "realurl":
                        RealUrl = reader.IsDBNull(i) ? "" : reader["realurl"].ToString();
                        break;
                    case "pid":
                        PID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["pid"]);
                        break;
                }
            }
        }

        //ID
        public int ID { get; set; }

        //软件ID
        public int SoftID { get; set; }

        //Url名称
        public string UrlName { get; set; }

        //真实地址
        public string RealUrl { get; set; }

        public int PID { get; set; }
    }
}