using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_CooperateCustomers
    {
        #region 构造函数

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_CooperateCustomers(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_CooperateCustomers()
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
                    case "cooperatename":
                        CooperateName = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["softid"]);
                        break;
                }
            }
        }

        #endregion

        #region 属性

        public int ID { get; set; }

        public string CooperateName { get; set; }

        public int SoftID { get; set; }

        #endregion
    }
}