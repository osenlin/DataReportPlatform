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
    public class D_HttpDownStatDownRankByAreaMImpl : MAbs_HttpDownLoad
    {

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "排行", "资源ID", "资源名称", "资源包名", "下载数", "上期排行"
                    };
            }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable(List<Dictionary<string,string>> list,bool DetailEqChart=true)
        {
            var tempList = new List<List<string>>();
            foreach (Dictionary<string, string> obj in list)
            {
                List<string> values = new List<string>();
                values.Add(obj["rank"]);
                values.Add(UtilityHelp.FormatNum<int>(int.Parse(obj["resid"])));
                values.Add(obj["resname"]);
                values.Add(obj["residentifier"]);
                values.Add(UtilityHelp.FormatNum<int>(int.Parse(obj["realdowncount"])));
                values.Add(string.IsNullOrEmpty(obj["lastrank"]) ? "--" : obj["lastrank"]);
                tempList.Add(values);
            }
            return tempList;
        }

        protected override Result GetDetailTable(HttpContext context)
        {

            var lists = GetData(context, true);
            var list = lists.Count != 0 ? lists[0] : new List<Dictionary<string,string>>();
            DataTablesRequest param = new DataTablesRequest(context.Request);
            JQueryDataTableData dt = new JQueryDataTableData();
            dt.sEcho = param.sEcho;
            dt.iTotalRecords = dt.iTotalDisplayRecords = list.Count;
            dt.aaData = ObjectParseListFillDetailTable(list);

            DateTime restime = Convert.ToDateTime(context.Request["begintime"]);
            int period = Convert.ToInt32(context.Request["period"]);
            if (1==period)
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
                dt.endtime = restime.AddDays(-1 * (restime.Day) ).ToString("yyyy-MM-dd");
            }
            return Result.GetSuccessedResult(dt, false);
        }

        protected override string DownloadExcelName
        {
            get { return "资源下载排行（区域）.xls"; }
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownRankByAreaImpl"; }
        }

        public override string ServiceName
        {
            get { return "资源下载排行（区域）"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/D_DownlaodStatistics/D_ResDownLoadRankByArea.aspx"; }
        }

        protected override List<List<Dictionary<string, string>>> GetData(HttpContext context, bool flag = false)
        {
            DateTime restime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime comparebeforetime = restime.AddDays(-1);
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int period = Convert.ToInt32(context.Request["period"]);
            int areaid = Convert.ToInt32(context.Request["areaid"]);
            int areatype = Convert.ToInt32(context.Request["areatype"]);
            int downtype = Convert.ToInt32(context.Request["downtype"]);
            int countryid = -1;
            int provinceid = -1;

            if (2 == areatype)
            {
                countryid = areaid;
                provinceid = -1;
            }

            if (1 == areatype)
            {
                countryid = 253;
                provinceid = areaid;
                if (areaid==-1)
                {
                    countryid = areaid;
                }
            }

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

            List<List<Dictionary<string, string>>> lists = new List<List<Dictionary<string, string>>>();

            List<Dictionary<string, string>> result = D_StatDownRank_Service.Instance.GetD_StatDownRankByAreaByCacheMap(restype, softs, platform,
                                                                                            comparebeforetime, restime, period,countryid, provinceid, downtype);
            if (result.Count == 0)
            {
                return new List<List<Dictionary<string, string>>>();
            }
            lists.Add(result);

            return lists;
        }
    }
}
