using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core;

namespace net91com.Reports.Tools
{
    /// <summary>
    /// 状态信息数据存取方法
    /// </summary>
    internal class DAEtlStatesHelper
    {
        /// <summary>
        /// 添加状态信息
        /// </summary>
        /// <param name="state"></param>
        public static void AddEtlState(EtlState state)
        {
            string cmdText = @"insert into EtlStates(`Key`,`Value`,`Type`,`AddTime`,`Description`) 
                               select ?Key,?Value,?Type,now(),?Description
                               from dual
                               where not exists(select * from EtlStates where `Key`=?Key);";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Key", state.Key),
                new MySqlParameter("?Value", state.Value),
                new MySqlParameter("?Type", (int)state.Type),
                new MySqlParameter("?Description", state.Description)
            };
            int rowCount = MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
            if (rowCount == 0)
                throw new ToUserException("状态信息已经存在!");
        }

        /// <summary>
        /// 删除状态信息
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteEtlState(int id)
        {
            string cmdText = "delete from EtlStates where `ID`=?ID;";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", id) };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 更新状态信息
        /// </summary>
        /// <param name="state"></param>
        public static void UpdateEtlState(EtlState state)
        {
            string cmdText = @"update EtlStates set `Key`=?Key,`Value`=?Value,`Type`=?Type,`Description`=?Description where `ID`=?ID";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?ID", state.ID),
                new MySqlParameter("?Key", state.Key),
                new MySqlParameter("?Value", state.Value),
                new MySqlParameter("?Type", (int)state.Type),
                new MySqlParameter("?Description", state.Description)
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 获取状态信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static EtlState GetEtlState(int id)
        {
            string cmdText = "select `ID`,`Key`,`Value`,`Type`,`AddTime`,`Description` from EtlStates where `ID`=?ID;";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", id) };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    return BindEtlState(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定分类的状态信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<EtlState> GetEtlStates(EtlStateTypeOptions type, string keyword, int pageIndex, int pageSize, ref int recordCount)
        {
            string where = " where `Type`=?Type";
            if (!string.IsNullOrEmpty(keyword))
                where += " and `Key` like '%" + keyword.Replace("'", "''") + "%'";
            string cmdText = "select count(*) from EtlStates" + where;            
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?Type", (int)type) };
            object result = MySqlHelper.ExecuteScalar(DACommonHelper.ConnectionString, cmdText, parameters);
            recordCount = Convert.ToInt32(result);
            cmdText = "select `ID`,`Key`,`Value`,`Type`,`AddTime`,`Description` from EtlStates" + where + string.Format(" order by `Key` limit {0},{1}", (pageIndex - 1) * pageSize, pageSize);
            List<EtlState> states = new List<EtlState>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    states.Add(BindEtlState(reader));
                }
            }
            return states;
        }

        /// <summary>
        /// 绑定系统信息实体
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static EtlState BindEtlState(MySqlDataReader reader)
        {
            EtlState state = new EtlState();
            state.ID = Convert.ToInt32(reader["ID"]);
            state.Key = reader["Key"].ToString();
            state.Value = reader["Value"].ToString();
            state.AddTime = Convert.ToDateTime(reader["AddTime"]);
            state.Type = (EtlStateTypeOptions)Convert.ToInt32(reader["Type"]);
            state.Description = reader["Description"] == null || reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString();
            return state;
        }
    }
}
