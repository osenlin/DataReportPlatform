using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.B_Other
{
    public class B_ResCateEntity
    {
        public int ResType { get; set; }

        public int CID { get; set; }

        public int PCID { get; set; }

        public string CName { get; set; }

        public B_ResCateEntity()
        {
        }

        public B_ResCateEntity(IDataReader reader) : this()
        {
            LoadFromDb(reader);
        }

        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {

                    case "restype":
                        ResType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ResType"]);
                        break;
                    case "cname":
                        CName = reader.IsDBNull(i) ? "" : reader["CName"].ToString();
                        break;
                    case "cid":
                        CID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["CID"]);
                        break;
                    case "pcid":
                        PCID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["PCID"]);
                        break;
                }
            }
        }
    }
}
