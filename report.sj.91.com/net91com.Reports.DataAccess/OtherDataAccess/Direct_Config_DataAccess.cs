using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.OtherDataAccess
{
    public class Direct_Config_DataAccess : BaseDataAccess
    {
        private static Direct_Config_DataAccess instance = null;
        private static readonly object obj = new object();

        public static Direct_Config_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Direct_Config_DataAccess();
                            instance._cachePreviousKey = "Direct_Config_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取地址配置列表
        /// </summary>
        /// <param name="softid"></param>
        /// <returns></returns>
        public List<Direct_Config> GetConfigList(int softid)
        {
            string sql = @"select ID, SoftID, UrlName, RealUrl, PID
                              from dbo.Direct_Config with(nolock)
                              where softid=@softid;";

            SqlParameter[] paras =
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid)
                };
            List<Direct_Config> list = new List<Direct_Config>();
            using (IDataReader dr = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, paras))
            {
                while (dr.Read())
                {
                    list.Add(new Direct_Config(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public int AddConfig(Direct_Config dir)
        {
            string sql = @"  
                    declare @mynum int;
                    select @mynum=count(1)  from  Direct_Config where name=@urlname and RealUrl=@realurl and softid=@softid
                    if(@mynum=0)
                    begin
                     INSERT INTO Direct_Config(SoftID, UrlName, RealUrl, PID)
                     values(@softid,@urlname,@realurl,@pid)  select  @@IDENTITY;
                    end
                    else
                    select 0";

            SqlParameter[] paras =
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, dir.SoftID),
                    SqlParamHelper.MakeInParam("@urlname", SqlDbType.VarChar, 100, dir.UrlName),
                    SqlParamHelper.MakeInParam("@realurl", SqlDbType.VarChar, 200, dir.RealUrl),
                    SqlParamHelper.MakeInParam("@pid", SqlDbType.Int, 4, dir.PID),
                };
            int result = 0;
            using (IDataReader dr = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, paras))
            {
                if (dr.Read())
                {
                    result = Convert.ToInt32(dr[0]);
                }
            }
            return result;
        }

        /// <summary>
        /// 更新跳转配置
        /// </summary>
        /// <returns></returns>
        public int UpdateConfig(Direct_Config dir)
        {
            string sql = string.Format(@"update dbo.Direct_Config
                                        set SoftID=@softid, UrlName=@urlname,RealUrl=@realurl,PID=@pid
                                        where ID=@id");
            SqlParameter[] paras =
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, dir.SoftID),
                    SqlParamHelper.MakeInParam("@urlname", SqlDbType.VarChar, 100, dir.UrlName),
                    SqlParamHelper.MakeInParam("@realurl", SqlDbType.VarChar, 200, dir.RealUrl),
                    SqlParamHelper.MakeInParam("@pid", SqlDbType.Int, 4, dir.PID),
                };
            return SqlHelper.ExecuteNonQuery(StatConn, CommandType.Text, sql, paras);
        }

        public int DeleteConfig(Direct_Config dir)
        {
            string sql = @"delete from Direct_Config
                           where ID=@id;";

            SqlParameter[] paras =
                {
                    SqlParamHelper.MakeInParam("@id", SqlDbType.Int, 4, dir.ID),
                };
            return SqlHelper.ExecuteNonQuery(StatConn, CommandType.Text, sql, paras);
        }

        /// <summary>
        /// 获取配置数据通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Direct_Config GetDirect_ConfigByID(int id)
        {
            string sql = string.Format(@"select ID, SoftID, UrlName, RealUrl, PID 
                                         where ID=@id");
            SqlParameter[] paras =
                {
                    SqlParamHelper.MakeInParam("@id", SqlDbType.Int, 4, id)
                };
            Direct_Config dir = null;
            using (IDataReader dr = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, paras))
            {
                if (dr.Read())
                {
                    dir = new Direct_Config(dr);
                }
            }
            return dir;
        }
    }
}