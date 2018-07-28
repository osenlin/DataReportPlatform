using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_StatUsersByISP
    {
        public DateTime StatDate { get; set; }

        public int Period { get; set; }

        public int SoftID { get; set; }

        public int Platform { get; set; }

        public int IspID { get; set; }

        public string NetMode { get; set; }

        public string IspName { get; set; }

        public int UserCount { get; set; }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_StatUsersByISP(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_StatUsersByISP()
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
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["softid"]);
                        break;
                    case "statdate":
                        int temp = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        StatDate = new DateTime(temp/10000, temp%10000/100, temp%100);
                        break;
                    case "period":
                        Period = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["period"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "ispid":
                        IspID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ispid"]);
                        break;
                    case "netmode":
                        NetMode = reader.IsDBNull(i) ? "0" : reader["netmode"].ToString();
                        break;
                    case "ispname":
                        IspName = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        break;
                    case "usercount":
                        UserCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["usercount"]);
                        break;
                }
            }
        }
    }
}