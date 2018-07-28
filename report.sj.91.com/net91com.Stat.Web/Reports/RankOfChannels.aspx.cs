using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Reports.Entities;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Services;
using net91com.Stat.Web.Reports.Services;
using Newtonsoft.Json;
using net91com.Core.Extensions;
using System.Text.RegularExpressions;
using net91com.Stat.Web.Base;
using net91com.Stat.Services.Entity;
using net91com.Core;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services;
using net91com.Reports.Services.CommonServices.SjqdUserStat;

using net91com.Reports.UserRights;
using net91com.Stat.Core;

namespace net91com.Stat.Web.Reports
{
    public partial class RankOfChannels : ReportBasePage 
    {
        ///// <summary>
        ///// 当前选择的日期
        ///// </summary>
        //protected DateTime StatDate;
       
        public string SjqdUrl { get; set; }
        public string SjqdUrlName { get; set; }
        public string SjqdParentUrl { get; set; }
        public int SoftID { get; set; }
        public int Platform { get; set; }
        //public string DescriptDate { get; set; }

        //排序方式(=0按新增量排序,=1按涨跌量排序)
        protected int OrderBy;
        //是否是降序
        protected bool Desc;

        /// <summary>
        /// 获取子渠道排行异步请求接口地址
        /// </summary>
        protected string GetRankOfSubChannelsUrl;

        /// <summary>
        /// 获取版本排行异步请求接口地址
        /// </summary>
        protected string GetRankOfVersionsUrl;

        /// <summary>
        /// 获取国家排行异步请求接口地址
        /// </summary>
        protected string GetRankOfCountriesUrl;

        /// <summary>
        /// 获取省排行异步请求接口地址
        /// </summary>
        protected string GetRankOfProvincesUrl;

        /// <summary>
        /// 获取市排行异步请求接口地址
        /// </summary>
        protected string GetRankOfCitiesUrl;

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID > 0).ToList(); }
        }

        
        protected void Page_Load(object sender, EventArgs e)
        {
            //权限验证
            loginService.HaveUrlRight();

            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                new PeriodOptions[] {
                    PeriodOptions.NaturalMonth,
                    PeriodOptions.Weekly,
                    PeriodOptions.Daily },
                    PeriodOptions.Daily);

            HeadControl1.IsPlatSingle = true;
            HeadControl1.IsHasNoPlat = true;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.HiddenTime = false;
            //HeadControl1.HiddenSingleTime = false;
            HeadControl1.MySupportSoft = AvailableSofts;
            GetQueryString();
            GetUrl();
        }

        /// <summary>
        /// 获取跳转地址
        /// </summary>
        private void GetUrl()
        {
            Right right = loginService.FindRight("Reports/SoftVersionSjqd.aspx");
            if (right != null)
            {
                SjqdUrl = right.ID.ToString();
                SjqdParentUrl = right.ParentID.ToString();
                SjqdUrlName = right.Name;
            }

        }
        protected void GetQueryString()
        {
            

            //默认模式
            if (HeadControl1.IsFirstLoad)
            {
                SoftID = CookieSoftid;
                Platform = CookiePlatid;
                //StatDate = UtilityService.GetInstance().GetMaxTimeCache(PeriodSelector1.SelectedPeriod, ReportType.UserUseNewActivity,CacheTimeOption.TenMinutes);
                //StatDate = StatDate.Date;
                EndTime = DateTime.Now.Date.AddDays(-1);
                BeginTime = EndTime.AddDays(-30);
                //HeadControl1.SingleTime = StatDate;
                HeadControl1.SoftID = SoftID.ToString();
                HeadControl1.PlatID = Platform.ToString();
            }
            else
            {
                SoftID = Convert.ToInt32(HeadControl1.SoftID);
                Platform = Convert.ToInt32(HeadControl1.PlatID);
                //StatDate =HeadControl1.SingleTime;
                BeginTime = HeadControl1.BeginTime;
                EndTime = HeadControl1.EndTime;
                //UtilityHelp.SpecificateSingleTime(ref StatDate, (Report_Period)Convert.ToInt32(PeriodSelector1.SelectedPeriod));
                SetRequestCookie(SoftID, Platform);
            }
            int period = Convert.ToInt32(PeriodSelector1.SelectedPeriod);
            

            OrderBy = string.IsNullOrEmpty(Request["orderby"]) ? (string.IsNullOrEmpty(Request["hOrderBy"]) ? 0 : Convert.ToInt32(Request["hOrderBy"])) : Convert.ToInt32(Request["orderby"]);
            Desc = string.IsNullOrEmpty(Request["Desc"]) ? (string.IsNullOrEmpty(Request["hDesc"]) ? true : Convert.ToBoolean(Request["hDesc"])) : Convert.ToBoolean(Request["desc"]);

            List<Sjqd_StatUsers> users = new StatUsersService().GetRankOfChannels(SoftID, Platform, 0, (int)PeriodSelector1.SelectedPeriod, ref BeginTime, ref EndTime);

            HeadControl1.BeginTime = BeginTime;
            HeadControl1.EndTime = EndTime;
            //SetDescriptDate(EndTime, (Report_Period)period);

            GetRankOfCitiesUrl = string.Format("/Services/GetMore.ashx?Action=GetRankOfAreas&AreaType=3&SoftIds={0}&Platforms={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&OrderBy={5}&Desc={6}&v={7}&ChannelTypes={8}", SoftID, Platform, period, BeginTime, EndTime, OrderBy, Desc, DateTime.Now.Ticks, (int)ChannelTypeOptions.Customer);
            GetRankOfProvincesUrl = string.Format("/Services/GetMore.ashx?Action=GetRankOfAreas&AreaType=2&SoftIds={0}&Platforms={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&OrderBy={5}&Desc={6}&v={7}&ChannelTypes={8}", SoftID, Platform, period, BeginTime, EndTime, OrderBy, Desc, DateTime.Now.Ticks, (int)ChannelTypeOptions.Customer);
            GetRankOfCountriesUrl = string.Format("/Services/GetMore.ashx?Action=GetRankOfAreas&AreaType=1&SoftIds={0}&Platforms={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&OrderBy={5}&Desc={6}&v={7}&ChannelTypes={8}", SoftID, Platform, period, BeginTime, EndTime, OrderBy, Desc, DateTime.Now.Ticks, (int)ChannelTypeOptions.Customer);
            GetRankOfVersionsUrl = string.Format("/Services/GetMore.ashx?Action=GetRankOfVersions&SoftIds={0}&Platforms={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&OrderBy={5}&Desc={6}&v={7}&ChannelTypes={8}", SoftID, Platform, period, BeginTime, EndTime, OrderBy, Desc, DateTime.Now.Ticks, (int)ChannelTypeOptions.Customer);
            GetRankOfSubChannelsUrl = string.Format("/Services/GetMore.ashx?Action=GetRankOfSubChannels&SoftIds={0}&Platforms={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&OrderBy={5}&Desc={6}&v={7}&ChannelTypes={8}", SoftID, Platform, period, BeginTime, EndTime, OrderBy, Desc, DateTime.Now.Ticks, (int)ChannelTypeOptions.Customer);

            
            
            if (Desc)
                users = (OrderBy == 0 ? users.OrderByDescending(a => a.NewUserCount) : users.OrderByDescending(a => a.NewUserCount - a.LastNewUserCount)).ToList();
            else
                users = (OrderBy == 0 ? users.OrderBy(a => a.NewUserCount) : users.OrderBy(a => a.NewUserCount - a.LastNewUserCount)).ToList();
            List<object> list = new List<object>();
            for (int i = 0; i < users.Count; i++)
            {
                list.Add(new
                {
                    RankIndex = i + 1,
                    ChannelID = users[i].ID,
                    ChannelName = users[i].Name,
                    NewUserCount = Utility.SetNum(users[i].NewUserCount),
                    NewUserCountDiff = Utility.SetNum(users[i].NewUserCount - users[i].LastNewUserCount),
                    ActiveUserCount = Utility.SetNum(users[i].ActiveUserCount),
                    ActiveUserRate = users[i].TotalUserCount == 0 ? "" : ((decimal)users[i].ActiveUserCount / users[i].TotalUserCount * 100).ToString("0.00") + "%",
                    OneTimeUserCount = users[i].TotalUserCount == 0 ? "" : Utility.SetNum(users[i].OneTimeUserCount) + (users[i].TotalUserCount == 0 ? "(--)" : ("(" + ((decimal)users[i].OneTimeUserCount / users[i].TotalUserCount * 100).ToString("0.00") + "%)")),
                    RetainedUserRate = users[i].OriginalNewUserCount == 0 ? "" : ((decimal)users[i].RetainedUserCount / users[i].OriginalNewUserCount * 100).ToString("0.00") + "%",
                    TotalUserCount = Utility.SetNum(users[i].TotalUserCount)
                });
            }
            Repeater1.DataSource = list;
            Repeater1.DataBind();

        }
        ///// <summary>
        ///// 设置描述日期
        ///// </summary>
        //protected void SetDescriptDate(DateTime time,Report_Period period)
        //{ 
        //    if (period == Report_Period.Daily)
        //    {
        //        DescriptDate = time.ToString("yyyy-MM-dd");
        //    }
        //    else if (period == Report_Period.Weekly)
        //    {
        //        DescriptDate = time.AddDays(-6).ToString("yyyy-MM-dd") + "至" + time.ToString("yyyy-MM-dd");
        //    }
        //    else 
        //    {
        //        DescriptDate = time.AddMonths(-1).AddDays(1).ToString("yyyy-MM-dd") + "至" + time.ToString("yyyy-MM-dd");
        //    }
        //}
    }
}