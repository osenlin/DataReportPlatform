using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Reports.Entities.DataBaseEntity;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using System.Data;
using net91com.Reports.Entities.ViewEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    /// <summary>
    /// 提供给外部渠道商数据的专用类
    /// </summary>
    public class ChannelCustomUsers_DataAccess : BaseDataAccess
    {
        private static ChannelCustomUsers_DataAccess instance = null;
        private static readonly object obj = new object();

        public static ChannelCustomUsers_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new ChannelCustomUsers_DataAccess();
                            instance._cachePreviousKey = "ChannelCustomUsers";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取外部渠道商用户数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetChannelCustomTables(int channelid, int soft, int platform, DateTime begintime,
                                                DateTime endtime)
        {
            string cmdText = string.Format(@"
select A.StatDate, 
sum(case when Modulus=0 then NewUserCount-isnull(NewUserCount_Shualiang,0) else (NewUserCount-isnull(NewUserCount_Shualiang,0))*Modulus end) as newnum,
sum(case when Modulus=0 then ActiveUserCount else ActiveUserCount*Modulus end) as activenum
,sum(LostUserCount) as lostnum ,sum(TotalUserCount) as  totalnum
from  U_StatChannelUsers A INNER JOIN (
    SELECT ChannelID FROM Cfg_Channels WHERE CCID IN(
        select ID from Cfg_ChannelCustomers where ID=@channelid
        union
        select ID FROM Cfg_ChannelCustomers WHERE PID=@channelid) B
ON A.ChannelID=B.ChannelID
where A.softid=@softid {0} and A.period=1 and A.StatDate between @begindate and @enddate  
Group By A.StatDate order by StatDate desc", platform == 0 ? " and A.Platform<252 " : " and  A.Platform=@platform ");
            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@softid", soft),
                    new MySqlParameter("@platform", platform),
                    new MySqlParameter("@begindate", begintime.ToString("yyyyMMdd")),
                    new MySqlParameter("@enddate", endtime.ToString("yyyyMMdd")),
                    new MySqlParameter("@channelid", channelid)
                };
            return SqlHelper.ExecuteDataset(StatConn, cmdText, parameters).Tables[0];
        }
    }
}