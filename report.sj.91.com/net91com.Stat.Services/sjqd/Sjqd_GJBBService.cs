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
    public static class Sjqd_GJBBService
    {
        private static string StatDB_MySQL_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        public static List<Sjqd_GJBB> GetSjqd_GJBBList(int platform, string gjbb, string gjbbname, int beginsize,
                                                       int pagesize, out int count)
        {
            count = 0;
            string datasql = string.Format(@" select *   from Cfg_GJBB
                                              where {2} {0} {1} ",
                                           gjbb == "" ? "" : " and GJBB like '%" + gjbb + "%'",
                                           gjbbname == "" ? "" : " and E_GJBB like '%" + gjbbname + "%'",
                                           platform == 0 ? " 1=1 " : "  Platform=?Platform ");
            string resultsql1 = string.Format(@"select  ID,Platform,GJBB,E_GJBB
                                               from({2}) as result
                                                order by id
                                               limit {0},{1} ;", beginsize + 1, beginsize + pagesize,
                                              datasql);
            string resultsql2 = string.Format(@"select count(1)
                                                from({0}) as result", datasql);
            string lastsql = resultsql1 + " " + resultsql2;

            var param = new MySqlParameter[]
                {
                    new MySqlParameter("?Platform", platform), 
                };
            List<Sjqd_GJBB> list = new List<Sjqd_GJBB>();
            using (IDataReader read = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, lastsql, param))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_GJBB()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Platform = Convert.ToInt32(read["Platform"]),
                            GJBB = read["gjbb"].ToString(),
                            E_GJBB = read["e_gjbb"].ToString()
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

        public static List<Sjqd_GJBB> GetSjqd_GJBBList(int platform, string gjbbname)
        {
            string resultsql1 = string.Format(@"select  ID,Platform,GJBB,E_GJBB
                                               from Cfg_GJBB as result
                                               where  E_GJBB='{0}'  and  {1};", gjbbname,
                                              platform == 0 ? " 1=1 " : "  Platform=" + platform);

            List<Sjqd_GJBB> list = new List<Sjqd_GJBB>();
            using (IDataReader read = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString,resultsql1))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_GJBB()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Platform = Convert.ToInt32(read["Platform"]),
                            GJBB = read["gjbb"].ToString(),
                            E_GJBB = read["e_gjbb"].ToString()
                        });
                }
            }
            return list;
        }

        public static int UpdateGjbb(Sjqd_GJBB gjbb)
        {
            string sql = @"update Cfg_GJBB set E_GJBB=?gjbb
                           where ID=?ID ";
            var param = new []
                {
                    new MySqlParameter("?ID",  gjbb.ID),
                    new MySqlParameter("?gjbb", gjbb.E_GJBB)
                };
            return MySqlHelper.ExecuteNonQuery(StatDB_MySQL_ConnString,sql, param);
        }
    }
}