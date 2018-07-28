using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class SjqdChannelCategories_DataAccess : BaseDataAccess
    {
        private static SjqdChannelCategories_DataAccess instance = null;
        private static readonly object obj = new object();

        public static SjqdChannelCategories_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new SjqdChannelCategories_DataAccess();
                            instance._cachePreviousKey = "SjqdChannelCategories_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        ///  编辑分类
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        public int UpdateChannelCategory(Sjqd_ChannelCategories cate)
        {
            string cmdText = string.Format(@"update Cfg_ChannelCategories set Name='{0}' where ID={1}",
                                           cate.Name.Replace("'", "''"), cate.ID);
            return MySqlHelper.ExecuteNonQuery(StatConn, cmdText);
        }

        /// <summary>
        ///  添加分类
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        public int AddChannelCategory(Sjqd_ChannelCategories cate)
        {
            string cmdText =
                string.Format(@"insert into Cfg_ChannelCategories(Name,SoftID,InDate) values('{0}',{1},NOW())",
                              cate.Name.Replace("'", "''"), cate.SoftID);
            return MySqlHelper.ExecuteNonQuery(StatConn, cmdText);
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteChannelCategory(int id)
        {
            string cmdText = string.Format(
                    @"delete from Cfg_ChannelCategories where ID={0} and not exists(select * from Cfg_ChannelCustomers where CID={0})",
                    id);
            return MySqlHelper.ExecuteNonQuery(StatConn, cmdText);
        }

        /// <summary>
        /// 根据渠道分类ID获取分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sjqd_ChannelCategories GetChannelCategory(int id)
        {
            string cmdText = string.Format(@"select ID,Name,SoftID,InDate from Cfg_ChannelCategories where ID={0}", id);
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(StatConn, cmdText))
            {
                if (reader.Read())
                {
                    return new Sjqd_ChannelCategories(reader);
                }
            }
            return null;
        }
    }
}