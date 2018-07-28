using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using net91com.Core;
using net91com.Core.Data;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 渠道相关的数据存取方法
    /// </summary>
    internal class DAChannelsHelper
    {
        #region 获取有权限的渠道ID列表(GetChannelIds)

        /// <summary>
        /// 获取指定用户指定软件指定渠道绑定的渠道ID列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelType"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static List<int> GetChannelIds(int userId, int softId, ChannelTypeOptions channelType, int[] channels)
        {
            string categoryIds = GetUserChannelRightIds(userId, 0, ChannelTypeOptions.Category);
            string customerIds = GetUserChannelRightIds(userId, 0, ChannelTypeOptions.Customer);
            string cmdText;
            bool filterChannels = channels != null && channels.Length > 0;
            string channelsString = filterChannels
                                        ? string.Join(",", channels.Select(a => a.ToString()).ToArray())
                                        : string.Empty;
            if (channelType == 0 || (channelType == ChannelTypeOptions.ChannelID && filterChannels))
            {
                cmdText = string.Format(@"select ID from (
                            select B.ID from Cfg_ChannelCategories A inner join Cfg_ChannelCustomers B on B.CID=A.ID and B.PID=0 and A.ID in ({0})
                            union
                            select ID from Cfg_ChannelCustomers where ID in ({1})) A", categoryIds,
                                        customerIds);
            }
            else if (channelType == ChannelTypeOptions.Category && filterChannels)
            {
                cmdText = string.Format(
                        "select B.ID from Cfg_ChannelCategories A inner join Cfg_ChannelCustomers B on B.CID=A.ID and B.PID=0 and A.ID in ({0}) and A.ID in ({1})",
                        categoryIds, channelsString);
            }
            else if (channelType == ChannelTypeOptions.Customer && filterChannels)
            {
                cmdText = string.Format(
                        "select ID from Cfg_ChannelCustomers where ID in ({0}) and ID in ({1})",
                        customerIds, channelsString);
            }
            else
            {
                return new List<int>();
            }
            customerIds = string.Empty;
            using (IDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                    customerIds += reader["ID"].ToString() + ",";
            }
            if (customerIds == string.Empty)
                return new List<int>();
            customerIds = customerIds.TrimEnd(',');
            customerIds = GetSubCustomerIds(customerIds, true);
            cmdText = string.Format("select distinct ChannelID from Cfg_Channels where CCID in ({0}) and ChannelID is not null", customerIds);
            if (channelType == ChannelTypeOptions.ChannelID)
            {
                cmdText += " and ChannelID in (" + channelsString + ")";
            }
            List<int> rights = new List<int>();
            using (IDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    rights.Add(Convert.ToInt32(reader["ChannelID"]));
                }
            }
            return rights;
        }

        /// <summary>
        /// 获取指定软件指定渠道绑定的渠道ID列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelType"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        public static List<int> GetChannelIds(int softId, ChannelTypeOptions channelType, int[] channels)
        {
            string cmdText = string.Empty;
            bool filterChannels = channels != null && channels.Length > 0;
            string channelsString = filterChannels
                                        ? string.Join(",", channels.Select(a => a.ToString()).ToArray())
                                        : string.Empty;
            if (channelType == 0 || (channelType == ChannelTypeOptions.ChannelID && filterChannels))
            {
                cmdText = string.Format("select distinct ChannelID from Cfg_Channels where SoftID={0} and ChannelID is not null", softId);
                if (channelType == ChannelTypeOptions.ChannelID)
                    cmdText += " and ChannelID in (" + channelsString + ")";
            }
            else if ((channelType == ChannelTypeOptions.Category || channelType == ChannelTypeOptions.Customer) && filterChannels)
            {
                if (channelType == ChannelTypeOptions.Category)
                    cmdText = string.Format("select B.ID,B.PID from Cfg_ChannelCategories A inner join Cfg_ChannelCustomers B on B.CID=A.ID and A.SoftID={0} and B.PID=0 and A.ID in ({1})", softId, channelsString);
                else if (channelType == ChannelTypeOptions.Customer)
                    cmdText = string.Format("select ID,PID from Cfg_ChannelCustomers where SoftID={0} and ID in ({1})", softId, channelsString);
                string customerIds = string.Empty;
                using (IDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
                {
                    while (reader.Read())
                        customerIds += reader["ID"].ToString() + ",";
                }
                if (customerIds == string.Empty)
                    return new List<int>();
                customerIds = customerIds.TrimEnd(',');
                customerIds = GetSubCustomerIds(customerIds, true);
                cmdText = string.Format("select distinct ChannelID from Cfg_Channels where CCID in ({0}) and ChannelID is not null", customerIds);
            }
            else
            {
                return new List<int>();
            }
            List<int> rights = new List<int>();
            using (IDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    rights.Add(Convert.ToInt32(reader["ChannelID"]));
                }
            }
            return rights;
        }

        #endregion

        #region 获取有权限的渠道列表(GetChannels)

        /// <summary>
        /// 获取没带子渠道的渠道列表(全部产品)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Dictionary<int, List<Channel>> GetChannelsWithoutSubChannels(int userId)
        {
            string categoryIds = GetUserChannelRightIds(userId, 0, ChannelTypeOptions.Category);
            string customerIds = GetUserChannelRightIds(userId, 0, ChannelTypeOptions.Customer);
            string cmdText = string.Format(
                    @"select B.ID,B.CID PID,B.Name,0 Platform,2 ChannelType,1 ParentChannelType,B.SoftID from Cfg_ChannelCategories A inner join Cfg_ChannelCustomers B on B.CID=A.ID and B.PID=0 and A.ID in ({0}) 
                      union 
                      select ID,case when PID=0 then CID else PID end PID,Name,0 Platform,2 ChannelType,case when PID=0 then 1 else 2 end ParentChannelType,SoftID from Cfg_ChannelCustomers where ID in ({1})",
                    categoryIds, customerIds);
            Dictionary<int, List<Channel>> channels = new Dictionary<int, List<Channel>>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    int softId = Convert.ToInt32(reader["SoftID"]);
                    if (!channels.ContainsKey(softId))
                        channels[softId] = new List<Channel>();
                    channels[softId].Add(BindChannel(reader));
                }
            }
            return channels;
        }

        /// <summary>
        /// 获取指定用户指定产品的渠道列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <returns></returns>
        public static List<Channel> GetChannels(int userId, int softId)
        {
            string categoryIds = GetUserChannelRightIds(userId, softId, ChannelTypeOptions.Category);
            string customerIds = GetUserChannelRightIds(userId, softId, ChannelTypeOptions.Customer);
            string cmdText = string.Format(@"
                                    select ID from (
                                        select B.ID from Sjqd_ChannelCategories A inner join Cfg_ChannelCustomers B on A.ID=B.CID and B.PID=0 and A.ID in ({0}) 
                                        union
                                        select ID end ParentChannelType from Cfg_ChannelCustomers where ID in ({1})) A", categoryIds, customerIds);
            customerIds = string.Empty;
            using (IDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                    customerIds += reader["ID"].ToString() + ",";
            }
            if (customerIds == string.Empty)
                return new List<Channel>();
            customerIds = customerIds.TrimEnd(',');
            customerIds = GetSubCustomerIds(customerIds, true);
            cmdText = string.Format(@"select ID,0 PID,Name,0 Platform,1 ChannelType,1 ParentChannelType from Cfg_ChannelCategories where ID in ({0})
                                      union all
                                      select distinct ID,case when PID=0 then CID else PID end PID,Name,0 Platform,2 ChannelType,case when PID=0 then 1 else 2 end ParentChannelType from Cfg_ChannelCustomers where ID in ({1})", categoryIds, customerIds);
            List<Channel> channels = new List<Channel>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    channels.Add(BindChannel(reader));
                }
            }
            return channels;
        }

        /// <summary>
        /// 获取指定用户指定产品的渠道列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="includeChannelIds">是否包含渠道ID</param>
        /// <returns></returns>
        public static List<Channel> GetChannels(int softId, bool includeChannelIds)
        {
            string cmdText = string.Format(
                        @"select ID,0 PID,Name,0 Platform,1 ChannelType,1 ParentChannelType from Cfg_ChannelCategories where SoftID={0} 
                         union all 
                         select ID,case when PID=0 then CID else PID end PID,Name,0 Platform,2 ChannelType,case when PID=0 then 1 else 2 end ParentChannelType from Cfg_ChannelCustomers where SoftID={0}", softId);
            if (includeChannelIds)
            {
                cmdText += string.Format(" union all select ChannelID ID,CCID PID,Name,Platform,3 ChannelType,2 ParentChannelType from Cfg_Channels where SoftID={0} and ChannelID is not null", softId);
            }
            List<Channel> channels = new List<Channel>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    channels.Add(BindChannel(reader));
                }
            }
            return channels;
        }

        //过滤不可见字符正则
        private static Regex regNonPrintChars = new Regex("[\x00-\x08\x0A-\x1F\"]");

        /// <summary>
        /// 绑定渠道信息实体
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Channel BindChannel(MySqlDataReader reader)
        {
            Channel channel = new Channel();
            channel.ID = Convert.ToInt32(reader["ID"]);
            string name = reader["Name"].ToString();
            channel.Name = regNonPrintChars.Replace(name, string.Empty);
            channel.Platform = (MobileOption) Convert.ToInt32(reader["Platform"]);
            channel.ParentID = Convert.ToInt32(reader["PID"]);
            channel.ChannelType = (ChannelTypeOptions) Convert.ToInt32(reader["ChannelType"]);
            channel.ParentChannelType = (ChannelTypeOptions) Convert.ToInt32(reader["ParentChannelType"]);
            
            return channel;
        }

        #endregion

        #region 获取指定渠道有权限的用户列表(GetUserIds)

        /// <summary>
        /// 获取指定渠道有权限的用户列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelRight"></param>
        /// <returns></returns>
        public static List<int> GetUserIds(int softId, ChannelRight channelRight)
        {
            string cmdText = string.Format(
                    "select UserID from R_UserChannelRights where SoftID={0} and ChannelType={1} and ChannelID={2}",
                    softId, (int) channelRight.ChannelType, channelRight.ChannelID);
            List<int> userIds = new List<int>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    userIds.Add(Convert.ToInt32(reader["UserID"]));
                }
            }
            return userIds;
        }

        #endregion

        #region 渠道权限相关方法

        /// <summary>
        /// 授予用户特定渠道权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelRights"></param>
        public static void AddUserChannelRights(int userId, int softId, List<ChannelRight> channelRights)
        {
            string cmdText = string.Format("delete from R_UserChannelRights where UserID={0} and SoftID={1};", userId, softId);
            if (channelRights.Count > 0)
            {
                string values = string.Empty;
                foreach (var r in channelRights)
                {
                    values += string.Format("({0},{1},{2},{3}),", userId, softId, (int) r.ChannelType, r.ChannelID);
                }
                values = values.TrimEnd(',');
                cmdText += "insert into R_UserChannelRights(UserID,SoftID,ChannelType,ChannelID) values" + values + ";";
            }
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText);
        }

        /// <summary>
        /// 授予用户特定渠道权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelRight"></param>
        public static void AddUserChannelRight(int userId, int softId, ChannelRight channelRight)
        {
            string cmdText = string.Format(@"insert into R_UserChannelRights(UserID,SoftID,ChannelType,ChannelID)
                                            select {0},{1},{2},{3} from dual where not exists(select * from R_UserChannelRights where UserID={0} and SoftID={1} and ChannelID={3} and ChannelType={2})"
                                           , userId, softId, (int) channelRight.ChannelType, channelRight.ChannelID);
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText);
        }

        /// <summary>
        /// 授予用户特定渠道权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelRight"></param>
        public static void DeleteUserChannelRight(int userId, int softId, ChannelRight channelRight)
        {
            string cmdText = string.Format(
                    "delete from R_UserChannelRights where UserID={0} and SoftID={1} and ChannelID={2} and ChannelType={3}",
                    userId, softId, channelRight.ChannelID, (int) channelRight.ChannelType);
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText);
        }

        /// <summary>
        /// 获取用户资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <returns></returns>
        public static List<ChannelRight> GetUserChannelRights(int userId, int softId)
        {
            string cmdText =  string.Format(
                    "SELECT distinct ChannelType,ChannelID FROM R_UserChannelRights WHERE UserID={0} and SoftID={1}",
                    userId, softId);
            List<ChannelRight> rights = new List<ChannelRight>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    ChannelRight right = new ChannelRight
                        {
                            ChannelID = Convert.ToInt32(reader["ChannelID"]),
                            ChannelType = (ChannelTypeOptions) Convert.ToInt32(reader["ChannelType"])
                        };
                    rights.Add(right);
                }
            }
            return rights;
        }

        /// <summary>
        /// 获取用户资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelType"></param>
        /// <returns></returns>
        private static string GetUserChannelRightIds(int userId, int softId, ChannelTypeOptions channelType)
        {
            string cmdText = string.Format(
                    "SELECT distinct ChannelID FROM R_UserChannelRights WHERE UserID={0} {1} and ChannelType={2}",
                    userId, softId == 0 ? "" : "and SoftID=" + softId.ToString(), (int) channelType);
            StringBuilder result = new StringBuilder();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    result.Append(reader["ChannelID"].ToString() + ",");
                }
            }
            return result.Length == 0 ? "-1" : result.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 获取所有子渠道商ID
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="includeSelf"></param>
        /// <returns></returns>
        private static string GetSubCustomerIds(string pid, bool includeSelf)
        {
            string subCustomerIds = string.Empty;
            string pidString = pid;
            while (true)
            {
                string cmdText = string.Format("select ID from Cfg_ChannelCustomers where PID in ({0})", pidString);
                using (IDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
                {
                    pidString = string.Empty;
                    while (reader.Read())
                        pidString += reader["ID"].ToString() + ",";
                    if (pidString == string.Empty)
                        break;
                    subCustomerIds += pidString.ToString();
                    pidString = pidString.TrimEnd(',');
                }
            }
            if (includeSelf)
                return pid + (subCustomerIds == string.Empty ? string.Empty : "," + subCustomerIds.TrimEnd(','));
            return subCustomerIds.TrimEnd(',');
        }

        #endregion
    }
}