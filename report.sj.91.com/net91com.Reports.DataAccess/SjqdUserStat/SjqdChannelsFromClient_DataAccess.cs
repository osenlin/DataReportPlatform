using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class SjqdChannelsFromClient_DataAccess
    {
        private static SjqdChannelsFromClient_DataAccess instance = null;
        private static readonly object obj = new object();
        protected static string StatConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private string _cachePreviousKey;

        public static SjqdChannelsFromClient_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new SjqdChannelsFromClient_DataAccess();
                            instance._cachePreviousKey = "SjqdChannelsFromClient_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取未绑定的渠道
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="plat"></param>
        /// <returns></returns>
        public List<Sjqd_ChannelFromClient> GetNotBoundChannelFromClientList(int softid)
        {
            string cmdText = string.Format(@"SELECT  AutoID,SoftID,Platform,Name,Bound
                FROM Cfg_ChannelsFromClient where Bound=0 and softid={0} order by name", softid);
            List<Sjqd_ChannelFromClient> channelList = new List<Sjqd_ChannelFromClient>();
            using (IDataReader dr = MySqlHelper.ExecuteReader(StatConn, cmdText))
            {
                while (dr.Read())
                {
                    channelList.Add(new Sjqd_ChannelFromClient(dr));
                }
            }
            return channelList;
        }

        public Sjqd_ChannelFromClient GetChannelFromClientByName(int softid, int platform, string name)
        {
            string sql = string.Format(@"SELECT  AutoID,SoftID,Platform,Name,Bound
                FROM Cfg_ChannelsFromClient where softid={0} and platform={1} and name='{2}'", softid, platform, name.Replace("'", "''"));
            using (IDataReader dr = MySqlHelper.ExecuteReader(StatConn, sql))
            {
                if (dr.Read())
                {
                    return new Sjqd_ChannelFromClient(dr);
                }
            }
            return new Sjqd_ChannelFromClient();
        }


        /// <summary>
        /// 设置一个渠道到对应的渠道商
        /// </summary>
        /// <param name="autoids">来自client 表id</param>
        /// <param name="customid"></param>
        /// <returns></returns>
        public int AddNewChannelFromClient(List<int> ids, int customid)
        {
            string autoids = string.Join(",", ids.Select(p => p.ToString()).ToArray());
            string cmdText = string.Format(@"update Cfg_ChannelsFromClient set Bound=1 where AutoID in ({0});
                                        update Cfg_Channels a inner join Cfg_ChannelsFromClient b on a.platform=b.platform 
                                            and a.softid=b.softid and a.Name=b.Name and b.AutoID in ({0}) 
                                        set a.CCID={1},a.ChannelID=b.AutoID;
                                        insert into Cfg_Channels(SoftID, Platform, Name, CCID, Modulus1, Modulus2, ChannelID)
                                        select A.SoftID,A.Platform,A.Name,{1},0,0,A.AutoID 
                                        from (select * from Cfg_ChannelsFromClient where AutoID in ({0})) A left outer join Cfg_Channels B 
                                        on A.platform=B.platform and A.softid=B.softid and A.Name=B.Name
                                        where B.SoftID is null;", autoids, customid);
            return MySqlHelper.ExecuteNonQuery(StatConn, cmdText);
        }
    }
}