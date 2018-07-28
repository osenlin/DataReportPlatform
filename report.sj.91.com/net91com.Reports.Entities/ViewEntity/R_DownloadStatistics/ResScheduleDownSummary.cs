using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResScheduleDownSummary
    {
        public ResScheduleDownSummary(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResScheduleDownSummary()
        {

        }

        public string AdPlatformName { get; set; }

        public Res91com.SoftModel.Enums.SoftADTypeOption AdType { get; set; }

        public string SeriesName { get; set; }

        public string SeatTypeName { get; set; }

        public string ConfigSeattypeName { get; set; }

        public int AdCount { get; set; }

        public int DownCount { get; set; }

        public int PriceTotal { get; set; }

        public double SpanHours { get; set; }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "adplatformname":
                        AdPlatformName = reader.IsDBNull(i) ? "" : reader.GetString(i);
                        break;
                    case "seattypename":
                        SeatTypeName = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        break;
                    case "seriesname":
                        SeriesName = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        break;
                    case "f_type":
                        AdType = reader.IsDBNull(i) ? Res91com.SoftModel.Enums.SoftADTypeOption.DEFAULT : (Res91com.SoftModel.Enums.SoftADTypeOption)Convert.ToInt32(reader["f_type"]);
                        break;
                    case "adcount":
                        AdCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["adcount"]);
                        break;
                    case "downcount":
                        DownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                        break;
                    case "pricetotal":
                        PriceTotal = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["pricetotal"]);
                        break;
                    case "configseattypename":
                        ConfigSeattypeName = reader.IsDBNull(i) ? "" : reader["configseattypename"].ToString();
                        break;
                    case "spanhours":
                        SpanHours = reader.IsDBNull(i) ? 0 : Math.Round(float.Parse(reader["spanhours"].ToString()), 1);
                        break; 

                }
            } 
        }
    }
}
