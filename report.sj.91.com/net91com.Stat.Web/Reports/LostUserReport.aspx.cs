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
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using System.Text.RegularExpressions;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports
{
    public partial class LostUserReport : ReportBasePage
    {
        /// <summary>
        /// htmltable 字符串
        /// </summary>
        protected string TableStr { get; set; }
        protected List<List<SoftUser>> ListAll = new List<List<SoftUser>>();
        /// tab 切换的字符
        protected List<string> TabStr = new List<string>();

        protected string ReportTitle { get; set; }
        /// 横轴显示的json字符
        protected string AxisJsonStr { get; set; }
        /// <summary>
        /// 具体点坐标的json字符
        /// </summary>
        protected string SeriesJsonStr { get; set; }
        /// <summary>
        /// 流失最大数据时间
        /// </summary>
        protected DateTime MaxTime { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            //初始化控件
            InitControls();
            //获取参数
            GetParams();

            //设置参数
            SetParams();

            //获取并绑定数据
            BindData();

            GetShowData();
        }


        /// <summary>
        /// 初始化控件部分属性值
        /// </summary>
        private void InitControls()
        {

            HeadControl1.MySupportSoft = AvailableSofts;
            HeadControl1.IsHasNoPlat = true;
            HeadControl1.IsSoftSingle = false;
            HeadControl1.IsPlatSingle = false;   
            HeadControl1.PlatWidth = "12%";
            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[] {
                    net91com.Stat.Core.PeriodOptions.NaturalMonth,
                    net91com.Stat.Core.PeriodOptions.Weekly },
                    net91com.Stat.Core.PeriodOptions.Weekly);
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        protected void GetParams()
        {
            //获取周期值
            net91com.Stat.Core.PeriodOptions period = PeriodSelector1.SelectedPeriod;

            //获取开始时间和结束时间
            DateTime beginTime = string.IsNullOrEmpty(Request["inputtimestart"])
                ? (HeadControl1.IsFirstLoad ? DateTime.MinValue : HeadControl1.BeginTime)
                : Convert.ToDateTime(Request["inputtimestart"]);
            DateTime endTime = string.IsNullOrEmpty(Request["inputtimeend"])
                ? (HeadControl1.IsFirstLoad ? DateTime.MinValue : HeadControl1.EndTime)
                : Convert.ToDateTime(Request["inputtimeend"]);

            //获取选择的软件及平台列表
            List<int> selectedSoftIds = string.IsNullOrEmpty(Request["inputsoftselect"])
                ? (HeadControl1.IsFirstLoad ? new List<int>() : HeadControl1.SoftID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList())
                : Request["inputsoftselect"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            List<MobileOption> selectedPlatforms = string.IsNullOrEmpty(Request["inputplatformselect"])
                ? (HeadControl1.IsFirstLoad ? new List<MobileOption>() : HeadControl1.PlatID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (MobileOption)int.Parse(p)).ToList())
                : Request["inputplatformselect"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (MobileOption)int.Parse(p)).ToList();

            //验证参数
            CheckParams(selectedSoftIds, selectedPlatforms, period, beginTime, endTime, ReportType.UserUseNewActivity);

            
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        protected void SetParams()
        {
            ///初次加载一些参数设置成默认值
            ReportTitle = "流失用户";
            AxisJsonStr = "{}";
            SeriesJsonStr = "[]";
            //设置控件的属性值
            HeadControl1.BeginTime = BeginTime;
            HeadControl1.EndTime = EndTime;
            HeadControl1.SoftID = string.Join(",", SelectedSofts.Select(p => p.ID.ToString()).ToArray());
            HeadControl1.PlatID = string.Join(",", SelectedPlatforms.Select(p => ((int)p).ToString()).ToArray()); 
            
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            MaxTime = DateTime.MinValue;
            ///选择所筛选的所有软件
            for (int softid = 0; softid < SelectedSofts.Count; softid++)
            {
                Soft SingleSoftInfo = SelectedSofts[softid];
                for (int i = 0; i < SelectedPlatforms.Count; i++)
                {
                    foreach (MobileOption tempplat in SingleSoftInfo.Platforms)
                    {
                        ///他选出的平台加上和自己支持平台的交集
                        if (tempplat == SelectedPlatforms[i] || SelectedPlatforms[i] == 0)
                        {
                            List<SoftUser> users = null;
                            users = Sjqd_StatUsersService.GetInstance().GetSoftUserListCache(BeginTime, EndTime, SingleSoftInfo.ID, (int)SelectedPlatforms[i], Period, loginService ,CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate)
                                .Where(p => p.LostNum != -1).ToList();
                            if (users != null && users.Count != 0)
                            {
                                DateTime maxTime = users.Max(p => p.StatDate);
                                if (maxTime > MaxTime)
                                    MaxTime = maxTime;
                                ListAll.Add(users);
                               
                            } 
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 生成所有曲线的JSON
        /// </summary>
        protected void GetShowData()
        {
            LineChart chart = new LineChart(BeginTime, MaxTime);
            StringBuilder tablesb = new StringBuilder();
            chart.Period = Period;
            for (int i = 0; i < ListAll.Count; i++)
            {
                string softName = GetSoft(ListAll[i][0].SoftId).Name;
                string platform = "_" + ((MobileOption)ListAll[i][0].Platform).GetDescription();
                chart.Y.Add(
                         new LineChartLine
                         {
                             Name = softName + platform +"_流失",
                             Show = true,
                             XIntervalDays = 0,
                             Points = ListAll[i].Where(p => p.LostNum != -1).Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.LostNum, DataContext = a }).ToList()
                         });
                chart.Y.Add(
                        new LineChartLine
                        {
                            Name = softName + platform + "_活跃",
                            Show = true,
                            XIntervalDays = 0,
                            Points = ListAll[i].Where(p => p.LostNum != -1).Select(a => new LineChartPoint { XValue = a.StatDate, YValue=a.UseNum, DataContext = a }).ToList()
                        });
                TabStr.Add(softName + platform);

                tablesb.Append(GetOneTableHtml(ListAll[i], i));

            }
            AxisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140{1},step:{2}}}}}"
               , chart.GetXJson()
               , ""
               , chart.Step);

            SeriesJsonStr = chart.GetYJson(
                delegate(LineChartPoint point)
                {
                    SoftUser user = (SoftUser)point.DataContext;
                    return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", point.Percent, point.Denominator);

                });
            TableStr = tablesb.ToString();
        }

        /// <summary>
        /// 形成每一个tab
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tableindex"></param>
        /// <returns></returns>
        protected string GetOneTableHtml(List<SoftUser> users, int tableindex)
        {
            users = users.OrderByDescending(p => p.StatDate).ToList(); 
            string tableName = string.Empty; 
            tableName = users[0].SoftId + "_" + users[0].Platform  + "_" + BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString()+"_"+(int)Period ;
             
            StringBuilder sb;
            if (tableindex == 0)
                sb = new StringBuilder(string.Format("<table id=\"tab{0}\" name=\"" + tableName + "\" class=\" tablesorter \" cellspacing=\"1\">", tableindex));
            else
                sb = new StringBuilder(string.Format("<table id=\"tab{0}\" name=\"" + tableName + "\"  class=\" tablesorter \" style=\"display:none\"  cellspacing=\"1\">", tableindex));
            
            return TableHelper.BuildUserLostUsers(users,tableName,tableindex,Period,false);
        }
   } 
}