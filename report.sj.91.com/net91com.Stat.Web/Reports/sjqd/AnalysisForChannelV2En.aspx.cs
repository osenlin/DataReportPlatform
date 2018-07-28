using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Stat.Web.Reports.Services;
using Newtonsoft.Json;
using net91com.Core.Extensions;
using net91com.Stat.Services;
using net91com.Core;
using net91com.Stat.Services.sjqd.Entity;

using net91com.Reports.UserRights;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Services.CommonServices.B_BaseTool;

namespace net91com.Stat.Web.Reports.sjqd
{
    public partial class AnalysisForChannelV2En : System.Web.UI.Page
    {
        /// 开始时间,前台也绑定了
        protected DateTime begintime;
        /// 结束时间，前台绑定了的
        protected DateTime endtime;
        //这个是平台字符串，前台绑定了的，格式例如 iphone,ipad,wm

        //用于前台显示周期
        public string MyPeriod { get; set; }
        /// <summary>
        /// htmltable 字符串
        /// </summary>
        public string tableStr { get; set; }
        /// 获取周期
        public net91com.Stat.Core.PeriodOptions Period { get; set; }

        public List<Soft> mySupportSoft;
        public string channelName;
        public int channelId { get; set; }
        public string reportTitle = "无数据";
        public string AxisJsonStr = "{}";
        public string SeriesJsonStr = "[]";
        ///每一条线一个List<SoftNewUser> 这里多条线
        public List<List<SoftUser>> listAll = new List<List<SoftUser>>();
        /// tab 切换的字符
        public List<string> TabStr = new List<string>();
        public Soft soft;

        /// 转换为坐标点json字符的中间类
        public List<SeriesJsonModel> seriesJsonStr = new List<SeriesJsonModel>();
        public List<string> tabStr = new List<string>();
        /// 软件列表
        public int softsid;
        //数据库交互对象
        protected UtilityService ds = UtilityService.GetInstance();
        /// 用户选择平台 
        protected int platformsid;
        ///x轴上控制系数 
        public int MaxNumCoef = -1;
        ///x轴上坐标(即时间)超过多少个开始利用控制系数
        public const int MaxNum = 20;
        public List<DateTime> X_DateTime = new List<DateTime>();
        public string countryHtml;
        private string selectCountryId;

        protected void Page_Load(object sender, EventArgs e)
        {
            //selectcountry = string.IsNullOrEmpty(Request["mycountry"]) ? "0" : Request["mycountry"];
            //List<string> countries = cubaService.GetAllCountries(CacheTimeOption.HalfDay);

            selectCountryId = Request["mycountry"] ?? string.Empty;

            List<B_AreaEntity> areaList = B_BaseToolService.Instance.GetCountriesCache();
            StringBuilder sb = new StringBuilder("<option value=\"\" select=\"selected\">不区分国家</option>");
            foreach (var country in areaList)
            {
                sb.AppendFormat("<option value=\"{0}\"{1}>{2}</option>", country.EnShortName, selectCountryId == country.EnShortName ? " selected=\"selected\"" : "", country.Name);
            }
            countryHtml = sb.ToString();
            try
            {
                channelId = Convert.ToInt32(net91com.Common.CryptoHelper.DES_Decrypt(Request["p"], "ndwebweb"));
                if (channelId <= 0)
                    ShowErrorMsg("无权访问");
                platformsid = Convert.ToInt32(Request["plat"]);
            }
            catch (Exception)
            {
                ShowErrorMsg("无权访问");
                return;
            }
            var node = new CfgChannelService().GetChannelCustomer(channelId);
            if (node == null)
            {
                ShowErrorMsg("该渠道商不存在！");
                return;
            }
            //之所以不从SjqdUtility 那个缓存中取，是缓存数据可能不一致
            int reporttype = node.ReportType;
            if (reporttype != 1)
            {
                ShowErrorMsg("无权访问！");
                return;

            }
            softsid = node.SoftID;
            channelName = node.Name;
            soft = new URBasicInfoService().GetSoft(softsid);
            DateTime dtrighttime = ds.GetMaxChannelUserTimeCache(softsid, MobileOption.None, net91com.Stat.Core.PeriodOptions.Daily,CacheTimeOption.TenMinutes);
            if (fromtime.Value == "" || totime.Value == "")
            {
                endtime = dtrighttime;
                begintime = endtime.AddDays(-30);
                fromtime.Value = begintime.ToString("yyyy-MM-dd");
                totime.Value = endtime.ToString("yyyy-MM-dd");
                softName.InnerHtml = soft.Name;
            }
            else
            {
                begintime = Convert.ToDateTime(fromtime.Value);
                endtime = Convert.ToDateTime(totime.Value);

            }
            if (begintime < node.MinViewTime)
            {
                begintime = node.MinViewTime;
            }
            //下午两点才能开放出去
            if (endtime >= dtrighttime && (DateTime.Now < dtrighttime.AddHours(38)))
            {
                endtime = dtrighttime.AddDays(-1);
            }
            if (begintime > endtime)
            {
                begintime = endtime;
            }
            fromtime.Value = begintime.ToString("yyyy-MM-dd");
            totime.Value = endtime.ToString("yyyy-MM-dd");
            channelCustomName.InnerText = channelName;
            if (platformsid <= 0)
                paltName.InnerHtml = "不区分平台";
            else
                paltName.InnerHtml = ((MobileOption)platformsid).GetDescription();
            BindData();

        }
        private void ShowErrorMsg(string msg)
        {
            Response.Clear();
            Response.Write(msg);
            Response.End();


        }

        protected void BindData()
        {
            StatUsersService suService = new StatUsersService();
            List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> users = null;
            if (selectCountryId == string.Empty)
            {
                users = suService.GetStatUsersByChannel(softsid, (int)platformsid, ChannelTypeOptions.Customer, channelId, (int)net91com.Stat.Core.PeriodOptions.Daily, begintime, endtime, false, true);
                
            }
            else
            {
                users = suService.GetStatUsersByArea(softsid, (int)platformsid, ChannelTypeOptions.Customer, channelId, selectCountryId, (int)net91com.Stat.Core.PeriodOptions.Daily, begintime, endtime, false, true);
            }
            List<SoftUser> usersCompare = new List<SoftUser>();
            foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in users)
            {
                SoftUser softuser = new SoftUser();
                softuser.StatDate = u.StatDate;
                softuser.SoftId = softsid;
                softuser.Platform = (int)platformsid;
                softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                softuser.Hour = u.StatHour;
                softuser.NewNum = u.NewUserCount;
                softuser.UseNum = u.ActiveUserCount;
                usersCompare.Add(softuser);
            }
            usersCompare = usersCompare.OrderBy(p => p.StatDate).ToList();
            if (usersCompare.Count != 0)
                listAll.Add(usersCompare);

            if (listAll.Count == 0)
                return;
            reportTitle = "渠道统计";
            //获取时间上的并集
            foreach (var item in listAll)
            {
                for (int tempindex = 0; tempindex < item.Count; tempindex++)
                {
                    if (!X_DateTime.Contains(item[tempindex].StatDate))
                    {
                        X_DateTime.Add(item[tempindex].StatDate);
                    }
                }
            }
            MaxNumCoef = X_DateTime.Count / 16 + 1;
            //设置x轴上的日期
            X_DateTime = X_DateTime.OrderBy(p => p).ToList();
            SetxAxisJson(X_DateTime);
            SeriesJsonStr = JsonConvert.SerializeObject(GetDataJsonList(listAll));
            StringBuilder sb = new StringBuilder();
            ///传入的tab 序列值
            int tabindex = 0;
            ///形成tablehtml 
            for (int j = 0; j < listAll.Count; j++)
            {

                int plat = Convert.ToInt32(listAll[j][0].Platform);
                if (plat == 0)
                    tabStr.Add("不区分平台");
                else
                    tabStr.Add(((MobileOption)plat).GetDescription());

                sb.Append(GetTableString(listAll[j], tabindex));
                tabindex++;

            }
            tableStr = sb.ToString();

        }
        protected void SetxAxisJson(List<DateTime> times)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DateTime item in times)
            {
                sb.Append("\"" + item.ToString("yy-MM-dd").Replace("-", "/") + "\"" + ",");
            }
            string str = sb.ToString().Substring(0, sb.ToString().Length - 1);
            AxisJsonStr = "{" + string.Format("categories:[{0}] ", str);
            //加逗号和右括号
            AxisJsonStr += ",labels:{ align:'left', rotation: -45, tickLength:80,tickPixelInterval:140 ,x:-30,y:45";
            //设置系数

            if (times.Count > MaxNum)
                //如果大于 一定数目的话就隔 MaxNumCoef显示x 轴上lable
                AxisJsonStr += ",step:" + MaxNumCoef.ToString();
            AxisJsonStr += "}}";




        }

        protected List<SeriesJsonModel> GetDataJsonList(List<List<SoftUser>> temp)
        {


            foreach (List<SoftUser> item in temp)
            {

                //构造一个json模型
                SeriesJsonModel sjModel2 = new SeriesJsonModel();
                //构造对应x轴上对应各个坐标点,一开始就是null ，先填充好
                for (int ii = 0; ii < X_DateTime.Count; ii++)
                {
                    DataLabels dl = new DataLabels();
                    dl.y = 0;
                    sjModel2.data.Add(dl);
                }

                if (item.Count > 0 && X_DateTime.Count <= MaxNum)
                {

                    sjModel2.name = ((MobileOption)item[0].Platform).GetDescription();

                    for (int j = 0; j < X_DateTime.Count; j++)
                    {
                        for (int i = 0; i < item.Count; i++)
                        {
                            if (item[i].StatDate == X_DateTime[j])
                            {

                                DataLabels dl = new DataLabels();
                                SmallDataLabels smalldata = new SmallDataLabels();

                                dl.y = item[i].NewNum;


                                smalldata.enabled = true;
                                dl.dataLabels = smalldata;
                                //替换掉以前的null
                                sjModel2.data[j] = dl;



                            }
                        }

                    }
                    seriesJsonStr.Add(sjModel2);

                }
                //当大于20个的时候线上的点要格一定数目显示
                else if (X_DateTime.Count > MaxNum)
                {
                    sjModel2.name = ((MobileOption)item[0].Platform).GetDescription();
                    for (int j = 0; j < X_DateTime.Count; j++)
                    {
                        for (int i = 0; i < item.Count; i++)
                        {
                            if (item[i].StatDate == X_DateTime[j])
                            {

                                DataLabels dl = new DataLabels();
                                SmallDataLabels smalldata = new SmallDataLabels();

                                dl.y = item[i].NewNum;
                                if (j % (MaxNumCoef) == 0)
                                    smalldata.enabled = true;

                                dl.dataLabels = smalldata;
                                //替换掉以前的null
                                sjModel2.data[j] = dl;


                            }
                        }

                    }
                    seriesJsonStr.Add(sjModel2);

                }

            }
            return seriesJsonStr;

        }
        protected string GetTableString(List<SoftUser> users, int tableindex)
        {
            string tableName = users[0].SoftId + "_" + users[0].Platform + "_" + begintime.ToShortDateString() + "_" + endtime.ToShortDateString() + "_" + net91com.Common.CryptoHelper.DES_Encrypt(channelId.ToString(), "ndwebweb");
            StringBuilder sb;
            if (tableindex == 0)
                sb = new StringBuilder(string.Format("<table id=\"tab{0}\" name=\"" + tableName + "\" class=\" tablesorter \" cellspacing=\"1\">", tableindex));
            else
                sb = new StringBuilder(string.Format("<table id=\"tab{0}\" name=\"" + tableName + "\"  class=\" tablesorter \" style=\"display:none\"  cellspacing=\"1\">", tableindex));
            sb.Append(@" <thead><tr><th>日期</th>
                  <th>新增用户</th></tr></thead>   ");
            sb.Append("<tbody>");
            int total = users.Sum(p => p.NewNum);
            foreach (SoftUser item in users)
            {
                string channel = item.ChannelName == null ? "不区分渠道" : item.ChannelName;
                //如果用户选择的是天的话，周六周天加特别样式
                if (Period == net91com.Stat.Core.PeriodOptions.Daily)
                {
                    //周六周天的话加特殊样式
                    if (item.StatDate.DayOfWeek == DayOfWeek.Sunday || item.StatDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        sb.Append("<tr class=\"tableover\"  >");
                        sb.Append(string.Format("<td style=\"color:Red;\" >{0}</td><td style=\"color:Red;\">{1}</td> ", item.StatDate.ToShortDateString(), Utility.SetNum(item.NewNum)));
                        sb.Append("</tr>");
                    }
                    else
                    {
                        sb.Append("<tr class=\"tableover\">");
                        sb.Append(string.Format("<td>{0}</td><td>{1}</td> ", item.StatDate.ToShortDateString(), Utility.SetNum(item.NewNum)));
                        sb.Append("</tr>");
                    }
                }

                else
                {
                    sb.Append("<tr class=\"tableover\">");
                    sb.Append(string.Format("<td>{0}</td><td>{1}</td> ", item.StatDate.ToShortDateString(), Utility.SetNum(item.NewNum)));
                    sb.Append("</tr>");
                }


            }
            sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", "总计", Utility.SetNum(total));
            sb.Append("</tbody></table>");
            return sb.ToString();
        }
    }
}