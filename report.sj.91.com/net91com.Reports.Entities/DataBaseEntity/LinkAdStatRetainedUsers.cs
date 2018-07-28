using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class LinkAdStatRetainedUsers
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LinkAdStatRetainedUsers()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public LinkAdStatRetainedUsers(IDataReader reader)
            : this()
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
                    case "originaldate":
                        int date = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["OriginalDate"]);
                        OriginalDate = new DateTime(date / 10000, date / 100 % 100, date % 100);
                        break;
                    case "softid":
                        SoftId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["SoftId"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "adid":
                        AdID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["AdID"]);
                        break;
                    case "newusers":
                        NewUsers = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["NewUsers"]);
                        break;
                    case "c_1":
                        C_1 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["C_1"]);
                        break;
                    case "c_2":
                        C_2 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["C_2"]);
                        break;
                    case "c_3":
                        C_3 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["C_3"]);
                        break;
                    case "c_4":
                        C_4 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["C_4"]);
                        break;
                    case "c_5":
                        C_5 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["C_5"]);
                        break;
                    case "c_6":
                        C_6 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["C_6"]);
                        break;
                    case "c_7":
                        C_7 = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["C_7"]);
                        break;
                }
            }

        }

        public DateTime OriginalDate { get; set; }
        public int SoftId { get; set; }
        public int Platform { get; set; }
        public int AdID { get; set; }
        public string PlanName { get; set; }
        public string UnionName { get; set; }
        public string Keyword { get; set; }
        public string MapMode { get; set; }
        public int NewUsers { get; set; }
        public int C_1 { get; set; }
        public int C_2 { get; set; }
        public int C_3 { get; set; }
        public int C_4 { get; set; }
        public int C_5 { get; set; }
        public int C_6 { get; set; }
        public int C_7 { get; set; }
    }
}
