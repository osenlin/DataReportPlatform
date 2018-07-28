using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Web.Base;
using net91com.Stat.Web.Reports.Services;
using net91com.Core.Extensions;
using System.Text;
using net91com.Stat.Services.Entity;
using net91com.Core;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.TransverseReports
{
    public partial class SoftXhTransverse : ReportBasePage 
    {
        #region 字段属性
        //这个是平台字符串，前台绑定了的，格式例如 iphone,ipad,wm
        protected string platformStr = "";
        //这个是软件字符,前台绑定的，格式例如 看书,空间，来电秀
        protected string softStr = "";
        
        /// 软件列表
        public int softsid;

        /// 用户选择平台数组 
        protected int platformsid;


        ///用户绑定图的title
        public string reportTitle { get; set; }

        /// <summary>
        /// 具体点坐标的json字符
        /// </summary>
        public string SeriesJsonStr { get; set; }
        /// <summary>
        /// htmltable 字符串
        /// </summary>
        public string tableStr { get; set; }

        public List<Sjqd_StatUsersBySbxh> allSoftlan;
        #endregion

        public DateTime maxTime { get; set; }

        public DateTime startTime { get; set; }

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

            GetQueryString();
            BindData();

        }
        protected void GetQueryString()
        {
            HeadControl1.MySupportSoft = AvailableSofts;

            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            HeadControl1.HiddenTime = true;
            HeadControl1.HiddenSingleTime = true;
            HeadControl1.HiddenPeriod = false;
            HeadControl1.IsHasNoPlat = false;
            
            
             
             
           
            if (HeadControl1.IsFirstLoad)
            {

                HeadControl1.SinglePeriod = ((int)net91com.Stat.Core.PeriodOptions.LatestOneMonth).ToString();
                softsid = CookieSoftid;
                platformsid = CookiePlatid;
                HeadControl1.SoftID = softsid.ToString();
                HeadControl1.PlatID = platformsid.ToString();
            }
            ///用户选择模式
            else
            {
                platformsid = Convert.ToInt32(HeadControl1.PlatID);
                softsid = Convert.ToInt32(HeadControl1.SoftID);
                SetRequestCookie(softsid, platformsid);
            }

            Period = (net91com.Stat.Core.PeriodOptions)Convert.ToInt32(HeadControl1.SinglePeriod);
        }

        /// </summary>获取数据加上绑定数据
        protected void BindData()
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
            startTime = DateTime.Now;
            switch (Period)
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
            reportTitle = Period == net91com.Stat.Core.PeriodOptions.All ? "软件机型分布(" + maxTime.ToString("yyyy-MM-dd") + "之前)" : "软件机型分布(" + startTime.ToString("yyyy-MM-dd") + "至" + maxTime.ToString("yyyy-MM-dd") + ")";
            allSoftlan = StatUsersBySbxhService.GetInStance().GetSoftSBXHTransverse(Period, Convert.ToInt32(maxTime.ToString("yyyyMMdd")), 
                                softsid, (MobileOption)platformsid);
            ///一条线都的不出来
            if (allSoftlan.Count == 0)
            {

                SeriesJsonStr = "[]";
                reportTitle = "无数据";
            }
            else
            {
                SeriesJsonStr = GetYlineJson(allSoftlan);
                
            }
            tableStr = GetTableString();
        }

        /// <summary>
        /// 形成每一个tab
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tableindex"></param>
        /// <returns></returns>
        protected string GetTableString()
        {
            StringBuilder sb = new StringBuilder();
            string tableName = softStr + "_" + platformStr + "_" + Period.GetDescription();
            sb = new StringBuilder("<table  class=\" tablesorter \"" + "name=\"" + tableName + "\"" + "cellspacing=\"1\">");

            sb.Append(@" <thead><tr><th>机型</th>
                <th>用户数</th>     
                <th>百分比</th>   
                <th></th>   
                </tr></thead>");
            decimal usercount = 0;
            int allcount = allSoftlan.Sum(p => p.UseCount);
            for (int i = 0; i < allSoftlan.Count; i++)
            {
                if (i < 100)
                {
                    sb.Append("<tr   class=\"tableover\"  >");
                    sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td><td><span class=\"find\" onclick=\"showLine({3},'{0}')\" id=\"lbl{3}\">查看每天</span></td> ",
                        (string.IsNullOrEmpty(allSoftlan[i].Sbxh) ? "未适配机型" : allSoftlan[i].Sbxh), allSoftlan[i].UseCount, Math.Round(Convert.ToDecimal(allSoftlan[i].UseCount) / allcount * 100, 2) + "%",i);
                    sb.Append("</tr>");
                    sb.AppendFormat("<tr class=\"divtr\" id=\"tr{0}\" style=\"display:none;height:400px;\"><td colspan=\"4\"><div id=\"div{0}\"> </div>" + "</td></tr>", i);
                }
                else
                {
                    usercount += allSoftlan[i].UseCount;
                }
            }
            if (usercount != 0)
            {
                sb.Append("<tr   class=\"tableover\"  >");
                sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td><td> </td> ", "其他", usercount, Math.Round((usercount / allcount * 100), 2) + "%");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
            return sb.ToString();


        }

        protected string GetYlineJson(List<Sjqd_StatUsersBySbxh> list)
        {
            int allUserCount = list.Sum(p => p.UseCount);
            StringBuilder sb = new StringBuilder();
            sb.Append("[{type: 'pie',name: 'Browser share',data: [");
            int count = 0;
            for (int i = 0; i < (list.Count > MaxPerNumber ? MaxPerNumber : list.Count); i++)
            {
                if (Convert.ToDecimal(list[i].UseCount) / allUserCount * 100 > MinPerRatio)
                {
                    sb.AppendFormat("['{0}',{1}],", string.IsNullOrEmpty(list[i].Sbxh) ? "未适配机型" : list[i].Sbxh, list[i].UseCount);
                    count++;
                }
            }
            if (list.Count > count)
            {
                int userCount = 0;
                var softList = list.Skip(count).Take(list.Count - count).ToList();
                foreach (var gjbb in softList)
                {
                    userCount += gjbb.UseCount;
                }
                sb.AppendFormat("['{0}',{1}],", "其他", userCount);
            }
            return sb.ToString().TrimEnd(',') + "]}]";
        }
    }
}