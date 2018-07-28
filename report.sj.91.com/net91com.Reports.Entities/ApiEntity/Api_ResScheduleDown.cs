using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_ResScheduleDown
    {
         [JsonProperty("code")]
        public int Code { get; set; }
         [JsonProperty("msg")]
        public string Msg { get; set; }
         [JsonProperty("items")]
        public List<ResScheduleDown> Downs = new List<ResScheduleDown>();
    }

    public class ResScheduleDown
    {
        [JsonProperty("showtimeId")]
        public long showtimeId { get; set; }
        [JsonProperty("AdPlatformName")]
        public string AdPlatformName { get; set; }
        [JsonProperty("producttypeName")]
        public string ProductTypeName { get; set; }
        [JsonProperty("softplatform")]
        public string SoftPlatform { get; set; }
        [JsonProperty("seattypeName")]
        public string SeatTypeName { get; set; }
        [JsonProperty("f_id")]
        public string F_id { get; set; }
        [JsonProperty("f_name")]
        public string F_Name { get; set; }
        [JsonProperty("startTime")]
        public string StartTime { get; set; }
        [JsonProperty("endTime")]
        public string EndTime { get; set; }
        [JsonProperty("startDate")]
        public string StartDate { get; set; }
        [JsonProperty("officename")]
        public string OfficeName { get; set; }
        [JsonProperty("statnumber")]
        public int StatNumber { get; set; }
        [JsonProperty("SeriesName")]
        public string SeriesName { get; set; }
        [JsonProperty("AdSeatIndex")]
        public string AdSeatIndex { get; set; }
        public int DownCount { get; set; }

        public string F_Softidentifier { get; set; }

        public Res91com.SoftModel.Enums.SoftADTypeOption AdType { get; set; }

        public double Price { get; set; }

        public double SpanHours { get; set; }

        public ResScheduleDown(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResScheduleDown()
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
                    case "showtimeid":
                        showtimeId = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["showtimeid"]);
                        break;
                    case "adplatformname":
                        AdPlatformName = reader.IsDBNull(i) ? "" : reader.GetString(i);
                        break;
                    case "producttypename":
                        ProductTypeName = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        break;
                    case "softplatform":
                        SoftPlatform = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        break;
                    case "seattypename":
                        SeatTypeName = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        break;
                    case "f_id":
                        F_id = reader.IsDBNull(i) ? "" : reader["f_id"].ToString();
                        break;
                    case "f_name":
                        F_Name = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        break;
                    case "starttime":
                        StartTime = reader.IsDBNull(i) ? string.Empty : reader["starttime"].ToString();
                        break;
                    case "endtime":
                        EndTime = reader.IsDBNull(i) ? string.Empty : reader["endtime"].ToString();
                        break;
                    case "startdate":
                        StartDate = reader.IsDBNull(i) ? string.Empty : reader["startdate"].ToString();
                        break;
                    case "officename":
                        OfficeName = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        break;
                    case "statnumber":
                        StatNumber = reader.IsDBNull(i) ? 0 :Convert.ToInt32(reader["statnumber"]);
                        break;
                    case "seriesname":
                        SeriesName = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        break;
                    case "adseatindex":
                        AdSeatIndex = reader.IsDBNull(i) ? string.Empty : reader["adseatindex"].ToString();
                        break;
                    case "f_type":
                        AdType = reader.IsDBNull(i) ? Res91com.SoftModel.Enums.SoftADTypeOption.DEFAULT : (Res91com.SoftModel.Enums.SoftADTypeOption)Convert.ToInt32(reader["f_type"]);
                        break;
                    case "price":
                        Price = reader.IsDBNull(i) ? 0 : Convert.ToDouble(reader["price"]);
                        break;
                    case "f_softidentifier":
                        F_Softidentifier = reader.IsDBNull(i) ? string.Empty : reader["f_softidentifier"].ToString();
                        break;
                    case "spanhours":
                        SpanHours = reader.IsDBNull(i) ? 0 : Math.Round(float.Parse(reader["spanhours"].ToString()), 1);
                        break; 
                }
            }
            DownCount = StatNumber;
        }
    }
}
