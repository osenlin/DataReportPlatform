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
using System.Text.RegularExpressions;
using net91com.Reports.UserRights;

namespace net91com.Stat.Services.sjqd
{
    /// <summary>
    /// 版本信息管理业务接口
    /// </summary>
    public static class Sjqd_SoftVersionsService
    {
        private static string StatDB_MySQL_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        /// <summary>
        /// 获取版本信息列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="versionLike"></param>
        /// <returns></returns>
        public static List<Sjqd_SoftVersions> GetSoftVersions(int softId, int platform, string versionLike = null)
        {
            string cmdText =
                string.Format(
                    @"select ID,Version,Hidden from Cfg_Versions where SoftID={0} and Platform={1} {2}"
                    , softId
                    , platform
                    ,
                    string.IsNullOrEmpty(versionLike)
                        ? string.Empty
                        : " and Version like '%" + versionLike.Replace("'", "''") + "%'");
            List<Sjqd_SoftVersions> list = new List<Sjqd_SoftVersions>();
            using (IDataReader read = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, cmdText))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_SoftVersions()
                        {
                            ID = Convert.ToInt32(read["id"]),
                            SoftID = softId,
                            Platform = platform,
                            Version = read["Version"].ToString(),
                            Hidden = Convert.ToInt16(read["Hidden"]) > 0 ? true : false,
                        });
                }
            }
            return list;
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static Sjqd_SoftVersions GetSoftVersion(int versionId)
        {
            string cmdText =
                string.Format(
                    "select SoftID,Platform,Version,Hidden from Cfg_Versions where ID={0}",
                    versionId);
            using (IDataReader reader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, cmdText))
            {
                if (reader.Read())
                {
                    return new Sjqd_SoftVersions()
                        {
                            ID = versionId,
                            SoftID = Convert.ToInt32(reader["SoftID"]),
                            Platform = Convert.ToInt32(reader["Platform"]),
                            Version = reader["Version"].ToString(),
                            Hidden = Convert.ToInt16(reader["Hidden"]) > 0 ? true : false
                        };
                }
            }
            return null;
        }

        /// <summary>
        /// 删除版本信息
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static int DeleteSoftVersion(int versionId)
        {
            Sjqd_SoftVersions version = GetSoftVersion(versionId);
            if (version == null)
                return 0;

            List<int> softIds = new List<int>();
            //特殊逻辑，91助手海外自动增加
            if (version.SoftID == 46 || version.SoftID == 90003 || version.SoftID == 10101576)
            {
                softIds.Add(46);
                softIds.Add(90003);
                softIds.Add(10101576);
            }
            else
            {
                softIds.Add(version.SoftID);
            }
            int rowCount = 0;
            foreach (int softId in softIds)
            {
                version.SoftID = softId;
                rowCount += DeleteSoftVersionInternal(version);

                int platform = version.Platform;
                switch (version.Platform)
                {
                    case 1:
                        version.Platform = 7;
                        break;
                    default:
                        continue;
                }
                rowCount += DeleteSoftVersionInternal(version);
                version.Platform = platform;
            }
            return rowCount;
        }

        /// <summary>
        /// 删除版本信息
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        private static int DeleteSoftVersionInternal(Sjqd_SoftVersions version)
        {
            string cmdText =
                string.Format(@"delete from Cfg_Versions where SoftID={0} and Platform={1} and Version='{2}'"
                              , version.SoftID
                              , version.Platform
                              , version.Version);
            if (MySqlHelper.ExecuteNonQuery(StatDB_MySQL_ConnString, cmdText) > 0)
            {
                //添加日志
                new URLoginService().AddLog("DeleteSoftVersion"
                                            , string.Format("删除版本(SoftID={0},Platform={1},Version={2})"
                                                            , version.SoftID, version.Platform, version.Version));

                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 更新版本信息
        /// </summary>
        /// <param name="SoftVersion"></param>
        /// <returns></returns>
        public static int UpdateSoftVersion(Sjqd_SoftVersions version)
        {
            Sjqd_SoftVersions version2 = GetSoftVersion(version.ID);
            if (version2 == null)
                return 0;

            version2.Hidden = version.Hidden;
            version2.IsStatisticsVersion = version.IsStatisticsVersion;
            List<int> softIds = new List<int>();
            //特殊逻辑，91助手海外自动增加
            if (version2.SoftID == 46 || version2.SoftID == 90003 || version2.SoftID == 10101576)
            {
                softIds.Add(46);
                softIds.Add(90003);
                softIds.Add(10101576);
            }
            else
            {
                softIds.Add(version2.SoftID);
            }
            int rowCount = 0;
            foreach (int softId in softIds)
            {
                version2.SoftID = softId;
                rowCount += UpdateSoftVersionInternal(version2);
                int platform = version2.Platform;
                switch (version2.Platform)
                {
                    case 1:
                        version2.Platform = 7;
                        break;
                    default:
                        continue;
                }
                rowCount += UpdateSoftVersionInternal(version2);
                version2.Platform = platform;
            }
            return rowCount;
        }

        /// <summary>
        /// 更新版本信息
        /// </summary>
        /// <param name="SoftVersion"></param>
        /// <returns></returns>
        private static int UpdateSoftVersionInternal(Sjqd_SoftVersions version)
        {
            string cmdText =
                string.Format(
                    @"update Cfg_Versions set Hidden={0} where SoftID={1} and Platform={2} and Version='{3}'"
                    , version.Hidden ? 1 : 0
                    , version.SoftID
                    , version.Platform
                    , version.Version);
            if (MySqlHelper.ExecuteNonQuery(StatDB_MySQL_ConnString, cmdText) > 0)
            {
                //添加日志
                new URLoginService().AddLog("UpdateSoftVersion"
                                            , string.Format("修改版本(SoftID={0},Platform={1},Version={2})"
                                                            , version.SoftID, version.Platform, version.Version));

                return 1;
            }
            return 0;
        }

        private static Regex versionRegex = new Regex(@"^\d+(\.\d+){1,3}$");

        /// <summary>
        /// 添加版本信息
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static int AddSoftVersion(Sjqd_SoftVersions version)
        {
            if (!versionRegex.IsMatch(version.Version))
                return 0;

            List<int> softIds = new List<int>();
            //特殊逻辑，91助手海外自动增加
            if (version.SoftID == 46 || version.SoftID == 90003 || version.SoftID == 10101576)
            {
                softIds.Add(46);
                softIds.Add(90003);
                softIds.Add(10101576);
            }
            else
            {
                softIds.Add(version.SoftID);
            }
            int rowCount = 0;
            foreach (int softId in softIds)
            {
                version.SoftID = softId;
                rowCount += AddSoftVersionInternal(version);
                int platform = version.Platform;
                switch (version.Platform)
                {
                    case 1:
                        version.Platform = 7;
                        break;
                    default:
                        continue;
                }
                rowCount += AddSoftVersionInternal(version);
                version.Platform = platform;
            }
            return rowCount;
        }

        /// <summary>
        /// 添加版本信息
        /// </summary>  
        /// <param name="version"></param>
        /// <returns></returns>
        private static int AddSoftVersionInternal(Sjqd_SoftVersions version)
        {
            string cmdText = string.Format(@"
	            insert into Cfg_Versions(`SoftID`,`Platform`,`Version`,`Hidden`) 
                select {0},{1},'{2}',{3} from dual where not exists(
                      select * from Cfg_Versions where SoftID={0} and Platform={1} and  version='{2}');
                select last_insert_id();"
                                           , version.SoftID
                                           , version.Platform
                                           , version.Version
                                           , version.Hidden ? 1 : 0);
            object result = MySqlHelper.ExecuteScalar(StatDB_MySQL_ConnString, cmdText);
            int verId = Convert.ToInt32(result);
            if (verId > 0)
            {
                //添加日志
                new URLoginService().AddLog("AddSoftVersion"
                                            , string.Format("添加版本(SoftID={0},Platform={1},Version={2})"
                                                            , version.SoftID, version.Platform, version.Version));

                return 1;
            }
            return 0;
        }
    }
}