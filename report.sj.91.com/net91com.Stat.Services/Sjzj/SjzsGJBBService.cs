using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core.Util;
using net91com.Stat.Services.Sjzj.Entity;
using System.Data.SqlClient;
using System.Data;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Extensions;

namespace net91com.Stat.Services.Sjzj
{
    public class SjzsGJBBService
    {
        private static string _connectionString = ConfigHelper.GetConnectionString("sjzsDB_ConnectionString");

        #region 固件版本管理(Sjzs__fwVersions)

        /// <summary>
        /// 获取固件版本数据列表
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static List<FwVersions> GetGJBBList(int platform, int pageIndex, int pageSize, out int Count)
        {
            string cmdText = @"                                WITH T AS (
	                                SELECT ROW_NUMBER() OVER(ORDER BY FwVersion ASC) row_id,id,[Platform],FwVersion,E_FwVersion FROM Sjzs__fwVersions {0}
                                )                                SELECT * FROM (SELECT row_id,id,[Platform],FwVersion,E_FwVersion FROM T WHERE row_id BETWEEN @pageIndex AND @pageSize                                UNION ALL                                 SELECT -1,count(0),'','','' FROM T)A ORDER BY row_id ";
            if (platform > 0 && platform != 99)
                cmdText = string.Format(cmdText, " WHERE [Platform]=@Platform");
            else
                cmdText = string.Format(cmdText, string.Empty);

            SqlParameter[] param = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@pageIndex", SqlDbType.Int, 4, (pageIndex - 1)*pageSize + 1),
                    SqlParamHelper.MakeInParam("@pageSize", SqlDbType.Int, 4, pageIndex*pageSize),
                    SqlParamHelper.MakeInParam("@Platform", SqlDbType.Int, 4, platform)
                };
            Count = 0;
            List<FwVersions> list = new List<FwVersions>();
            using (IDataReader read = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmdText, param))
            {
                while (read.Read())
                {
                    if (read["row_id"].ToString() == "-1")
                        Count = read["id"].ToString().ToInt32(0);
                    else
                    {
                        list.Add(new FwVersions()
                            {
                                ID = read["id"].ToString().ToInt32(0),
                                Platform = read["Platform"].ToString().ToEnum<MobileOption>(MobileOption.All),
                                FwVersion =
                                    read["FwVersion"] != null && read["FwVersion"] != DBNull.Value
                                        ? read["FwVersion"].ToString()
                                        : string.Empty,
                                E_FwVersion =
                                    read["E_FwVersion"] != null && read["E_FwVersion"] != DBNull.Value
                                        ? read["E_FwVersion"].ToString()
                                        : string.Empty
                            });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 更新固件版本
        /// </summary>
        /// <param name="gjb"></param>
        /// <returns></returns>
        public static int Update(FwVersions gjb)
        {
            string cmdText = @"UPDATE Sjzs__fwVersions SET E_FwVersion = @E_FwVersion WHERE ID=@id";
            SqlParameter[] param = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@id", SqlDbType.Int, 4, gjb.ID),
                    SqlParamHelper.MakeInParam("@E_FwVersion", SqlDbType.VarChar, 100, gjb.E_FwVersion)
                };
            return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmdText, param);
        }

        #endregion

        #region 设备型号管理(Sjzs__Jixing)

        /// <summary>
        /// 获取设备型号数据列表
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static List<Jixings> GetSBXHList(int platform, int pageIndex, int pageSize, out int Count)
        {
            string cmdText = @"WITH T AS (
                                SELECT ROW_NUMBER() OVER(ORDER BY jixing ASC) row_id,ID,[platform],jixing,E_jixing FROM Sjzs__Jixing {0}
                                )
                                SELECT * FROM (SELECT row_id,ID,[platform],jixing,E_jixing FROM T WHERE row_id BETWEEN @startIndex AND @endIndex
                                UNION ALL
                                SELECT -1,COUNT(0),'','','' FROM T)A ORDER BY row_id";
            if (platform > 0 && platform != 99)
                cmdText = string.Format(cmdText, "WHERE [Platform]=@Platform");
            else
                cmdText = string.Format(cmdText, string.Empty);
            SqlParameter[] param = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@Platform", SqlDbType.Int, 4, platform),
                    SqlParamHelper.MakeInParam("@startIndex", SqlDbType.Int, 4, (pageIndex - 1)*pageSize + 1),
                    SqlParamHelper.MakeInParam("@endIndex", SqlDbType.Int, 4, pageIndex*pageSize)
                };

            Count = 0;
            List<Jixings> list = new List<Jixings>();
            using (IDataReader read = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmdText, param))
            {
                while (read.Read())
                {
                    if (read["row_id"].ToInt32(0) == -1)
                        Count = read["id"].ToInt32(0);
                    else
                    {
                        list.Add(new Jixings()
                            {
                                ID = read["id"].ToInt32(0),
                                Platform = read["platform"].ToString().ToEnum<MobileOption>(MobileOption.All),
                                Jixing =
                                    read["jixing"] != null && read["jixing"] != DBNull.Value
                                        ? read["jixing"].ToString()
                                        : string.Empty,
                                E_Jixing =
                                    read["E_jixing"] != null && read["E_jixing"] != DBNull.Value
                                        ? read["E_jixing"].ToString()
                                        : string.Empty
                            });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 修改设备型号
        /// </summary>
        /// <param name="sbx"></param>
        /// <returns></returns>
        public static int UpdateSBXH(Jixings sbx)
        {
            string cmdText = "UPDATE Sjzs__Jixing SET E_jixing = @E_jixing WHERE ID = @ID";
            SqlParameter[] param = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@E_jixing", SqlDbType.VarChar, 100, sbx.E_Sbxh),
                    SqlParamHelper.MakeInParam("@ID", SqlDbType.Int, 4, sbx.ID)
                };
            return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmdText, param);
        }

        #endregion

        #region 软件版本管理(Sjzs__Versions)

        /// <summary>
        /// 获取平台版本数据列表
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static List<SjzsVersions> GetSoftVersionsList(int platform, int pageIndex, int pageSize, out int Count)
        {
            string cmdText = @"WITH T AS (
                                SELECT ROW_NUMBER() OVER(ORDER BY version ASC) row_id,ID,[platform],version,E_version FROM Sjzs__Versions {0}
                                )
                                SELECT * FROM (SELECT row_id,ID,[platform],version,E_version FROM T WHERE row_id BETWEEN @startIndex AND @endIndex
                                UNION ALL
                                SELECT -1,COUNT(0),'','','' FROM T)A ORDER BY row_id";
            if (platform > 0 && platform != 99)
                cmdText = string.Format(cmdText, "WHERE [Platform]=@Platform");
            else
                cmdText = string.Format(cmdText, string.Empty);
            SqlParameter[] param = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@Platform", SqlDbType.Int, 4, platform),
                    SqlParamHelper.MakeInParam("@startIndex", SqlDbType.Int, 4, (pageIndex - 1)*pageSize + 1),
                    SqlParamHelper.MakeInParam("@endIndex", SqlDbType.Int, 4, pageIndex*pageSize)
                };

            Count = 0;
            List<SjzsVersions> list = new List<SjzsVersions>();
            using (IDataReader read = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmdText, param))
            {
                while (read.Read())
                {
                    if (read["row_id"].ToInt32(0) == -1)
                        Count = read["id"].ToInt32(0);
                    else
                    {
                        list.Add(new SjzsVersions()
                            {
                                ID = read["id"].ToInt32(0),
                                SoftId = 0,
                                Platform = read["platform"].ToString().ToEnum<MobileOption>(MobileOption.All),
                                Version =
                                    read["version"] != null && read["version"] != DBNull.Value
                                        ? read["version"].ToString()
                                        : string.Empty,
                                E_Version =
                                    read["e_version"] != null && read["e_version"] != DBNull.Value
                                        ? read["e_version"].ToString()
                                        : string.Empty
                            });
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 修改平台版本
        /// </summary>
        /// <param name="softVer"></param>
        /// <returns></returns>
        public static int UpdateSoftVersions(SjzsVersions softVer)
        {
            string cmdText = "UPDATE Sjzs__Versions SET E_Version = @E_Version WHERE ID = @ID";
            SqlParameter[] param = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@E_Version", SqlDbType.VarChar, 100, softVer.E_Version),
                    SqlParamHelper.MakeInParam("@ID", SqlDbType.Int, 4, softVer.ID)
                };
            return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmdText, param);
        }

        #endregion

        #region 手机助手操作系统(Sjzs__OsVersions)

        /// <summary>
        /// 获取手机助手操作系统数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static List<SjzsOsVersions> GetOsVersionsList(int pageIndex, int pageSize, out int Count)
        {
            string cmdText = @"WITH T AS ( 
                              SELECT ROW_NUMBER() OVER(ORDER BY OsVersion) row_id,[ID],[OsVersion],[E_OsVersion] FROM dbo.Sjzs__OsVersions WITH(NOLOCK)
                              )
                              SELECT * FROM (
                              SELECT row_id,[ID],[OsVersion],[E_OsVersion] FROM T WHERE row_id BETWEEN @startIndex AND @endIndex
                              UNION ALL
                              SELECT -1,COUNT(0),'','' FROM T)A ORDER BY A.row_id";

            SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter()
                        {
                            ParameterName = "@startIndex",
                            SqlDbType = SqlDbType.Int,
                            Size = 4,
                            Value = (pageIndex - 1)*pageSize + 1
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@endIndex",
                            SqlDbType = SqlDbType.Int,
                            Size = 4,
                            Value = pageIndex*pageSize
                        }
                };
            Count = 0;
            List<SjzsOsVersions> list = new List<SjzsOsVersions>();
            using (IDataReader reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmdText, parms))
            {
                while (reader.Read())
                {
                    if (reader["row_id"].ToInt32() == -1)
                        Count = reader["id"].ToInt32();
                    else
                        list.Add(new SjzsOsVersions()
                            {
                                ID = reader["id"].ToInt32(),
                                OsVersion =
                                    reader["OsVersion"] != null && reader["OsVersion"] != DBNull.Value
                                        ? reader["OsVersion"].ToString()
                                        : string.Empty,
                                E_OsVersion =
                                    reader["E_OsVersion"] != null && reader["E_OsVersion"] != DBNull.Value
                                        ? reader["E_OsVersion"].ToString()
                                        : string.Empty
                            });
                }
            }
            return list;
        }

        /// <summary>
        /// 更新操作系统
        /// </summary>
        /// <param name="os"></param>
        /// <returns></returns>
        public static int Update(SjzsOsVersions os)
        {
            string cmdText = "  UPDATE dbo.Sjzs__OsVersions SET E_OsVersion=@E_OsVersion WHERE ID=@ID";
            SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter()
                        {
                            ParameterName = "@E_OsVersion",
                            SqlDbType = SqlDbType.VarChar,
                            Size = 100,
                            Value = os.E_OsVersion
                        },
                    new SqlParameter() {ParameterName = "@ID", SqlDbType = SqlDbType.Int, Size = 4, Value = os.ID}
                };
            return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmdText, parms);
        }

        #endregion
    }
}