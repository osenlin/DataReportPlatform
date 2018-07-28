using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StatDownRank_SUM
    {
    #region
         public int SoftID { get; set; }

        public DateTime StatDate { get; set; }

        public int Period { get; set; }

        public int ResType { get; set; }

        public string ResName { get; set; }

        public string ResIdentifier { get; set; }

        public int ResID { get; set; }

        public int Platform { get; set; }

        public int ProjectSource { get; set; }

        public int VersionID { get; set; }

        public int ProvinceID { get; set; }

        public int CountryID { get; set; }

        public int PCID { get; set; }

        public int CID { get; set; }

        public int StatType { get; set; }

        public int DownCount { get; set; }

        public int DownCountByUpdating { get; set; }

        public int DownCountBySearching { get; set; }

        public int ScheduleDowncount { get; set; }

        public int Position { get; set; }

        public int RealDownCount { get; set; }

        public int Rank { get; set; }

        public string LastRank { get; set; }

        #endregion

         /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public D_StatDownRank_SUM(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        public D_StatDownRank_SUM(IDataReader reader,int rank)
            : this()
        {
            LoadFromDb(reader);
            Rank = rank;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public D_StatDownRank_SUM()
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
                    case "resid":
                        ResID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["resid"]);
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
                    case "realdowncount":
                        RealDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["realdowncount"]);
                        break;
                    case "downcountbyupdating":
                        DownCountByUpdating = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcountbyupdating"]);
                        break;
                    case "downcountbysearching":
                        DownCountBySearching = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcountbysearching"]);
                        break;
                    case "provinceid":
                        ProvinceID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["provinceid"]);
                        break;
                    case "countryid":
                        CountryID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["countryid"]);
                        break;
                    case "versionid":
                        VersionID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["versionid"]);
                        break;
                    case "pcid":
                        PCID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["pcid"]);
                        break;
                    case "cid":
                        CID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["cid"]);
                        break;
                    case "scheduledowncount":
                        ScheduleDowncount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["countryid"]);
                        break;
                    case "rank":
                        Rank = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["rank"]);
                        break;
                    case "resname":
                        ResName = reader.IsDBNull(i) ? "": reader["resname"].ToString();
                        break;
                    case "residentifier":
                        ResIdentifier = reader.IsDBNull(i) ? "":reader["residentifier"].ToString().ToLower();
                        break;
                }     
            }
        }
    }
}
