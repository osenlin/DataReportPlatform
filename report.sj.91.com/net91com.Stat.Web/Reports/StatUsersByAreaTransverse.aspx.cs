using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Web.Base;
using net91com.Core.Extensions;
using System.Text;
using net91com.Stat.Services.Entity;
using net91com.Core;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports
{
    public partial class StatUsersByAreaTransverse : ReportBasePage 
    {
        #region 字段属性

        /// <summary>
        /// 地区类型
        /// </summary>
        protected int AreaType;

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

        /// <summary>
        /// EXCEL文件下载请求参数
        /// </summary>
        protected string ExcelDownUrl;

        /// <summary>
        /// 获取每天曲线数据的请求接口地址
        /// </summary>
        protected string GetStatUsersByAreaLineUrl;

        #endregion

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

            AreaType = 1;

            HeadControl1.MySupportSoft = AvailableSofts;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            HeadControl1.HiddenTime = false;
            HeadControl1.IsHasNoPlat = true;
            HeadControl1.Channel1.MaxCheckNumber = 1;
            HeadControl1.Channel1.IsHasNoPlat = true;
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[]
                    {
                        net91com.Stat.Core.PeriodOptions.NaturalMonth,
                        net91com.Stat.Core.PeriodOptions.Weekly,
                        net91com.Stat.Core.PeriodOptions.Daily,
                        net91com.Stat.Core.PeriodOptions.LatestOneWeek,
                        net91com.Stat.Core.PeriodOptions.LatestTwoWeeks,
                        net91com.Stat.Core.PeriodOptions.LatestOneMonth,
                        net91com.Stat.Core.PeriodOptions.LatestThreeMonths
                    },
                net91com.Stat.Core.PeriodOptions.LatestOneMonth);
            Period = PeriodSelector1.SelectedPeriod;

            GetQueryString();
            BindData();
        }

        protected void GetQueryString()
        {
            if (HeadControl1.IsFirstLoad)
            {
                softsid = CookieSoftid;
                platformsid = CookiePlatid;
                EndTime = DateTime.Now.Date.AddDays(-1);
                BeginTime = EndTime.AddDays(-30);
                HeadControl1.PlatID = platformsid.ToString();
                HeadControl1.SoftID = softsid.ToString();                
                HeadControl1.Channel1.SoftId = HeadControl1.SoftID.ToString();
                HeadControl1.Channel1.Platform = HeadControl1.PlatID.ToString();
            }
            else
            {
                platformsid = Convert.ToInt32(HeadControl1.PlatID);
                softsid = Convert.ToInt32(HeadControl1.SoftID);
                BeginTime = HeadControl1.BeginTime;
                EndTime = HeadControl1.EndTime;
                SetRequestCookie(softsid, platformsid);

            }
            HeadControl1.Channel1.PeriodCheck = false;
            Period = PeriodSelector1.SelectedPeriod;
        }

        /// <summary>
        /// 获取数据加上绑定数据
        /// </summary>
        protected void BindData()
        {
            int channelId = 0;
            ChannelTypeOptions channelType = ChannelTypeOptions.Category;
            if (HeadControl1.Channel1.ChannelValues.Count > 0)
            {
                channelId =  HeadControl1.Channel1.ChannelValues[0].ChannelValue;
                channelType = HeadControl1.Channel1.ChannelValues[0].ChannelType;
            }
           StatUsersService suService = new StatUsersService();
            List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> users = AreaType == 1
                ? suService.GetRankOfCountries(softsid, platformsid, channelType, channelId, (int)Period, ref BeginTime, ref EndTime)
                : (AreaType == 2 ? suService.GetRankOfProvinces(softsid, platformsid, channelType, channelId, (int)Period, ref BeginTime, ref EndTime)
                : suService.GetRankOfCities(softsid, platformsid, channelType, channelId, (int)Period, ref BeginTime, ref EndTime));

            if (reporttype.Value == "0")
            {
                users = users.OrderByDescending(a => a.NewUserCount).ToList();
            }
            else
            {
                users = users.OrderByDescending(a => a.ActiveUserCount).ToList();
            }

            HeadControl1.BeginTime = BeginTime;
            HeadControl1.EndTime = EndTime;
            reportTitle = string.Format("地区分布({0:yyyy-MM-dd}至{1:yyyy-MM-dd})", BeginTime, EndTime);  
            
            ExcelDownUrl = string.Format("/Services/ExcelDownloader.ashx?Action=GetStatUsersByAreaTransverse&SoftID={0}&Platform={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&ChannelIds={5}&ChannelTypes={6}&ChannelTexts={7}&v={8}&AreaType={9}", softsid, platformsid, (int)Period, BeginTime, EndTime, HeadControl1.Channel1.SelectedValue, HeadControl1.Channel1.SelectedCate, HttpUtility.UrlEncode(HeadControl1.Channel1.SelectedText), DateTime.Now.Ticks, AreaType);
            GetStatUsersByAreaLineUrl = string.Format("/Services/GetMore.ashx?Action=GetStatUsersByAreaLine&SoftIds={0}&Platforms={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&ChannelIds={5}&ChannelTypes={6}&ChannelTexts={7}&v={8}", softsid, platformsid, (int)net91com.Stat.Core.PeriodOptions.Daily, BeginTime, EndTime, HeadControl1.Channel1.SelectedValue, HeadControl1.Channel1.SelectedCate, HttpUtility.UrlEncode(HeadControl1.Channel1.SelectedText), DateTime.Now.Ticks);

            if (users.Count == 0)
            {
                SeriesJsonStr = "[]";
                reportTitle = "无数据";
            }
            else
            {
                SeriesJsonStr = GetYlineJson(users);
            }
            tableStr = GetTableString(users);
        }

        /// <summary>
        /// 形成每一个tab
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tableindex"></param>
        /// <returns></returns>
        protected string GetTableString(List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> users)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class=\" tablesorter \" cellspacing=\"1\">");
            sb.Append(@"<thead><tr style=""text-align:center;""><th>排名</th><th>地区</th><th>新增用户</th><th>新增占比</th><th>涨跌量</th><th>活跃用户</th><th>活跃占比</th><th>留存率(上周期)</th><th>操作</th></tr></thead><tbody>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.AppendFormat(@"<tr style=""text-align:right;""><td>{0}</td><td style=""text-align:left;"">{1}</td><td>{2:N0}</td>
                                  <td>{3:0.00}%</td><td>{4:N0}</td><td>{5:N0}</td><td>{6:0.00}%</td><td>{7:0.00}</td>
                                  <td style=""text-align:left;""><span class=""find"" onclick=""showLine('{8}')"" id=""lbl{8}"">查看每天</span></td></tr>", i + 1
                                                                                                               , users[i].Name
                                                                                                               , users[i].NewUserCount
                                                                                                               , users[i].NewUserPercent * 100
                                                                                                               , users[i].NewUserCount - users[i].LastNewUserCount
                                                                                                               , users[i].ActiveUserCount
                                                                                                               , users[i].ActiveUserPercent * 100
                                                                                                               , users[i].LastNewUserCount > 0 ? (((double)users[i].RetainedUserCount) *100 / users[i].LastNewUserCount).ToString("0.00") + "%" : ""
                                                                                                               , users[i].IdName);
                sb.AppendFormat("<tr class=\"divtr\" id=\"tr{0}\" style=\"display:none;height:400px;\"><td colspan=\"9\"><div id=\"div{0}\"></div></td></tr>", users[i].IdName);
            }
            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        protected string GetYlineJson(List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> users)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[{type: 'pie',name: 'Browser share',data: [");
            int count = 0;
            for (int i = 0; i < (users.Count > MaxPerNumber ? MaxPerNumber : users.Count); i++)
            {
                double percent = reporttype.Value == "0" ? users[i].NewUserPercent : users[i].ActiveUserPercent;
                int userCount = reporttype.Value == "0" ? users[i].NewUserCount : users[i].ActiveUserCount;
                if (percent * 100 > MinPerRatio)
                {
                    sb.AppendFormat("['{0}',{1}],", users[i].Name, userCount);
                    count++;
                }
            }
            if (users.Count > count)
            {
                int userCount = 0;
                foreach (var user in users.Skip(count).Take(users.Count - count))
                {
                    userCount += reporttype.Value == "0" ? user.NewUserCount : user.ActiveUserCount;
                }
                sb.AppendFormat("['{0}',{1}],", "其他", userCount);
            }
            return sb.ToString().TrimEnd(',') + "]}]";
        }
    }
}