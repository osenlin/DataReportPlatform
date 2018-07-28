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
    public static class Sjqd_SBXHService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        public static List<Sjqd_SBXH> GetSjqd_SBXHList(int platform, string esbxh, 
                                                       string sbxh, int beginsize, int pagesize, out int count)
        {
            count = 0;
            string datasql = string.Format(@" select * from Cfg_SBXH
                                            where {0} {1} {2}",
                                           platform == 0 ? "Platform!=?platform" : " Platform=?platform  ",
                                           esbxh == "" ? "" : " and E_SBXH like '%" + esbxh + "%'",
                                           sbxh == "" ? "" : " and SBXH like '%" + sbxh + "%'");
            string resultsql1 = string.Format(@"select  ID,Platform,SBXH,E_SBXH
                                               from({2}) as result
                                                order by id
                                               limit {0} ,{1} ;", beginsize + 1, beginsize + pagesize,
                                              datasql);
            string resultsql2 = string.Format(@"select count(1)
                                               from({0}) as result", datasql);
            string lastsql = resultsql1 + " " + resultsql2;

            var param = new []
                {
                    new MySqlParameter("?Platform",platform)
                };

            List<Sjqd_SBXH> list = new List<Sjqd_SBXH>();
            using (IDataReader read = MySqlHelper.ExecuteReader(statdbConn, lastsql, param))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_SBXH()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Platform = Convert.ToInt32(read["platform"]),
                            SBXH = read["sbxh"].ToString(),
                            E_SBXH = read["e_sbxh"].ToString()
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

        public static List<Sjqd_SBXH> GetSjqd_SBXHList(int platform, List<String> lstsbxh)
        {
            List<Sjqd_SBXH> list = new List<Sjqd_SBXH>();

            string resultsql1 = string.Format(@"select  ID,Platform,SBXH,E_SBXH
                                               from Cfg_SBXH 
                                               where platform={0} {1};", platform,
                                                                                  lstsbxh.Count == 0 ? " and 1=0" : string.Format(" and id in({0})", string.Join(",", lstsbxh.ToArray())));

            using (IDataReader read = MySqlHelper.ExecuteReader(statdbConn,resultsql1))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_SBXH()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Platform = Convert.ToInt32(read["platform"]),
                            SBXH = read["sbxh"].ToString(),
                            E_SBXH = read["e_sbxh"].ToString()
                        });
                }
            }
            return list;
        }

        public static List<Sjqd_SBXH> GetSjqd_SBXHList(int platform, string mobilename, string e_sbxh)
        {
            string resultsql1 = string.Format(@"select  ID,Platform,SBXH,E_SBXH
                                               from Cfg_SBXH as result
                                               where platform={0} {1};", platform,
                                              e_sbxh == "" ? "" : string.Format(" and e_sbxh='{0}'", e_sbxh));

            List<Sjqd_SBXH> list = new List<Sjqd_SBXH>();
            using (IDataReader read = MySqlHelper.ExecuteReader(statdbConn,resultsql1))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_SBXH()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            Platform = Convert.ToInt32(read["platform"]),
                            SBXH = read["sbxh"].ToString(),
                            E_SBXH = read["e_sbxh"].ToString()
                        });
                }
            }
            return list;
        }

        public static int UpdateSBXH(Sjqd_SBXH SBXH)
        {
            string sql = @"update Cfg_SBXH set E_SBXH=?E_SBXH
                           where ID=?ID ";
            var param = new []
                {
                    new MySqlParameter("?ID",          SBXH.ID),
                    new MySqlParameter("?E_SBXH",      SBXH.E_SBXH)
                };
            return MySqlHelper.ExecuteNonQuery(statdbConn,sql, param);
        }
    }
}