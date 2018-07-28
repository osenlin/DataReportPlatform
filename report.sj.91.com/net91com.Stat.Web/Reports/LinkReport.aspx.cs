using net91com.Core;
using net91com.Core.Extensions;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Services.CommonServices.Other;
using net91com.Reports.UserRights;
using net91com.Stat.Services;
using net91com.Stat.Web.Base;
using net91com.Stat.Web.Reports.Controls;
using net91com.Stat.Web.Reports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Web.Reports
{
    public partial class LinkReport : ReportBasePage
    {
        public string tableStr { get; set; }
        public string AxisJsonStr = "{}";
        public string SeriesJsonStr = "[]";

        //多条线
        public List<List<LinkTagCount>> ListAll = new List<List<LinkTagCount>>();

        /// tab 切换的字符
        public List<string> TabStr = new List<string>();

        /// 用户选择平台数组 
        protected List<int> PlatformsidList = new List<int>();

        public List<int> LinkTagIdList = new List<int>();

        /// 转换为坐标点json字符的中间类
        public List<SeriesJsonModel> seriesJsonStr = new List<SeriesJsonModel>();

        public List<string> tabStr = new List<string>();
        
        /// 软件列表
        public int softsid;

        /// <summary>
        /// 获取基础数据类
        /// </summary>
        protected UtilityService ds = UtilityService.GetInstance();

        /// <summary>
        /// 定义开始比较时间
        /// </summary>
        public DateTime BeginCompareTime { get; set; }

        /// <summary>
        /// 定义结束比较时间
        /// </summary>
        public DateTime EndCompareTime { get; set; }

        public List<DateTime> X_DateTime = new List<DateTime>();
        
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
            HeadControl1.IsHasNoPlat = true;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = false;
            HeadControl1.LinkTag1.IsHasNoPlat = true;
            Period = net91com.Stat.Core.PeriodOptions.Daily;
            GetQueryString();
            BindData();
        }

        protected void GetQueryString()
        {
            if (HeadControl1.IsFirstLoad)
            {
                EndTime = DateTime.Today.AddDays(-1);
                BeginTime = EndTime.AddDays(-30);
                softsid = CookieSoftid;
                PlatformsidList = new List<int> { CookiePlatid };
                HeadControl1.PlatID = PlatformsidList[0].ToString();
                HeadControl1.SoftID = softsid.ToString();
                HeadControl1.BeginTime = BeginTime;
                HeadControl1.EndTime = EndTime;
                HeadControl1.LinkTag1.SoftId = HeadControl1.SoftID.ToString();
                HeadControl1.LinkTag1.Platform = HeadControl1.PlatID.ToString();
            }
            else
            {
                BeginTime = HeadControl1.BeginTime;
                EndTime = HeadControl1.EndTime;
                softsid = Convert.ToInt32(HeadControl1.SoftID);
                var platformsids = HeadControl1.PlatID.Split(',');
                foreach (string item in platformsids)
                {
                    PlatformsidList.Add(Convert.ToInt32(item));
                }
                SetRequestCookie(softsid, PlatformsidList[0]);

            }
            HeadControl1.LinkTag1.PeriodCheck = false;
        }

        protected void BindData()
        {
            List<LinkTagCount> data;

            if (HeadControl1.LinkTag1.Values.Count == 0)
            {
                for (int i = 0; i < PlatformsidList.Count; i++)
                {
                    data = LinkTagService.Instance.GetTagCountCache(
                        BeginTime, EndTime, softsid, PlatformsidList[i], Period, 0, 0, "", false,CacheTimeOption.TenMinutes)
                        .OrderBy(p => p.StatDate).ToList();
                    if (data.Count > 0)
                    {
                        ListAll.Add(data);
                    }
                }
            }
            else
            {
                List<SelectLinkTagValue> tags = HeadControl1.LinkTag1.Values;
                for (int i = 0, count = tags.Count; i < count; i++)
                {
                    SelectLinkTagValue tag = tags[i];
                    data = LinkTagService.Instance.GetTagCountCache(
                        BeginTime, EndTime, softsid, tag.Platform, Period, 0, tag.TagValue, tag.TagText, tag.IsCategory,CacheTimeOption.TenMinutes)
                        .OrderBy(p => p.StatDate).ToList();
                    if (data.Count > 0)
                    {
                        //记住有数据渠道的编号
                        LinkTagIdList.Add(i);
                        ListAll.Add(data);
                    }
                }
            }

            if (ListAll.Count == 0)
                return;

            GetAllLineJson();
            StringBuilder sb = new StringBuilder();
            //传入的tab 序列值
            int tabindex = 0;
            //形成tablehtml
            for (int j = 0; j < ListAll.Count; j++)
            {
                string plat = ((MobileOption)ListAll[j][0].Platform).GetDescription();
                if ((MobileOption)ListAll[j][0].Platform == MobileOption.None)
                    plat = "不区分平台";
                string tag = ListAll[j][0].TagName;
                if (string.IsNullOrEmpty(tag))
                    tag = "不区分标签";
                tabStr.Add(plat + "_" + tag);
                sb.Append(GetTableString(ListAll[j], tabindex));
                tabindex++;
            }
            tableStr = sb.ToString();
        }

        /// <summary>
        /// 生成所有曲线的JSON
        /// </summary>
        protected void GetAllLineJson()
        {
            LineChart chart = new LineChart(BeginTime, EndTime);
            chart.Period = Period;
            chart.NeedForecastPoint = (Period == net91com.Stat.Core.PeriodOptions.Daily || Period == net91com.Stat.Core.PeriodOptions.Weekly || Period == net91com.Stat.Core.PeriodOptions.Monthly);
            SetChart(chart, ListAll);
            AxisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140{1},step:{2}}}}}"
                , chart.GetXJson()
                , Period == net91com.Stat.Core.PeriodOptions.Hours ? ",rotation:-45,x:-40,y:60" : (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay ? ",x:-5" : ",rotation:-45,x:-30,y:45")
                , chart.Step);

            SeriesJsonStr = chart.GetYJson(
                delegate(LineChartPoint point)
                {
                    LinkTagCount user = (LinkTagCount)point.DataContext;
                    if (point.Type != -1)
                        return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", point.Percent, point.Denominator);
                    else
                        return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", "", "0");
                });
        }
        /// <summary>
        /// 设置曲线
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="list"></param>
        /// <param name="iscompare"></param>
        protected void SetChart(LineChart chart, List<List<LinkTagCount>> list, bool iscompare = false)
        {
            for (int i = 0; i < list.Count; i++)
            {
                string softName = GetSoft(list[i][0].SoftId).Name;
                string platform = "_" + ((MobileOption)list[i][0].Platform).GetDescription();
                if (platform == "_None")
                {
                    platform = "_不区分平台";
                }
                string tagName = list[i][0].TagName;
                List<LineChartPoint> MyPoints = null;
                MyPoints = list[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.StatCount, DataContext = a }).ToList();
                LineChartLine line = new LineChartLine
                {
                    Name = softName + platform + tagName,
                    Show = true,
                    Points = MyPoints
                };
                chart.Y.Add(line);
            }
        }

        /// <summary>
        /// 形成每一个tab
        /// </summary>
        /// <returns></returns>
        protected string GetTableString(List<LinkTagCount> data, int tableindex)
        {
            data = data.OrderByDescending(p => p.StatDate).ToList();
            string tableName = string.Empty;
            if (HeadControl1.LinkTag1.Values.Count == 0)
            {
                // soft_plat_begin_end_tagid_tagtype
                tableName = data[0].SoftId + "_" + data[0].Platform + "_" +
                            BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_0_0";
            }
            else
            {
                var tags = HeadControl1.LinkTag1.Values;
                tableName = data[0].SoftId + "_" + data[0].Platform + "_" +
                            BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_" +
                            tags[LinkTagIdList[tableindex]].TagValue + "_" +
                            (tags[LinkTagIdList[tableindex]].IsCategory ? "1" : "0");
            }
            return TableHelper.BuildLinkTagTable(data, false, tableindex, tableName);
        }
    }
}