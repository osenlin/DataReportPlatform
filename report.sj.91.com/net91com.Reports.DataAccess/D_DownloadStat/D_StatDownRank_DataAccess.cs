using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.DataAccess.DataAccesssUtil;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;

namespace net91com.Reports.DataAccess.D_DownloadStat
{
    public class D_StatDownRank_DataAccess : BaseDataAccess
    {
        public List<D_StatDownRank_SUM> GetD_StatDownRankByClass(int restype, int softid, int platform,
                                                                    DateTime begintime, int period, int pcid, int cid, int downtype)
        {
            string sql = string.Format(@"
                                select A.SoftID,A.Platform,A.ResType,A.StatDate,
		                                A.PCID,A.CID,A.DownCount,ResIdentifier
                                from D_StatDownRankByClass A 
                                where A.SOftID=?softid and A.Platform=?platform 
                                and StatDate = ?begindate  and restype=?restype
                                and PCID=?pcid and cid=?cid 
                                and period=?period and downtype=?downtype
                                order by DownCount desc
                                limit 200
                                ");

            var parameters = new[]
                {
                    new MySqlParameter("?softid",softid),
                    new MySqlParameter("?platform", platform),
                    new MySqlParameter("?period", period),
                    new MySqlParameter("?downtype",downtype),
                    new MySqlParameter("?restype", restype),
                    new MySqlParameter("?pcid", pcid),
                    new MySqlParameter("?cid",cid),
                    new MySqlParameter("?begindate",begintime.ToString("yyyyMMdd"))
                };
            List<D_StatDownRank_SUM> lists = new List<D_StatDownRank_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                int rank = 1;
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownRank_SUM(dataReader, rank));
                    rank++;
                }
            }
            return lists;
        }

        public List<Dictionary<string, string>> GetD_StatDownRankByClassMap(int restype, int softid, int platform,
                                                            DateTime begintime, DateTime endtime, int period, int pcid, int cid, int downtype)
        {
            string sql = string.Format(@"
                                select A.Rank lastrank,B.* from (
                                 select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate,
                                                A.PCID,A.CID,A.RealDownCount,ResName,ResIdentifier,
                                        ROW_NUMBER() over(order by RealDownCount desc) Rank
                                        from D_StatDownRankByClass A with(nolock)
                                        where A.SOftID=@softid and A.Platform=@platform 
                                        and StatDate = @enddate  and restype=@restype
                                        and PCID=@pcid and cid=@cid 
                                        and period=@period and downtype=@downtype
                                ) B left join (
                                 select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate,
                                                A.PCID,A.CID,A.RealDownCount,ResName,ResIdentifier,
                                        ROW_NUMBER() over(order by RealDownCount desc) Rank
                                        from D_StatDownRankByClass A with(nolock)
                                        where A.SOftID=@softid and A.Platform=@platform 
                                        and StatDate = @begindate  and restype=@restype
                                        and PCID=@pcid and cid=@cid 
                                        and period=@period and downtype=@downtype
                                ) A on A.ResID=B.ResID and A.SoftID=B.SoftID and A.ResType=B.ResType and A.PCID=B.PCID
                                and A.CID=B.CID
            ");

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@downtype", SqlDbType.Int, 2, downtype),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@pcid", SqlDbType.Int, 4, pcid),
                    SqlParamHelper.MakeInParam("@cid", SqlDbType.Int, 4, cid),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                    ,
                };
            using (var reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                return RelationDBDataSetUtil.ParseDataSet(reader);
            }
        }

        public List<D_StatDownRank_SUM> GetD_StatDownRankByArea(int restype, int softid, int platform,
                                                            DateTime begintime, int period, string areaid, int downtype)
        {
            string sql = string.Format(@"
                                select A.SoftID,A.Platform,A.ResType,A.StatDate,
                                       A.DownCount,ResIdentifier
                                from D_StatDownRankByArea A 
                                where A.SoftID={0} and A.Platform={1} 
                                      and StatDate = {2} and restype={3}
                                      and Area='{4}' 
                                      and period={5} and downtype={6}
                                order by A.DownCount desc
                                limit 200
                            ", softid, platform, begintime.ToString("yyyyMMdd"), restype, areaid, period, downtype);

            var lists = new List<D_StatDownRank_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                int rank = 1;
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownRank_SUM(dataReader, rank));
                    rank++;
                }
            }
            return lists;
        }

        public List<Dictionary<string, string>> GetD_StatDownRankByAreaMap(int restype, int softid, int platform,
                                                    DateTime begintime, DateTime endtime, int period, int countryid, int provinceid, int downtype)
        {
            string sql = string.Format(@"
                            select B.Rank lastrank,A.* from (        
                                select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate,
                                                    CountryID,ProvinceID,A.RealDownCount,ResName,ResIdentifier,
                                                    ROW_NUMBER() over(order by RealDownCount desc) Rank
                                                    from D_StatDownRankByArea A with(nolock)
                                                    where A.SOftID=@softid and A.Platform=@platform 
                                                    and StatDate = @enddate  and restype=@restype
                                                    and CountryID=@CountryID {0}  
                                                    and period=@period and downtype=@downtype
                                ) A left join (
                                select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate,
                                                    CountryID,ProvinceID,A.RealDownCount,ResName,ResIdentifier,
                                                    ROW_NUMBER() over(order by RealDownCount desc) Rank
                                                    from D_StatDownRankByArea A with(nolock)
                                                    where A.SOftID=@softid and A.Platform=@platform 
                                                    and StatDate = @begindate  and restype=@restype
                                                    and CountryID=@CountryID {0} 
                                                    and period=@period and downtype=@downtype
                                ) B 
                            on A.ResID=B.ResID and A.SoftID=B.SoftID and A.ResType=B.ResType  
                                    ", provinceid == -1 ? "" : " and ProvinceID=@ProvinceID");

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@downtype", SqlDbType.Int, 2, downtype),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@CountryID", SqlDbType.Int, 4, countryid),
                    SqlParamHelper.MakeInParam("@ProvinceID", SqlDbType.Int, 4, provinceid),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };
            using (var reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                return RelationDBDataSetUtil.ParseDataSet(reader);
            }
        }

        public List<D_StatDownRank_SUM> GetD_StatDownRankByExtendAttrLst_Identifer(
                                            int restype,
                                            int softid,
                                            int platform,
                                            DateTime begintime,
                                            int period,
                                            int extendAttrlst,
                                            int stattype)
        {
            string sql = string.Format(@"
                                select A.SoftID,A.Platform,A.ResType,A.StatDate
                                     ,A.DownCount,ResIdentifier
                                from D_StatDownRankByExtendAttributeLst A 
                                where A.SoftID={0} and A.Platform={1} 
                                and StatDate = {2}  and restype={3}
                                and ExtendAttrLst={4}  
                                and period={5} and stattype=1
                                order by A.DownCount desc
                                limit 200 ", softid, platform, begintime.ToString("yyyyMMdd")
                                                              , restype, extendAttrlst, period);

            List<D_StatDownRank_SUM> lists = new List<D_StatDownRank_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                int rank = 1;
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownRank_SUM(dataReader, rank));
                    rank++;
                }
            }
            return lists;
        }

        public List<Dictionary<string, string>> GetD_StatDownRankByExtendAttrLstMap(int restype, int softid, int platform,
                                            DateTime begintime, DateTime endtime, int period, int extendAttrlst, int stattype)
        {
            string sql = string.Format(@"
                            select B.Rank lastrank,A.* from (        
                                select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate
                                                    ,A.DownCount,ResName,ResIdentifier,
                                                    ROW_NUMBER() over(order by DownCount desc) Rank
                                                    from D_StatDownRankByExtendAttributeLst A with(nolock)
                                                    where A.SOftID=@softid and A.Platform=@platform 
                                                    and StatDate = @enddate  and restype=@restype
                                                    and ExtendAttributeLst=@ExtendAttributeLst  
                                                    and period=@period and stattype=1
                                ) A left join (
                                select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate
                                                    ,A.DownCount,ResName,ResIdentifier,
                                                    ROW_NUMBER() over(order by DownCount desc) Rank
                                                    from D_StatDownRankByExtendAttributeLst A with(nolock)
                                                    where A.SOftID=@softid and A.Platform=@platform 
                                                    and StatDate = @begindate  and restype=@restype
                                                    and ExtendAttributeLst=@ExtendAttributeLst  
                                                    and period=@period and stattype=1
                                ) B 
                            on A.ResID=B.ResID and A.SoftID=B.SoftID and A.ResType=B.ResType  
                                    ");

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@stattype", SqlDbType.TinyInt, 1, stattype),
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@ExtendAttributeLst", SqlDbType.Int, 4, extendAttrlst),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };
            using (var reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                return RelationDBDataSetUtil.ParseDataSet(reader);
            }
        }

        public List<Dictionary<string, string>> GetD_StatDownCPAByCacheMap(int restype, int softid, int platform,
                                                    DateTime begintime, DateTime endtime, int period, int countryid, int provinceid, int downtype)
        {
            string sql = string.Format(@"
    select 
        a.resid,
        sum(a.downcount) downcount,
        sum(a.usercount) usercount,
        sum(a.usercount) * 1.0 / sum(a.downcount) as userrate,
        sum(SetupSuccessCount) SetupSuccessCount,
        sum(DownUserCount) DownUserCount,
        b.f_name,
        b.f_identifier 
    from D_StatDownCpaByArea a inner join Softs_CPA b on a.resid = b.resid
    where b.softid = @softid and statdate between @begindate and @enddate and b.restype = @restype {0} {1}
    group by a.resid,b.f_name,b.f_identifier
    order by sum(a.downcount) desc",
                                                                                      countryid != -1 ? " and areaid = @countryid" : "",
                                                                                      platform != 0 ? " and b.platform=@platform" : "");

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@downtype", SqlDbType.Int, 2, downtype),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@CountryID", SqlDbType.Int, 4, countryid),
                    SqlParamHelper.MakeInParam("@ProvinceID", SqlDbType.Int, 4, provinceid),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };
            using (var reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                return RelationDBDataSetUtil.ParseDataSet(reader);
            }
        }

        public List<Dictionary<string, string>> GetD_StatDownCPAAndApiByCacheMap(int restype, int softid, int platform,
                                                    DateTime begintime, DateTime endtime, int period, int countryid, int provinceid, int downtype, string positions, string adsource)
        {
            string sql = string.Format(@"
    select 
	    b.identifier f_identifier,min(b.softname) f_name,a.campaignID,
	    min(b.bid) bid,
	    sum(downCount) downCount,
	    sum(activationCount) activationCount,
	    sum(setupSuccessCount) setupSuccessCount,
	    sum(showCount) showCount,
        sum(browseCount) browseCount
    from D_StatDownCPA a with(nolock) inner join Cfg_SynchroApiAdvertList b with(nolock)
     on a.campaignID=b.campaignID and a.adsource=b.adsource
    where a.restype=@restype and a.softid=@softid and statdate between @begindate and @enddate {0} {1} {2} {3}
    group by b.identifier,a.campaignID",
                               countryid != -1 ? " and areaid=@countryid" : "",
                               platform != 0 ? " and a.platform=@platform" : "",
                               string.IsNullOrEmpty(positions) ? "" : string.Format(" and position in ({0}) ", positions),
                               adsource == "-1" || string.IsNullOrEmpty(adsource) ? "" : string.Format(" and a.adsource in ({0}) ", adsource));

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@downtype", SqlDbType.Int, 2, downtype),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@CountryID", SqlDbType.Int, 4, countryid),
                    SqlParamHelper.MakeInParam("@ProvinceID", SqlDbType.Int, 4, provinceid),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };
            using (var reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                return RelationDBDataSetUtil.ParseDataSet(reader);
            }
        }

        public List<Dictionary<string, string>> GetD_StatDownCPAAndApiDailyByCacheMap(int restype, int softid, int platform,
                                                    DateTime begintime, DateTime endtime, int period, int countryid, int provinceid, int downtype, string positions, string adsource, string version)
        {
            string sql = string.Format(@"
 select 
  StatDate,b.SoftName,b.Bid,appVersion,platform,areaid,position,a.adsource,
  downsuccesscount,downCount,activationCount,setupSuccessCount,showCount,browseCount
 into #tmp_log
 from D_StatDownCPA a with(nolock) inner join Cfg_SynchroApiAdvertList b with(nolock)
  on a.campaignID=b.campaignID and a.adsource=b.adsource
 where a.restype=@restype and a.softid=@softid and statdate between @begindate and @enddate;

 select
  StatDate,min(softname) f_name,min(bid) bid,
  sum(downsuccesscount) downsuccesscount,
  sum(downCount) downCount,
  sum(activationCount) activationCount,
  sum(setupSuccessCount) setupSuccessCount,
  sum(showCount) showCount,
  sum(browseCount) browseCount
 from #tmp_log
 where 1=1 {0} {1} {2} {3} {4}
 group by statdate
 order by statdate",
 countryid != -1 ? " and areaid=@countryid" : "",
 platform != 0 ? " and platform=@platform" : "",
 !string.IsNullOrEmpty(positions) ? string.Format(" and position in ({0})", positions) : "",
 adsource == "-1" || string.IsNullOrEmpty(adsource) ? "" : string.Format(" and adsource in ({0})", adsource),
 !string.IsNullOrEmpty(version) ? string.Format(" and appVersion=@version", version) : "");

            SqlParameter[] parameters = new SqlParameter[]
            {
                SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                SqlParamHelper.MakeInParam("@downtype", SqlDbType.Int, 2, downtype),
                SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                SqlParamHelper.MakeInParam("@CountryID", SqlDbType.Int, 4, countryid),
                SqlParamHelper.MakeInParam("@ProvinceID", SqlDbType.Int, 4, provinceid),
                SqlParamHelper.MakeInParam("@version", SqlDbType.VarChar, 100, version),
                SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
            };
            return RelationDBDataSetUtil.ParseDataSet(SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters));
        }
        public List<Dictionary<string, string>> GetD_StatDownCPAAndApiByResIdByCacheMap(int restype, int softid, int platform,
                                                    DateTime begintime, DateTime endtime, int period, int countryid, int provinceid, int downtype, string positions, string identifier, int compaignId, string adsource)
        {
            string sql = string.Format(@"
    select 
	    a.statDate, b.identifier f_identifier,min(b.softname) f_name,a.campaignID,
	    min(b.bid) bid,
	    sum(downCount) downCount,
	    sum(activationCount) activationCount,
	    sum(setupSuccessCount) setupSuccessCount,
	    sum(showCount) showCount,
        sum(browseCount) browseCount
    from D_StatDownCPA a with(nolock) inner join Cfg_SynchroApiAdvertList b with(nolock)
     on a.campaignID=b.campaignID and a.adsource=b.adsource
    where a.restype=@restype and a.softid=@softid and statdate between @begindate and @enddate {0} {1} {2} and b.identifier='{3}' and a.campaignID={4} {5}
    group by a.statDate,b.identifier,a.campaignID",
                               countryid != -1 ? " and areaid=@countryid" : "",
                               platform != 0 ? " and a.platform=@platform" : "",
                               string.IsNullOrEmpty(positions) ? "" : string.Format(" and position in ({0}) ", positions),
                               identifier,
                               compaignId,
                               adsource == "-1" || string.IsNullOrEmpty(adsource) ? "" : string.Format(" and a.adsource in ({0}) ", adsource));

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@CountryID", SqlDbType.Int, 4, countryid),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };
            using (var reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                return RelationDBDataSetUtil.ParseDataSet(reader);
            }
        }

        public List<D_StatDownRank_SUM> GetD_StatDownRankBySoft(int restype, int softid, int platform,
                                                            DateTime begintime, int period, int downtype)
        {
            string sql = string.Format(@"
                                        select A.SoftID,A.Platform,A.ResType,A.StatDate,
                                                A.DownCount,ResIdentifier
                                                from D_StatDownRankBySoft A 
                                                where A.SOftID=?softid and A.Platform=?platform 
                                                and StatDate = ?begindate  and restype=?restype
                                                and period=?period and downtype=?downtype
                                        order by A.DownCount desc
                                        limit 200
                            ");

            var parameters = new[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform",platform),
                    new MySqlParameter("?period",period),
                    new MySqlParameter("?downtype", downtype),
                    new MySqlParameter("?restype",restype),
                    new MySqlParameter("?begindate", begintime.ToString("yyyyMMdd"))
                    
                };
            List<D_StatDownRank_SUM> lists = new List<D_StatDownRank_SUM>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql, parameters))
            {
                int rank = 1;
                while (dataReader.Read())
                {
                    lists.Add(new D_StatDownRank_SUM(dataReader, rank));
                    rank++;
                }
            }
            return lists;
        }

        /// <summary>
        /// 平均下载量统计按渠道类型
        /// </summary>
        /// <returns></returns>
        public List<D_StatDownCountsByResIDEntity> GetD_StatDownCountRankByAuthorID(int period, int softid, int platform, int restype, DateTime begintime, DateTime endtime)
        {
            string sql = string.Format(@"
                            select B.Rank lastrank,A.* from (        
                                select top 200 A.SoftID,A.Platform,A.ResType,A.StatDate,
                                                A.DownCount,ResCount,AuthorID,
                                                ROW_NUMBER() over(order by DownCount desc) Rank
                                                from D_StatDownRankByAuthorID A with(nolock)
                                                where A.SoftID={0} and A.Platform={1} 
                                                and StatDate = {4} and restype={2}
                                                and period={3}) A
                            left join (
                                select top 200 A.SoftID,A.Platform,A.ResType,A.StatDate,
                                                A.DownCount,ResCount,AuthorID,
                                                ROW_NUMBER() over(order by DownCount desc) Rank
                                                from D_StatDownRankByAuthorID A with(nolock)
                                                where A.SoftID={0} and A.Platform={1} 
                                                and StatDate = {5}  and restype={2}
                                                and period={3} 
                            ) B on  A.AuthorID=B.AuthorID and A.SoftID=B.SoftID and A.ResType=B.ResType 
                            and A.Platform=B.Platform and A.ResType=B.ResType 
                          ", softid, platform, restype, period, endtime.ToString("yyyyMMdd"), begintime.ToString("yyyyMMdd"));

            List<D_StatDownCountsByResIDEntity> lst = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                lst = new List<D_StatDownCountsByResIDEntity>();
                while (reader.Read())
                {
                    lst.Add(new D_StatDownCountsByResIDEntity(reader));
                }
            }
            return lst;
        }

        /// <summary>
        /// 平均下载量统计按渠道类型
        /// </summary>
        /// <returns></returns>
        public List<D_StatDownCountsByResIDEntity> GetD_StatDownCountRankByAuthorID_Identifier(int period, int softid, int platform, int restype, DateTime begintime)
        {
            string sql = string.Format(@"
                                select  A.SoftID,A.Platform,A.ResType,A.StatDate,
                                                A.DownCount,ResCount,AuthorID
                                                from D_StatDownRankByAuthorID A
                                                where A.SoftID={0} and A.Platform={1} 
                                                and StatDate = {4}  and restype={2}
                                                and period={3} 
                                order by A.DownCount desc
                                limit  200
                          ", softid, platform, restype, period, begintime.ToString("yyyyMMdd"));

            List<D_StatDownCountsByResIDEntity> lst = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
                lst = new List<D_StatDownCountsByResIDEntity>();
                int rank = 0;
                while (reader.Read())
                {
                    lst.Add(new D_StatDownCountsByResIDEntity(reader, rank));
                    rank++;
                }
            }
            return lst;
        }


        public List<Dictionary<string, string>> GetD_StatDownRankBySoftMap(int restype, int softid, int platform,
                                                    DateTime begintime, DateTime endtime, int period, int downtype)
        {
            string sql = string.Format(@"
                            select B.Rank lastrank,A.* from (        
                                select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate,
                                                A.RealDownCount,ResName,ResIdentifier,
                                                ROW_NUMBER() over(order by RealDownCount desc) Rank
                                                from D_StatDownRankBySoft A with(nolock)
                                                where A.SOftID=@softid and A.Platform=@platform 
                                                and StatDate = @enddate  and restype=@restype
                                                and period=@period and downtype=@downtype) A
                            left join (
                                select top 200 A.SoftID,A.Platform,A.ResType,A.ResID,A.StatDate,
                                                A.RealDownCount,ResName,ResIdentifier,
                                                ROW_NUMBER() over(order by RealDownCount desc) Rank
                                                from D_StatDownRankBySoft A with(nolock)
                                                where A.SOftID=@softid and A.Platform=@platform 
                                                and StatDate = @begindate  and restype=@restype
                                                and period=@period and downtype=@downtype
                            ) B on  A.ResID=B.ResID and A.SoftID=B.SoftID and A.ResType=B.ResType 
                         ");

            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@downtype", SqlDbType.Int, 2, downtype),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                    ,
                };
            using (var reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                return RelationDBDataSetUtil.ParseDataSet(reader);
            }

        }

        public List<D_StatDownCPAEntity> GetD_StatDownCPAByResIDCache(int restype, int softid, int platform, DateTime begintime, DateTime endtime, int resid, int areaid)
        {
            string sql = string.Format(@"
    select 
        statdate,
        @resid resid,
        b.f_name resname,
        b.f_identifier residentifier,
        sum(a.downcount) downcount,
        sum(a.SetupSuccessCount) SetupSuccessCount,
        sum(a.DownUserCount) DownUserCount,
        sum(a.usercount) usercount,
        sum(a.usercount) * 1.0 / sum(a.downcount) as userrate
    from D_StatDownCpaByArea a inner join Softs_CPA b on a.resid = b.resid
    where a.softid = @softid
     and statdate between @begindate and @enddate
     and a.resid = @resid and a.restype = @restype {0} {1}
    group by a.statdate,b.f_name,b.f_identifier
    order by a.statdate",
                        areaid != -1 ? " and areaid = @areaid" : "",
                        platform != 0 ? " and a.platform=@platform" : "");

            var parameters = new[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt, 2, restype),
                    SqlParamHelper.MakeInParam("@areaid", SqlDbType.Int, 4, areaid),
                    SqlParamHelper.MakeInParam("@resid", SqlDbType.Int, 4, resid),
                    SqlParamHelper.MakeInParam("@begindate", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@enddate", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };

            var list = new List<D_StatDownCPAEntity>();
            using (IDataReader read = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, parameters))
            {
                while (read.Read())
                {
                    list.Add(new D_StatDownCPAEntity(read));
                }
            }
            return list;
        }
    }
}
