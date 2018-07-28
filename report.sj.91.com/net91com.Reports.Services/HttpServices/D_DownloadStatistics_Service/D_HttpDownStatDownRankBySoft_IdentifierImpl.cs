using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownRankBySoft_IdentifierImpl : Abs_HttpDownLoad<D_StatDownRank_SUM>
    {

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "排行", "资源(ID/包名)", "资源名称","下载数", "上期排行"
                    };
            }
        }

        protected override List<List<T>> GetData<T>(HttpContext context, bool flag = false)
        {
            DateTime restime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime comparebeforetime = restime.AddDays(-1);
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int period = Convert.ToInt32(context.Request["period"]);
            int downtype = Convert.ToInt32(context.Request["downtype"]);
            if (3 == period)
            {
                comparebeforetime = B_BaseToolService.GetWeekUpOfDate(restime, DayOfWeek.Sunday, -1);
                restime = B_BaseToolService.GetWeekUpOfDate(restime, DayOfWeek.Sunday, 0);
            }
            if (12 == period)
            {
                comparebeforetime = restime.AddDays(-1 * (restime.Day)).AddMonths(-1);
                restime = restime.AddDays(-1 * (restime.Day));
            }
            List<List<D_StatDownRank_SUM>> lists = new List<List<D_StatDownRank_SUM>>();

            List<D_StatDownRank_SUM> resultbefore = D_StatDownRank_Service.Instance.GetD_StatDownRankBySoftByCache(restype, softs, platform,
                                                                                            comparebeforetime, period, downtype);

            List<D_StatDownRank_SUM> resultrestime = D_StatDownRank_Service.Instance.GetD_StatDownRankBySoftByCache(restype, softs, platform,
                                                                                            restime, period, downtype);

            var resulttemp = (from itrestime in resultrestime
                              join itbefore in resultbefore on itrestime.ResIdentifier equals itbefore.ResIdentifier into os
                              from t in os.DefaultIfEmpty()
                              select new D_StatDownRank_SUM
                              {
                                  StatDate = itrestime.StatDate,
                                  SoftID = itrestime.SoftID,
                                  Platform = itrestime.Platform,
                                  ResType = itrestime.ResType,                    
                                  DownCount = itrestime.DownCount,
                                  ResIdentifier = itrestime.ResIdentifier,
                                  Rank = itrestime.Rank,
                                  LastRank =t==null?"--": t.Rank.ToString()
                              }).OrderBy(p => p.Rank).ToList();

            var reslst = resulttemp.Select(p => p.ResIdentifier).ToList();

            int projectsourcetype = new B_BaseTool_DataAccess().GetProjectSourceTypeBySoftId2(softs);
            List<B_ResInfo> lstresinfo = new B_BaseToolService().GetResInfo(restype, projectsourcetype, reslst);
            var result = (from item in resulttemp
                          join itemresinfo in lstresinfo on item.ResIdentifier equals itemresinfo.ResIdentifier into os
                          from t in os.DefaultIfEmpty()
                          select new D_StatDownRank_SUM
                          {
                              StatDate = item.StatDate,
                              SoftID = item.SoftID,
                              Platform = item.Platform,
                              ResType = item.ResType,
                              ResIdentifier = item.ResIdentifier,
                              Rank = item.Rank,
                              DownCount = item.DownCount,
                              LastRank = item.LastRank,
                              ResName = t == null ? "" : t.ResName
                          }).ToList();

            if (result.Count == 0)
            {
                return new List<List<D_StatDownRank_SUM>>() as List<List<T>>;
            }
            lists.Add(result);

            return lists as List<List<T>>;
        }

        protected override Result GetDetailTable<T>(HttpContext context)
        {
            var lists = GetData<T>(context, true);
            var list = lists.Count != 0 ? lists[0] : new List<T>();
            DataTablesRequest param = new DataTablesRequest(context.Request);
            JQueryDataTableData dt = new JQueryDataTableData();
            dt.sEcho = param.sEcho;
            dt.iTotalRecords = dt.iTotalDisplayRecords = list.Count;
            dt.aaData = ObjectParseListFillDetailTable(list);

            DateTime restime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime comparebeforetime = restime.AddDays(-1);
            int period = Convert.ToInt32(context.Request["period"]);
            if (1 == period)
            {
                dt.begintime = restime.ToString("yyyy-MM-dd");
                dt.endtime = restime.ToString("yyyy-MM-dd");
            }
            if (3 == period)
            {
                dt.begintime = B_BaseToolService.GetWeekUpOfDate(restime, DayOfWeek.Monday, -1).ToString("yyyy-MM-dd");
                dt.endtime = B_BaseToolService.GetWeekUpOfDate(restime, DayOfWeek.Sunday, 0).ToString("yyyy-MM-dd");
            }
            if (12 == period)
            {
                dt.begintime = restime.AddDays(-1 * (restime.Day)+1).AddMonths(-1).ToString("yyyy-MM-dd");
                dt.endtime = restime.AddDays(-1 * (restime.Day)).ToString("yyyy-MM-dd");
            }
            return Result.GetSuccessedResult(dt, false);
        }

        protected override string DownloadExcelName
        {
            get { return "资源下载排行.xls"; }
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownRankBySoft_IdentifierImpl"; }
        }

        public override string ServiceName
        {
            get { return "资源下载排行"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/D_DownlaodStatistics/D_ResDownLoadRankBySoft_Identifier.aspx"; }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list, bool DetailEqChart = true)
        {
            var tempList = new List<List<string>>();
            if (list is List<D_StatDownRank_SUM>)
            {
                var temp = list as List<D_StatDownRank_SUM>;
                foreach (D_StatDownRank_SUM obj in temp)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.Rank.ToString());
                    values.Add(obj.ResIdentifier);
                    values.Add(obj.ResName);
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownCount));
                    values.Add(obj.LastRank.Equals("0") ? "--" : obj.LastRank);
                    tempList.Add(values);
                }
            }
            return tempList;
        }
    }
}
