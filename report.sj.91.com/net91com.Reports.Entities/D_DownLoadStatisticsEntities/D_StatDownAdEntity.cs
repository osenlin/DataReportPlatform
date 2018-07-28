using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StatDownAdEntity
    {
        public int Positiontype { get; set; }

        public int Postion { get; set; }
        
        public int PostionIndex { get; set; }

        public int DownCount { get; set; }

        public int DownCountSuccess { get; set; }

        public int InstallSuccess { get; set; }

        public int InstallFail { get; set; }

        public decimal Price { get; set; }

        public string PositionName { get; set; }

        public int ListDownCount { get; set; }

        public int DetailDownCount { get; set; }

        public int Resid { get; set; }

        public string ResIdentifier { get; set; }

        public string ResName { get; set; }

        public int AdDownCount { get; set; }

        public DateTime AdStartTime { get; set; }

        public DateTime AdEndTime { get; set; }

        public int ShowCount { get; set; }

        public D_StatDownAdEntity()
        {
        }

        public D_StatDownAdEntity(IDataReader reader):this()
        {
            LoadFromDb(reader);
        }

        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "positiontype":
                        Positiontype = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["positiontype"]));
                        break;
                    case "downcount":
                        DownCount = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]));
                        break;
                    case "downcountsuccess":
                        DownCountSuccess = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcountsuccess"]));
                        break;
                    case "installsuccess":
                        InstallSuccess = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["installsuccess"]));
                        break;
                    case "installfail":
                        InstallFail = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["installfail"]));
                        break;
                    case "position":
                        Postion = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["position"]));
                        break;
                    case "positionindex":
                        PostionIndex = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["positionindex"]));
                        break;
                    case "price":
                        Price = (reader.IsDBNull(i) ? 0 : Convert.ToDecimal(reader["price"]));
                        break;
                    case "residentifier":
                        ResIdentifier = reader.IsDBNull(i) ? "" :reader["residentifier"].ToString();
                        break;
                    case "resid":
                        Resid = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["resid"]);
                        break;
                    case "resname":
                        ResName = reader.IsDBNull(i) ? "" : reader["resname"].ToString();
                        break;
                    case "addowncount":
                        AdDownCount = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["addowncount"]));
                        break;
                    case "adstarttime":
                        AdStartTime = reader.IsDBNull(i) ? DateTime.MinValue : Convert.ToDateTime(reader["adstarttime"]);
                        break;
                    case "adendtime":
                        AdEndTime = reader.IsDBNull(i) ? DateTime.MinValue : Convert.ToDateTime(reader["adendtime"]);
                        break;
                    case "showcount":
                        ShowCount = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["showcount"]));
                        break;
                            
                }
            }
        }
    }
}
