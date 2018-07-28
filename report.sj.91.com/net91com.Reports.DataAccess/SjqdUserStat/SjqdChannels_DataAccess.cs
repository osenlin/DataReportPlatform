using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class SjqdChannels_DataAccess
    {
        private static SjqdChannels_DataAccess instance = null;
        private static readonly object obj = new object();
        protected static string StatDB_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private string _cachePreviousKey;

        public static SjqdChannels_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new SjqdChannels_DataAccess();
                            instance._cachePreviousKey = "SjqdChannels_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取所有渠道 根据渠道商id
        /// </summary>
        /// <param name="customerid"></param>
        /// <returns></returns>
        public List<Sjqd_Channels> GetChannelsByCustomID(int customerid, int softid)
        {
            string cmdText = String.Format(
                    @"SELECT AutoID,`ChannelID`,`SoftID`,`Platform` ,`Name`,`CCID`,`Modulus1`
                        FROM `Cfg_Channels`
                        where `CCID`={0} and `SoftID`={1} order by `Name` desc", customerid, softid);
            List<Sjqd_Channels> channelList = new List<Sjqd_Channels>();
            using (IDataReader dr = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
            {
                while (dr.Read())
                {
                    channelList.Add(new Sjqd_Channels(dr));
                }
            }
            return channelList;
        }

        public Sjqd_Channels GetChannelByName(int softid, int platform, string name)
        {
            string cmdText = string.Format(
                @"SELECT AutoID,`ChannelID`,`SoftID`,`Platform`,`Name`,`CCID`,`Modulus1`
                    FROM `Cfg_Channels`
                    where `SoftID`={0} and `Platform`={1} and `Name`='{2}'", softid, platform, name.Replace("'", "''"));
            Sjqd_Channels c = new Sjqd_Channels();
            using (IDataReader dr = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
            {
                if (dr.Read())
                {
                    c = new Sjqd_Channels(dr);
                }
            }
            return c;
        }


        /// <summary>
        /// 删除渠道id 的绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteChannelBind(int id)
        {
            string cmdText = string.Format(
                @"update Cfg_ChannelsFromClient A inner join Cfg_Channels B on A.AutoID=B.ChannelID and A.Bound=1 and B.AutoID={0} set A.Bound=0;
                  delete from Cfg_Channels where AutoID={0};", id);
            return MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
        }

        /// <summary>
        /// 更新渠道ID相关系数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="m1"></param>
        /// <returns></returns>
        public int UpdateChannelModulus(int id, string name, decimal m1)
        {
            string cmdText = string.Format("update Cfg_Channels set Modulus1={0} where AutoID={1};", m1, id);
            return MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
        }

        /// <summary>
        /// 自定义渠道ID
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="softid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int DefineIsExsit(int platform, int softid, string name)
        {
            string cmdText = string.Format(@"select count(1) from Cfg_Channels   
                        where `Name`='{0}' and SoftID={1} and Platform={2}", name.Replace("'", "''"), softid, platform);
            using (MySqlDataReader dr = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
            {
                if (dr.Read())
                {
                    return Convert.ToInt32(dr[0]);
                }
            }
            return 0;
        }


        /// <summary>
        /// 添加自定义渠道(若渠道表若没有这个数据,添加一条记录并且到fromclient表中若也有这个记录则设置为绑定状态)
        /// </summary>
        /// <param name="customid"></param>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="platform"></param>
        /// <param name="softid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int AddDefineChannel(int customid, decimal m1, decimal m2, int platform, int softid, string name)
        {
            string cmdText = string.Format(@"update Cfg_Channels set Modulus1={0},Modulus2={1}
	                                    where Name='{2}' and SoftID={3} and Platform={4};", m1, m2, name.Replace("'", "''"), softid, platform);
            int rowCount = MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
            if (rowCount == 0)
            {
                cmdText = string.Format(@"insert into Cfg_Channels(SoftID,Platform,`Name`,CCID,Modulus1,Modulus2,ChannelID)
	                                values({0},{1},'{2}',{3},{4},{5},null);", softid, platform, name.Replace("'", "''"), customid, m1, m2);
                rowCount = MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
            }
            if (rowCount > 0)
            {
                cmdText = string.Format(@"update Cfg_Channels A inner join Cfg_ChannelsFromClient B on A.SoftID=B.SoftID and A.Platform=B.Platform and A.Name=B.Name  
                                             and A.SoftID={0} and A.Platform={1} and A.Name='{2}' and B.Bound=0 set A.ChannelID=B.AutoID, B.Bound=1;", softid, platform, name.Replace("'", "''"));
                rowCount = MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
            }
            return rowCount;
        }

        /// <summary>
        /// 设置渠道ids的M1系数
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="m1"></param>
        /// <returns></returns>
        public int UpdateM1ByChannels(List<int> channelids, decimal m1)
        {
            string ids = string.Join(",", channelids.Select(p => p.ToString()).ToArray());
            string cmdText = string.Format(@"update Cfg_Channels set Modulus1={0} where AutoID in ({1}) ;", m1, ids);
            return MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
        }
    }
}