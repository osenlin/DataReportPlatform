using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResouceDownCountByCate
    {
        #region 构造函数

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public ResouceDownCountByCate(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResouceDownCountByCate()
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
                        int m = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(m / 10000, m % 10000 / 100, m % 100);
                        break;
                    case "period":
                        Period = reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["period"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0: Convert.ToInt32(reader["platform"]);
                        break;
                    case "projectsource":
                        ProjectSource = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["projectsource"]);
                        break;
                    case "restype":
                        ResType = reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["restype"]);
                        break;
                    case "pcid":
                        PCID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["pcid"]);
                        break;
                    case "cid":
                        CID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["cid"]);
                        break;
                    case "downcount":
                        Downcount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                        break;
                    case "downsuccesscount":
                        Downsuccesscount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downsuccesscount"]);
                        break;
                    case "downfailedcount":
                        Downfailedcount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downfailedcount"]);
                        break;
                    case "setupsuccesscount":
                        Setupsuccesscount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupsuccesscount"]);
                        break;
                    case "setupfailedcount":
                        Setupfailedcount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupfailedcount"]);
                        break;


                }
            }
        }

        #endregion

            public int GetStatDownByStatType(int i)
            {
                switch ( i)
                {
                    case 1:
                        return Downcount;
                    case 4:
                        return Downsuccesscount;
                    case 8:
                        return Downfailedcount;
                    case 5:
                        return Setupsuccesscount;
                    case 6:
                        return Setupfailedcount;
                    default:
                        return Downcount;
                         
                }
            }


        #region 属性


        

        public int Period { get; set; }

        public int ProjectSource { get; set; }

        public int ResType { get; set; }

        public int Downsuccesscount { get; set; }

        public int Downcount { get; set; }

        public int Downfailedcount { get; set; }

        public int Setupsuccesscount { get; set; }

        public int Setupfailedcount { get; set; }

        public DateTime StatDate { get; set; }

        public int Platform { get; set; }

        public int PCID { get; set; }

        public int CID { get; set; }

        #endregion
    }
}
