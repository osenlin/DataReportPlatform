using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StatDownCountsByResIDEntity
    {
        public DateTime StatDate { get; set; }

        public int DownCount { get; set; }

        public int ResID { get; set; }

        public int ResType { get; set; }

        public int AuthorID { get; set; }

        public string  AuthorName { get; set; }

        public int ResCount { get; set; }

        public int Rank { get; set; }

        public int LastRank { get; set; }

        public D_StatDownCountsByResIDEntity()
        {
        }

        public D_StatDownCountsByResIDEntity(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        public D_StatDownCountsByResIDEntity(IDataReader reader,int rank)
            : this()
        {
            LoadFromDb(reader);
            Rank = rank;
        }

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
                    case "downcount":
                        DownCount = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]));
                        break;
                    case "resid":
                        ResID = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["resid"]));
                        break;
                    case "restype":
                        ResType = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["restype"]));
                        break;
                    case "authorid":
                        AuthorID = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["authorid"]));
                        break;
                    case "rescount":
                        ResCount = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["rescount"]));
                        break;
                    case "lastrank":
                        LastRank = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["lastrank"]));
                        break;
                    case "rank":
                        Rank = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["rank"]));
                        break; 
                }
            }
        }
    }
}
