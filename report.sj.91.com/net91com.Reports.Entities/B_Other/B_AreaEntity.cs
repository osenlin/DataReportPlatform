using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.B_Other
{
    public class B_AreaEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }

        public string EnShortName { get; set; }

        public B_AreaEntity()
        {
        }

        public B_AreaEntity(IDataReader reader)
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
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["Name"].ToString();
                        break;
                    case "enshortname":
                        EnShortName = reader.IsDBNull(i) ? "" : reader["EnShortName"].ToString();
                        break;
                    case "parentid":
                        ParentID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ParentID"]);
                        break;
                }
            }
        }
    }
}