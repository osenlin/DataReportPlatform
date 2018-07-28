using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StatDownCPAEntity
    {
       #region 属性

        public DateTime StatDate { get; set; }
        public int ResID { get; set; }
        public long DownCount { get; set; }
        public long UserCount { get; set; }
        public long SetupSuccessCount { get; set; }
        public long DownUserCount { get; set; }
        public string ResName { get; set; }
        public string ResIdentifier { get; set; }

        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public D_StatDownCPAEntity(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public D_StatDownCPAEntity()
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
                        int tempdate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(tempdate/10000, tempdate%10000/100, tempdate%100);
                        break;
                    case "resid":
                        ResID = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["resid"]));
                        break;
                    case "downcount":
                        DownCount = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["downcount"]));
                        break;
                    case "usercount":
                        UserCount = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["usercount"]));
                        break;
                    case "setupsuccesscount":
                        SetupSuccessCount = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["setupsuccesscount"]));
                        break;
                    case "downusercount":
                        DownUserCount = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["downusercount"]));
                        break;
                    case "resname":
                        ResName = reader["resname"].ToString();
                        break;
                    case "residentifier":
                        ResIdentifier = reader["residentifier"].ToString();
                        break;
                }
            }
        }
    }
}
