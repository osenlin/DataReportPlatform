using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class SjqdChannelCustomers_DataAccess
    {
        private static SjqdChannelCustomers_DataAccess instance = null;
        private static readonly object obj = new object();
        protected static string StatDB_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private string _cachePreviousKey;

        public static SjqdChannelCustomers_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new SjqdChannelCustomers_DataAccess();
                            instance._cachePreviousKey = "SjqdChannelCustomers_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 添加渠道商(关于山寨机参数在添加时候默认要设置成null的，保证继承关系)
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int AddChannelCustomer(Sjqd_ChannelCustomers customer)
        {
            if (!new URLoginService().AvailableSofts.Exists(a => a.ID == customer.SoftID))
            {
                throw new NotRightException();
            }
            string cmdText = string.Format(@"insert into Cfg_ChannelCustomers(`Name`,IsRealtime,PID,CID,Modulus1,Modulus2,ReportType,AddTime,SoftID) 
                                                values('{0}',{1},{2},{3},{4},{5},{6},now(),{7});
                                             select ID from Cfg_ChannelCustomers where `Name`='{0}' and PID={2} and CID={3};"
                                                            , customer.Name.Replace("'", "''")
                                                            , customer.IsRealtime
                                                            , customer.PID
                                                            , customer.CID
                                                            , customer.Modulus1
                                                            , customer.Modulus2
                                                            , customer.ReportType
                                                            , customer.SoftID);            
            object result = MySqlHelper.ExecuteScalar(StatDB_ConnString, cmdText);
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取渠道商对象根据渠道商id(包括准确的cateid，softid)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sjqd_ChannelCustomers GetChannelCustomer(int id)
        {
            string cmdText = string.Format(@"SELECT `ID`,`Name`,`IsRealtime`,`PID`,`CID`,`Modulus1`,`ReportType`,
                                                ShowType,SoftID,Modulus_Shanzhai,MinViewTime,AddTime
                                         FROM Cfg_ChannelCustomers 
                                         where ID={0}", id);
            using (IDataReader reader = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
            {
                if (reader.Read())
                {
                    return new Sjqd_ChannelCustomers(reader);
                }
            }
            return null;
        }      

        /// <summary>
        /// 获取渠道系数和山寨机真实继承的系数(递归)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sjqd_ChannelCustomers GetCustomerModulus(int id)
        {
            int tempId = id;
            while (true)
            {
                string cmdText = string.Format("select ID,PID,Modulus1 from Cfg_ChannelCustomers where ID={0}", tempId);
                using (IDataReader reader = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
                {
                    if (reader.Read())
                    {
                        Sjqd_ChannelCustomers customer = new Sjqd_ChannelCustomers(reader);
                        if (customer.Modulus1 == 0 && customer.PID > 0)
                        {
                            tempId = customer.PID;
                            continue;
                        }
                        customer.ID = id;
                        return customer;
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// 修改渠道商位置 返回2 表示名称相同的合并了， 返回1 表示没有出现名称相同，只是改了pid
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="targetType"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public int ChangeChannelCustomerPosition(int targetId, int targetType, int sourceId)
        {
            Sjqd_ChannelCustomers sourceCustomer = GetChannelCustomer(sourceId);
            if (sourceCustomer == null)
                throw new NotRightException();

            if (!new URLoginService().AvailableSofts.Exists(a => a.ID == sourceCustomer.SoftID))
                throw new NotRightException();

            string cmdText;
            string customerIds = sourceId.ToString();
            string pid = sourceId.ToString();
            while (true)
            {
                cmdText = string.Format("select ID from Cfg_ChannelCustomers where PID in ({0})", pid);
                using (IDataReader reader = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
                {
                    pid = string.Empty;
                    while (reader.Read())
                        pid += "," + reader["ID"].ToString();
                    if (pid == string.Empty)
                        break;                    
                    customerIds += pid.ToString();
                    pid = pid.TrimStart(',');
                }
            }
            string commonCmdText = @"update Cfg_ChannelCustomers set CID={0} where ID in ({1}) and CID<>{0};";
            int parentId;            
            //移到某分类下
            if (targetType == 1)
            {
                cmdText = string.Format("select ID from Cfg_ChannelCustomers where PID=0 and CID={0} and `Name`='{1}'",
                                        targetId, sourceCustomer.Name.Replace("'", "''"));
                object result = MySqlHelper.ExecuteScalar(StatDB_ConnString, cmdText);
                cmdText = string.Format(commonCmdText, targetId, customerIds);
                if (result == null || result == DBNull.Value)
                {
                    cmdText += string.Format("update Cfg_ChannelCustomers set PID=0 where ID={0} and PID<>0;", sourceId);
                    MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
                    return 1;
                }
                parentId = Convert.ToInt32(result);
            }
            else
            {
                Sjqd_ChannelCustomers targetCustomer = GetChannelCustomer(targetId);
                if (targetCustomer == null || targetCustomer.SoftID != sourceCustomer.SoftID || targetId == sourceId)
                    throw new NotRightException();

                cmdText = string.Format("select ID from Cfg_ChannelCustomers where PID={0} and Name='{1}'", targetId,
                                        sourceCustomer.Name.Replace("'", "''"));
                object result = MySqlHelper.ExecuteScalar(StatDB_ConnString, cmdText);
                cmdText = string.Format(commonCmdText, targetCustomer.CID, customerIds);
                if (result == null || result == DBNull.Value)
                {
                    cmdText += string.Format("update Cfg_ChannelCustomers set PID={1} where ID={0} and PID<>{1};",
                                             sourceId, targetId);
                    MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
                    return 1;
                }
                parentId = Convert.ToInt32(result);
            }
            cmdText += string.Format(@"update Cfg_ChannelCustomers set PID={0} where PID={1};
	                                   update Cfg_Channels set CCID={0} where CCID={1};
                                       delete from Cfg_ChannelCustomers where ID={1}  
                                            and not exists(select * from Cfg_ChannelCustomers where PID={1}) 
                                            and not exists(select * from Cfg_Channels where CCID={1});", parentId,
                                     sourceId);
            MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
            return 2;
        }


        /// <summary>
        /// 根据channelid搜索对应渠道商列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="softid"></param>
        /// <returns></returns>
        public List<int> GetCustomIDsByChannelId(string key, int softid)
        {
            List<int> ids = new List<int>();
            string cmdText = string.Format(@"select CCID from Cfg_Channels where SoftID={0} and `Name` like '%{1}%'", softid, key.Replace("'", "''"));
            using (IDataReader reader = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
            {
                while (reader.Read())
                {
                    ids.Add(Convert.ToInt32(reader[0]));
                }
            }
            return ids;
        }

        /// <summary>
        /// 通过父级渠道ID找出对应的孩子渠道商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Sjqd_ChannelCustomers> GetSjqdCustomersByParentId(int id)
        {
            string cmdText = "SELECT ID,`Name`,IsRealtime,PID,CID,Modulus1,Modulus2,ReportType FROM Cfg_ChannelCustomers where PID=" + id.ToString();
            List<Sjqd_ChannelCustomers> lists = new List<Sjqd_ChannelCustomers>();
            using (IDataReader dr = MySqlHelper.ExecuteReader(StatDB_ConnString, cmdText))
            {
                while (dr.Read())
                {
                    lists.Add(new Sjqd_ChannelCustomers(dr));
                }
            }
            return lists;
        }

        /// <summary>
        /// 删除渠道商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteChannelCustomer(int id)
        {
            string cmdText = string.Format(
                             @"select count(*) from (
                                    select ID from Cfg_ChannelCustomers where PID={0}
                                    union all
                                    select AutoID ID from Cfg_Channels where CCID={0}) A", id);
            object result = MySqlHelper.ExecuteScalar(StatDB_ConnString, cmdText);
            if (Convert.ToInt32(result) == 0)
            {
                cmdText = string.Format("delete from Cfg_ChannelCustomers where ID={0}", id);
                return MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
            }
            return 0;
        }

        /// <summary>
        /// 更新渠道商
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="canUpdateName">能修改名称</param>
        /// <returns></returns>
        public int UpdateChannelCustomer(Sjqd_ChannelCustomers customer, bool canUpdateName = false)
        {
            string cmdText = string.Format(
                    @"update Cfg_ChannelCustomers set {0}Modulus1={1},PID={2},CID={3},ShowType={4},ReportType={5},MinViewTime='{6}' where ID={7}"
                    , canUpdateName ? "`Name`='" + customer.Name.Replace("'", "''") + "'," : ""
                    , customer.Modulus1
                    , customer.PID
                    , customer.CID
                    , customer.ShowType
                    , customer.ReportType
                    , customer.MinViewTime.ToString("yyyy-MM-dd HH:mm:ss")
                    , customer.ID);
            return MySqlHelper.ExecuteNonQuery(StatDB_ConnString, cmdText);
        }

        /// <summary>
        /// 获取同级渠道商和对应名称个数
        /// </summary>
        /// <returns></returns>
        public int GetCustomerCount(int cid, int pid, string name)
        {
            string cmdText = string.Format("select count(1) from Cfg_ChannelCustomers where cid={0} and pid={1} and name='{2}'", cid, pid, name.Replace("'", "''"));
            return Convert.ToInt32(MySqlHelper.ExecuteScalar(StatDB_ConnString, cmdText));
        }
    }
}