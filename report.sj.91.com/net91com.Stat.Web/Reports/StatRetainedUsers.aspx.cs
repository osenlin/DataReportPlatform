using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Stat.Web.Reports.Services;
using Newtonsoft.Json;
using net91com.Core.Extensions;
using net91com.Stat.Web.Base;
using Res91com.ResourceDataAccess;
using net91com.Stat.Services.Entity;
using net91com.Core;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services;

using net91com.Reports.UserRights;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.SjqdUserStat;

namespace net91com.Stat.Web.Reports
{
    public partial class StatRetainedUsers : ReportBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            //权限验证
            loginService.HaveUrlRight();

            base.OnInit(e);
        }

        /// <summary>
        /// 数据项名称
        /// </summary>
        protected List<string> DataNames = new List<string>();

        /// <summary>
        /// 平台id  
        /// </summary>
        protected int PlatformID
        {
            get
            {
                if (HeadControl1.IsFirstLoad)
                {
                    return CookiePlatid;
                }
                else
                {
                    return Convert.ToInt32(HeadControl1.PlatID);
                }
            }
        }
        /// <summary>
        /// 软件列表
        /// </summary>
        protected int SoftID
        {
            get
            {
                if (HeadControl1.IsFirstLoad)
                {
                    return CookieSoftid;
                }
                else
                {
                    return Convert.ToInt32(HeadControl1.SoftID);
                }
            }
        }
        /// <summary>
        /// 表格HTML
        /// </summary>
        protected string TablesHtml { get; set; }
        /// <summary>
        /// 用户绑定图的title
        /// </summary>
        protected string ReportTitle { get; set; }
        /// <summary>
        /// 横轴显示的json字符
        /// </summary>
        protected string AxisJsonStr;
        /// <summary>
        /// 具体点坐标的json字符
        /// </summary>
        protected string SeriesJsonStr;
        /// <summary>
        /// EXCEL文件下载请求参数
        /// </summary>
        protected string ExcelDownUrl;        

        protected void Page_Load(object sender, EventArgs e)
        {
            HeadControl1.MySupportSoft = AvailableSofts;
            HeadControl1.IsHasNoPlat = true;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            HeadControl1.HiddenCompareTime = true;
            HeadControl1.Channel1.IsHasNoPlat = true;
            HeadControl1.Channel1.MaxCheckNumber = 1;

            GetQueryString();
            BindData();
        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => (a.ID > 0)).ToList(); }
        }

        protected void GetQueryString()
        {
            //初次加载一些参数设置成默认值
            ReportTitle = "新增用户留存率";
            AxisJsonStr = "{}";
            SeriesJsonStr = "[]";
            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                    new net91com.Stat.Core.PeriodOptions[] {
                            net91com.Stat.Core.PeriodOptions.NaturalMonth,
                            net91com.Stat.Core.PeriodOptions.Weekly,
                            net91com.Stat.Core.PeriodOptions.Daily },
                            net91com.Stat.Core.PeriodOptions.Daily);
            Period = PeriodSelector1.SelectedPeriod;
            if (HeadControl1.IsFirstLoad)
            {
                //获取默认时间
                EndTime = DateTime.Now.Date.AddDays(-1); //ds.GetMaxTimeCache(Period, ReportType.UserRetained,Core.CacheTimeOption.TenMinutes);
                BeginTime = EndTime.AddDays(-50);
                HeadControl1.PlatID = CookiePlatid.ToString();
                HeadControl1.SoftID = CookieSoftid.ToString();
                HeadControl1.BeginTime = BeginTime;
                HeadControl1.EndTime = EndTime;
                HeadControl1.Channel1.SoftId = HeadControl1.SoftID.ToString();
                HeadControl1.Channel1.Platform = HeadControl1.PlatID.ToString();
            }
            else //用户选择模式
            {                 
                BeginTime =HeadControl1.BeginTime;
                EndTime =HeadControl1.EndTime;
                SetRequestCookie(Convert.ToInt32(HeadControl1.SoftID), Convert.ToInt32(HeadControl1.PlatID));

            }
            HeadControl1.Channel1.PeriodCheck = false;
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
                channelId = HeadControl1.Channel1.ChannelValues[0].ChannelValue;
                channelType = HeadControl1.Channel1.ChannelValues[0].ChannelType;
            }
            StatUsersService suService = new StatUsersService();
            List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatRetainedUsers> users = suService.GetStatRetainedUsers(SoftID, PlatformID, channelId, channelType, (int)Period, BeginTime, EndTime);

            ExcelDownUrl = string.Format("/Services/ExcelDownloader.ashx?Action=GetStatRetainedUsers&SoftID={0}&Platform={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&ChannelIds={5}&ChannelTypes={6}&ChannelTexts={7}&v={8}", SoftID, PlatformID, (int)Period, BeginTime, EndTime, HeadControl1.Channel1.SelectedValue, HeadControl1.Channel1.SelectedCate, HttpUtility.UrlEncode(HeadControl1.Channel1.SelectedText), DateTime.Now.Ticks);

            SeriesJsonStr = LineChartHelper.BuildStatRetainedUsersLine(users, Period, BeginTime, EndTime, out AxisJsonStr);

            TablesHtml = TableTemplateHelper.BuildStatRetainedUsersTable((int)Period, users, false, string.Empty);
        }
        
    }
}
 
  
 
 