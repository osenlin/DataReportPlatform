using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Util;
using net91com.Stat.Services.sjqd.Entity;
using System.Data.SqlClient;
using net91com.Core.Data;
using System.Data;

namespace net91com.Stat.Services.sjqd
{
    public static class Sjqd_CpuService
    {
        private static string StatDB_MySQL_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        public static List<Sjqd_Lan> GetSjqd_LanList(string lan, string lanName, int beginsize, int pagesize,
                                                     out int count)
        {
            count = 0;
            string datasql = string.Format(@" select *   from Cfg_CPU
                                              where 1=1 {0} {1}  ",
                                           lanName == "" ? "" : " and  E_CPU like '%" + lanName + "%'",
                                           lan == "" ? "" : " and  CPU like '%" + lan + "%'");
            string resultsql1 = string.Format(@"select  ID,CPU,E_CPU
                                               from({2}) as result
                                                order by id 
                                               limit {0} ,{1} ;", beginsize + 1, beginsize + pagesize,
                                              datasql);
            string resultsql2 = string.Format(@"select count(1)
                                                from({0}) as result", datasql);
            string lastsql = resultsql1 + " " + resultsql2;


            List<Sjqd_Lan> list = new List<Sjqd_Lan>();
            using (IDataReader read = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString,lastsql))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_Lan()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Lan = read["CPU"].ToString(),
                            E_Lan = read["E_CPU"].ToString()
                        });
                }
                if (read.NextResult())
                {
                    if (read.Read())
                    {
                        count = Convert.ToInt32(read[0]);
                    }
                }
            }
            return list;
        }

        public static List<Sjqd_Lan> GetSjqd_LanList(string lanName)
        {
            string resultsql1 = string.Format(@"select  ID,CPU,E_CPU
                                               from Cfg_CPU as result
                                               where E_CPU='{0}' ;", lanName);

            List<Sjqd_Lan> list = new List<Sjqd_Lan>();
            using (IDataReader read = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString,resultsql1))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_Lan()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Lan = read["CPU"].ToString(),
                            E_Lan = read["E_CPU"].ToString()
                        });
                }
            }
            return list;
        }

        public static int UpdateLan(Sjqd_Lan Lan)
        {
            string sql = @"update Cfg_CPU set E_CPU=?lan
                           where ID=?ID ";
            var param = new []
                {
                    new MySqlParameter("?ID", Lan.ID),
                    new MySqlParameter("?lan", Lan.E_Lan)
                };
            return MySqlHelper.ExecuteNonQuery(StatDB_MySQL_ConnString, sql, param);
        }
    }
}