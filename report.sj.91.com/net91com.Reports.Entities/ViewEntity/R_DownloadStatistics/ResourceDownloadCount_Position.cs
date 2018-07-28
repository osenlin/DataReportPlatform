using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResourceDownloadCount_Position
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public ResourceDownloadCount_Position(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResourceDownloadCount_Position()
        {
        }

        #region 属性
 
        
 
        public DateTime StatDate { get; set; }

        public int Downsuccesscount { get; set; }

        public int Downcount { get; set; }

        public int Downfailedcount { get; set; }

        public int Setupsuccesscount { get; set; }

        public int Setupfailedcount { get; set; }

        public decimal Downseccesspercent { get; set; }

        public decimal Downfailedpercent { get; set; }

        public decimal SetUpSeccessPercent { get; set; }

        public decimal SetUpFailedPercent { get; set; }

        public int PositionIndex { get; set; }

        public int ResID { get; set; }

        public int RowID { get; set; }

        public string ResName { get; set; }

        public string PositionName { get; set; }
        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {

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
                    case "statdate":
                        int m = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(m / 10000, m % 10000 / 100, m % 100);
                        break;
                    case "positionindex":
                        PositionIndex = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader[i]);
                        break;
                    case  "resid":
                        ResID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader[i]);
                        break;
                   
                }
            }
            int totaldown = Downsuccesscount + Downfailedcount;
            int totalsetup = Setupfailedcount + Setupsuccesscount;
            Downseccesspercent = totaldown == 0 ? -1 : (decimal)(Downsuccesscount * 1.0 / totaldown);
            Downfailedpercent = totaldown == 0 ? -1 : (decimal)(Downfailedcount * 1.0 / totaldown);
            SetUpSeccessPercent = totalsetup == 0 ? -1 : (decimal)(Setupsuccesscount * 1.0 / totalsetup);
            SetUpFailedPercent = totalsetup == 0 ? -1 : (decimal)(Setupfailedcount * 1.0 / totalsetup);
        }

        public object GetIndexValue(int index)
        {
            switch (index)
            {
                case 0:
                    return this.StatDate;
                case 1:
                    return this.Downcount;
                case 2:
                    return this.Downsuccesscount;
                case 3:
                    return this.Downfailedcount;
                case 4:
                    return this.Setupsuccesscount;
                case 5:
                    return this.Setupfailedcount;
                case 6:
                    return this.Downseccesspercent;
                case 7:
                    return this.Downfailedpercent;
                case 8:
                    return this.SetUpSeccessPercent;
                case 9:
                    return this.SetUpFailedPercent;
                default:
                    return this.StatDate;
            }
        }
        public int GetStatDownByStatType(int i)
        {
            switch (i)
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
    }
}
