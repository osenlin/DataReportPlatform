using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Web.Base;
using net91com.Core.Extensions;
using net91com.Core;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.TransverseReports
{
    public partial class SoftAreaTransverse : ReportBasePage
    {
        #region 字段属性
        //这个是平台字符串，前台绑定了的，格式例如 iphone,ipad,wm
        protected string platformStr = "";
        //这个是软件字符,前台绑定的，格式例如 看书,空间，来电秀
        protected string softStr = "";
        //用于前台显示周期
        public string MyPeriod { get; set; }


        /// 单个软件信息实体
        public SoftInfo softinfo { get; set; }

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
        /// 选择的类型
        /// </summary>
        public string CustomType { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime startTime { get; set; }

        public DateTime maxTime { get; set; }
        /// <summary>
        /// 渠道类型
        /// </summary>
        protected int ChannelCate = 0;
        /// <summary>
        /// 选择渠道
        /// </summary>
        protected string ChannelValues = "";

        public List<Sjqd_StatUsersByArea> allSoftArea;
        #endregion

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => (a.ID != -9 && a.ID != -10 && a.ID != -11)).ToList(); }
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
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("0", "中国");
            dic.Add("1", "世界");
            Dictionary<int, string> customPeriodDic = new Dictionary<int, string>();
            customPeriodDic.Add(0, "全部");
            customPeriodDic.Add(6, "最近一周");
            customPeriodDic.Add(7, "最近两周");
            customPeriodDic.Add(8, "最近一个月");
            customPeriodDic.Add(9, "最近三个月");
            customPeriodDic.Add(100, "最近一周(自然周)"); 
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            HeadControl1.IsHasNoPlat = false;
            HeadControl1.HiddenTime = true;
            HeadControl1.HiddenPeriod = false;
            HeadControl1.CustomTypeSource = dic;
            HeadControl1.HiddenCustomType = false;
            HeadControl1.CustomPeriodDic = customPeriodDic; 
            //初次加载一些参数设置成默认值
            reportTitle = "地区分布";
            if (HeadControl1.IsFirstLoad)
            {
                softsid = CookieSoftid;
                platformsid = CookiePlatid;
                HeadControl1.SoftID = softsid.ToString();
                HeadControl1.PlatID = platformsid.ToString();
                HeadControl1.CustomType = "0";
                HeadControl1.SinglePeriod = ((int)PeriodOptions.LatestOneMonth).ToString();
                HeadControl1.Channel1.SoftId = HeadControl1.SoftID;
                HeadControl1.Channel1.Platform = HeadControl1.PlatID;

            }
            //用户选择模式
            else
            {
                platformsid = Convert.ToInt32(HeadControl1.PlatID);
                softsid = Convert.ToInt32(HeadControl1.SoftID);
                SetRequestCookie(softsid, platformsid);
            }
            //设置文字，只有一个对象
            if (HeadControl1.SinglePeriod != "100")
                Period = (PeriodOptions)Convert.ToInt32(HeadControl1.SinglePeriod);
            else
                Period = PeriodOptions.LatestOneWeek;
            CustomType = HeadControl1.CustomType;

            if (HeadControl1.Channel1.ChannelValues.Count != 0)
            {
                ChannelValues = string.Join(",", HeadControl1.Channel1.ChannelValues.Select(p => p.ChannelValue.ToString()).ToList().ToArray());
                ChannelCate = Convert.ToInt32(HeadControl1.Channel1.ChannelValues[0].ChannelType);
            }
        }

        /// </summary>获取数据加上绑定数据
        protected void BindData()
        {
            int[] arr = new int[] { 68, 69, -9, 58, 9, 57, 60, 61 };
            if(arr.Contains<int>(softsid))
            {
                string otherKeyString = softsid == 51 ? "SoftID51_" : "";
                maxTime = UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistributionForPC, CacheTimeOption.TenMinutes, otherKeyString);
            }
            else
            {
                string otherKeyString = softsid == 51 ? "SoftID51_" : "";
                maxTime = UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes, otherKeyString);
            }
            if (HeadControl1.SinglePeriod == "100")
                maxTime = GetLatestNaturalWeekTime();
            startTime = DateTime.Now;
            switch (Period)
            {
                case PeriodOptions.LatestOneMonth:
                    startTime = maxTime.AddMonths(-1).AddDays(1);
                    break;
                case PeriodOptions.LatestOneWeek:
                    startTime = maxTime.AddDays(-6);
                    break;
                case PeriodOptions.LatestThreeMonths:
                    startTime = maxTime.AddMonths(-3).AddDays(1);
                    break;
                case PeriodOptions.LatestTwoWeeks:
                    startTime = maxTime.AddDays(-13);
                    break;
                case PeriodOptions.All:
                    startTime = DateTime.MinValue;
                    break;
                    //最近一周自然周
                default:
                    startTime = maxTime.AddDays(-6);
                    break; 
            }
             reportTitle = Period == PeriodOptions.All ? "地区分布(" + maxTime.ToString("yyyy-MM-dd") + "之前)" : "地区分布(" + startTime.ToString("yyyy-MM-dd") + "至" + maxTime.ToString("yyyy-MM-dd") + ")";
             
            var channels=HeadControl1.Channel1.ChannelValues;
            //HeadControl1.CustomType == "0" 表示国内 
            if(HeadControl1.CustomType == "0")
            {
                if (channels.Count == 0)
                {
                    allSoftArea = new StatUsersByAreaService(true).GetSoftAreaTransverseWithChina(Period,
                   Convert.ToInt32(maxTime.ToString("yyyyMMdd")),
                   softsid, (MobileOption)platformsid);
                }
                else
                {
                    allSoftArea = new StatUsersByAreaService(true).GetSoftAreaTransverseWithChinaByChannels(
                  softsid, (MobileOption)platformsid, Period,
                  Convert.ToInt32(maxTime.ToString("yyyyMMdd")), (ChannelTypeOptions)ChannelCate, ChannelValues);
                }
            }
               
            else
            {
                if (channels.Count == 0)
                {
                    allSoftArea = new StatUsersByAreaService(true).GetSoftAreaTransverseWithWorld(Period,
                   Convert.ToInt32(maxTime.ToString("yyyyMMdd")),
                   softsid, (MobileOption)platformsid);
                }
                else
                {
                    allSoftArea = new StatUsersByAreaService(true).GetSoftAreaTransverseWithWorldByChannels(
                  softsid, (MobileOption)platformsid, Period,
                  Convert.ToInt32(maxTime.ToString("yyyyMMdd")), (ChannelTypeOptions)ChannelCate, ChannelValues);
                }
            
            }
               
            //一条线都的不出来
            if (allSoftArea.Count == 0)
            {
                SeriesJsonStr = "[]";
                reportTitle = "无数据";
            }
            else
            {
                SeriesJsonStr = GetYlineJson(allSoftArea);
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

            sb.Append(@" <thead><tr><th>地区</th>
                <th>用户数</th>     
                <th>百分比</th> 
                <th></th>  
                <th></th>  
                </tr></thead>");
            decimal usercount = 0;
            int all = allSoftArea.Sum(p => p.UseCount);
            for (int i = 0; i < allSoftArea.Count; i++)
            {
                if (i < 100)
                {
                    sb.Append("<tr   class=\"tableover\"  >");
                    sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td>"+
                                     "<td><span class=\"find\" onclick=\"showLine({3},'{0}')\" id=\"lbl{3}\">查看每天</span></td>"+
                                     "<td><span class=\"find2\" onclick=\"showDetail('{0}','{4}','{5}')\">{6}</span></td>",
                        (string.IsNullOrEmpty(allSoftArea[i].AreaName) ? "未知区域" : allSoftArea[i].AreaName), allSoftArea[i].UseCount,
                        Math.Round(Convert.ToDecimal(allSoftArea[i].UseCount) / all * 100, 2) + "%",i,
                        ChannelValues, ChannelCate, HeadControl1.CustomType == "0"?"查看城市":"");
                    sb.Append("</tr>");
                    sb.AppendFormat("<tr class=\"divtr\" id=\"tr{0}\" style=\"display:none;height:400px;\"><td colspan=\"5\"><div id=\"div{0}\"> </div>" + "</td></tr>", i);
                }
                else
                {
                    usercount += allSoftArea[i].UseCount;
                }
            }
            if (usercount != 0)
            {
                sb.Append("<tr   class=\"tableover\"  >");
                sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td> <td></td> ", "其他", usercount, Math.Round((usercount / all * 100), 2) + "%");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
            return sb.ToString();


        }

        protected string GetYlineJson(List<Sjqd_StatUsersByArea> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[{type: 'pie',name: 'Browser share',data: [");
            int count = 0;
            int alluser = list.Sum(p => p.UseCount);
            for (int i = 0; i < (list.Count > MaxPerNumber ? MaxPerNumber : list.Count); i++)
            {
                if (Convert.ToDecimal(list[i].UseCount) / alluser * 100 > MinPerRatio)
                {
                    sb.AppendFormat("['{0}',{1}],", string.IsNullOrEmpty(list[i].AreaName) ? "未知地区" : list[i].AreaName, list[i].UseCount);
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

        protected DateTime GetLatestNaturalWeekTime()
        { 
            DateTime dtTimeNow=DateTime.Now.AddDays(-1);
            DateTime NatrualLastOneWeek;
            while (dtTimeNow.DayOfWeek != DayOfWeek.Sunday)
            {
                dtTimeNow=dtTimeNow.AddDays(-1);
            }
            NatrualLastOneWeek = dtTimeNow;
            return NatrualLastOneWeek;
        }
    }
}