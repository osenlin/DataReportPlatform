using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Services.Entity;
using System.Text;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Core.Extensions;
using net91com.Core;
using net91com.Stat.Services;

using net91com.Reports.UserRights;
using net91com.Stat.Web.Base;

namespace net91com.Stat.Web.Reports.TransverseReports
{
    public partial class ResolutionReport : ReportBasePage 
    {
        protected net91com.Stat.Core.PeriodOptions MyPeriod { get; set; }
        protected string SeriesJsonStr { get; set; }
        protected string reportTitle { get; set; }
        protected string platformStr { get; set; }
        protected string softStr { get; set; }
        protected List<Resolution> list { get; set; }
        protected int softsid;
        protected int platformsid;

        protected DateTime maxTime { get; set; }
        protected string tableStr { get; set; }
        protected DateTime startTime { get; set; }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID > 0).ToList(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            HeadControl1.MySupportSoft = AvailableSofts;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            HeadControl1.IsHasNoPlat = false;
            HeadControl1.HiddenTime = true;
            HeadControl1.HiddenSingleTime = true;
            HeadControl1.HiddenPeriod = false;
            reportTitle = "分辨率分布";
            if (HeadControl1.IsFirstLoad)
            {
                softsid = CookieSoftid;
                platformsid = CookiePlatid;
                HeadControl1.SoftID = softsid.ToString();
                HeadControl1.PlatID = platformsid.ToString();
                HeadControl1.SinglePeriod = ((int)net91com.Stat.Core.PeriodOptions.LatestOneMonth).ToString();
            }
            else
            {
                platformsid = Convert.ToInt32(HeadControl1.PlatID);
                softsid = Convert.ToInt32(HeadControl1.SoftID);
                SetRequestCookie(softsid, platformsid);
            }
            MyPeriod = (net91com.Stat.Core.PeriodOptions)Convert.ToInt32(HeadControl1.SinglePeriod);
            Bind();
        }
        private void Bind()
        {

            int[] arr = new int[] { 68, 69, -9, 58, 9, 57, 60, 61 };
            if (arr.Contains<int>(softsid))
            {
                string otherKeyString = softsid == 51 ? "SoftID51_" : "";
                maxTime = UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistributionForPC, CacheTimeOption.TenMinutes, otherKeyString);
            }
            else
            {
                string otherKeyString = softsid == 51 ? "SoftID51_" : "";
                maxTime = UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes, otherKeyString);
            }
            startTime=DateTime.Now ;
            switch (MyPeriod)
            {
                case net91com.Stat.Core.PeriodOptions.LatestOneMonth:
                    startTime = maxTime.AddMonths(-1).AddDays(1);
                    break;
                case net91com.Stat.Core.PeriodOptions.LatestOneWeek:
                    startTime = maxTime.AddDays(-6);
                    break;
                case net91com.Stat.Core.PeriodOptions.LatestThreeMonths:
                    startTime = maxTime.AddMonths(-3).AddDays(1);
                    break;
                case net91com.Stat.Core.PeriodOptions.LatestTwoWeeks:
                    startTime = maxTime.AddDays(-13);
                    break;
                case net91com.Stat.Core.PeriodOptions.All:
                    startTime = DateTime.MinValue;
                    break;
            }
            reportTitle = MyPeriod == net91com.Stat.Core.PeriodOptions.All ? "分辨率分布(" + maxTime.ToString("yyyy-MM-dd") + "之前)" : "分辨率分布(" + startTime.ToString("yyyy-MM-dd") + "至" + maxTime.ToString("yyyy-MM-dd") + ")";
            list = TerminalService.GetInstance().GetResolutions(softsid, platformsid, (int)MyPeriod, Convert.ToInt32(maxTime.ToString("yyyyMMdd")));
            if (list.Count == 0)
            {

                SeriesJsonStr = "[]";
                reportTitle = "无数据";
            }
            else
            {
                SeriesJsonStr = GetYlineJson(list);

            }
            tableStr = GetTableString();
        }
        protected string GetYlineJson(List<Resolution> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[{type: 'pie',name: 'Browser share',data: [");
            int count = 0;
            int totalcount=list.Sum(p=>p.UseCount);
            for (int i = 0, j = (list.Count > MaxPerNumber ? MaxPerNumber : list.Count);i<j ; i++)
            {
                if (Convert.ToDecimal(list[i].UseCount) / totalcount * 100 > MinPerRatio)
                {
                    sb.AppendFormat("['{0}',{1}],", string.IsNullOrEmpty(list[i].ResolutionStr) ? "未知分辨率" : list[i].ResolutionStr, list[i].UseCount);
                    count++;
                }
            }
            if (list.Count > count)
            {
                int userCount = 0;
                var softList = list.Skip(count).Take(list.Count - count).ToList();
                foreach (var resolu in softList)
                {
                    userCount += resolu.UseCount;
                }
                sb.AppendFormat("['{0}',{1}],", "其他", userCount);
            }
            return sb.ToString().TrimEnd(',') + "]}]";
        }

        protected string GetTableString()
        {
            StringBuilder sb = new StringBuilder();
            string tableName = softStr + "_" + platformStr + "_" + MyPeriod.GetDescription();
            sb = new StringBuilder("<table  class=\" tablesorter \"" + "name=\"" + tableName + "\"" + "cellspacing=\"1\">");

            sb.Append(@" <thead><tr><th>分辨率</th>
                <th>用户数</th>     
                <th>百分比</th>
                <th></th>        
                </tr></thead>");
            decimal usercount = 0;
            int allcount = list.Sum(p => p.UseCount);
            for (int i = 0; i < list.Count; i++)
            {
                if (i < 100)
                {
                    sb.Append("<tr   class=\"tableover\"  >");
                    sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td><td><span class=\"find\" onclick=\"showLine({3},'{0}')\" id=\"lbl{3}\">查看每天</span></td>", string.IsNullOrEmpty(list[i].ResolutionStr) ? "未适配分辨率" : list[i].ResolutionStr,
                        list[i].UseCount, Math.Round(Convert.ToDecimal(list[i].UseCount) / allcount * 100, 2) + "%", i);
                    sb.Append("</tr>");
                    sb.AppendFormat("<tr class=\"divtr\" id=\"tr{0}\" style=\"display:none;height:400px;\"><td colspan=\"4\"><div id=\"div{0}\"> </div>" + "</td></tr>", i);
                }
                else
                {
                    usercount += list[i].UseCount;
                }
            }
            if (usercount != 0)
            {
                sb.Append("<tr   class=\"tableover\"  >");
                sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td> ", "其他", usercount, Math.Round((usercount / allcount * 100), 2) + "%");
                sb.Append("</tr>");

            }
            sb.Append("</tbody></table>");
            return sb.ToString();


        }
    }
}