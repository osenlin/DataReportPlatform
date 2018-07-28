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
    public class Sjqd_PhoneInfoService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static Sjqd_PhoneInfoService service;
        private string _cachePreviousKey;

        private Sjqd_PhoneInfoService()
        {
        }

        private string BuildCacheKey(params object[] args)
        {
            StringBuilder sbCacheKey = new StringBuilder(_cachePreviousKey);
            foreach (object obj in args)
            {
                sbCacheKey.Append("_");
                sbCacheKey.Append(obj);
            }
            return sbCacheKey.ToString();
        }

        public static Sjqd_PhoneInfoService GetInstance()
        {
            if (service == null)
            {
                service = new Sjqd_PhoneInfoService();
                service._cachePreviousKey = "Sjqd_PhoneInfoService";
            }
            SqlHelper.CommandTimeout = 120;
            return service;
        }

        /// <summary>
        /// 获取手机信息
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="resolution"></param>
        /// <param name="price"></param>
        /// <param name="frequency"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public List<Sjqd_PhoneInfo> GetSjqd_PhoneInfoList(string name, string pixel, string resolution, string frequency,
                                                          string alias)
        {
            var param = new MySqlParameter[5]
                {
                    new MySqlParameter("?pixel",     "%" + pixel + "%"),
                    new MySqlParameter("?resolution","%" + resolution + "%"),
                    new MySqlParameter("?FREQUENCY", "%" + frequency + "%"),
                    new MySqlParameter("?Alias",     "%" + alias + "%"),
                    new MySqlParameter("?name",      "%" + name + "%")
                };


            string sql =
                string.Format(@"select ID, name, userCount, userPercent, pixel, resolution, PRICE, FREQUENCY, Alias
                         from Cfg_PhoneInfo
                         where 1=1 {0} {1} {2} {3} {4}",
                              string.IsNullOrEmpty(pixel) ? "" : (" and pixel like ?pixel"),
                              string.IsNullOrEmpty(resolution) ? "" : (" and resolution like ?resolution"),
                              string.IsNullOrEmpty(frequency) ? "" : (" and FREQUENCY like ?FREQUENCY"),
                              string.IsNullOrEmpty(alias) ? "" : (" and Alias like ?Alias"),
                              string.IsNullOrEmpty(name) ? "" : (" and name like ?name"));


            List<Sjqd_PhoneInfo> list = new List<Sjqd_PhoneInfo>();

            using (MySqlCommand cmd=new MySqlCommand(sql,new MySqlConnection(statdbConn)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                using (IDataReader read = cmd.ExecuteReader())
                {
                    while (read.Read())
                    {
                        Sjqd_PhoneInfo phone = new Sjqd_PhoneInfo();
                        object obj = read["ID"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.ID = Convert.ToInt32(obj);
                        }
                        obj = read["name"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.name = obj.ToString();
                        }
                        obj = read["userCount"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.userCount = Convert.ToInt32(obj);
                        }
                        obj = read["userPercent"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.userPercent = Convert.ToDecimal(obj);
                        }
                        obj = read["pixel"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.pixel = obj.ToString();
                        }
                        obj = read["resolution"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.resolution = obj.ToString();
                        }
                        obj = read["PRICE"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.price = Convert.ToInt32(obj);
                        }
                        obj = read["FREQUENCY"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.frequency = obj.ToString();
                        }
                        obj = read["Alias"];
                        if (obj != null && obj != DBNull.Value)
                        {
                            phone.Alias = obj.ToString();
                        }
                        list.Add(phone);
                    }
                }
            }
 
            return list;
        }

        /// <summary>
        /// 删除一个手机信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int DeletePhoneInfo(int ID)
        {
            string sql = string.Format("delete from Cfg_PhoneInfo where ID={0}", ID);
            return MySqlHelper.ExecuteNonQuery(statdbConn,  sql);
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="phoneInfo"></param>
        /// <returns></returns>
        public int AddUpdatePhoneInfo(Sjqd_PhoneInfo phoneInfo)
        {
            var param = new MySqlParameter[9]
                {
                    new MySqlParameter("?pixel",       phoneInfo.pixel),
                    new MySqlParameter("?resolution", phoneInfo.resolution),
                    new MySqlParameter("?price",      phoneInfo.price),
                    new MySqlParameter("?frequency",  phoneInfo.frequency),
                    new MySqlParameter("?Alias",      phoneInfo.Alias),
                    new MySqlParameter("?ID",         phoneInfo.ID),
                    new MySqlParameter("?name",       phoneInfo.name),
                    new MySqlParameter("?userCount",   phoneInfo.userCount),
                    new MySqlParameter("?userPercent", phoneInfo.userPercent)
                };

             //update Cfg_PhoneInfo
             //                        set pixel=?pixel ,resolution=?resolution,PRICE=?price,FREQUENCY=?frequency,
             //                        Alias=?Alias
             //                        where ID=?ID;

            string sql = @"
                                   
                                     insert into Sjqd_PhoneInfo
                                                 values(?name,?userCount,?userPercent,?pixel,?resolution,?price,?frequency,?Alias)";
            return SqlHelper.ExecuteNonQuery(statdbConn, CommandType.Text, sql);
        }
    }
}