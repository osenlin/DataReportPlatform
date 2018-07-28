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
using net91com.Stat.Services.Entity;
using net91com.Core;
using net91com.Stat.Web.Reports.sjqd;
using net91com.Stat.Services.sjqd;
using System.Text.RegularExpressions;
using net91com.Stat.Services;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.UserRights;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Services.CommonServices.B_BaseTool;

namespace net91com.Stat.Web.Reports
{
    public partial class StatUsersByVersion : ReportBasePage
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        protected B_VersionEntity DefaultVersion;

        /// <summary>
        /// 数据项名称
        /// </summary>
        protected List<string> DataNames = new List<string>();
        ///// <summary>
        ///// 数据参数
        ///// </summary>
        //protected List<string> DataParams = new List<string>();

        // 平台id  
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
        // 软件列表
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

        
        public string reportTitle = "无数据";
        public string AxisJsonStr = "{}";
        public string SeriesJsonStr = "[]";
        //多条线
        public List<List<Sjqd_StatUsers>> ListAll = new List<List<Sjqd_StatUsers>>();

        public int usercateType
        {
            get { return Convert.ToInt32(Request["t"] == "" ? "0" : Request["t"]); }
        } 

        /// <summary>
        /// 获取基础数据类
        /// </summary>
        protected UtilityService ds = UtilityService.GetInstance();

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { 
                var list= loginService.AvailableSofts.Where(a => a.ID > 0).ToList();
                if (list.Count == 0)
                    throw new NotRightException();
                return list;
            }
        }

        /// <summary>
        /// EXCEL文件下载请求参数
        /// </summary>
        protected string ExcelDownUrl;

        #region 获取版本列表(列表框使用)(GetVersionDict)

        private Dictionary<string, string> versionDict = null;
        /// <summary>
        /// 获取地区列表(列表框使用)
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetVersionDict()
        {
            if (versionDict == null)
            {
                List<B_VersionEntity> versionList = B_BaseToolService.Instance.GetVersionCache(SoftID, PlatformID);

                if (versionList.Count==0)
                {
                    versionList = B_BaseToolService.Instance.GetVersionCache(SoftID, 4);
                }

                DefaultVersion = versionList[0];
                versionDict = new Dictionary<string, string>();
                foreach (B_VersionEntity version in versionList)
                    versionDict.Add(version.Version, version.Version);
            }
            return versionDict;
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();
         
            HeadControl1.MySupportSoft = AvailableSofts;
            HeadControl1.IsHasNoPlat = false;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            HeadControl1.IsVersionSingle = false;
            HeadControl1.HiddenCompareTime = true;
            HeadControl1.HiddenVersion = false;
            HeadControl1.IsHasNoVersion = false;
            HeadControl1.NewVersionInfo = true;
            if (HeadControl1.IsFirstLoad)
            {
                GetVersionDict();
                HeadControl1.VersionID = DefaultVersion.Version;
            }
            //让弹出的渠道商下拉控件也支持不区分平台            
            HeadControl1.Channel1.IsHasNoPlat = false;
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[]
                    {
                        net91com.Stat.Core.PeriodOptions.NaturalMonth,
                        net91com.Stat.Core.PeriodOptions.Weekly,
                        net91com.Stat.Core.PeriodOptions.Daily
                    },
                net91com.Stat.Core.PeriodOptions.Daily);
            Period = PeriodSelector1.SelectedPeriod;
            GetQueryString();
            SetStandardTime();           
            BindData();

        }

        protected void GetQueryString()
        {
            if (HeadControl1.IsFirstLoad)
            {
                EndTime = DateTime.Now.Date.AddDays(-1);
                BeginTime = EndTime.AddDays(-30);
                HeadControl1.PlatID = CookiePlatid.ToString();
                HeadControl1.SoftID = CookieSoftid.ToString();
                inputzhouqi.Value = Period.GetDescription();
                HeadControl1.BeginTime = BeginTime;
                HeadControl1.EndTime = EndTime;
                HeadControl1.Channel1.SoftId = HeadControl1.SoftID.ToString();
                HeadControl1.Channel1.Platform = HeadControl1.PlatID.ToString();
            }
            else
            {
                BeginTime = HeadControl1.BeginTime;
                EndTime = HeadControl1.EndTime;
                SetRequestCookie(Convert.ToInt32(HeadControl1.SoftID), Convert.ToInt32(HeadControl1.PlatID));

            }
            HeadControl1.Channel1.PeriodCheck = false;
        }

        protected void BindData()
        {
            StatUsersService suService = new StatUsersService();

            reportTitle = "分版本统计";
            var channels = HeadControl1.Channel1.ChannelValues;
            List<string> versionIds = HeadControl1.VersionID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();




            //不区分渠道
            if (HeadControl1.Channel1.ChannelValues.Count == 0)
            {
                foreach (string versionId in versionIds)
                {
                    ListAll.Add(suService.GetStatUsersByVersion(SoftID, PlatformID, ChannelTypeOptions.Category, 0, versionId, (int)Period, BeginTime, EndTime));
                    DataNames.Add(versionId);
                }
            }
            else
            {
                for (int i = 0; i < channels.Count; i++)
                {
                    foreach (string versionId in versionIds)
                    {
                        List<Sjqd_StatUsers> users = suService.GetStatUsersByVersion(SoftID, PlatformID, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), versionId, (int)Period, BeginTime, EndTime);
                        ListAll.Add(users);
                        DataNames.Add(string.Format("{0}-{1}", channels[i].ChannelText, versionId));
                    }
                }
            }

            string versionNames = string.Join(",", versionIds.ToArray());
            ExcelDownUrl = string.Format("/Services/ExcelDownloader.ashx?Action=GetStatUsersByVersion&SoftID={0}&Platform={1}&Period={2}&StartDate={3:yyyy-MM-dd}&EndDate={4:yyyy-MM-dd}&VersionIds={5}&VersionNames={6}&ChannelIds={7}&ChannelTypes={8}&ChannelTexts={9}&v={10}", SoftID, PlatformID, (int)Period, BeginTime, EndTime, HeadControl1.VersionID, HttpUtility.UrlEncode(versionNames), HeadControl1.Channel1.SelectedValue, HeadControl1.Channel1.SelectedCate, HttpUtility.UrlEncode(HeadControl1.Channel1.SelectedText), DateTime.Now.Ticks);

            if (ListAll.Count > 0)
            {

                GetAllLineJson();
                StringBuilder tablesBuilder = new StringBuilder();
                //形成tablehtml
                for (int i = 0; i < ListAll.Count; i++)
                {
                    tablesBuilder.Append(
                        TableHelper.BuildStatUsersByVersionTable(
                            ListAll[i].OrderByDescending(p => p.StatDate).ToList()
                            , (int)Period
                            , false
                            , i));
                }
                TablesHtml = tablesBuilder.ToString();
            }
        }

        /// <summary>
        /// 生成所有曲线的JSON
        /// </summary>
        protected void GetAllLineJson()
        {
            LineChart chart = new LineChart(BeginTime, EndTime);
            bool isNewUserReport = reporttype.Value == "0";
            chart.Period = Period;
            for (int i = 0; i < ListAll.Count; i++)
            {
                LineChartLine line = new LineChartLine
                {
                    Name = DataNames[i], 
                    Show = true,
                    XIntervalDays = 0,
                    Points = ListAll[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = isNewUserReport ? a.NewUserCount : a.ActiveUserCount, DataContext = a }).ToList()
                };
                chart.Y.Add(line);
            }
            AxisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140{1},step:{2}}}}}"
                , chart.GetXJson()
                , Period == net91com.Stat.Core.PeriodOptions.Hours ? ",rotation:-45,x:-40,y:60" : (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay ? ",x:-5" : ",rotation:-45,x:-30,y:45")
                , chart.Step);

            SeriesJsonStr = chart.GetYJson(
                delegate(LineChartPoint point)
                {
                    return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", "", "0");
                });
        }

        /// <summary>
        /// 严格规范时间范围(每小时)
        /// </summary>
        protected void SetStandardTime()
        {
            TimeSpan dtSpan = EndTime - BeginTime;
            int days = dtSpan.Days;
            if (Period == net91com.Stat.Core.PeriodOptions.Hours && days > 10)
            {
                BeginTime = EndTime.AddDays(-10);
            }
            HeadControl1.BeginTime = BeginTime;
        }
    }
}