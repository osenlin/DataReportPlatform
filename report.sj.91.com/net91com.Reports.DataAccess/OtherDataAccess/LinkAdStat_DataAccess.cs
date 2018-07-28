using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.OtherDataAccess
{
    public class LinkAdStat_DataAccess: BaseDataAccess 
    {
        private static LinkAdStat_DataAccess instance = null;
        private static readonly object obj = new object();
        public static LinkAdStat_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new LinkAdStat_DataAccess();
                            instance._cachePreviousKey = "LinkAdStat_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }

        public List<LinkAdCount> GetList(int softId, int platform, DateTime beginDate, DateTime endDate)
        {
            string cmdText = @"
 select a.adid,e.PlanName,f.UnionName,c.Keyword,d.MapMode,
  a.linkcount,a.statcount_15,a.statcount_30,a.statcount_New_15,a.statcount_New_30
 from (
  select adid,SUM(LinkCount) LinkCount,
   SUM(case when intervaltimes=15 then ActUsers else 0 end) statcount_15,
   SUM(case when intervaltimes=30 then ActUsers else 0 end) statcount_30,
   SUM(case when intervaltimes=15 then NewUsers else 0 end) statcount_New_15,
   SUM(case when intervaltimes=30 then NewUsers else 0 end) statcount_New_30
  from Link_Ad_StatCount with(nolock) 
  where statdate between " + beginDate.ToString("yyyyMMdd") + @" and " + endDate.ToString("yyyyMMdd") + @"
   and softid=" + softId + (platform > 0 ? @" and platform=" + platform : "") + @"
  group by adid
 ) a
 inner join Cfg_Link_Ad b with(nolock) on a.adid=b.ID
 inner join Cfg_Link_AdKeyword c with(nolock) on b.KeywordID=c.ID
 inner join Cfg_Link_AdMapMode d with(nolock) on b.MapMode=d.ID
 inner join Cfg_Link_AdPlan e with(nolock) on b.PlanID=e.ID
 inner join Cfg_Link_AdUnion f with(nolock) on b.UnionID=f.ID
";
            List<LinkAdCount> list = new List<LinkAdCount>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, cmdText))
            {
                while (reader.Read())
                {
                    list.Add(new LinkAdCount(reader));
                }
            }
            return list;
        }

        public List<LinkAdCount> GetList_Hours(int softId, int platform, DateTime beginDate, DateTime endDate, int adId)
        {
            string cmdText = @"
 select hours,SUM(LinkCount) LinkCount,
  SUM(case when intervaltimes=15 then ActUsers else 0 end) statcount_15,
  SUM(case when intervaltimes=30 then ActUsers else 0 end) statcount_30,
  SUM(case when intervaltimes=15 then NewUsers else 0 end) statcount_New_15,
  SUM(case when intervaltimes=30 then NewUsers else 0 end) statcount_New_30
 from Link_Ad_StatCountByHours with(nolock) 
 where statdate between " + beginDate.ToString("yyyyMMdd") + @" and " + endDate.ToString("yyyyMMdd") + @"
  and adid=" + adId + @" and softid=" + softId + (platform > 0 ? @" and platform=" + platform : "") + @"
 group by hours
";
            List<LinkAdCount> list = new List<LinkAdCount>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, cmdText))
            {
                while (reader.Read())
                {
                    list.Add(new LinkAdCount(reader));
                }
            }
            return list;
        }

        public List<LinkAdCount> GetList_Area(int softId, int platform, DateTime beginDate, DateTime endDate, int adId)
        {
            string cmdText = @"
 select b.E_Country+' '+b.E_Province+' '+b.City area,
  a.linkcount,a.statcount_15,a.statcount_30,a.statcount_New_15,a.statcount_New_30
 from (
  select areaid,SUM(LinkCount) LinkCount,
   SUM(case when intervaltimes=15 then ActUsers else 0 end) statcount_15,
   SUM(case when intervaltimes=30 then ActUsers else 0 end) statcount_30,
   SUM(case when intervaltimes=15 then NewUsers else 0 end) statcount_New_15,
   SUM(case when intervaltimes=30 then NewUsers else 0 end) statcount_New_30
  from Link_Ad_StatCountByArea with(nolock) 
  where statdate between " + beginDate.ToString("yyyyMMdd") + @" and " + endDate.ToString("yyyyMMdd") + @"
   and adid=" + adId + @" and softid=" + softId + (platform > 0 ? @" and platform=" + platform : "") + @"
  group by areaid
 ) a
 inner join Sjqd_Areas b with(nolock) on a.areaid=b.ID
";
            List<LinkAdCount> list = new List<LinkAdCount>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, cmdText))
            {
                while (reader.Read())
                {
                    list.Add(new LinkAdCount(reader));
                }
            }
            return list;
        }

        public List<LinkAdCount> GetList_Keyword(int softId, int platform, DateTime beginDate, DateTime endDate, int adId)
        {
            string cmdText = @"
 select keyword searchword,SUM(LinkCount) LinkCount,
  SUM(case when intervaltimes=15 then ActUsers else 0 end) statcount_15,
  SUM(case when intervaltimes=30 then ActUsers else 0 end) statcount_30,
  SUM(case when intervaltimes=15 then NewUsers else 0 end) statcount_New_15,
  SUM(case when intervaltimes=30 then NewUsers else 0 end) statcount_New_30
 from Link_Ad_StatCountByKeyword with(nolock) 
 where statdate between " + beginDate.ToString("yyyyMMdd") + @" and " + endDate.ToString("yyyyMMdd") + @"
  and adid=" + adId + @" and softid=" + softId + (platform > 0 ? @" and platform=" + platform : "") + @"
 group by keyword
";
            List<LinkAdCount> list = new List<LinkAdCount>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, cmdText))
            {
                while (reader.Read())
                {
                    list.Add(new LinkAdCount(reader));
                }
            }
            return list;
        }

        public List<LinkAdStatRetainedUsers> GetList_RetainedUsers(int softId, int platform, DateTime beginDate, DateTime endDate, int adId)
        {
            string cmdText = @"
 select OriginalDate,Newusers
  ,sum(case when rno=1 then RetainedUsers else 0 end) c_1
  ,sum(case when rno=2 then RetainedUsers else 0 end) c_2
  ,sum(case when rno=3 then RetainedUsers else 0 end) c_3
  ,sum(case when rno=4 then RetainedUsers else 0 end) c_4
  ,sum(case when rno=5 then RetainedUsers else 0 end) c_5
  ,sum(case when rno=6 then RetainedUsers else 0 end) c_6
  ,sum(case when rno=7 then RetainedUsers else 0 end) c_7
 from (
  select OriginalDate,sum(NewUsers) NewUsers,sum(RetainedUsers) RetainedUsers,
   ROW_NUMBER()over(partition by OriginalDate,sum(NewUsers) order by OriginalDate,StatDate) rno
  from Link_Ad_StatRetainedUsers with(nolock)
  where OriginalDate between " + beginDate.ToString("yyyyMMdd") + @" and " + endDate.ToString("yyyyMMdd") + @"
   and adid=" + adId + @" and softid=" + softId + (platform > 0 ? @" and platform=" + platform : "") + @"
  group by OriginalDate,StatDate
 ) a
 group by OriginalDate,Newusers
 order by OriginalDate
";
            List<LinkAdStatRetainedUsers> list = new List<LinkAdStatRetainedUsers>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(StatConn, CommandType.Text, cmdText))
            {
                while (reader.Read())
                {
                    list.Add(new LinkAdStatRetainedUsers(reader));
                }
            }
            return list;
        }
    }
}
