using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    public class Sjqd_StatChannelRetainedUsers_DataAccess : BaseDataAccess
    {
        private static Sjqd_StatChannelRetainedUsers_DataAccess instance = null;
        private static readonly object obj = new object();

        public static Sjqd_StatChannelRetainedUsers_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new Sjqd_StatChannelRetainedUsers_DataAccess();
                            instance._cachePreviousKey = "Sjqd_StatChannelRetainedUsers_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 开放对外的留存用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelIds"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="minTime">最小可查看时间</param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetChannelRetainUsersForOut(int softId, MobileOption platform,
                                                                        List<int> channelIds, int period,
                                                                        DateTime beginDate, DateTime endDate,
                                                                        DateTime minTime)
        {
            if (platform == 0)
                return GetChannelRetainUsersForOut(softId, channelIds, period, beginDate, endDate, minTime);

            if (channelIds.Count == 0)
                return new List<Sjqd_StatRetainedUsers>();
            string ids = string.Join(",", channelIds.Select(p => p.ToString()).ToArray());
            string sql = string.Format(@"declare @statendtime datetime =@begintime;
                            declare @statbegindate datetime;
                            declare @begindateint int;
                            declare @enddateint int;
                            create table #retaintable(originaldate int,OriginalNewUserCount int)
                            while(@statendtime<=@endtime)
                            begin
	
	                            set @enddateint=CAST( convert(varchar(8),@statendtime,112) as int);
	                            if(@period=3)
	                            begin
		                            set @statbegindate=dateadd(day,-6,@statendtime);
		                            set @begindateint=CAST( convert(varchar(8),@statbegindate,112) as int);
		
		                            insert into #retaintable
		                            select @enddateint originaldate, SUM( (NewUserCount-isnull(NewUserCount_Shualiang,0))*(case when Modulus=0 then 1 else Modulus end)) 
		                            from dbo.Sjqd_StatChannelUsers
		                            where softid=@softid and platform=@platform and Period=1
		                            and StatDate between @begindateint and @enddateint 
		                            and ChannelID in({0}) and StatDate>=@mindateint
		
		                            set @statendtime=dateadd(day,7,@statendtime);

	                            end 
	                            else if(@period=5) 
	                            begin 
		                            set @statbegindate=dateadd(day,1,dateadd(MONTH,-1,@statendtime));
		                            set @begindateint=CAST( convert(varchar(8),@statbegindate,112) as int);
	    
		                            insert into #retaintable
		                            select @enddateint originaldate, SUM((NewUserCount-isnull(NewUserCount_Shualiang,0))*(case when Modulus=0 then 1 else Modulus end)) 
		                            from dbo.Sjqd_StatChannelUsers
		                            where softid=@softid and platform=@platform and Period=1
		                            and StatDate between @begindateint and @enddateint 
		                            and ChannelID  in({0}) and StatDate>=@mindateint
	
		                            set @statendtime=dateadd(MONTH,1,@statendtime);
	                            end 
	                            else
	                            begin--period=1
		 
		                            insert into #retaintable
		                            select @enddateint originaldate, SUM((NewUserCount-isnull(NewUserCount_Shualiang,0))*(case when Modulus=0 then 1 else Modulus end)) 
		                            from dbo.Sjqd_StatChannelUsers
		                            where softid=@softid and platform=@platform and Period=1
		                            and StatDate= @enddateint 
		                            and ChannelID in({0}) and StatDate>=@mindateint
	
		                            set @statendtime=dateadd(day,1,@statendtime);
 
	                            end
	                            print(@statendtime);
	                            print(@begindateint);
	                            print(@enddateint);

                            end

                            set @begindateint=CAST( convert(varchar(8),@begintime,112) as int);
                            set @enddateint=CAST( convert(varchar(8),@endtime,112) as int);

                            select B.StatDate,B.OriginalDate,B.Period,B.SoftID,B.Platform,A.OriginalNewUserCount,
                            A.OriginalNewUserCount*1.0/B.OriginalNewUserCount*B.RetainedUserCount RetainedUserCount
                            from #retaintable A 
                            inner join
                            (
	                            select 
	                            StatDate,OriginalDate,@period Period,@softid SoftID,@platform Platform,
	                            SUM(OriginalNewUserCount) originalnewusercount,
	                            SUM(RetainedUserCount) RetainedUserCount from  dbo.U_StatRetainedUsers with(nolock) 
	                            where  OriginalDate between @begindateint and @enddateint 
	                            and softid=@softid and platform=@platform and Period=@period and ChannelID in({0})
	                            group by OriginalDate,StatDate
                            ) B
                            on A.originaldate=B.OriginalDate 
                            where A.OriginalNewUserCount!=0
                            order by B.OriginalDate desc,B.StatDate desc", ids);
            SqlParameter[] paras =
                {
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softId),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@begintime", SqlDbType.DateTime, 8, beginDate),
                    SqlParamHelper.MakeInParam("@endtime", SqlDbType.DateTime, 8, endDate),
                    SqlParamHelper.MakeInParam("@mindateint", SqlDbType.Int, 4, minTime.ToString("yyyyMMdd")),
                };
            List<Sjqd_StatRetainedUsers> list = new List<Sjqd_StatRetainedUsers>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, paras))
            {
                while (reader.Read())
                {
                    list.Add(new Sjqd_StatRetainedUsers(reader));
                }
            }
            return list;
        }

        /// <summary>
        /// 开放对外的留存用户数据(不区分平台)
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelIds"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="minTime">最小可查看时间</param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetChannelRetainUsersForOut(int softId,
                                                                        List<int> channelIds, int period,
                                                                        DateTime beginDate, DateTime endDate,
                                                                        DateTime minTime)
        {
            if (channelIds.Count == 0)
                return new List<Sjqd_StatRetainedUsers>();
            string ids = string.Join(",", channelIds.Select(p => p.ToString()).ToArray());
            string sql = string.Format(@"declare @statendtime datetime =@begintime;
                            declare @statbegindate datetime;
                            declare @begindateint int;
                            declare @enddateint int;
                            create table #retaintable(originaldate int,OriginalNewUserCount int)
                            while(@statendtime<=@endtime)
                            begin
	
	                            set @enddateint=CAST( convert(varchar(8),@statendtime,112) as int);
	                            if(@period=3)
	                            begin
		                            set @statbegindate=dateadd(day,-6,@statendtime);
		                            set @begindateint=CAST( convert(varchar(8),@statbegindate,112) as int);
		
		                            insert into #retaintable
		                            select @enddateint originaldate, SUM( (NewUserCount-isnull(NewUserCount_Shualiang,0))*(case when Modulus=0 then 1 else Modulus end)) 
		                            from dbo.Sjqd_StatChannelUsers
		                            where softid=@softid and platform<252 and Period=1
		                            and StatDate between @begindateint and @enddateint 
		                            and ChannelID in({0}) and StatDate>=@mindateint
		
		                            set @statendtime=dateadd(day,7,@statendtime);

	                            end 
	                            else if(@period=5) 
	                            begin 
		                            set @statbegindate=dateadd(day,1,dateadd(MONTH,-1,@statendtime));
		                            set @begindateint=CAST( convert(varchar(8),@statbegindate,112) as int);
	    
		                            insert into #retaintable
		                            select @enddateint originaldate, SUM((NewUserCount-isnull(NewUserCount_Shualiang,0))*(case when Modulus=0 then 1 else Modulus end)) 
		                            from dbo.Sjqd_StatChannelUsers
		                            where softid=@softid and platform<252 and Period=1
		                            and StatDate between @begindateint and @enddateint 
		                            and ChannelID  in({0}) and StatDate>=@mindateint
	
		                            set @statendtime=dateadd(MONTH,1,@statendtime);
	                            end 
	                            else
	                            begin--period=1
		 
		                            insert into #retaintable
		                            select @enddateint originaldate, SUM((NewUserCount-isnull(NewUserCount_Shualiang,0))*(case when Modulus=0 then 1 else Modulus end)) 
		                            from dbo.Sjqd_StatChannelUsers
		                            where softid=@softid and platform<252 and Period=1
		                            and StatDate= @enddateint 
		                            and ChannelID in({0}) and StatDate>=@mindateint
	
		                            set @statendtime=dateadd(day,1,@statendtime);
 
	                            end
	                            print(@statendtime);
	                            print(@begindateint);
	                            print(@enddateint);

                            end

                            set @begindateint=CAST( convert(varchar(8),@begintime,112) as int);
                            set @enddateint=CAST( convert(varchar(8),@endtime,112) as int);

                            select B.StatDate,B.OriginalDate,B.Period,B.SoftID,B.Platform,A.OriginalNewUserCount,
                            A.OriginalNewUserCount*1.0/B.OriginalNewUserCount*B.RetainedUserCount RetainedUserCount
                            from #retaintable A 
                            inner join
                            (
	                            select 
	                            StatDate,OriginalDate,@period Period,@softid SoftID, 0 Platform,
	                            SUM(OriginalNewUserCount) originalnewusercount,
	                            SUM(RetainedUserCount) RetainedUserCount from  dbo.U_StatRetainedUsers with(nolock) 
	                            where  OriginalDate between @begindateint and @enddateint 
	                            and softid=@softid and platform<252 and Period=@period and ChannelID in({0})
	                            group by OriginalDate,StatDate
                            ) B
                            on A.originaldate=B.OriginalDate 
                            where A.OriginalNewUserCount!=0
                            order by B.OriginalDate desc,B.StatDate desc", ids);
            SqlParameter[] paras =
                {
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softId),
                    SqlParamHelper.MakeInParam("@begintime", SqlDbType.DateTime, 8, beginDate),
                    SqlParamHelper.MakeInParam("@endtime", SqlDbType.DateTime, 8, endDate),
                    SqlParamHelper.MakeInParam("@mindateint", SqlDbType.Int, 4, minTime.ToString("yyyyMMdd")),
                };
            List<Sjqd_StatRetainedUsers> list = new List<Sjqd_StatRetainedUsers>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql, paras))
            {
                while (reader.Read())
                {
                    list.Add(new Sjqd_StatRetainedUsers(reader));
                }
            }
            return list;
        }
    }
}