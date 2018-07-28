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
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports
{
    public partial class LifecycleReport : ReportBasePage
    {
        ///最近接选择日期的日期
        public DateTime StartDateTime{get;set;}
         /// 平台id  
        protected int PlatformID { get; set; }
        /// 软件列表
        public int SoftID{get;set;} 
        //数据库交互对象
        protected StatLifecycleService dls = new StatLifecycleService(true);
        protected UtilityService ds = UtilityService.GetInstance();

        ///用户绑定图的title
        public string ReportTitle { get; set; }
        /// 横轴显示的json字符
        public string AxisJsonStr { get; set; }
        /// <summary>
        /// 具体点坐标的json字符
        /// </summary>
        public string SeriesJsonStr { get; set; }
        ///查出来数据集合
        public List<Sjqd_StatLifecycle> softs = new List<Sjqd_StatLifecycle>();
        /// 转换为坐标点json字符的中间类
        public List<SeriesJsonModel> seriesJsonStr = new List<SeriesJsonModel>();

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
            HeadControl1.IsHasNoPlat = false;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            HeadControl1.HiddenTime = true;
            HeadControl1.HiddenSingleTime = false;
            GetQueryString();
            BindData();

        }
        protected void GetQueryString()
        {
            //初次加载一些参数设置成默认值
            ReportTitle = "生命周期";
            AxisJsonStr = "{}";
            SeriesJsonStr = "[]";
            if (HeadControl1.IsFirstLoad)
            {

                SoftID = CookieSoftid;
                PlatformID = CookiePlatid;
                StartDateTime = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.UserLifecycle,CacheTimeOption.TenMinutes).AddMonths(-2);
                HeadControl1.SoftID = SoftID.ToString();
                HeadControl1.PlatID = PlatformID.ToString();
                HeadControl1.SingleTime = StartDateTime;
            }
            else  //用户选择模式
            {

                string beginstr = Request["inputtimestart"] == null ? "" : Request["inputtimestart"];
                StartDateTime = HeadControl1.SingleTime;
                PlatformID = Convert.ToInt32(HeadControl1.PlatID);
                SoftID = Convert.ToInt32(HeadControl1.SoftID);
                SetRequestCookie(SoftID, PlatformID);
            }
        }

        /// </summary>获取数据加上绑定数据
        protected void BindData()
        {
             softs = dls.GetSoftLifeCycleCache(SoftID, PlatformID, StartDateTime,CacheTimeOption.TenMinutes);
            if (softs.Count == 0)
                return;
            ConvertLifeSoft(ref softs);
             
            if (softs.Count != 0)
            {
                //设置x轴上的日期
                SetxAxisJson(softs);
                //设置点，形成具体点 的json字符串
                SeriesJsonStr = JsonConvert.SerializeObject(GetDataJsonList(softs)); ;
                StringBuilder sb = new StringBuilder();               
            }          
        }

        /// <summary>
        /// 获取x轴的数据
        /// </summary>
        /// <param name="softs"></param>
        protected void SetxAxisJson(List<Sjqd_StatLifecycle> softs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in softs)
            {
                sb.Append("\"第" + item.Days + "天\"" + ",");
            }
            string str = sb.ToString().Substring(0, sb.ToString().Length - 1);
            AxisJsonStr = "{" + string.Format("categories:[{0}] ", str);
            ///加逗号和右括号
            AxisJsonStr += ",labels:{ align:'left',rotation: -45,  tickLength:80,tickPixelInterval:140,x:-8,y:40";
            AxisJsonStr += "}}";
        }

        /// <summary>
        /// json转换函数
        /// </summary>
        /// <param name="softs"></param>
        /// <returns></returns>
        protected List<SeriesJsonModel> GetDataJsonList(List<Sjqd_StatLifecycle> softs)
        {
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            sjModel2.data = new List<object>();
            for (int i = 0; i < softs.Count; i++)
            {
                string plat = softs[0].Platform.ToString().ToEnum<MobileOption>(MobileOption.None).ToString();
                if (plat == "None")
                {
                    plat = "不区分平台";
                }
                sjModel2.name = GetSoft(softs[0].SoftID).Name + "_" + plat + "(" + softs[0].StatDate.ToShortDateString() + ")";
                DataLabels dl = new DataLabels();
                SmallDataLabels smalldata = new SmallDataLabels();
                dl.y = Math.Round((softs[i].RetainedUserCount / (double)softs[i].NewUserCount) * 100, 2);
                dl.growth = dl.y + "%";
                dl.Denominator = softs[i].NewUserCount;
                smalldata.enabled = true;
                dl.dataLabels = smalldata;
                sjModel2.data.Add(dl);
            }
            seriesJsonStr.Add(sjModel2);
            return seriesJsonStr;
        }

        protected void ConvertLifeSoft(ref List<Sjqd_StatLifecycle> softs)
        {
            for (int i = softs.Count - 1; i >= 0; i--)
            {
                if (i == softs.Count - 1)
                    softs[i].RetainedUserCount = softs[i].RetainedUserCount;
                else
                    softs[i].RetainedUserCount = softs[i].RetainedUserCount + softs[i + 1].RetainedUserCount;
            }
            //增加一个点起始点，100%
            Sjqd_StatLifecycle life = new Sjqd_StatLifecycle();
            life.Days = 0;
            life.NewUserCount = softs[0].NewUserCount;
            life.RetainedUserCount = softs[0].NewUserCount;
            life.Platform = softs[0].Platform;
            life.SoftID = softs[0].SoftID;
            life.StatDate = softs[0].StatDate;
            softs.Insert(0, life);
            softs = softs.Where(p => p.Days < 3 || p.Days % 3 == 0).Take(31).ToList();
        }
    }
}