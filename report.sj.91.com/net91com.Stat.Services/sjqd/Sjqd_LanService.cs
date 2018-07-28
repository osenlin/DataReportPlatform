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
    public static class Sjqd_LanService
    {
        private static string StatDB_MySQL_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        public static List<Sjqd_Lan> GetSjqd_LanList(string lan, string lanName, int beginsize, int pagesize,
                                                     out int count)
        {
            count = 0;
            string datasql = string.Format(@" select *   from Cfg_Lan
                                              where 1=1 {0} {1}  ",
                                           lanName == "" ? "" : " and  E_Lan like '%" + lanName + "%'",
                                           lan == "" ? "" : " and  Lan like '%" + lan + "%'");
            string resultsql1 = string.Format(@"select  ID,Lan,E_Lan
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
                            Lan = read["Lan"].ToString(),
                            E_Lan = read["E_Lan"].ToString()
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
            string resultsql1 = string.Format(@"select  ID,Lan,E_Lan
                                               from Cfg_Lan as result
                                               where E_Lan='{0}' ;", lanName);

            List<Sjqd_Lan> list = new List<Sjqd_Lan>();
            using (IDataReader read = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString,resultsql1))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_Lan()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Lan = read["Lan"].ToString(),
                            E_Lan = read["E_Lan"].ToString()
                        });
                }
            }
            return list;
        }

        public static int UpdateLan(Sjqd_Lan Lan)
        {
            string sql = @"update Cfg_Lan set E_Lan=?lan
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