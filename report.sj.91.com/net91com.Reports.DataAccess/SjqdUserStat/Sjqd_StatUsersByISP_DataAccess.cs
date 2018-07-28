using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class Sjqd_StatUsersByISP_DataAccess : BaseDataAccess
    {
        private static Sjqd_StatUsersByISP_DataAccess instance = null;

        private static readonly object obj = new object();

        public static Sjqd_StatUsersByISP_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_StatUsersByISP_DataAccess();
                            instance._cachePreviousKey = "Sjqd_StatUsersByISP_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        public List<Sjqd_StatUsersByISP> GetStatUsersByISP(DateTime begintime, DateTime endtime, int softid,
                                                           int platform, int period, string ispName, string netmode)
        {
            string sql = string.Format(@"select statdate,
                            ?softid softid,?period period ,?platform platform,SUM(usercount) usercount,
                            ?netmode netmode,?ispname IspName
                            from U_StatUsersByISP A 
                            inner join Cfg_ISP B
                            on A.IspID=B.id
                            and A.softid=?softid {0} and A.statdate between ?begintime and ?endtime {1}
                            and A.period=?period 
                            inner join Cfg_NetModes C
                            on C.ID=A.netmode {2}
                            group by statdate
                            order by statdate desc",
                                       platform == 0 ? "" : " and A.platform=?platform ",
                                       ispName == "所有运营商" ? "" : " and B.E_Name=?ispname ",
                                       netmode == "所有网络" ? "" : " and C.E_Name=?netmode ");

            MySqlParameter[] paras =
                {
                    new MySqlParameter("?begintime", begintime.ToString("yyyyMMdd")),
                    new MySqlParameter("?endtime", endtime.ToString("yyyyMMdd")),
                    new MySqlParameter("?netmode", netmode),
                    new MySqlParameter("?ispname", ispName),
                    new MySqlParameter("?platform", platform),
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?period", period)
                };
            List<Sjqd_StatUsersByISP> result = new List<Sjqd_StatUsersByISP>();
            using (var reader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, paras))
            {
                while (reader.Read())
                {
                    result.Add(new Sjqd_StatUsersByISP(reader));
                }
            }
            return result;
        }

        /// <summary>
        /// 获取多表头table
        /// </summary>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="ispid"></param>
        /// <param name="netmode"></param>
        /// <returns></returns>
        public DataTable GetStatUsersByISPTable(DateTime begintime, DateTime endtime, int softid, int platform,
                                                int period, List<string> ispid, List<string> netmode)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ispid.Count; i++)
            {
                for (int j = 0; j < netmode.Count; j++)
                {
                    if (netmode[j] == "所有网络" && ispid[i] == "所有运营商")
                    {
                        sb.AppendFormat(" sum(UserCount) '{0}',",
                                        ispid[i] + "_" + netmode[j]);
                    }
                    else
                    {
                        sb.AppendFormat(" sum(case when  {0} {3} {1}  then UserCount else 0 end) '{2}',",
                                        netmode[j] == "所有网络" ? "" : string.Format(" C.E_Name='{0}'", netmode[j]),
                                        ispid[i] == "所有运营商" ? "" : string.Format(" B.E_Name='{0}' ", ispid[i]),
                                        ispid[i] + "_" + netmode[j],
                                        (netmode[j] == "所有网络" || ispid[i] == "所有运营商") ? "" : " and ");
                    }
                }
            }

            string sql = string.Format(@"select statdate '日期',
                            {1}
                            from U_StatUsersByISP A 
                            inner join Cfg_ISP B
                            on A.IspID=B.id
                            and A.softid=?softid  and A.statdate between ?begintime and ?endtime {0}
                            and A.period=?period 
                            inner join Cfg_NetModes C
                            on C.ID=A.netmode  
                            group by statdate
                            order by statdate desc",
                                       platform == 0 ? "" : " and A.platform=?platform ",
                                       sb.ToString().TrimEnd(','));
            MySqlParameter[] paras =
                {
                    new MySqlParameter("?begintime", begintime.ToString("yyyyMMdd")),
                    new MySqlParameter("?endtime", endtime.ToString("yyyyMMdd")),
                    new MySqlParameter("?platform", platform),
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?period", period)
                };

            using (MySqlConnection conn = new MySqlConnection(Mysql_Statdb_Connstring))
            {
                conn.Open();
                return MySqlHelper.ExecuteDataset(conn, sql, paras).Tables[0];
            }
        }
    }
}