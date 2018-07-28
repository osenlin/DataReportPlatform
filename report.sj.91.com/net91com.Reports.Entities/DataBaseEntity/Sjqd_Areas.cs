using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_Areas
    {
        #region 构造函数

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_Areas(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_Areas()
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
                    case "country":
                        Country = reader.IsDBNull(i) ? "" : reader["country"].ToString();
                        break;
                    case "province":
                        Province = reader.IsDBNull(i) ? "" : reader["province"].ToString();
                        break;
                    case "city":
                        City = reader.IsDBNull(i) ? "" : reader["city"].ToString();
                        break;
                    case "e_country":
                        E_Country = reader.IsDBNull(i) ? "" : reader["e_country"].ToString();
                        break;
                    case "e_province":
                        E_Province = reader.IsDBNull(i) ? "" : reader["e_province"].ToString();
                        break;
                }
            }
        }

        #endregion

        #region 属性

        public int ID { get; set; }

        public string Country { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string E_Country { get; set; }

        public string E_Province { get; set; }

        #endregion
    }
}