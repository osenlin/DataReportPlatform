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
using net91com.Stat.Services.sjqd.Entity;
using System.Text.RegularExpressions;
using net91com.Stat.Services;
using net91com.Reports.UserRights;
using net91com.Reports.Services.CommonServices.SjqdUserStat;

namespace net91com.Stat.Web.Reports
{
    public partial class SoftVersionSjqd : ReportBasePage
    {

        public string tableStr { get; set; }
        public string channelName;
        public int channelId { get; set; }
        public string reportTitle = "无数据";
        public string AxisJsonStr = "{}";
        public string SeriesJsonStr = "[]";
        //多条线
        public List<List<SoftUser>> ListAll = new List<List<SoftUser>>();

        /// <summary>
        /// 比较的对比曲线
        /// </summary>
        public List<List<SoftUser>> ListAllForCompare = new List<List<SoftUser>>();

        /// tab 切换的字符
        public List<string> TabStr = new List<string>();


        /// 用户选择平台数组 
        protected List<int> PlatformsidList = new List<int>();

        /// <summary>
        /// 是否是特殊周期
        /// </summary>
        public bool IsSuperPeriod { get; set; }

        public List<int> QuDaoList = new List<int>();

        ///里程碑曲线
        public List<List<Sjqd_MileStoneConfig>> mileStoneDate = new List<List<Sjqd_MileStoneConfig>>();

        //设置计算预测的数据的list 
        public List<ForecastSoftUser> ForeCastUserDatas = new List<ForecastSoftUser>();   

        /// 转换为坐标点json字符的中间类
        public List<SeriesJsonModel> seriesJsonStr = new List<SeriesJsonModel>();

        public List<string> tabStr = new List<string>();

        public int usercateType
        {
            get { return Convert.ToInt32(Request["t"] == "" ? "0" : Request["t"]); }
        }

        public int paraPlat
        {
            get
            {
                return
                    Convert.ToInt32(string.IsNullOrEmpty(Request["inputplatformselect"])
                                        ? "0"
                                        : Request["inputplatformselect"]);
            }
        }

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
        /// 判断是否为渠道数据
        /// </summary>
        public bool QuDao = false;

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
            //让弹出的渠道商下拉控件也支持不区分平台
            HeadControl1.Channel1.IsHasNoPlat = true;
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[]
                    {
                        net91com.Stat.Core.PeriodOptions.NaturalMonth,
                        net91com.Stat.Core.PeriodOptions.Of2Weeks,
                        net91com.Stat.Core.PeriodOptions.Weekly,
                        net91com.Stat.Core.PeriodOptions.Daily,
                        net91com.Stat.Core.PeriodOptions.Hours,
                        net91com.Stat.Core.PeriodOptions.TimeOfDay
                    },
                net91com.Stat.Core.PeriodOptions.Daily);
            Period = PeriodSelector1.SelectedPeriod;
            GetQueryString();
            SetCompareTime();
            SetStandardTime();
            IsSuperPeriod = (Period == net91com.Stat.Core.PeriodOptions.Hours || Period == net91com.Stat.Core.PeriodOptions.TimeOfDay);
            BindData();

        }

        protected void GetQueryString()
        {
            //Request["channeltype"] ==1 就是分类id了
            if (!string.IsNullOrEmpty(Request["channelid"]) && !string.IsNullOrEmpty(Request["channeltype"]))
            {

                channelId = Convert.ToInt32(Request["channelid"]);
                string name = "";
                //传过来是分类
                if (Request["channeltype"] != "1")
                {

                    var temp = new CfgChannelService().GetChannelCustomer(channelId);
                    softsid = temp.SoftID;
                    name = temp.Name;
                    HeadControl1.Channel1.ParentId = temp.PID == 0 ? "Categorie_" + temp.CID : "Customer_" + temp.PID;
                }
                else
                {
                    var temp = new CfgChannelService().GetChannelCategory(channelId);
                    ;
                    softsid = temp.SoftID;
                    name = temp.Name;
                    HeadControl1.Channel1.ParentId = "0";
                }
                //没有权限
                var softinfo = AvailableSofts.Find(p => p.ID == softsid);
                if (softinfo == null)
                {
                    Response.Redirect("/Reports/NoRight.aspx");
                }
                else
                {
                    HeadControl1.SoftID = softinfo.ID.ToString();
                    PlatformsidList = new List<int> {paraPlat};
                    HeadControl1.PlatID = PlatformsidList[0].ToString();
                    EndTime = ds.GetMaxTimeCache(Period, ReportType.UserUseNewActivity,CacheTimeOption.TenMinutes);
                    BeginTime = EndTime.AddDays(-30);
                    inputzhouqi.Value = Period.GetDescription();
                    HeadControl1.BeginTime = BeginTime;
                    HeadControl1.EndTime = EndTime;
                    HeadControl1.Channel1.SoftId = HeadControl1.SoftID;
                    HeadControl1.Channel1.Platform = HeadControl1.PlatID;
                    HeadControl1.Channel1.SelectedValue = channelId.ToString();
                    HeadControl1.Channel1.SelectedText = name;
                    HeadControl1.Channel1.SelectedCate = Request["channeltype"];
                    //记住查看用户类型
                    usercate.Value = usercateType.ToString();

                }
                
            }
            //不是其他页面通过中间页面跳转过来的
            else
            {
                if (HeadControl1.IsFirstLoad)
                {
                    EndTime =
                        EndTime =
                        ds.GetMaxTimeCache(Period, ReportType.UserUseNewActivity,CacheTimeOption.TenMinutes);
                    BeginTime = EndTime.AddDays(-30);
                    softsid = CookieSoftid;
                    PlatformsidList = new List<int> {CookiePlatid};
                    HeadControl1.PlatID = PlatformsidList[0].ToString();
                    HeadControl1.SoftID = softsid.ToString();
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
                    softsid = Convert.ToInt32(HeadControl1.SoftID);
                    var platformsids = HeadControl1.PlatID.Split(',');
                    foreach (string item in platformsids)
                    {
                        PlatformsidList.Add(Convert.ToInt32(item));
                    }
                    SetRequestCookie(softsid, PlatformsidList[0]);

                }
                HeadControl1.Channel1.PeriodCheck = false;
                
            }
            //开启对比功能
            HeadControl1.HiddenCompareTime = false;

            /////有渠道商权限
            //LimitChannelCustom(Convert.ToInt32(HeadControl1.Channel1.SoftId));
        }

        protected void BindData()
        {
            List<SoftUser> users;
            //是否给外部部人看的,自己内部人看，不用乘以系数的结果，给外部要乘
            bool isForOut = (usercate.Value == "11");
            if (isForOut)
            {
                reportTitle = "分渠道统计(外部)";
            }
            else
            {
                reportTitle = "分渠道统计(内部)";
            }
            //不区分渠道
            if (HeadControl1.Channel1.ChannelValues.Count == 0)
            {
                for (int i = 0; i < PlatformsidList.Count; i++)
                {
                    if (IsSuperPeriod)
                    {
                        //users =
                        //    StatUsersByHourService.GetInstance()
                        //                          .GetHourUserDataCache(softsid, PlatformsidList[i], BeginTime, EndTime,
                        //                                                Period, loginService,
                        //                                               CacheTimeOption.TenMinutes)
                        //                          .OrderBy(p => p.StatDate)
                        //                          .ToList();

                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                        ssUsers = suService.GetStatUsersByHour(softsid, PlatformsidList[i], ChannelTypeOptions.Category, 0, (int)Period, BeginTime, EndTime);
                        users = new List<SoftUser>();
                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                        {
                            SoftUser softuser = new SoftUser();
                            softuser.StatDate = u.StatDate;
                            softuser.SoftId = softsid;
                            softuser.Platform = PlatformsidList[i];
                            softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                            softuser.Hour = u.StatHour;
                            softuser.NewNum = u.NewUserCount;
                            softuser.UseNum = u.ActiveUserCount;
                            users.Add(softuser);
                        }
                        users = users.OrderBy(p => p.StatDate).ToList();

                        //获取对比曲线
                        if (HeadControl1.CompareTimeType > 0)
                        {
                            //List<SoftUser> usersCompare =
                            //    StatUsersByHourService.GetInstance()
                            //                          .GetHourUserDataCache(softsid, PlatformsidList[i],
                            //                                                BeginCompareTime, EndCompareTime, Period,
                            //                                                loginService,
                            //                                               CacheTimeOption.TenMinutes)
                            //                          .OrderBy(p => p.StatDate).ToList();

                            List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsersComp;
                            ssUsersComp = suService.GetStatUsersByHour(softsid, PlatformsidList[i], ChannelTypeOptions.Category, 0, (int)Period, BeginCompareTime, EndCompareTime);
                            List<SoftUser> usersCompare = new List<SoftUser>();
                            foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsersComp)
                            {
                                SoftUser softuser = new SoftUser();
                                softuser.StatDate = u.StatDate;
                                softuser.SoftId = softsid;
                                softuser.Platform = PlatformsidList[i];
                                softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                                softuser.Hour = u.StatHour;
                                softuser.NewNum = u.NewUserCount;
                                softuser.UseNum = u.ActiveUserCount;
                                usersCompare.Add(softuser);
                            }
                            usersCompare = usersCompare.OrderBy(p => p.StatDate).ToList();

                            if (usersCompare.Count != 0)
                            {
                                ListAllForCompare.Add(usersCompare);

                            }
                        }
                    }
                    else
                    {
                        users =
                            Sjqd_StatUsersService.GetInstance()
                                                 .GetSoftUserListCache(BeginTime, EndTime, softsid, PlatformsidList[i],
                                                                       Period, loginService,
                                                                      CacheTimeOption.TenMinutes)
                                                 .OrderBy(p => p.StatDate)
                                                 .ToList();

                            //获取计算预测数据
                       ForeCastUserDatas.AddRange(Sjqd_StatUsersService.GetInstance().GetForecastSoftUserCache(
                           softsid, PlatformsidList[i], Period, loginService,CacheTimeOption.TenMinutes));
                        

                        //获取对比曲线
                        if (HeadControl1.CompareTimeType > 0)
                        {
                            List<SoftUser> usersCompare =
                                Sjqd_StatUsersService.GetInstance()
                                                     .GetSoftUserListCache(BeginCompareTime, EndCompareTime, softsid,
                                                                           PlatformsidList[i], Period, loginService,
                                                                          CacheTimeOption.TenMinutes)
                                                     .OrderBy(p => p.StatDate)
                                                     .ToList();

                            if (usersCompare.Count != 0)
                            {
                                ListAllForCompare.Add(usersCompare);

                            }
                        }

                    }
                    if (users.Count != 0)
                    {
                        ListAll.Add(users);
                        //添加里程碑列表集(若ListAll添加了，里程碑也要添加)
                        //mileStoneDate.Add(Sjqd_MileStoneConfigService.GetInstance()
                        //                        .GetMileStoneDatesByCache(softsid, PlatformsidList[i], BeginTime, EndTime));

                    }

                }
            }
            else
            {
                QuDao = true;
                var channels = HeadControl1.Channel1.ChannelValues;
                if (IsSuperPeriod)
                {

                    for (int i = 0; i < channels.Count; i++)
                    {
                        //小时和每小时将其和到一条曲线中去了
                        //users = Sjqd_StatUsersByChannelsService.GetInstance().GetChannelUsersByHoursDataCache(
                        //    BeginTime, EndTime, softsid, channels[i].Platform, Period, channels[i].ChannelType,
                        //    Convert.ToInt32(channels[i].ChannelValue), channels[i].ChannelText, loginService,
                        //   CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();

                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                        ssUsers = suService.GetStatUsersByHour(softsid, channels[i].Platform, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), (int)Period, BeginTime, EndTime);
                        users = new List<SoftUser>();
                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                        {
                            SoftUser softuser = new SoftUser();
                            softuser.StatDate = u.StatDate;
                            softuser.SoftId = softsid;
                            softuser.Platform = channels[i].Platform;
                            softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                            softuser.Hour = u.StatHour;
                            softuser.NewNum = u.NewUserCount;
                            softuser.UseNum = u.ActiveUserCount;
                            users.Add(softuser);
                        }
                        users = users.OrderBy(p => p.StatDate).ToList();

                        if (users.Count != 0)
                        {
                            //记住有数据渠道的编号
                            QuDaoList.Add(i);
                            ListAll.Add(users);
                            //添加里程碑列表集(若ListAll添加了，里程碑也要添加)
                            //mileStoneDate.Add(Sjqd_MileStoneConfigService.GetInstance()
                            //                        .GetMileStoneDatesByCache(softsid, channels[i].Platform, BeginTime, EndTime));

                        }
                        //获取对比曲线
                        if (HeadControl1.CompareTimeType > 0)
                        {
                            //List<SoftUser> usersCompare =
                            //    Sjqd_StatUsersByChannelsService.GetInstance().GetChannelUsersByHoursDataCache(
                            //        BeginCompareTime, EndCompareTime, softsid, channels[i].Platform, Period,
                            //        channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue),
                            //        channels[i].ChannelText, loginService,CacheTimeOption.TenMinutes)
                            //                                   .OrderBy(p => p.StatDate)
                            //                                   .ToList();

                            List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsersComp;
                            ssUsersComp = suService.GetStatUsersByHour(softsid, channels[i].Platform, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), (int)Period, BeginCompareTime, EndCompareTime);
                            List<SoftUser> usersCompare = new List<SoftUser>();
                            foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsersComp)
                            {
                                SoftUser softuser = new SoftUser();
                                softuser.StatDate = u.StatDate;
                                softuser.SoftId = softsid;
                                softuser.Platform = channels[i].Platform;
                                softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                                softuser.Hour = u.StatHour;
                                softuser.NewNum = u.NewUserCount;
                                softuser.UseNum = u.ActiveUserCount;
                                usersCompare.Add(softuser);
                            }
                            usersCompare = usersCompare.OrderBy(p => p.StatDate).ToList();

                            if (usersCompare.Count != 0)
                            {
                                ListAllForCompare.Add(usersCompare);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < channels.Count; i++)
                    {
                        users =
                            Sjqd_StatUsersByChannelsService.GetInstance()
                                                           .GetSoftUserChanelListCache(BeginTime, EndTime, softsid,
                                                                                       channels[i].Platform
                                                                                       , Period, channels[i].ChannelType,
                                                                                       Convert.ToInt32(
                                                                                           channels[i].ChannelValue),
                                                                                       channels[i].ChannelText, isForOut,
                                                                                       loginService,
                                                                                      CacheTimeOption.TenMinutes)
                                                           .OrderBy(p => p.StatDate)
                                                           .ToList();
                        if (users.Count != 0)
                        {
                            //记住有数据渠道的编号
                            QuDaoList.Add(i);
                            ListAll.Add(users);
                            //添加里程碑列表集(若ListAll添加了，里程碑也要添加)
                            //mileStoneDate.Add(Sjqd_MileStoneConfigService.GetInstance()
                            //                        .GetMileStoneDatesByCache(softsid, channels[i].Platform, BeginTime, EndTime));

                            //if (Period == net91com.Stat.Core.PeriodOptions.Daily)
                            //{
                            //    ForecastSoftUser softUser = Sjqd_StatUsersByChannelsService.GetInstance().GetStatUsersForRealTime(softsid,
                            //                                                           channels[i].Platform, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), loginService, CacheTimeOption.TenMinutes);
                            //    if (softUser != null)
                            //        ForeCastUserDatas.Add(softUser);
                            //}
                        }
                        //获取对比曲线
                        if (HeadControl1.CompareTimeType > 0)
                        {
                            List<SoftUser> usersCompare =
                                Sjqd_StatUsersByChannelsService.GetInstance()
                                                               .GetSoftUserChanelListCache(BeginCompareTime,
                                                                                           EndCompareTime, softsid,
                                                                                           channels[i].Platform, Period,
                                                                                           channels[i].ChannelType,
                                                                                           Convert.ToInt32(
                                                                                               channels[i].ChannelValue),
                                                                                           channels[i].ChannelText,
                                                                                           isForOut, loginService,
                                                                                          CacheTimeOption
                                                                                               .TenMinutes)
                                                               .OrderBy(p => p.StatDate)
                                                               .ToList();

                            if (usersCompare.Count != 0)
                            {
                                ListAllForCompare.Add(usersCompare);
                            }
                        }
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
                string plat = ((MobileOption) ListAll[j][0].Platform).GetDescription();
                if ((MobileOption) ListAll[j][0].Platform == MobileOption.None)
                    plat = "不区分平台";
                string channel = ListAll[j][0].ChannelName;
                if (string.IsNullOrEmpty(channel))
                    channel = "不区分渠道";
                tabStr.Add(plat + "_" + channel);
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
            SetChart(chart, ListAllForCompare,true);
            AxisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140{1},step:{2}}}}}"
                , chart.GetXJson()
                , Period == net91com.Stat.Core.PeriodOptions.Hours ? ",rotation:-45,x:-40,y:60" : (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay ? ",x:-5" : ",rotation:-45,x:-30,y:45")
                , chart.Step);

            SeriesJsonStr = chart.GetYJson(
                delegate(LineChartPoint point)
                {
                    SoftUser user = (SoftUser)point.DataContext;
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
        protected void SetChart(LineChart chart, List<List<SoftUser>> list,bool iscompare=false)
        {
            //对比线前后时间差值
            int intervalDays = 0;
            if (iscompare && ListAll[0].Count != 0 &&ListAllForCompare.Count!=0&& ListAllForCompare[0].Count != 0)
            {
                intervalDays = ListAll[0][0].StatDate.Subtract(ListAllForCompare[0][0].StatDate).Days;
            }
            for (int i = 0; i < list.Count; i++)
            {
                string softName = GetSoft(list[i][0].SoftId).Name;
                string platform = "_" + ((MobileOption)list[i][0].Platform).GetDescription();
                if (platform == "_None") platform = "_不区分平台";
                string channelName = QuDao ? "_" + list[i][0].ChannelName : string.Empty;
                List<LineChartPoint> MyPoints = null;
                if (reporttype.Value == "0")
                {
                    MyPoints =
                        list[i].Select(
                            a => new LineChartPoint { XValue = a.StatDate, YValue = a.NewNum, DataContext = a }).ToList();
                }
                else
                {
                    MyPoints =
                        list[i].Select(
                            a =>
                            new LineChartPoint
                            {
                                XValue = a.StatDate,
                                YValue = a.ActiveNum,
                                DataContext = a,
                                Denominator = a.TotalNum,
                                Percent = a.ActivityPercent
                            }).ToList();
                }
                LineChartLine line = new LineChartLine
                {
                    Name = softName + platform + channelName + (iscompare?"_对比线":""),
                    Show = true,
                    XIntervalDays = intervalDays,
                    Points = MyPoints
                };

                if (ForeCastUserDatas.Count > 0 && reporttype.Value == "0" && !iscompare)
                {

                    List<ForecastSoftUser> tempMyForecastUser = ForeCastUserDatas.Where(p => p.Period == (int)Period).Where(p => p.SoftId == ListAll[i][0].SoftId && p.Platform == ListAll[i][0].Platform && ListAll[i][0].ChannelID == p.ChannelID).OrderByDescending(p => p.StatDate).ToList();
                    SoftUser user = GetForeCastSoftUser(tempMyForecastUser);
                    if (user != null)
                        line.ForecastPoint = new LineChartPoint { YValue = user.NewNum, DataContext = user };
                }
                //mileStoneDate[i].ForEach(a => line.AddMarker(a.MileStoneDate, Period, a.Remarks));
                chart.Y.Add(line);
            }
        }


        //计算预估时间
        protected SoftUser GetForeCastSoftUser(List<ForecastSoftUser> softUser)
        {
            if (softUser.Count == 0)
                return null;

            ///第0个是边缘点,对应最后统计的那个值
            SoftUser su = new SoftUser();
            if (softUser.Count > 1)
            {
                List<double> countDouble = new List<double>();
                for (int i = softUser.Count - 1; i >= 0; i--)
                {
                    double tempdouble;
                    if (i == 0)
                        break;
                    if (softUser[i - 1].TotalUserCount == 0)
                        tempdouble = 0;
                    else
                        tempdouble = softUser[i - 1].NewUserCount / (double)softUser[i].TotalUserCount;
                    countDouble.Add(tempdouble);
                }
                double allDouble = 0;
                foreach (var item in countDouble)
                {
                    allDouble += item;
                }
                su.Growth = (allDouble / countDouble.Count * 100).ToString("0.00") + "%";
                su.NewNum = (int)(allDouble / countDouble.Count * softUser[0].TotalUserCount);
            }
            else if (softUser.Count == 1)  //如果只有一个参考值,则可以认为是实时计算结果
            {
                su.Growth = "";
                su.NewNum = softUser[0].NewUserCount;
            }
            su.Period = softUser[0].Period;
            su.Platform = softUser[0].Platform;
            su.TotalNum = su.NewNum + softUser[0].TotalUserCount;
            su.StatDate = softUser[0].ForecaseDate;
            return su;

        }

        /// <summary>
        /// 形成每一个tab
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tableindex"></param>
        /// <returns></returns>
        protected string GetTableString(List<SoftUser> users, int tableindex)
        {
            users = users.OrderByDescending(p => p.StatDate).ToList();
            string tableName = string.Empty;
            //没有选择渠道
            if (HeadControl1.Channel1.ChannelValues.Count == 0)
                tableName = users[0].SoftId + "_" + users[0].Platform + "_" + (int) Period + "_" +
                            BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_" + 0 + "_" + 1 + "_" +
                            (usercate.Value != "11" ? "0" : "1");
            else
            {
                var channels = HeadControl1.Channel1.ChannelValues;
                tableName = users[0].SoftId + "_" + users[0].Platform + "_" + (int) Period + "_" +
                            BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_" +
                            channels[QuDaoList[tableindex]].ChannelValue + "_" +
                            (int) channels[QuDaoList[tableindex]].ChannelType + "_" +
                            (usercate.Value != "11" ? "0" : "1");
            }
            return TableTemplateHelper.BuildStatUsersTable(users[0].SoftId, (MobileOption)users[0].Platform, false, true, Period, "new,active", true, users, false, tableindex, tableName);
        }

        ///严格规范时间范围(每小时)
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


        /// <summary>
        /// 设置比较的开始时间和结束时间
        /// </summary>
        protected void SetCompareTime()
        {
            int type = HeadControl1.CompareTimeType;
            int daysSpan = 0;
            daysSpan = (EndTime - BeginTime).Days;
            if (HeadControl1.CompareTimeType > 0)
            {
                switch (type)
                {
                    case 1:
                        BeginCompareTime = BeginTime.AddDays(-7);
                        EndCompareTime = BeginCompareTime.AddDays(daysSpan);
                        break;
                    case 2:
                        BeginCompareTime = BeginTime.AddMonths(-1);
                        EndCompareTime = BeginCompareTime.AddDays(daysSpan);
                        break;
                    case 3:
                        EndCompareTime = HeadControl1.CompareBeginTime.AddDays(daysSpan);
                        BeginCompareTime = HeadControl1.CompareBeginTime;
                        break;
                }


            }
            BeginCompareTime = new DateTime(BeginCompareTime.Year, BeginCompareTime.Month, BeginCompareTime.Day);
            EndCompareTime = new DateTime(EndCompareTime.Year, EndCompareTime.Month, EndCompareTime.Day);
        }

        /// <summary>
        /// 获取里程碑比较时间
        /// </summary>
        /// <param name="begintime"></param>
        /// <returns></returns>
        protected DateTime GetMileStoneBeginTime(DateTime begintime)
        {
            DateTime dtMileBegin = begintime;
            switch (Period)
            {

                case net91com.Stat.Core.PeriodOptions.Daily:
                    dtMileBegin = begintime.AddDays(0);
                    break;
                case net91com.Stat.Core.PeriodOptions.Hours:
                    dtMileBegin = begintime.AddDays(0);
                    break;
                case net91com.Stat.Core.PeriodOptions.Monthly:
                    dtMileBegin = begintime.AddMonths(-1).AddDays(1);
                    break;
                case net91com.Stat.Core.PeriodOptions.TimeOfDay:
                    dtMileBegin = begintime.AddDays(0);
                    break;
                case net91com.Stat.Core.PeriodOptions.LatestTwoWeeks:
                    dtMileBegin = begintime.AddDays(-13).AddDays(1);
                    break;
                case net91com.Stat.Core.PeriodOptions.Weekly:
                    dtMileBegin = begintime.AddDays(-6);
                    break;
                default:
                    break;
            }
            return dtMileBegin;
        }
    }
}