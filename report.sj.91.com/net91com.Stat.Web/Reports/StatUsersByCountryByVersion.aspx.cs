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
    public partial class StatUsersByCountryByVersion : ReportBasePage
    {
        /// <summary>
        /// 当前地区
        /// </summary>
        protected B_AreaEntity DefaultArea;
        /// <summary>
        /// 当前版本
        /// </summary>
        protected B_VersionEntity DefaultVersion;

        /// <summary>
        /// 数据项名称
        /// </summary>
        protected List<string> DataNames = new List<string>();

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
                    versionDict[version.Version] = version.Version;
            }
            return versionDict;
        }

        #endregion
        
        #region 获取地区列表(列表框使用)(GetAreaDict)

        private Dictionary<string, string> areaDict = null;
        /// <summary>
        /// 获取地区列表(列表框使用)
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetAreaDict()
        {
            if (areaDict == null)
            {
                List<B_AreaEntity> areaList = B_BaseToolService.Instance.GetCountriesCache();
                DefaultArea = areaList[0];
                areaDict = new Dictionary<string, string>();
                foreach (B_AreaEntity area in areaList)
                    areaDict[area.EnShortName] = area.Name;
            }
            return areaDict;
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
            HeadControl1.CustomTypeSource = GetAreaDict();
            HeadControl1.IsCustomTypeSingle = false;
            HeadControl1.HiddenCustomType = false;
            HeadControl1.HiddenChannel = true;
            if (HeadControl1.IsFirstLoad)
            {
                GetVersionDict();
                HeadControl1.VersionID = DefaultVersion.Version;
                HeadControl1.CustomType = !string.IsNullOrEmpty(Request["area"]) ? Request["area"] : DefaultArea.EnShortName;
            }
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
                HeadControl1.BeginTime = BeginTime;
                HeadControl1.EndTime = EndTime;
            }
            else
            {
                BeginTime = HeadControl1.BeginTime;
                EndTime = HeadControl1.EndTime;
                SetRequestCookie(Convert.ToInt32(HeadControl1.SoftID), Convert.ToInt32(HeadControl1.PlatID));

            }
        }

        protected void BindData()
        {
            reportTitle = "分国家分版本统计(海外)";

            List<string> versionIds = HeadControl1.VersionID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> countryIds = HeadControl1.CustomType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            StatUsersService suService = new StatUsersService();           
            Dictionary<string, List<Sjqd_StatUsers>> users = suService.GetStatUsersByCountryByVersionEn(SoftID, PlatformID, versionIds, countryIds, BeginTime, EndTime);

            foreach (string ver in versionIds)
            {
                foreach (var area in countryIds)
                {
                    string key = string.Format("{0}-{1}", ver, area);
                    ListAll.Add(users.ContainsKey(key) ? users[key] : new List<Sjqd_StatUsers>());
                    DataNames.Add(string.Format("{0}-{1}", ver, GetAreaDict()[area]));
                }
            }
   
            string versionIdsString = string.Join(",", versionIds.Select(a => a.ToString()).ToArray());
            string countryIdsString = string.Join(",", countryIds.Select(a => a.ToString()).ToArray());
            string versionNames = versionIdsString;
            string countries = string.Join(",", countryIds.Select(a => GetAreaDict()[a.ToString()]).ToArray());
            ExcelDownUrl = string.Format("/Services/ExcelDownloader.ashx?Action=GetStatUsersByCountryByVersionEn&SoftID={0}&Platform={1}&StartDate={2:yyyy-MM-dd}&EndDate={3:yyyy-MM-dd}&VersionIds={4}&VersionNames={5}&CountryIds={6}&Countries={7}&v={8}", SoftID, PlatformID, BeginTime, EndTime, versionIdsString, HttpUtility.UrlEncode(versionNames), countryIdsString, HttpUtility.UrlEncode(countries), DateTime.Now.Ticks);

            if (ListAll.Count > 0)
            {

                GetAllLineJson();
                StringBuilder tablesBuilder = new StringBuilder();
                //形成tablehtml
                for (int i = 0; i < ListAll.Count; i++)
                {
                    tablesBuilder.Append(
                        TableHelper.BuildStatUsersByCountryByVersionEnTable(
                            ListAll[i].OrderByDescending(p => p.StatDate).ToList()
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
            chart.Period = net91com.Stat.Core.PeriodOptions.Daily;
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
            AxisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140,rotation:-45,x:-30,y:45,step:{1}}}}}"
                , chart.GetXJson()
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