using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StatDownCountsByExtendAttrLst_SUM
    {
    #region
         public int SoftID { get; set; }

        public DateTime StatDate { get; set; }

        public int Period { get; set; }

        public int ResType { get; set; }

        public int Platform { get; set; }

        public int ProjectSource { get; set; }

        public int StatType { get; set; }

        public int DownCount { get; set; }

        public int ProbationDownCount { get; set; }

        public int RainBowDownCount { get; set; }

        public int ProbationAndRainBowDownCount { get; set; }

        public int PayDownCount { get; set; }


        #endregion

         /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public D_StatDownCountsByExtendAttrLst_SUM(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public D_StatDownCountsByExtendAttrLst_SUM()
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
                        SoftID = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["softid"]));
                        break;
                    case "statdate":
                        int tempdate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(tempdate / 10000, tempdate % 10000 / 100, tempdate % 100);
                        break;
                    case "restype":
                        ResType =  (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["restype"]));
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "projectsource":
                        ProjectSource = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["projectsource"]);
                        break;
                    case "period":
                        Period = reader.IsDBNull(i) ? 1 :Convert.ToInt32(reader["period"]);
                        break;
                    case "stattype":
                        StatType = reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["stattype"]);
                        break;
                    case "downcount":
                        DownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                        break;
                    case "probationdowncount":
                        ProbationDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["probationdowncount"]);
                        break;
                    case "rainbowdowncount":
                        RainBowDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["rainbowdowncount"]);
                        break;
                    case "probationandrainbowdowncount":
                        ProbationAndRainBowDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["probationandrainbowdowncount"]);
                        break;
                    case "paydowncount":
                        PayDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["paydowncount"]);
                        break;
                }     
            }
        }
    }
}
