using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_Channels
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_Channels(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_Channels()
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
                    case "channelid":
                        ChannelID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["channelid"]);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["softid"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "ccid":
                        CCID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ccid"]);
                        break;
                    case "modulus1":
                        Modulus1 = reader.IsDBNull(i) ? 0 : Convert.ToDecimal(reader["modulus1"]);
                        break;
                    case "modulus_shanzhai":
                        //-1表示继承  0 表示关闭 1 表示开启
                        Modulus_Shanzhai = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["modulus_shanzhai"]);

                        break;
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["name"].ToString();
                        break;
                }
            }
        }

        /// <summary>
        /// 自增id(用于编辑修改的)
        /// </summary>
        public int AutoID { get; set; }

        /// <summary>
        /// 真正的渠道ID(关联其他表的)
        /// </summary>
        public int ChannelID { get; set; }

        public int SoftID { get; set; }

        public int Platform { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 上级渠道商
        /// </summary>
        public int CCID { get; set; }

        public decimal Modulus1 { get; set; }
        //-1表示继承  0 表示关闭 1 表示开启
        public int Modulus_Shanzhai { get; set; }
    }
}