using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Sjqd_LoginLog
    {
        public Sjqd_LoginLog()
        {
        }

        public Sjqd_LoginLog(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "softversion":
                        SoftVersion = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        break;
                    case "fromway":
                        Fromway = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        break;
                    case "intimes":
                        LoginTime = reader.IsDBNull(i) ? DateTime.MinValue : reader.GetDateTime(i);
                        break;
                }
            }
        }

        public int SoftID { get; set; }
        public string SoftName { get; set; }
        public string SoftVersion { get; set; }
        public string Fromway { get; set; }
        public DateTime LoginTime { get; set; }
    }
}