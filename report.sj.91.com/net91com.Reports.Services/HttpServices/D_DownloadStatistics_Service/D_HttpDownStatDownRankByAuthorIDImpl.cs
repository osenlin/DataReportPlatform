using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownRankByAuthorIDImpl : Abs_HttpDownLoad<D_StatDownCountsByResIDEntity>
    {

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "排行", "作者ID", "作者", "资源数", "下载数", "上期排行"
                    };
            }
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

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list, bool DetailEqChart = true)
        {
            var lstlstname = new List<List<string>>();
            var temp = list as List<D_StatDownCountsByResIDEntity>;
            foreach (D_StatDownCountsByResIDEntity obj in temp)
            {
                var values = new List<string>();
                values.Add(obj.Rank.ToString());
                values.Add(obj.AuthorID.ToString());
                values.Add(obj.AuthorName);
                values.Add(UtilityHelp.FormatNum<int>(obj.ResCount));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownCount));
                values.Add(obj.LastRank.ToString());
                lstlstname.Add(values);
            }
            return lstlstname;
        }


        protected override string DownloadExcelName
        {
            get { return "下载量统计（按作者）.xls"; }
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownRankByAuthorIDImpl"; }
        }

        public override string ServiceName
        {
            get { return "资源下载排行（按作者）"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/D_DownlaodStatistics/D_ResDownLoadRankByAuthorID.aspx"; }
        }

        protected override List<List<T>> GetData<T>(HttpContext context, bool flag = false)
        {
            DateTime restime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime comparebeforetime = restime.AddDays(-1);

            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int period = Convert.ToInt32(context.Request["period"]);

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

            List<List<D_StatDownCountsByResIDEntity>> lists = new List<List<D_StatDownCountsByResIDEntity>>();

            var lstauthor = B_BaseToolService.Instance.GetAuthorEntityCache();

            var result = D_StatDownRank_Service.Instance.GetD_StatDownCountRankByAuthorIDCache(period,restype, softs, platform,
                                                                                            comparebeforetime,restime);
            var res = from item1 in result.AsEnumerable()
                       join item2 in lstauthor.AsEnumerable() on item1.AuthorID equals item2.AuthorID
                       select new D_StatDownCountsByResIDEntity()
                       {
                           StatDate = item1.StatDate,
                           ResID = item1.ResID,
                           AuthorID = item1.AuthorID,
                           AuthorName = item2.AuthorName,
                           DownCount = item1.DownCount,
                           ResCount = item1.ResCount,
                           Rank = item1.Rank,
                           LastRank = item1.LastRank
                       };
            
            if (res.Count() != 0)
            {
                lists.Add(res.ToList<D_StatDownCountsByResIDEntity>());
            }
          
            return lists as List<List<T>>;

        }

    }
}
