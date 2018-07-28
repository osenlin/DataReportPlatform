using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class Sjqd_StatMac2Imei_DataAccess : BaseDataAccess
    {
        private static Sjqd_StatMac2Imei_DataAccess instance = null;
        private static readonly object obj = new object();

        public static Sjqd_StatMac2Imei_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_StatMac2Imei_DataAccess();
                            instance._cachePreviousKey = "Sjqd_StatMac2Imei_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        public List<Sjqd_StatMAC2IMEI> GetStatMAC2IMEI(string channels, DateTime begintime, DateTime endtime)
        {
            string cmdText = @"
 select A.StatDate,A.ChannelID,SUM(NewMacs) NewMacs,SUM(NewUsers1) NewUsers1,SUM(NewUsers7) NewUsers7,SUM(NewUsers14) NewUsers14
 from Sjqd_StatMAC2IMEI A with(nolock)
 where A.StatDate between " + begintime.ToString("yyyyMMdd") + @" and " + endtime.ToString("yyyyMMdd") + @"
  and A.ChannelID in (" + channels + @")
 group by A.StatDate,A.ChannelID
 order by A.StatDate";
            List<Sjqd_StatMAC2IMEI> result = new List<Sjqd_StatMAC2IMEI>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, cmdText))
            {
                while (reader.Read())
                {
                    result.Add(new Sjqd_StatMAC2IMEI(reader));
                }
            }
            return result;
        }
    }
}