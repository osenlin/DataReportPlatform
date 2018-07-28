using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Reports.Entities;
using net91com.Reports.Services;
using net91com.Stat.Web.Reports.Services;
using Newtonsoft.Json;
using net91com.Core.Extensions;
using System.Text.RegularExpressions;
using net91com.Stat.Web.Base;
using net91com.Stat.Services.Entity;
using net91com.Core;
//using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services;

using net91com.Reports.UserRights;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Stat.Web.Reports
{
    public partial class Top20Areas : ReportBasePage
    {
        //public DateTime endtime;

        public string SjqdUrl { get; set; }
        public string SjqdUrlName { get; set; }
        public string SjqdParentUrl { get; set; }
        public int SoftID { get; set; }
        public int Platform { get; set; }
        public int days { get; set; }
        //public string DescriptDate { get; set; }

        //排序方式(=0按新增量排序,=1按涨跌量排序)
        protected int OrderBy;
        //是否是降序
        protected bool Desc;

        //数据库交互对象
        protected UtilityService ds = UtilityService.GetInstance();

        //public List<AreaRankInfo> ranks ;
        protected void Page_Load(object sender, EventArgs e)
        {
            //权限验证
            loginService.HaveUrlRight();

            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[] {
                    net91com.Stat.Core.PeriodOptions.NaturalMonth,
                    net91com.Stat.Core.PeriodOptions.Weekly,
                    net91com.Stat.Core.PeriodOptions.Daily },
                    net91com.Stat.Core.PeriodOptions.Daily);

            HeadControl1.IsPlatSingle = true;
            HeadControl1.IsHasNoPlat = false;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.HiddenTime = false;
            HeadControl1.MySupportSoft = AvailableSofts;
            GetQueryString();
            GetUrl();
        }

        /// <summary>
        /// 获取跳转地址
        /// </summary>
        private void GetUrl()
        {
            Right right = loginService.FindRight("Reports/StatUsersByArea.aspx");
            if (right != null)
            {
                SjqdUrl = right.ID.ToString();
                SjqdParentUrl = right.ParentID.ToString();
                SjqdUrlName = right.Name;
            }

        }
        protected void GetQueryString()
        {
            OrderBy = string.IsNullOrEmpty(Request["orderby"]) ? (string.IsNullOrEmpty(Request["hOrderBy"]) ? 0 : Convert.ToInt32(Request["hOrderBy"])) : Convert.ToInt32(Request["orderby"]);
            Desc = string.IsNullOrEmpty(Request["Desc"]) ? (string.IsNullOrEmpty(Request["hDesc"]) ? true : Convert.ToBoolean(Request["hDesc"])) : Convert.ToBoolean(Request["desc"]);

            HeadControl1.IsHasNoPlat = true;

            //默认模式
            if (HeadControl1.IsFirstLoad)
            {
                SoftID = CookieSoftid;
                Platform = CookiePlatid;
                EndTime = DateTime.Now.Date.AddDays(-1);
                BeginTime = EndTime.AddDays(-30);
                HeadControl1.SoftID = SoftID.ToString();
                HeadControl1.PlatID = Platform.ToString();
            }
            else
            {
                SoftID = Convert.ToInt32(HeadControl1.SoftID);
                Platform = Convert.ToInt32(HeadControl1.PlatID);
                BeginTime = HeadControl1.BeginTime;
                EndTime = HeadControl1.EndTime;
                SetRequestCookie(SoftID, Platform);
            }
            List<Sjqd_StatUsers> ranks = new StatUsersService().GetRankOfCountries(SoftID, Platform, ChannelTypeOptions.Category, 0, (int)PeriodSelector1.SelectedPeriod, ref BeginTime, ref EndTime);
            HeadControl1.BeginTime = BeginTime;
            HeadControl1.EndTime = EndTime;
            switch (OrderBy)
            {
                case 0:
                    ranks = (Desc ? ranks.OrderByDescending(a => a.NewUserCount) : ranks.OrderBy(a => a.NewUserCount)).ToList();
                    break;
                case 1:
                    ranks = (Desc ? ranks.OrderByDescending(a => a.NewUserCount - a.LastNewUserCount) : ranks.OrderBy(a => a.NewUserCount - a.LastNewUserCount)).ToList();
                    break;
                default:
                    ranks = (Desc ? ranks.OrderByDescending(a => (double)a.RetainedUserCount / a.OriginalNewUserCount) : ranks.OrderBy(a => (double)a.RetainedUserCount / a.OriginalNewUserCount)).ToList();
                    break;
            }

            days = (EndTime - BeginTime).Days + 1;

            List<object> list = new List<object>();
            for (int i = 0; i < ranks.Count; i++)
            {
                list.Add(new
                {
                    RankIndex = i + 1,
                    ID = ranks[i].ID,
                    Name = ranks[i].Name,
                    NewUserCount = Utility.SetNum(ranks[i].NewUserCount),
                    NewUserCountDiff = Utility.SetNum(ranks[i].NewUserCount - ranks[i].LastNewUserCount),
                    ActiveUserCount = Utility.SetNum(ranks[i].ActiveUserCount),
                    RetainedUserRate = ranks[i].LastNewUserCount == 0 ? "" : ((decimal)ranks[i].RetainedUserCount / ranks[i].LastNewUserCount * 100).ToString("0.00") + "%",
                    RetainedUserAvgRate = (ranks[i].RetainedUserCountDailyRate * 100).ToString("0.00") + "%",
                    AvgDownCount = (ranks[i].DownUserCount == 0 ? 0.00 : (ranks[i].DownCount * 1.0 / ranks[i].DownUserCount)).ToString("0.00"),
                    AvgDownCount_One = (ranks[i].DownCountNotUpdate == 0 ? 0.00 : (ranks[i].DownCountNotUpdate * 1.0 / ranks[i].DownUserCountNotUpdate)).ToString("0.00")
                });
            }
            Repeater1.DataSource = list;
            Repeater1.DataBind();
        }
    }
}