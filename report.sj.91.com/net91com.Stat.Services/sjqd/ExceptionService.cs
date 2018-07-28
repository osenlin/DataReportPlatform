using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Core.Web;
using System.Text;
using net91com.Core.Extensions;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd
{
    public class ExceptionService
    {
        private static string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        private static ExceptionService _instance = null;
        private static object o = new object();

        private ExceptionService()
        {
        }

        public static ExceptionService Instance
        {
            get
            {
                if (_instance == null)
                    lock (o)
                        _instance = new ExceptionService();
                return _instance;
            }
        }

        public List<Exception_StatCount> GetCount(int softid, int platformid, int versionid, int period,
                                                  DateTime begintime, DateTime endtime)
        {
            string sql = string.Empty;
            if (versionid == 0)
            {
                sql = @"
 select StatDate,Period,SoftID,Platform,sum(ExceptionCount) ExceptionCount, sum(UseUsers) UseUsers
 from Exception_StatCount with(nolock)
 where SoftID=@softid and platform=@platform and StatDate between @begintime and @endtime and Period=@period
 group by StatDate,Period,SoftID,Platform";
            }
            else
            {
                sql = @"
 select StatDate,Period,SoftID,Platform,sum(ExceptionCount) ExceptionCount, sum(UseUsers) UseUsers
 from Exception_StatCountByVersion with(nolock)
 where SoftID=@softid and platform=@platform and VersionID in (select ID from Sjqd_SoftVersions where E_VersionID=@versionid) and StatDate between @begintime and @endtime and Period=@period
 group by StatDate,Period,SoftID,Platform";
            }
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platformid),
                    SqlParamHelper.MakeInParam("@versionid", SqlDbType.Int, 4, versionid),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@begintime", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd")))
                    ,
                    SqlParamHelper.MakeInParam("@endtime", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd")))
                };
            List<Exception_StatCount> data = new List<Exception_StatCount>();
            using (IDataReader dataReader = SqlHelper.ExecuteReader(statdbConn, CommandType.Text, sql, parameters))
            {
                while (dataReader.Read())
                {
                    data.Add(new Exception_StatCount(dataReader));
                }
            }
            return data;
        }

        public List<Exception_StatCount> GetList(int softid, int platform, int versionid, int period, DateTime begintime,
                                                 DateTime endtime, int pageIndex, int pageSize, out int count)
        {
            count = 0;
            string sql = string.Empty;
            if (versionid == 0)
            {
                sql = @"
 select row_number() over(order by StatDate desc) rno,StatDate,Period,SoftID,Platform,ExceptionMessage,ExceptionCount,UseUsers
 into #tmp_exceptionlog
 from Exception_StatCount with(nolock)
 where SoftID=@softid and platform=@platform and StatDate between @begintime and @endtime and Period=@period;
 select @@ROWCOUNT;
 select StatDate,Period,SoftID,Platform,ExceptionMessage,ExceptionCount,UseUsers
 from #tmp_exceptionlog where rno between @pageStart and @pageEnd;
 drop table #tmp_exceptionlog;";
            }
            else
            {
                sql = @"
 select row_number() over(order by StatDate desc) rno,StatDate,Period,SoftID,Platform,ExceptionMessage,ExceptionCount,UseUsers
 into #tmp_exceptionlog
 from Exception_StatCountByVersion with(nolock)
 where SoftID=@softid and platform=@platform and StatDate between @begintime and @endtime and Period=@period
  and VersionID in (select ID from Sjqd_SoftVersions where E_VersionID=@versionid);
 select @@ROWCOUNT;
 select StatDate,Period,SoftID,Platform,ExceptionMessage,ExceptionCount,UseUsers
 from #tmp_exceptionlog where rno between @pageStart and @pageEnd;
 drop table #tmp_exceptionlog;";
            }
            int pageStart = (Convert.ToInt32(pageIndex) - 1)*Convert.ToInt32(pageSize) + 1;
            int pageEnd = Convert.ToInt32(pageIndex)*Convert.ToInt32(pageSize);
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@softid", SqlDbType.Int, 4, softid),
                    SqlParamHelper.MakeInParam("@platform", SqlDbType.TinyInt, 1, platform),
                    SqlParamHelper.MakeInParam("@versionid", SqlDbType.Int, 4, versionid),
                    SqlParamHelper.MakeInParam("@period", SqlDbType.TinyInt, 1, period),
                    SqlParamHelper.MakeInParam("@begintime", SqlDbType.Int, 4, int.Parse(begintime.ToString("yyyyMMdd")))
                    ,
                    SqlParamHelper.MakeInParam("@endtime", SqlDbType.Int, 4, int.Parse(endtime.ToString("yyyyMMdd"))),
                    SqlParamHelper.MakeInParam("@pageStart", SqlDbType.Int, 4, pageStart),
                    SqlParamHelper.MakeInParam("@pageEnd", SqlDbType.Int, 4, pageEnd)
                };
            List<Exception_StatCount> list = new List<Exception_StatCount>();
            DataSet ds = SqlHelper.ExecuteDataset(statdbConn, CommandType.Text, sql, parameters);
            if (ds != null && ds.Tables.Count == 2)
            {
                count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    Exception_StatCount obj = new Exception_StatCount();
                    int date = Convert.ToInt32(dr["StatDate"]);
                    obj.StatDate = new DateTime(date/10000, date%10000/100, date%100);
                    obj.Period = (net91com.Stat.Core.PeriodOptions) Convert.ToInt32(dr["Period"]);
                    obj.SoftID = Convert.ToInt32(dr["SoftID"]);
                    obj.Platform = (MobileOption) Convert.ToInt32(dr["Platform"]);
                    obj.ExceptionMessage = dr["ExceptionMessage"].ToString();
                    obj.ExceptionCount = Convert.ToInt32(dr["ExceptionCount"]);
                    obj.UseUsers = Convert.ToInt32(dr["UseUsers"]);
                    list.Add(obj);
                }
            }
            return list;
        }
    }
}