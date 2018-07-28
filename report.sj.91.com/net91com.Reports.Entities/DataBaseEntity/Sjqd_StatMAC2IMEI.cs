using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_StatMAC2IMEI
    {
        public DateTime StatDate;
        public int SoftID;
        public int ChannelID;
        public int NewMacs;
        public int NewUsers1;
        public int NewUsers7;
        public int NewUsers14;

        #region 构造函数

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_StatMAC2IMEI(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_StatMAC2IMEI()
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
                    case "statdate":
                        int tempstatdate = reader.IsDBNull(i) ? 20130101 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(tempstatdate/10000, tempstatdate%10000/100, tempstatdate%100);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["softid"]);
                        break;
                    case "channelid":
                        ChannelID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["channelid"]);
                        break;
                    case "newmacs":
                        NewMacs = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["newmacs"]);
                        break;
                    case "newusers1":
                        NewUsers1 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["newusers1"]);
                        break;
                    case "newusers7":
                        NewUsers7 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["newusers7"]);
                        break;
                    case "newusers14":
                        NewUsers14 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["newusers14"]);
                        break;
                }
            }
        }

        #endregion
    }
}