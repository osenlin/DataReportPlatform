using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;

namespace net91com.Reports.DataAccess.D_DownloadStat
{
    public class D_StatDownPositionDistribution_DataAccess:BaseDataAccess
    {
        protected static string commputEnConn = ConfigHelper.GetConnectionString("ComputingDB_En_ConnString");
        protected static string commputConn = ConfigHelper.GetConnectionString("ComputingDB_CN_ConnString");
    
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistribution(int restype, int softid, int platform, DateTime begintime, DateTime endtime, int ProjectSource, string versionid, int isupdate, int period, string areaid, int IsDiffPageType = 0, int stattype = 1)
        {
            //0不包含更新，1包含更新
            string showdowntype = isupdate == 0 ? " and B.DownType in (1,3) " : " and B.DownType in (1,2,3,4) ";
            string sql = string.Format(@"
                                select * from (
                                SELECT A.SoftID,Period
                                      ,Platform,A.ResType,A.ProjectSource
                                      ,StatType,max(PositionID) PositionID
                                      ,sum(DownCount) DownCount,
                                B.PageName {1},B.Name PositionName,0 ByTag
                                FROM {2} A  
                                inner join Cfg_DownPositions B  
	                                on B.Position=A.PositionID 
	                                and B.ProjectSource=A.ProjectSource
	                                and B.ResType=A.ResType
                                    and B.SoftID=A.SoftID and B.ByTag=0
                                where   A.SoftID={5} and PLATFORM={6} and A.ResType={7}  and A.StatType={8}
                                      and Period={9} and VersionID in ('{4}') and A.ProjectSource={10} {0} 
                                      and StatDate between {11} and {12} {3}
                                group by A.SoftID,Period,Platform,A.ResType,A.ProjectSource
                                      ,StatType,B.PageName{1},B.Name
                                union all 
                                SELECT A.SoftID,Period
                                      ,Platform,A.ResType,A.ProjectSource
                                      ,StatType,-1 PositionID
                                      ,sum(DownCount) DownCount,
                                B.PageName {1},'' PositionName,1 ByTag
                                FROM {2} A  
                                inner join Cfg_DownPositions B  
	                                on B.Position=A.PositionID 
	                                and B.ProjectSource=A.ProjectSource
	                                and B.ResType=A.ResType
                                    and B.SoftID=A.SoftID and B.ByTag=1
                                where A.SoftID={5} and PLATFORM={6} and A.ResType={7}  and A.StatType={8}  
                                      and Period={9} and VersionID in ('{4}') and A.ProjectSource={10} {0} 
                                      and StatDate between {11} and {12} {3}
                                group by A.SoftID,Period,Platform,A.ResType,A.ProjectSource
                                      ,StatType,B.PageName{1}) A
                                order by DownCount desc"
                                        , showdowntype
                                        , IsDiffPageType == 0 ? " " : ",B.PageType "
                                        , areaid == "-1" ? "D_StatDownPositionDistribution" : "D_StatDownPositionDistributionByArea"
                                        , areaid == "-1" ? "" : string.Format(" and areaid='{0}' ",areaid)
                                        , versionid.Replace(",", "','"), softid, platform, restype, stattype, period, ProjectSource, begintime.ToString("yyyyMMdd"), endtime.ToString("yyyyMMdd"));

            
            List<D_StatDownPositionDistribution> lists = new List<D_StatDownPositionDistribution>();
            using (MySqlCommand cmd=new MySqlCommand(sql,new MySqlConnection(Mysql_Statdb_Connstring)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 300;
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lists.Add(new D_StatDownPositionDistribution(dataReader));
                    }
                }
            }

            return lists;
        }

        //获取顶层专辑类的日期明细
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionByTagClassDetail(int restype,
                                                                                                      int softid,
                                                                                                      int platform,
                                                                                                      DateTime begintime,
                                                                                                      DateTime endtime,
                                                                                                      int ProjectSource,
                                                                                                      string versionid,
                                                                                                      int isupdate,
                                                                                                      int period,
                                                                                                      string pagename,
                                                                                                      string areaid,
                                                                                                      int IsDiffPageType = 0,
                                                                                                      int stattype = 1)
        {
            //IsDiffPageType 0不区分页面类型
            //0不包含更新，1包含更新
            string showdowntype = isupdate == 0 ? " and B.DownType in (1,3)  " : " and B.DownType in (1,2,3,4) ";
            string sql = string.Format(@"
                                SELECT StatDate,sum(DownCount) DownCount
                                FROM {1} A  
                                inner join Cfg_DownPositions B 
	                                on B.Position=A.PositionID 
	                                and B.ProjectSource=A.ProjectSource
	                                and B.ResType=A.ResType  and B.ByTag=1
                                    and B.SoftID=A.SoftID
                                where  A.SoftID={4} and PLATFORM={5} and A.ResType={6}   and A.StatType={7} 
                                      and Period={8} and VersionID in ('{3}') and A.ProjectSource={9} {0} 
                                      and StatDate between {10} and {11} and B.pagename='{12}'  {2}
                                group by StatDate,A.ProjectSource,A.ResType,A.SoftID,A.Period,A.Platform
                                order by StatDate desc"
                                            , showdowntype
                                            , areaid == "-1" ? "D_StatDownPositionDistribution" : "D_StatDownPositionDistributionByArea"
                                            , areaid == "-1" ? "" : string.Format(" and areaid='{0}'",areaid)
                                            , versionid.Replace(",", "','")
                                            ,softid,platform,restype,stattype,period,ProjectSource,begintime.ToString("yyyyMMdd")
                                            ,endtime.ToString("yyyyMMdd"),pagename);

            List<D_StatDownPositionDistribution> lists = new List<D_StatDownPositionDistribution>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring,sql))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionByTag(int restype, int softid,
                                                                                           int platform,
                                                                                           DateTime begintime,
                                                                                           DateTime endtime,
                                                                                           int ProjectSource,
                                                                                           string versionid, int isupdate,
                                                                                           int period, string pagename,
                                                                                           string pagetype,
                                                                                           string areaid,
                                                                                           int IsDiffPageType = 0,
                                                                                           int stattype = 1)
        {
            //IsDiffPageType 0不区分页面类型
            //0不包含更新，1包含更新
            string showdowntype = isupdate == 0 ? " and B.DownType in (1,3)  " : " and B.DownType in (1,2,3,4) ";
            string sql = string.Format(@"
                                SELECT PositionID,B.Name PositionName,B.PageName {2},sum(DownCount) DownCount
                                FROM {3} A 
                                inner join Cfg_DownPositions B  
	                                on B.Position=A.PositionID 
	                                and B.ProjectSource=A.ProjectSource
	                                and B.ResType=A.ResType  and B.ByTag=1
                                    and B.SoftID=A.SoftID
                                where   A.SoftID=?softid and PLATFORM=?platform and A.ResType=?restype   and A.StatType=?stattype 
                                      and Period=?period and VersionID in ('{5}') and A.ProjectSource=?projectsource {0} and StatDate between ?begindate and ?enddate 
                                      and B.pagename=?pagename  {1}   {4} 
                                group by PositionID,B.PageName {2},B.Name,A.ProjectSource,A.ResType,A.SoftID,A.Period,A.Platform
                                order by DownCount desc"
                                        , showdowntype, pagetype == "" ? "" : " and B.PageType=?pagetype "
                                        , IsDiffPageType == 0 ? " " : ",B.PageType "
                                        , areaid == "-1" ? "D_StatDownPositionDistribution" : "D_StatDownPositionDistributionByArea"
                                        , areaid == "-1" ? "" : string.Format(" and areaid='{0}' ",areaid)
                                        , versionid.Replace(",","','"));
            
            var parameters = new []
                {
                    new MySqlParameter("?softid",softid),
                    new MySqlParameter("?platform",platform),
                    new MySqlParameter("?stattype",stattype),
                    new MySqlParameter("?period",period),
                    new MySqlParameter("?projectsource",ProjectSource),
                    new MySqlParameter("?pagename",pagename),
                    new MySqlParameter("?pagetype",pagetype),
                    new MySqlParameter("?restype",restype),
                    new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")),
                    new MySqlParameter("?enddate",endtime.ToString("yyyyMMdd"))
                };
            List<D_StatDownPositionDistribution> lists = new List<D_StatDownPositionDistribution>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionByTagDetail(int restype, int softid,
                                                                                                 int platform,
                                                                                                 DateTime begintime,
                                                                                                 DateTime endtime,
                                                                                                 int ProjectSource,
                                                                                                 string versionid,
                                                                                                 int isupdate,
                                                                                                 int period,
                                                                                                 int positionid,
                                                                                                 string areaid,
                                                                                                 int IsDiffPageType = 0,
                                                                                                 int stattype = 1)
        {
            //IsDiffPageType 0不区分页面类型

            //0不包含更新，1包含更新
            string showdowntype = isupdate == 0 ? " and B.DownType in (1,3)  " : " and B.DownType in (1,2,3,4) ";
            string sql = string.Format(@"
                                SELECT StatDate,PositionID,B.Name PositionName,B.PageName, sum(DownCount) downcount
                                FROM {1} A 
                                inner join Cfg_DownPositions B 
	                                on B.Position=A.PositionID 
	                                and B.ProjectSource=A.ProjectSource
	                                and B.ResType=A.ResType  
                                    and B.SoftID=A.SoftID and B.ByTag=1 and B.Position=?positionid
                                where  A.SoftID=?softid and PLATFORM=?platform and A.ResType=?restype   and A.StatType=?stattype 
                                      and Period=?period and VersionID in ('{3}') and A.ProjectSource=?projectsource {0}  
                                      and StatDate between ?begindate and ?enddate {2}
                                group by StatDate,PositionID,B.Name,B.PageName
                                order by StatDate desc"
                                        , showdowntype
                                        , areaid == "-1" ? "D_StatDownPositionDistribution" : "D_StatDownPositionDistributionByArea"
                                        , areaid == "-1" ? "" :string.Format( " and areaid='{0}' ",areaid)
                                        , versionid.Replace(",", "','"));

            var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid",softid),
                    new MySqlParameter("?platform", platform),
                    new MySqlParameter("?stattype",stattype),
                    new MySqlParameter("?period", period),
                    new MySqlParameter("?positionid",positionid),
                    new MySqlParameter("?projectsource",ProjectSource),
                    new MySqlParameter("?restype", restype),
                    new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")),
                    new MySqlParameter("?enddate", endtime.ToString("yyyyMMdd"))
                };
            List<D_StatDownPositionDistribution> lists = new List<D_StatDownPositionDistribution>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        public List<D_StatDownPositionDistribution> GetD_StatDownPositionDistributionDetail(int restype,
                                                                                            int softid,
                                                                                            int platform,
                                                                                            DateTime begintime,
                                                                                            DateTime endtime,
                                                                                            int ProjectSource,
                                                                                            string versionid,
                                                                                            int isupdate,
                                                                                            int period,
                                                                                            int positionid,
                                                                                            string pagename,
                                                                                            string areaid,
                                                                                            int isdiffpagetype = 0,
                                                                                            int stattype = 1)
        {
            //showdowntype:0不包含更新，1包含更新 
            string showdowntype = isupdate == 0 ? " and B.DownType in (1,3)  " : " and B.DownType in (1,2,3,4) ";
            string sql = string.Format(@"
                                Select StatDate,sum(DownCount) DownCount
                                FROM {1} A 
                                inner join Cfg_DownPositions B  
	                                on B.Position=A.PositionID 
	                                and B.ProjectSource=A.ProjectSource
	                                and B.ResType=A.ResType and B.SoftID=A.SoftID 
                                where A.SoftID={5} and PLATFORM={6} and A.ResType={7} and A.StatType={8}   
                                      and Period={9} and VersionID in ('{4}') and A.ProjectSource={10} {0}
                                      and StatDate between {11} and {12} {2} {3}
                                group by StatDate
                                order by StatDate desc"
                                        , showdowntype
                                        , areaid == "-1" ? "D_StatDownPositionDistribution" : "D_StatDownPositionDistributionByArea"
                                        , areaid == "-1" ? "" : string.Format(" and areaid='{0}'",areaid),
                                        (isdiffpagetype == 0 && !pagename.Equals("" + positionid)) ? string.Format(" and B.PageName='{0}'",pagename) : string.Format(" and B.Position={0}",positionid)
                                        , versionid.Replace(",", "','"), softid, platform, restype, stattype, period, ProjectSource, begintime.ToString("yyyyMMdd"), endtime.ToString("yyyyMMdd")
                                        );

            List<D_StatDownPositionDistribution> lists = new List<D_StatDownPositionDistribution>();
            using (var dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        /// <summary>
        /// API日分发统计
        /// </summary>
        public List<D_StatDownPositionDistribution> GetD_StatDownApi(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddDays(-1).ToString("yyyyMMdd"));

            return GetD_StatDownApi(startdate, statdate);
        }
        /// <summary>
        /// Api月分发统计
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownApiMonth(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddMonths(-1).ToString("yyyyMM01"));

            return GetD_StatDownApi(startdate, statdate);
        }

        private static List<D_StatDownPositionDistribution> GetD_StatDownApi(int startdate, int enddate)
        {
            var sql = string.Format(@"
SELECT 
	StatDate,Platform,sum(DownCount) DownCount
FROM 
	D_StatDownPositionDistribution
where 
	((ProjectSource = 2200 and PositionID between 1 and 21) or (ProjectSource = 6300 and PositionID = 1))
	and Period = 1 and ResType = 1 and StatType = 1 and VersionID = '-1'
	and StatDate between ?startdate and ?enddate
	and Platform in (1,4)
	group by StatDate,Platform");

            var parameters = new[]
                {
                    new MySqlParameter("?enddate",  enddate),
                    new MySqlParameter("?startdate", startdate)
                };

            var lists = new List<D_StatDownPositionDistribution>();
            using (var dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        /// <summary>
        /// 苹果园阿拉丁日分发统计
        /// </summary>
        public List<D_StatDownPositionDistribution> GetD_StatDownAladin(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddDays(-1).ToString("yyyyMMdd"));

            return GetD_StatDownAladin(startdate, statdate);
        }
        /// <summary>
        /// 苹果园阿拉丁月分发统计
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownAladinMonth(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddMonths(-1).ToString("yyyyMM01"));

            return GetD_StatDownAladin(startdate, statdate);
        }

        private static List<D_StatDownPositionDistribution> GetD_StatDownAladin(int startdate, int enddate)
        {
            var sql = string.Format(@"
    SELECT 
	    StatDate,sum(DownCount) DownCount
    FROM 
	   D_StatDownPositionDistribution 
    where 
	    ProjectSource in (8500,8501)
	    and Period = 1 
	    and ResType in (19,22)
	    and StatType = 1 and VersionID = '-1'
	    and StatDate between ?startdate and ?enddate
	    and Platform in (1,4)
	    group by StatDate");

            var parameters = new[]
                {
                    new MySqlParameter("?enddate", enddate),
                    new MySqlParameter("?startdate", startdate)
                };

            var lists = new List<D_StatDownPositionDistribution>();
            using (var dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        /// <summary>
        /// Web日分发统计
        /// </summary>
        public List<D_StatDownPositionDistribution> GetD_StatDownWeb(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddDays(-1).ToString("yyyyMMdd"));

            return GetD_StatDownWeb(startdate, statdate);
        }
        /// <summary>
        /// Web月分发统计
        /// </summary>
        /// <param name="statdate"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownWebMonth(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddMonths(-1).ToString("yyyyMM01"));

            return GetD_StatDownWeb(startdate, statdate);
        }

        private static List<D_StatDownPositionDistribution> GetD_StatDownWeb(int startdate, int enddate)
        {
            var sql = string.Format(@"
SELECT 
	StatDate,Platform,sum(DownCount) DownCount
FROM 
	D_StatDownPositionDistribution 
where 
	not ((ProjectSource = 2200 and PositionID between 1 and 21) or (ProjectSource = 6300 and PositionID = 1)) and ProjectSource = 2200
	and Period = 1 and ResType = 1 and StatType = 1 and VersionID = '-1'
	and StatDate between ?startdate and ?enddate
	and Platform = 4
	group by StatDate,Platform
union
SELECT 
	StatDate,Platform,sum(DownCount) DownCount
FROM 
	D_StatDownPositionDistribution 
where 
	ProjectSource in (3900,1600,2100,4000,4900,4910,200,1100,1200,1500,1900,3100,3200,3300,2700,4800,2600,2300,5100,5200,5500,7600,8900,9500)	
	and Period = 1 and ResType = 1 and StatType = 1 and VersionID = '-1'
	and StatDate between ?startdate and ?enddate
	and Platform = 1
	group by StatDate,Platform
");

            var parameters = new[]
                {
                    new MySqlParameter("?enddate", enddate),
                    new MySqlParameter("?startdate",startdate)
                };

            var lists = new List<D_StatDownPositionDistribution>();
            using (var dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<D_StatDownPositionDistribution> GetD_StatDownMonthWebAndApi(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddMonths(-1).ToString("yyyyMMdd"));

            var sql = string.Format(@"
SELECT 
	Platform,sum(DownCount) DownCount
FROM 
	D_StatDownPositionDistribution
where 
	ProjectSource = 2200
	and Period = 1 and ResType = 1 and StatType = 1 and VersionID = '-1'
	and StatDate between ?startdate and ?enddate
	and Platform in (1,4)
	group by Platform");

            var parameters = new[]
                {
                    new MySqlParameter("?enddate", enddate.ToString("yyyyMMdd")),
                    new MySqlParameter("?startdate",startdate)
                };

            var lists = new List<D_StatDownPositionDistribution>();
            using (var dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        /// <summary>
        /// UV日分发统计
        /// </summary>
        public List<D_StatDownPositionDistribution> GetD_StatDownUV(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddDays(-1).ToString("yyyyMMdd"));

            var sql = string.Format(@"
		select Platform, StatDate, Sum(UserCount) DownCount,-1 projectSource		
		from D_StatUsersByIP 
		where ClientType = 2 and Platform in (4,9) and Period = 1 and StatDate between ?startdate and ?enddate and projectSource = -1
		group by platform,statdate
	union
		select Platform, StatDate, Sum(UserCount) DownCount, 8900 projectSource
		from D_StatUsersByIP 
		where Platform in (1,7) and Period = 1 and StatDate between ?startdate and ?enddate
		and projectsource in (3900,1600,2100,4000,4900,4910,200,1100,1200,1500,1900,3100,3200,3300,2700,4800,2600,2300,5100,5200,5500,7600,8900,9500)
		group by platform,statdate
	union
		select Platform, StatDate, Sum(UserCount) DownCount, 5000 projectSource
		from D_StatUsersByIP 
		where Platform in (1,7) and Period = 1 and StatDate between ?startdate and ?enddate
		and projectsource in (5000)
		group by platform,statdate
	union
		select Platform, StatDate, Sum(UserCount) DownCount, 8500 projectSource
		from D_StatUsersByIP 
		where Platform in (1,7) and Period = 1 and StatDate between ?startdate and ?enddate
		and projectsource in (8500,8501)
		group by platform,statdate

		");

            var parameters = new[]
                {
                    new MySqlParameter("?enddate", enddate.ToString("yyyyMMdd")),
                    new MySqlParameter("?startdate",startdate)
                };

            var lists = new List<D_StatDownPositionDistribution>();
            using (var dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }

        /// <summary>
        /// 统计每个资源ID在每个位置的下载量
        /// </summary>
        /// <param name="restype"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="projectsource"></param>
        /// <param name="version"></param>
        /// <param name="areatype"></param>
        /// <param name="lstresid"></param>
        /// <returns></returns>
        public List<D_StatDownPositionDistribution> GetD_StatDownPositionByResIDCacheDetail(int restype, int softid, int platform, DateTime begintime, DateTime endtime, int projectsource, int version, int areatype, int areaid, List<int> lstresid)
        {
            string conn = areatype == 1 ? commputConn : commputEnConn;
            string sql = string.Format(@"
                          SELECT Position positionid, 
                                 SUM(case when stattype=1 then DownCount else 0 end) downcount
                                FROM [{0}] A  WITH(NOLOCK)
                          where SoftID=@soft and Platform=@platform and DownloadDate between @begindate and @enddate 
                          and ResourceID in ({1}) {2}
                          and ResourceType =@restype  {3} {4}
                          group by Position", "ResDownload_Sum_", string.Join(",", lstresid.Select(p => p.ToString()).ToArray()),
                           version == -1 ? "" : " and VersionID=@versionid ",
                           projectsource == -1 ? "" : " and SourceID=@sourceid",
                            areaid == -1 ? "" : " and AreaID="+areaid);

            string resultsql = sql.Replace("ResDownload_Sum_", "ResDownload_Sum_" + begintime.ToString("yyyy"));


            SqlParameter[] sp = new SqlParameter[]
                {
             
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@soft", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@sourceid", SqlDbType.SmallInt, 2, projectsource),
                    SqlParamHelper.MakeInParam("@versionid", SqlDbType.Int, 4, version),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };
            List<D_StatDownPositionDistribution> lists = new List<D_StatDownPositionDistribution>();
            using (IDataReader reader = SqlHelper.ExecuteReader(conn, CommandType.Text, resultsql, sp))
            {
                while (reader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(reader));
                }
            }
            return lists;
        }



        public List<D_StatDownPositionDistribution> GetD_StatDownWebPgzs(int statdate)
        {

            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddMonths(-1).ToString("yyyyMM01"));
            return GetD_StatDownWebPgzs(startdate, statdate);
        }

        public List<D_StatDownPositionDistribution> GetD_StatDownWebPgzsMonth(int statdate)
        {
            var enddate = new DateTime(statdate / 10000, (statdate % 10000) / 100, statdate % 100);
            var startdate = int.Parse(enddate.AddMonths(-1).ToString("yyyyMM01"));

            return GetD_StatDownWebPgzs(startdate, statdate);
        }

        private static List<D_StatDownPositionDistribution> GetD_StatDownWebPgzs(int startdate, int enddate)
        {

            var sql = string.Format(@"
                SELECT 
	                StatDate,Platform,sum(DownCount) DownCount
                FROM 
	                D_StatDownPositionDistribution
                where 
	                ProjectSource in (5000)	
	                and Period = 1 and ResType = 22 and StatType = 1 and VersionID = '-1'
	                and StatDate between ?startdate and ?enddate
	                and Platform = 1
	                group by StatDate,Platform
                ");

            var parameters = new[]
                {
                    new MySqlParameter("?enddate", enddate),
                    new MySqlParameter("?startdate", startdate)
                };

            var lists = new List<D_StatDownPositionDistribution>();
            using (var dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownPositionDistribution(dataReader));
                }
            }
            return lists;
        }
    }
}



