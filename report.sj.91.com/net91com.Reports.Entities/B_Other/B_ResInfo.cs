using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.B_Other
{
    public class B_ResInfo
    {
        public int AreaType { get; set; }

        public int ResType { get; set; }

        public int ResId { get; set; }

        public string ResName { get; set; }

        public string ResIdentifier { get; set; }

        public int Pcid { get; set;}

        public int Cid { get; set; }

        public int AuthorId { get; set; }

        public int Flag { get; set; }

        public int Platform { get; set; }

        public B_ResInfo(IDataReader reader)
        {
            LoadInfo(reader);
        }

        public void LoadInfo(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {

                    case "areatype":
                        AreaType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["AreaType"]);
                        break;
                    case "restype":
                        ResType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ResType"]);
                        break;
                    case "resid":
                        ResId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ResId"]);
                        break;
                    case "resname":
                        ResName = reader.IsDBNull(i) ? "" : reader["ResName"].ToString();
                        break;
                    case "residentifier":
                        ResIdentifier = reader.IsDBNull(i) ? "" : reader["ResIdentifier"].ToString().ToLower();
                        break;
                    case "authorid":
                        AuthorId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["authorid"]);
                        break;
                    case "pcid":
                        Pcid = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["Pcid"]);
                        break;
                    case "cid":
                        Cid = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["Cid"]);
                        break;
                    case "flag":
                        Flag = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["Flag"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["Platform"]);
                        break;
                }
            }
        }


    }
}
