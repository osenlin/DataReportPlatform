using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.DataAccess.DataAccesssUtil;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.UserRights;

namespace net91com.Reports.DataAccess.D_DownloadStat
{
    public class D_StatDownCount_DataAccess:BaseDataAccess
    {
        protected static string commputEnConn = ConfigHelper.GetConnectionString("ComputingDB_En_ConnString");
        protected static string commputConn = ConfigHelper.GetConnectionString("ComputingDB_CN_ConnString");
        /// <summary>
        /// 下载汇总数据按产品
        /// </summary>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownBySoft_SUM(int restype, int softid, int platform,
                                                                              DateTime begintime, DateTime endtime,
                                                                              ChannelTypeOptions selectchanneltype,
                                                                              string channelname, int selectchannelvalue,
                                                                              int sourceid, List<string> e_versionid, int period,
                                                                              string countryid, string province, List<string> statypelist,
                                                                              URLoginService loginService,
                                                                              int areatype = 1)
        {
            List<int> rangeChannelIds = loginService == null
                ? new URChannelsService().GetChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue })
                : loginService.GetAvailableChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue });

            string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());
            if (selectchannelvalue != 0 && rangeChannelIds.Count == 0)
                return new List<D_StatDownCountsBySoft_SUM>();

            string wheresource = "  and projectsource=?sourceid ";
            string wheresoftid = " and A.SoftID=?softid";
            //string sqlwith = "";
            if (sourceid == -1)
            {
                wheresource = " and projectsource=-1 ";
            }
            else if (sourceid == 2 || sourceid == 9)
            {
//                sqlwith = @"  with cte as(
//	                            select softid2 softid,projectsource from [R_ProjectSourcesBySoft]
//	                            where parentprojectsource=?sourceid and ProjectSourceType=?projectsourcetype
//                              )";
                wheresource =
                   @" and projectsource in(select distinct projectsource from ( 
                                 select projectsource from R_ProjectSourcesBySoft
	                            where parentprojectsource=?sourceid and ProjectSourceType=?projectsourcetype
                    ) A ) ";
                wheresoftid = @" and A.softid in (select distinct softid from (
                                    select softid2 softid,projectsource from R_ProjectSourcesBySoft
	                            where parentprojectsource=?sourceid and ProjectSourceType=?projectsourcetype
                    ) A )";
            }


            string wherecountry = "";
            if (countryid == "-1" && province == "-1")
            {
                wherecountry = " and province='-1' and country='-1'";
            }
            if (areatype == 2 && countryid != "-1")
            {
                wherecountry = " and country=?country ";
            }
            //国内7月之后都没地区的概念
            if (areatype == 1 && province != "-1")
            {
                //增加country='CN'，保证索引可以被用上
                wherecountry = " and country='CN' and province=?province";
            }

            string sql = string.Format(@"
                    select statdate,
                    ?sourceid sourceid,
                    softversion E_Version,
                    ?platform platform, 
                    ?softid softid,
                    '{5}' channelname,
                    ?country country,
                    ?province province,
                    SUM(case when stattype=1 then downcount else 0 end) downcount,
                    SUM(case when stattype=1 then DownCountBySilenceUpdating else 0 end) DownCountBySlience ,
                    SUM(case when stattype=4 then downcount else 0 end) DownSuccessCount,
                    SUM(case when stattype=4 then DownCountBySilenceUpdating else 0 end) DownSuccessCountBySlience,
                    SUM(case when stattype=4 then DownCountByUpdating else 0 end) DownSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=8 then downcount else 0 end) DownFailCount,
                    SUM(case when stattype=8 then DownCountBySilenceUpdating else 0 end) DownFailCountBySlience,
                    SUM(case when stattype=8 then DownCountByUpdating else 0 end) DownFailCountByUpdateNoSlience,
                    SUM(case when stattype=5 then downcount else 0 end) SetUpSuccessCount,
                    SUM(case when stattype=5 then DownCountBySilenceUpdating else 0 end) SetUpSuccessCountBySlience,
                    SUM(case when stattype=5 then DownCountByUpdating else 0 end) SetUpSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=6 then downcount else 0 end) SetUpFailCount,
                    SUM(case when stattype=6 then DownCountBySilenceUpdating else 0 end) SetUpFailCountBySlience,
                    SUM(case when stattype=6 then DownCountByUpdating else 0 end) SetUpFailCountByUpdateNoSlience,
                    SUM(case when stattype=1 then downcountbyupdating else 0 end) DownCountByUpdating,
                    SUM(case when stattype=1 then DownCountBySearching else 0 end) DownCountBySearching,
                    SUM(case when stattype=1 then gamedowncount else 0 end) GameDownCount,
                    SUM(case when stattype=1 then GameDownCountByUpdating else 0 end) GameDownCountByUpdating,
                    SUM(case when stattype=1 then GameDownCountBySearching else 0 end) GameDownCountBySearching,
                    SUM(case when stattype=1 then ScheduleDownCount else 0 end) ScheduleDownCount
                     from D_StatDownCountsBySoft_Sum A
                    where   period=?period and statdate between ?begindate and ?enddate
                      {0} {1} {2} {3} {4}  {6} {7} 
                    group by statdate,softversion
                    order by statdate",
                  e_versionid.Count == 0 ? " and A.SoftVersion='-1' " : string.Format(" and A.SoftVersion in ({0})", string.Join(",", e_versionid.ConvertAll(p => "'" + p.ToString() + "'").ToArray())),
                  wheresource,
                  platform == -1 ? " " : string.Format("  and A.platform={0} ", platform),
                  restype == -1 ? "  " : " and restype=?restype ",
                  selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString),
                  channelname,
                  wheresoftid,
                  wherecountry
                  //province == "-1" ? " and Province=''  " : "and Province=?province and country is null",
                  //areatype == 1 ? " and country='' " : "and Country=?country "
                  );

            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();

            using (MySqlCommand cmd = new MySqlCommand(sql,new MySqlConnection(Mysql_Statdb_Connstring)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 300;
                cmd.Parameters.Add(new MySqlParameter("?softid", softid));
                cmd.Parameters.Add(new MySqlParameter("?platform", platform));
                cmd.Parameters.Add(new MySqlParameter("?period", period));
                cmd.Parameters.Add(new MySqlParameter("?sourceid", sourceid));
                cmd.Parameters.Add(new MySqlParameter("?restype", restype));
                cmd.Parameters.Add(new MySqlParameter("?begindate", begintime.ToString("yyyyMMdd")));
                cmd.Parameters.Add(new MySqlParameter("?enddate", endtime.ToString("yyyyMMdd")));
                cmd.Parameters.Add(new MySqlParameter("?country", countryid));
                cmd.Parameters.Add(new MySqlParameter("?province", province));
                cmd.Parameters.Add(new MySqlParameter("?projectsourcetype", areatype));

                using (var dataReader=cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lists.Add(new D_StatDownCountsBySoft_SUM(dataReader));
                    }
                }
            }         
            return lists;

        }

        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownByChannel_SUM(int restype,
                                                                            int softid,
                                                                            int platform,
                                                                            DateTime begintime,
                                                                            DateTime endtime,
                                                                            ChannelTypeOptions selectchanneltype,
                                                                            string channelname,
                                                                            int selectchannelvalue,
                                                                            List<string> e_versionid,
                                                                            int period,
                                                                            URLoginService loginService)
        {
            List<int> rangeChannelIds = loginService == null
                ? new URChannelsService().GetChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue })
                : loginService.GetAvailableChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue });

            string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());
            if (selectchannelvalue != 0 && rangeChannelIds.Count == 0)
                return new List<D_StatDownCountsBySoft_SUM>();
            string sql = string.Format(@"select statdate,
                    softversion E_Version,
                    ?platform platform, 
                    ?softid softid,
                    '{4}' channelname, 
                    SUM(case when stattype=1 then downcount else 0 end) downcount,
                    SUM(case when stattype=1 then DownCountBySilenceUpdating else 0 end) DownCountBySlience ,
                    SUM(case when stattype=4 then downcount else 0 end) DownSuccessCount,
                    SUM(case when stattype=4 then DownCountBySilenceUpdating else 0 end) DownSuccessCountBySlience,
                    SUM(case when stattype=4 then DownCountByUpdating else 0 end) DownSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=8 then downcount else 0 end) DownFailCount,
                    SUM(case when stattype=8 then DownCountBySilenceUpdating else 0 end) DownFailCountBySlience,
                    SUM(case when stattype=8 then DownCountByUpdating else 0 end) DownFailCountByUpdateNoSlience,
                    SUM(case when stattype=5 then downcount else 0 end) SetUpSuccessCount,
                    SUM(case when stattype=5 then DownCountBySilenceUpdating else 0 end) SetUpSuccessCountBySlience,
                    SUM(case when stattype=5 then DownCountByUpdating else 0 end) SetUpSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=6 then downcount else 0 end) SetUpFailCount,
                    SUM(case when stattype=6 then DownCountBySilenceUpdating else 0 end) SetUpFailCountBySlience,
                    SUM(case when stattype=6 then DownCountByUpdating else 0 end) SetUpFailCountByUpdateNoSlience,
                    SUM(case when stattype=1 then downcountbyupdating else 0 end) DownCountByUpdating,
                    SUM(case when stattype=1 then DownCountBySearching else 0 end) DownCountBySearching,
                    SUM(case when stattype=1 then gamedowncount else 0 end) GameDownCount,
                    SUM(case when stattype=1 then GameDownCountByUpdating else 0 end) GameDownCountByUpdating,
                    SUM(case when stattype=1 then GameDownCountBySearching else 0 end) GameDownCountBySearching,
                    SUM(case when stattype=1 then ScheduleDownCount else 0 end) ScheduleDownCount
                     from D_StatDownCountsByVersion_Sum A 
                    where A.SoftID=?softid and period=?period and statdate between ?begindate and ?enddate
                      {0} {1} {2} {3}   
                    group by statdate,SoftVersion
                    order by statdate",
                  e_versionid.Count == 0 ? " and A.SoftVersion='-1' " : string.Format(" and A.SoftVersion in ({0})", string.Join(",", e_versionid.ConvertAll(p => "'" + p.ToString() + "'").ToArray())),
                  platform == -1 ? " " : string.Format("  and A.platform={0} ", platform),
                  restype == -1 ? "  " : " and restype=?restype ",
                  selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString),
                  channelname);
            var parameters = new []
			{
				new MySqlParameter("?softid", softid ),
                new MySqlParameter("?platform",platform ), 
                new MySqlParameter("?period",period),
                new MySqlParameter("?restype",restype),
                new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")),
                new MySqlParameter("?enddate",endtime.ToString("yyyyMMdd"))
			};
            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring,sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownCountsBySoft_SUM(dataReader));
                }
            }
            return lists;

        }


        /// <summary>
        /// 下载汇总数据按产品
        /// </summary>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownBySoft_SUMByArea(int restype, int softid, int platform,
                                                                              DateTime begintime, DateTime endtime,
                                                                              ChannelTypeOptions selectchanneltype,
                                                                              string channelname, int selectchannelvalue,
                                                                              int sourceid, List<string> e_versionid, int period,
                                                                              int countryid, int province,
                                                                              URLoginService loginService)
        {
            List<int> rangeChannelIds = loginService == null
                ? new URChannelsService().GetChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue })
                : loginService.GetAvailableChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue });

            string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());
            if (selectchannelvalue != 0 && rangeChannelIds.Count == 0)
                return new List<D_StatDownCountsBySoft_SUM>();
            string sql = string.Format(@"
                    select B.Name CountryName,A.* from (
                    select Country,
                    ?sourceid sourceid,
                    (case when SoftVersion='-1' then '不区分版本' else SoftVersion end) E_Version,
                    ?platform platform, 
                    ?softid softid,
                    '{5}' channelname,
                    SUM(case when stattype=1 then downcount else 0 end) downcount,
                    SUM(case when stattype=1 then DownCountBySlienceUpdating else 0 end) DownCountBySlience ,
                    SUM(case when stattype=4 then downcount else 0 end) DownSuccessCount,
                    SUM(case when stattype=4 then DownCountBySlienceUpdating else 0 end) DownSuccessCountBySlience,
                    SUM(case when stattype=4 then DownCountByUpdating else 0 end) DownSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=8 then downcount else 0 end) DownFailCount,
                    SUM(case when stattype=8 then DownCountBySlienceUpdating else 0 end) DownFailCountBySlience,
                    SUM(case when stattype=8 then DownCountByUpdating else 0 end) DownFailCountByUpdateNoSlience,
                    SUM(case when stattype=5 then downcount else 0 end) SetUpSuccessCount,
                    SUM(case when stattype=5 then DownCountBySlienceUpdating else 0 end) SetUpSuccessCountBySlience,
                    SUM(case when stattype=5 then DownCountByUpdating else 0 end) SetUpSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=6 then downcount else 0 end) SetUpFailCount,
                    SUM(case when stattype=6 then DownCountBySlienceUpdating else 0 end) SetUpFailCountBySlience,
                    SUM(case when stattype=6 then DownCountByUpdating else 0 end) SetUpFailCountByUpdateNoSlience,
                    SUM(case when stattype=1 then downcountbyupdating else 0 end) DownCountByUpdating,
                    SUM(case when stattype=1 then DownCountBySearching else 0 end) DownCountBySearching,
                    SUM(case when stattype=1 then gamedowncount else 0 end) GameDownCount,
                    SUM(case when stattype=1 then GameDownCountByUpdating else 0 end) GameDownCountByUpdating,
                    SUM(case when stattype=1 then GameDownCountBySearching else 0 end) GameDownCountBySearching,
                    SUM(case when stattype=1 then ScheduleDownCount else 0 end) ScheduleDownCount
                    from D_StatDownCountsBySoft_Sum A 
                    where A.SoftID=?softid and period=?period and statdate between ?begindate and ?enddate
                      {0} {1} {2} {3} {4} and country!=-1  
                    group by Country,SoftVersion) A inner join Cfg_Areas B
                    on A.CountryID=B.ID",
                  e_versionid.Count == 0 ? " and A.SoftVersion='-1' " : string.Format(" and A.SoftVersion in ({0})", string.Join(",", e_versionid.ConvertAll(p => "'" + p.ToString() + "'").ToArray())),
                  sourceid == -1 ? " and sourceid=-1 " : " and sourceid=?sourceid",
                  platform == -1 ? " " : string.Format("  and A.platform={0} ", platform),
                  restype == 0 ? "  " : " and restype=?restype ",
                  selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString),
                  channelname);
            var parameters = new MySqlParameter[]
			{
				new MySqlParameter("?softid", softid ),
                new MySqlParameter("?platform",platform ), 
                new MySqlParameter("?period",period),
                new MySqlParameter("?sourceid",sourceid),
                new MySqlParameter("?restype",restype),
                new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")),
                new MySqlParameter("?enddate",endtime.ToString("yyyyMMdd"))
			};
            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring,sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownCountsBySoft_SUM(dataReader));
                }
            }
            return lists;

        }


        /// <summary>
        /// 平均下载量的统计
        /// 2014-10-20
        /// update:将大表进行拆分
        /// time:2014-11-25
        /// </summary>
        /// <returns></returns>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownAvgCount(int restype,
                                                                      int softid,
                                                                      int platform,
                                                                      DateTime begintime,
                                                                      DateTime endtime,
                                                                      ChannelTypeOptions selectchanneltype,
                                                                      string channelname,
                                                                      int selectchannelvalue,
                                                                      int sourceid,
                                                                      List<int> e_versionid,
                                                                      int period,
                                                                      int stattype,
                                                                      int areaid,
                                                                      int modetype,
                                                                      URLoginService loginService,
                                                                      int areatype = 1)
        {
            List<int> rangeChannelIds = loginService == null
    ? new URChannelsService().GetChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue })
    : loginService.GetAvailableChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue });

            string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());
            if (selectchannelvalue != 0 && rangeChannelIds.Count == 0)
                return new List<D_StatDownCountsBySoft_SUM>();

            var bstool = new B_BaseTool_DataAccess();
            var lstversion = bstool.GetVersions(e_versionid);
            B_AreaEntity entity = bstool.GetAreabyid(areaid);

            string tablesql = string.Format(@" 
                            from D_StatDownAvgCount A 
                            where period=?period and statdate between ?begindate 
                                  and ?enddate and restype=?restype and softid=?softid  {0} {1} 
                            group by statdate "
                            , platform == 0 ? " " : "  and A.platform=?platform ",
                            selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString));
            if (modetype == 2)
            {
                tablesql = string.Format(@"
                    from D_StatDownAvgCountByVersion A
                    where A.SoftID=?softid and period=?period and stattype=?stattype 
                        and statdate between ?begindate and ?enddate
                      {0}  {1} {2} and area='{3}'  and restype=?restype
                    group by statdate,SoftVersion
                    order by statdate desc",
                          e_versionid.Count == 0 ? " and A.SoftVersion='-1' " : string.Format(" and A.SoftVersion in ({0})", string.Join(",", lstversion.ConvertAll(p => "'" + p.Version + "'").ToArray())),
                          platform == 0 ? " " : "  and A.platform=?platform  ",
                          selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString),
                          entity.EnShortName
                   );
            }           
            else if (modetype == 4)
            {
                tablesql = string.Format(@"
                  from D_StatDownAvgCountByArea A 
                  where A.SoftID=?softid and period=?period and stattype=?stattype 
                        and statdate between ?begindate and ?enddate
                       {0} {1}  and area='{2}' and restype=?restype
                    group by statdate
                    order by statdate desc",
                  platform == 0 ? " " : "  and A.platform=?platform  ",
                  selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString),entity.EnShortName
                  );
            }

            string selectql = string.Format(@"select statdate,
                    ?sourceid sourceid,
                    {0} 
                    ?platform platform, 
                    ?softid softid,
                    '{1}' channelname,
                    SUM(case when stattype=1  then downcount else 0 end) downcount,
                    SUM(case when stattype=1  then NewUserDownCount else 0 end) NewUserDownCount,
                    SUM(case when stattype=1  then NewUserCount else 0 end) NewUserCount,
                    SUM(case when stattype=1  then UserCount else 0 end) UserCount,

                    SUM(case when stattype=1  then downcountnotupdate else 0 end) DownCountExceptAllUpdating,
                    SUM(case when stattype=1  then NewUserDownCountNotUpdate else 0 end) NewUserDownCountExceptAllUpdating,
                    SUM(case when stattype=1  then NewUserCountNotUpdate else 0 end) NewUserCountExceptAllUpdating,
                    SUM(case when stattype=1  then UserCountNotUpdate else 0 end) UserCountExceptAllUpdating,
                    SUM(case when stattype=1  then UserCountUpdate else 0 end) UserCountUpdateing,
                    
                    SUM(case when stattype=1  then UserCountNoSilence else 0 end) UserCountNoSilence,
                    SUM(case when stattype=1  then UserCountUpdateNoSilence else 0 end) UserCountUpdateNoSilence,
                    SUM(case when stattype=1  then UserCountNoUpdateNoSearch else 0 end) UserCountNoUpdateNoSearch,

                    SUM(case when stattype=1  then downcountsilence else 0 end) downcountbyslienceupdating,
                    SUM(case when stattype=1  then UserCountSearch else 0 end) UserCountSearch,
                    SUM(case when stattype=1  then DownCountSearch else 0 end) DownCountBySearching,
                    SUM(case when stattype=1  then UserCountSilence else 0 end) UserCountSilenceUpdateing {2}",
                       modetype == 2 ? " coalesce(SoftVersion,'不区分版本') E_Version," : " '不区分版本' E_Version,",
                       //modetype == 2 ? "SoftVersion SoftVersion ," : "'-1' SoftVersion ,",
                       channelname,
                       tablesql);

            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();

            LogHelper.WriteInfo(selectql+" \n softid"+softid+" \n enshortname"+entity.EnShortName);
            using (MySqlCommand cmd=new MySqlCommand(selectql,new MySqlConnection(Mysql_Statdb_Connstring)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 300;
                cmd.Parameters.Add(new MySqlParameter("?softid", softid));
                cmd.Parameters.Add(new MySqlParameter("?platform",platform )); 
                cmd.Parameters.Add(new MySqlParameter("?period"   ,period));
                cmd.Parameters.Add(new MySqlParameter("?sourceid" ,sourceid));
                cmd.Parameters.Add(new MySqlParameter("?restype"  ,restype));
                cmd.Parameters.Add(new MySqlParameter("?stattype" ,stattype));
                cmd.Parameters.Add(new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")));
                cmd.Parameters.Add(new MySqlParameter("?enddate"  ,endtime.ToString("yyyyMMdd")));
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lists.Add(new D_StatDownCountsBySoft_SUM(dataReader));
                    }
                }
            }
            LogHelper.WriteInfo("list.count="+lists.Count);
            return lists;
        }
    

        /// <summary>
        /// 平均下载量统计按渠道类型
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="restype"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="channeltype"></param>
        /// <returns></returns>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownAvgCountByChannelType(int softid, int platform, int restype, DateTime begintime, DateTime endtime, int channeltype)
        {
            string sql = string.Format(@"
                          SELECT 
                                StatDate,
                                SUM(case when stattype=1  then downcount else 0 end) downcount,
                                SUM(case when stattype=1  then NewUserDownCount else 0 end) NewUserDownCount,
                                SUM(case when stattype=1  then NewUserCount else 0 end) NewUserCount,
                                SUM(case when stattype=1  then UserCount else 0 end) UserCount,

                                SUM(case when stattype=1  then downcountnotupdate else 0 end) DownCountExceptAllUpdating,
                                SUM(case when stattype=1  then NewUserDownCountNotUpdate else 0 end) NewUserDownCountExceptAllUpdating,
                                SUM(case when stattype=1  then NewUserCountNotUpdate else 0 end) NewUserCountExceptAllUpdating,
                                SUM(case when stattype=1  then UserCountNotUpdate else 0 end) UserCountExceptAllUpdating,
                                SUM(case when stattype=1  then UserCountUpdate else 0 end) UserCountUpdateing,

                                SUM(case when stattype=1  then downcountsilence else 0 end) downcountbyslienceupdating,
                                SUM(case when stattype=1  then UserCountSilence else 0 end) UserCountSilenceUpdateing 
                          FROM D_StatDownAvgCountByChannelType
                          where period=1 and softid=?softid {0}  and statdate between ?begindate and ?enddate
                          and channeltype=?channeltype and restype=?restype
                          group by StatDate
                          ", platform == -1 ? "" : " and PLATFORM=" + platform);
            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    
                    new MySqlParameter("?softid"   ,softid),
                    new MySqlParameter("?restype"  ,restype),
                    new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")),
                    new MySqlParameter("?enddate",endtime.ToString("yyyyMMdd")),
                    new MySqlParameter("?channeltype",channeltype)
                };
            List<D_StatDownCountsBySoft_SUM> lst = null;
            using (IDataReader reader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                lst = new List<D_StatDownCountsBySoft_SUM>();
                while (reader.Read())
                {
                    lst.Add(new D_StatDownCountsBySoft_SUM(reader));
                }
            }
            return lst;
        }

        /// <summary>
        /// 统计下载按分类
        /// 2014-10-21
        /// </summary>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownCountCateSum(int restype, int softid, int platform, DateTime begintime, DateTime endtime, int period, int pcid, int cid, int downtype)
        {
            string sql = string.Format(@"
                    

                    select statdate, ?platform platform,?pcid pcid,?softid softid,?restype restype,
                    SUM(case when stattype=1 and downtype=-1 then downcount else 0 end) downcount,
                    SUM(case when stattype=1 and downtype=5 then downcount else 0 end) DownCountExceptAllUpdating ,
                    SUM(case when stattype=1 and downtype=3 then downcount else 0 end) DownCountBySearching,

                    SUM(case when stattype=4 and downtype=-1 then downcount else 0 end) DownSuccessCount,
                    SUM(case when stattype=4 and downtype=5 then downcount else 0 end) DownSuccessCountByExceptAllUpdate,

                    SUM(case when stattype=8 and downtype=-1 then downcount else 0 end) DownFailCount,
                    SUM(case when stattype=8 and downtype=5 then downcount else 0 end) DownFailCountByExceptAllUpdate,

                    SUM(case when stattype=5 and downtype=-1 then downcount else 0 end) SetUpSuccessCount,
                    SUM(case when stattype=5 and downtype=5 then downcount else 0 end) SetUpSuccessCountByExceptAllUpdate,

                    SUM(case when stattype=6 and downtype=-1 then downcount else 0 end) SetUpFailCount,
                    SUM(case when stattype=6 and downtype=5 then downcount else 0 end) SetUpFailCountByExceptAllUpdate
      
                     from D_StatDownCateCountsBySoft_Sum A 
                    where A.SoftID=?softid and period=?period  and statdate between ?begindate and ?enddate 
                        {0} {1} {2} {3} 
                    group by statdate
                    order by statdate
                   ", platform == 0 ? " " : "  and A.platform=?platform",
                    restype == 0 ? "  " : " and restype=?restype ",
                    pcid == 0 ? "" : "and pcid=?pcid ",
                    cid == 0 ? "" : "and cid=?cid");
            var parameters = new MySqlParameter[]
			{
				new MySqlParameter("?softid", softid ),
                new MySqlParameter("?platform",platform ), 
                new MySqlParameter("?period",period),
                new MySqlParameter("?restype",restype),
                new MySqlParameter("?downtype",downtype),
                new MySqlParameter("?pcid", pcid ),
                new MySqlParameter("?cid",cid ),
                new MySqlParameter("?begindate",int.Parse(begintime.ToString("yyyyMMdd"))),
                new MySqlParameter("?enddate",int.Parse(endtime.ToString("yyyyMMdd"))),
			};
            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownCountsBySoft_SUM(dataReader));
                }
            }
            return lists;
        }

        /// <summary>
        /// 资源ID下载量查询
        /// </summary>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownCountSumByResID(int softid, int platform, List<int> resid,
                                                                             int restype,
                                                                             DateTime begintime, DateTime endtime,
                                                                             List<int> lstposition,
                                                                             int version,
                                                                             int sourceid,
                                                                             int areaid,
                                                                             int areatype)
        {
            String areaName=new B_BaseTool_DataAccess().GetAreabyid(areaid).EnShortName;

            String versionName="-1";
            if (version!=-1)
            {
                versionName = new B_BaseTool_DataAccess().GetVersionById(version).Version;
            }
            
            string sql = string.Format(@"                        
                          SELECT statdate, softid,platform, restype {5} {8} {6} 
                                 SUM(case when stattype=1 then DownCount else 0 end) downcount,
                                 SUM(case when stattype=2 then DownCount else 0 end) browsecount,
                                 SUM(case when stattype=4 then DownCount else 0 end) downsuccesscount,
                                 SUM(case when stattype=8 then DownCount else 0 end) downfailcount,
                                 SUM(case when stattype=5 then DownCount else 0 end) setupsuccesscount,
                                 SUM(case when stattype=6 then DownCount else 0 end) setupfailcount,
                                 SUM(case when stattype=4 and DownType  in(1,3) then DownCount else 0 end) downsuccesscountbyexceptallupdate,
                                 SUM(case when stattype=5 and DownType  in(1,3) then DownCount else 0 end) setupsuccesscountbyexceptallupdate,
                                 SUM(case when DownType in (2,4) and  stattype=1  then DownCount else 0 end) downcountbyupdating,
                                 SUM(case when DownType=3 and  stattype=1  then DownCount else 0 end) downcountbysearching
                                FROM {0} A
                          where SoftID=?soft and Platform=?platform and statdate between ?begindate and ?enddate 
                          and ResID in ({1}) {2}
                          and restype =?restype  {3} {4} {7}
                          group by statdate, softid,platform,restype", "D_ResourceLog_Sum_",
                           string.Join(",", resid.Select(p => p.ToString()).ToArray()),
                           lstposition.Count == 0 ? "" : string.Format("and Position in ({0})", string.Join(",", lstposition.Select(p => p.ToString()).ToArray())),
                            version == -1 ? "" : " and SoftVersion=?SoftVersion ",
                            sourceid == -1 ? "" : " and ProjectSource=?ProjectSource",
                            string.Format(", {0} VersionID ", version),
                            string.Format(", {0} SourceID ,", sourceid),
                            areaid == -1 ? "" : " and area=?areaName",
                            string.Format(", {0} area ", areaid));

            string resultsql = sql.Replace("D_ResourceLog_Sum_", "D_ResourceLog_Sum_" + begintime.ToString("yyyy"));
            if (endtime.Year - begintime.Year >= 1)
            {
                resultsql += " union all " + sql.Replace("D_ResourceLog_Sum_", "D_ResourceLog_Sum_" + endtime.ToString("yyyy"));
            }

            resultsql = "select * from (" + resultsql + ") A order by statdate desc";

            //LogHelper.WriteInfo(resultsql+"  areatype:"+areatype+" softid:"+softid);
            var sp = new MySqlParameter[]
                {
                    new MySqlParameter("?areaName",      areaName),
                    new MySqlParameter("?platform",      platform),
                    new MySqlParameter("?soft",          softid),
                    new MySqlParameter("?restype",       restype),
                    new MySqlParameter("?ProjectSource", sourceid),
                    new MySqlParameter("?SoftVersion",   versionName),
                    new MySqlParameter("?begindate",     int.Parse(begintime.ToString("yyyyMMdd"))),
                    new MySqlParameter("?enddate",       int.Parse(endtime.ToString("yyyyMMdd")))
                };
            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();
            using (IDataReader reader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, resultsql, sp))
            {
                while (reader.Read())
                {
                    lists.Add(new D_StatDownCountsBySoft_SUM(reader));
                }
            }
            return lists;
        }

        /// <summary>
        /// 下载量统计按版本分布品
        /// add:Lin,Zhihuang
        /// addDate:2014-12-19
        /// </summary>
        public object GetD_StatDownByVersionDistribution_SUMByCache(int restype, int softid, int platform, DateTime begintime, DateTime endtime, ChannelTypeOptions selectchanneltype, string channelname, int selectchannelvalue, List<int> e_versionid, int period, URLoginService loginService)
        {
            List<int> rangeChannelIds = loginService == null
    ? new URChannelsService().GetChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue })
    : loginService.GetAvailableChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue });

            string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());
            if (selectchannelvalue != 0 && rangeChannelIds.Count == 0)
                return new List<D_StatDownCountsBySoft_SUM>();
            string sql = string.Format(@"select 
                    (case when A.SoftVersion is null or A.softversion='' then '未知' else A.softversion end) E_Version,
                    ?platform platform, 
                    ?softid softid,
                    '{3}' channelname,
                    SUM(case when stattype=1 then downcount else 0 end) downcount,
                    SUM(case when stattype=1 then DownCountBySilenceUpdating else 0 end) DownCountBySlience ,
                    SUM(case when stattype=4 then downcount else 0 end) DownSuccessCount,
                    SUM(case when stattype=4 then DownCountBySilenceUpdating else 0 end) DownSuccessCountBySlience,
                    SUM(case when stattype=4 then DownCountByUpdating else 0 end) DownSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=8 then downcount else 0 end) DownFailCount,
                    SUM(case when stattype=8 then DownCountBySilenceUpdating else 0 end) DownFailCountBySlience,
                    SUM(case when stattype=8 then DownCountByUpdating else 0 end) DownFailCountByUpdateNoSlience,
                    SUM(case when stattype=5 then downcount else 0 end) SetUpSuccessCount,
                    SUM(case when stattype=5 then DownCountBySilenceUpdating else 0 end) SetUpSuccessCountBySlience,
                    SUM(case when stattype=5 then DownCountByUpdating else 0 end) SetUpSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=6 then Downcount else 0 end) SetUpFailCount,
                    SUM(case when stattype=6 then DownCountBySilenceUpdating else 0 end) SetUpFailCountBySlience,
                    SUM(case when stattype=6 then DownCountByUpdating else 0 end) SetUpFailCountByUpdateNoSlience,
                    SUM(case when stattype=1 then downcountbyupdating else 0 end) DownCountByUpdating,
                    SUM(case when stattype=1 then DownCountBySearching else 0 end) DownCountBySearching,
                    SUM(case when stattype=1 then gamedowncount else 0 end) GameDownCount,
                    SUM(case when stattype=1 then GameDownCountByUpdating else 0 end) GameDownCountByUpdating,
                    SUM(case when stattype=1 then GameDownCountBySearching else 0 end) GameDownCountBySearching,
                    SUM(case when stattype=1 then ScheduleDownCount else 0 end) ScheduleDownCount
                     from D_StatDownCountsByVersion_Sum A 
                    where A.SoftID=?softid and period=?period and statdate between ?begindate and ?enddate
                       {0} {1} {2} and SoftVersion!='-1'  
                    group by (case when A.SoftVersion is null or A.softversion='' then '未知' else A.softversion end)
                    order by downcount desc",
                  platform == -1 ? " " : string.Format("  and A.platform={0} ", platform),
                  restype == -1 ? "  " : " and restype=?restype ",
                  selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString),
                  channelname);
            var parameters = new []
			{
				new MySqlParameter("?softid",softid ), 
                new MySqlParameter("?platform",platform ), 
                new MySqlParameter("?period",period),
                new MySqlParameter("?restype",restype),
                new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")),
                new MySqlParameter("?enddate",endtime.ToString("yyyyMMdd"))
			};
            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring,sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownCountsBySoft_SUM(dataReader));
                }
            }
            return lists;
        }


        /// <summary>
        /// 下载量统计按地区分布品
        /// add:Lin,Zhihuang
        /// addDate:2014-12-30
        /// </summary>
        public List<D_StatDownCountsBySoft_SUM> GetD_StatDownByAreaDistribution_SUM(int restype, int softid, int platform,
                                                                              DateTime begintime,
                                                                              DateTime endtime,
                                                                              ChannelTypeOptions selectchanneltype,
                                                                              string channelname,
                                                                              int selectchannelvalue,
                                                                              int period,
                                                                              int areatype,
                                                                              URLoginService loginService)
        {
            List<int> rangeChannelIds = loginService == null
                ? new URChannelsService().GetChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue })
                : loginService.GetAvailableChannelIds(softid, selectchanneltype, new int[] { selectchannelvalue });

            string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());
            if (selectchannelvalue != 0 && rangeChannelIds.Count == 0)
                return new List<D_StatDownCountsBySoft_SUM>();

            string selectgroupsql = "Country";
            if (areatype == 1)
            {
                selectgroupsql = "province";
            }
            string sql = string.Format(@"
                    select {3},ifnull(b.Name,'未知') countryname,
                    SUM(case when stattype=1 then downcount else 0 end) downcount,
                    SUM(case when stattype=1 then DownCountBySilenceUpdating else 0 end) DownCountBySlience ,
                    SUM(case when stattype=4 then downcount else 0 end) DownSuccessCount,
                    SUM(case when stattype=4 then DownCountBySilenceUpdating else 0 end) DownSuccessCountBySlience,
                    SUM(case when stattype=4 then DownCountByUpdating else 0 end) DownSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=8 then downcount else 0 end) DownFailCount,
                    SUM(case when stattype=8 then DownCountBySilenceUpdating else 0 end) DownFailCountBySlience,
                    SUM(case when stattype=8 then DownCountByUpdating else 0 end) DownFailCountByUpdateNoSlience,
                    SUM(case when stattype=5 then downcount else 0 end) SetUpSuccessCount,
                    SUM(case when stattype=5 then DownCountBySilenceUpdating else 0 end) SetUpSuccessCountBySlience,
                    SUM(case when stattype=5 then DownCountByUpdating else 0 end) SetUpSuccessCountByUpdateNoSlience,
                    SUM(case when stattype=6 then downcount else 0 end) SetUpFailCount,
                    SUM(case when stattype=6 then DownCountBySilenceUpdating else 0 end) SetUpFailCountBySlience,
                    SUM(case when stattype=6 then DownCountByUpdating else 0 end) SetUpFailCountByUpdateNoSlience,
                    SUM(case when stattype=1 then downcountbyupdating else 0 end) DownCountByUpdating,
                    SUM(case when stattype=1 then DownCountBySearching else 0 end) DownCountBySearching,
                    SUM(case when stattype=1 then gamedowncount else 0 end) GameDownCount,
                    SUM(case when stattype=1 then GameDownCountByUpdating else 0 end) GameDownCountByUpdating,
                    SUM(case when stattype=1 then GameDownCountBySearching else 0 end) GameDownCountBySearching,
                    SUM(case when stattype=1 then ScheduleDownCount else 0 end) ScheduleDownCount
                     from D_StatDownCountsBySoft_Sum a
                      left join (select Name,EnShortName from Cfg_Areas where  EnShortName<>'' ) b on   a.{3}=b.EnShortName 
                    where a.SoftID=?softid and period=?period and statdate between ?begindate and ?enddate
                      {0} {1} {2}  and softversion='-1' and ProjectSource=-1   and country!='-1' 
                    group by {3},b.Name
                    order by downcount desc
                    ",
                  platform == -1 ? " " : string.Format("  and a.platform={0} ", platform),
                  restype == -1 ? "  " : " and restype=?restype ",
                  selectchannelvalue == 0 ? " and ChannelID=-1  " : string.Format(" and ChannelID  in({0}) ", channelIdsString),
                  selectgroupsql);

            var parameters = new []
			{
				new MySqlParameter("?softid",softid ), 
                new MySqlParameter("?platform",platform ), 
                new MySqlParameter("?period",period),
                new MySqlParameter("?restype",restype),
                new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd")),
                new MySqlParameter("?enddate",endtime.ToString("yyyyMMdd"))
			};
            List<D_StatDownCountsBySoft_SUM> lists = new List<D_StatDownCountsBySoft_SUM>();
            using (IDataReader dataReader =MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownCountsBySoft_SUM(dataReader));
                }
            }
            return lists;

        }

        /// <summary>
        /// 扩展属性的下载量统计
        /// </summary>
        /// <returns></returns>
        public List<D_StatDownCountsByExtendAttrLst_SUM> GetD_StatDownByExtendAttrLst_SUM(
            int restype, int softid, int platform, 
            DateTime begintime, DateTime endtime, 
            int period, int extendAttrLst,int stattype)
        {
            string sql = string.Format(@"
                select statdate,
                    Sum(DownCount) downcount
                    from D_StatDownCountsByExtendAttr_Sum
                    where  Stattype={0} and period=1 and statdate between {1} and {2}
                    and restype={3} and softid={4} and platform={5}  and ExtendAttrlst={6}
                    group by statdate
            ", stattype,begintime.ToString("yyyyMMdd"),endtime.ToString("yyyyMMdd"),restype,softid,platform,extendAttrLst);

            var lists = new List<D_StatDownCountsByExtendAttrLst_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownCountsByExtendAttrLst_SUM(dataReader));
                }
            }
            return lists;
        }
    }
}