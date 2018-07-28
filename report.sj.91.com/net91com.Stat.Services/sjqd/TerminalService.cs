using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core.Util;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Core;
using net91com.Core.Web;
using net91com.Core.Extensions;
using System.Data.SqlClient;
using net91com.Core.Data;
using System.Data;
using net91com.Core;

namespace net91com.Stat.Services.sjqd
{
    public class TerminalService
    {
        #region 静态字段

        /// <summary>
        /// 统计数据库连接串
        /// </summary>
        private static string statDBConnectString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");


        /// <summary>
        /// 缓存key前缀
        /// </summary>
        private string _cacheKey;

        #endregion

        private static TerminalService service;

        private TerminalService()
        {
        }

        public static TerminalService GetInstance()
        {
            if (service == null)
            {
                service = new TerminalService();
                service._cacheKey = "TerminalService_";
            }
            SqlHelper.CommandTimeout = 120;
            return service;
        }

        #region 获取分辨率分布统计数据

        /// <summary>
        /// 获取分辨率分布统计数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public List<Resolution> GetResolutions(int softId, int platform, int period, int statdate)
        {
            string dataKey = UtilHelper.BuildCacheKey(_cacheKey, "GetResolutions", softId, platform, period, statdate);
            List<Resolution> list = CacheHelper.Get<List<Resolution>>(dataKey);
            if (list != null)
                return list;

            string cmdText = @" select Resolution,sum(userscount) userCount from
                                 (
	                                SELECT case when b.Resolution='' or b.Resolution is null then 0 else SbxhID end SbxhID,userscount,isnull(b.Resolution,'') Resolution 
	                                FROM 
	                                (
		                                SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount 
		                                FROM Sjqd_StatUsersBySbxh WITH(NOLOCK)
		                                WHERE Period=@period AND StatDate =  @StatDate  AND SoftID=@SoftID AND [Platform]=@Platform
		                                GROUP BY SbxhID
	                                ) A 
	                                LEFT JOIN 
	                                Sjqd_SBXH b WITH(NOLOCK) ON A.SbxhID=b.ID
                                ) A
                                group by Resolution
                                ORDER BY sum(userscount) DESC";
            SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter()
                        {
                            ParameterName = "@period",
                            SqlDbType = System.Data.SqlDbType.TinyInt,
                            Size = 1,
                            Value = period
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@StatDate",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Size = 4,
                            Value = statdate
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@SoftID",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Size = 4,
                            Value = softId
                        },
                    new SqlParameter()
                        {
                            ParameterName = "@Platform",
                            SqlDbType = System.Data.SqlDbType.TinyInt,
                            Size = 1,
                            Value = platform
                        }
                };

            list = new List<Resolution>();
            using (IDataReader read = SqlHelper.ExecuteReader(statDBConnectString, CommandType.Text, cmdText, param))
            {
                while (read.Read())
                {
                    list.Add(new Resolution()
                        {
                            UseCount = Convert.ToInt32(read["usercount"]),
                            ResolutionStr = read["Resolution"].ToString()
                        });
                }
            }
            if (list.Count > 0)
                CacheHelper.Set<List<Resolution>>(dataKey, list, CacheTimeOption.TenMinutes);
            return list;
        }

        #endregion

        /// <summary>
        /// 获取每一天
        /// </summary>
        /// <returns></returns>
        public List<Resolution> GetResolutionsByDates(DateTime begintime, DateTime endtime, int softid, int platform,
                                                      string Resolution)
        {
            if (Resolution == "未适配分辨率")
            {
                Resolution = "";
            }
            string key = UtilHelper.BuildCacheKey("GetResolutionsByDates", begintime, endtime, softid, platform,
                                                  Resolution);


            List<Resolution> list = CacheHelper.Get<List<Resolution>>(key);
            if (list == null)
            {
                string cmdText = @"
                                select @Resolution Resolution,sum(userscount) userCount,StatDate from
                                 (
	                                SELECT case when b.Resolution='' or b.Resolution is null then 0 else SbxhID end SbxhID,userscount,StatDate,
                                    isnull(b.Resolution,'') Resolution 
	                                FROM 
	                                (
		                                SELECT SbxhID,SUM(NewUserCount+ActiveUserCount) userscount ,StatDate
		                                FROM Sjqd_StatUsersBySbxh WITH(NOLOCK)
		                                WHERE Period=@period AND StatDate  between @begintime and @endtime  AND SoftID=@SoftID AND [Platform]=@Platform
		                                GROUP BY SbxhID,StatDate
	                                ) A 
	                                LEFT JOIN 
	                                Sjqd_SBXH b WITH(NOLOCK) ON A.SbxhID=b.ID
                                ) A
                                where Resolution=@Resolution
                                group by StatDate
                                ORDER BY  StatDate asc 
                                ";

                SqlParameter[] param = new SqlParameter[]
                    {
                        new SqlParameter()
                            {
                                ParameterName = "@period",
                                SqlDbType = System.Data.SqlDbType.TinyInt,
                                Size = 1,
                                Value = 1
                            },
                        new SqlParameter()
                            {
                                ParameterName = "@begintime",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Size = 4,
                                Value = begintime.ToString("yyyyMMdd")
                            },
                        new SqlParameter()
                            {
                                ParameterName = "@endtime",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Size = 4,
                                Value = endtime.ToString("yyyyMMdd")
                            },
                        new SqlParameter()
                            {
                                ParameterName = "@SoftID",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Size = 4,
                                Value = softid
                            },
                        new SqlParameter()
                            {
                                ParameterName = "@Platform",
                                SqlDbType = System.Data.SqlDbType.TinyInt,
                                Size = 1,
                                Value = (int) platform
                            },
                        new SqlParameter()
                            {
                                ParameterName = "@Resolution",
                                SqlDbType = System.Data.SqlDbType.NVarChar,
                                Size = 50,
                                Value = Resolution
                            }
                    };

                list = new List<Resolution>();

                using (IDataReader read = SqlHelper.ExecuteReader(statDBConnectString, CommandType.Text, cmdText, param)
                    )
                {
                    while (read.Read())
                    {
                        list.Add(new Resolution()
                            {
                                UseCount = Convert.ToInt32(read["usercount"]),
                                ResolutionStr = read["Resolution"].ToString(),
                                IntStatDate = Convert.ToInt32(read["StatDate"])
                            });
                    }
                }

                if (list.Count > 0)
                    CacheHelper.Set<List<Resolution>>(key, list, CacheTimeOption.TenMinutes);
            }
            return list;
        }
    }
}