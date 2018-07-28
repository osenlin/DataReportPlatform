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
//using BY.AccessControlCore;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports
{
    public partial class NewUserReport : ReportBasePage
    {
        public List<List<Sjqd_MileStoneConfig>> mileStoneDate = new List<List<Sjqd_MileStoneConfig>>();

        public List<int> QuDaoList = new List<int>();

        public bool QuDao { get; set; }
        /// <summary>
        /// 定义开始比较时间
        /// </summary>
        public DateTime BeginCompareTime { get; set; }
        /// <summary>
        /// 定义结束比较时间
        /// </summary>
        public DateTime EndCompareTime { get; set; }

        ///获取真正支持的平台数组
        protected List<int> RealSoftLine = new List<int>();

        //数据库交互对象
        protected UtilityService ds = UtilityService.GetInstance();

        ///用户绑定图的title
        public string ReportTitle { get; set; }
        /// 横轴显示的json字符
        public string AxisJsonStr { get; set; }
        /// <summary>
        /// 具体点坐标的json字符
        /// </summary>
        public string SeriesJsonStr { get; set; }
        /// <summary>
        /// htmltable 字符串
        /// </summary>
        public string TableStr { get; set; }

        ///设置计算预测的数据的list 
        public List<ForecastSoftUser> ForeCastUserDatas = new List<ForecastSoftUser>();

        ///每一条线一个List<SoftNewUser> 这里多条线
        public List<List<SoftUser>> ListAll = new List<List<SoftUser>>();
        /// <summary>
        /// 比较的对比曲线
        /// </summary>
        public List<List<SoftUser>> ListAllForCompare = new List<List<SoftUser>>();

        /// tab 切换的字符
        public List<string> TabStr = new List<string>();

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
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControls()
        {

            //初始化控件的部分属性
            HeadControl1.Channel1.PeriodCheck = false;
            HeadControl1.MySupportSoft = AvailableSofts;
            //让弹出的渠道商下拉控件也支持不区分平台
            HeadControl1.Channel1.IsHasNoPlat = true;
            HeadControl1.IsHasNoPlat = true;
            HeadControl1.IsSoftSingle = false;
            HeadControl1.IsPlatSingle = false;
            HeadControl1.HiddenCompareTime = false;

            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[] {
                    net91com.Stat.Core.PeriodOptions.NaturalMonth,
                    net91com.Stat.Core.PeriodOptions.Of2Weeks,
                    net91com.Stat.Core.PeriodOptions.Weekly,
                    net91com.Stat.Core.PeriodOptions.Daily,
                    net91com.Stat.Core.PeriodOptions.Hours,
                    net91com.Stat.Core.PeriodOptions.TimeOfDay, },
                    net91com.Stat.Core.PeriodOptions.Daily);
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
                                            ? (HeadControl1.IsFirstLoad
                                                   ? new List<int>()
                                                   : HeadControl1.SoftID.Split(new char[] { ',' },
                                                                               StringSplitOptions.RemoveEmptyEntries)
                                                                 .Select(p => int.Parse(p))
                                                                 .ToList())
                                            : Request["inputsoftselect"].Split(new char[] { ',' },
                                                                               StringSplitOptions.RemoveEmptyEntries)
                                                                        .Select(p => int.Parse(p))
                                                                        .ToList();
            List<MobileOption> selectedPlatforms = string.IsNullOrEmpty(Request["inputplatformselect"])
                                                       ? (HeadControl1.IsFirstLoad
                                                              ? new List<MobileOption>()
                                                              : HeadControl1.PlatID.Split(new char[] { ',' },
                                                                                          StringSplitOptions
                                                                                              .RemoveEmptyEntries)
                                                                            .Select(p => (MobileOption)int.Parse(p))
                                                                            .ToList())
                                                       : Request["inputplatformselect"].Split(new char[] { ',' },
                                                                                              StringSplitOptions
                                                                                                  .RemoveEmptyEntries)
                                                                                       .Select(
                                                                                           p =>
                                                                                           (MobileOption)int.Parse(p))
                                                                                       .ToList();

            //验证参数
            CheckParams(selectedSoftIds, selectedPlatforms, period, beginTime, endTime, ReportType.UserUseNewActivity);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        protected void SetParams()
        {
            //初次加载一些参数设置成默认值
            ReportTitle = "新增用户";
            AxisJsonStr = "{}";
            SeriesJsonStr = "[]";
            //设置控件的属性值
            HeadControl1.BeginTime = BeginTime;
            HeadControl1.EndTime = EndTime;
            HeadControl1.HiddenCompareTime = false;
            HeadControl1.SoftID = string.Join(",", SelectedSofts.Select(p => p.ID.ToString()).ToArray());
            HeadControl1.PlatID = string.Join(",", SelectedPlatforms.Select(p => ((int)p).ToString()).ToArray());
            HeadControl1.Channel1.SoftId = SelectedSofts[0].ID.ToString();
            HeadControl1.Channel1.Platform = HeadControl1.PlatID;
            /////有渠道商权限限制
            //LimitChannelCustom(Convert.ToInt32(HeadControl1.Channel1.SoftId));

            //规范比较时间
            SetCompareTime();

        }

        /// <summary>
        /// 获取数据加上绑定数据
        /// </summary>
        protected void BindData()
        {
            ///进入分渠道统计
            if (HeadControl1.Channel1.ChannelValues.Count != 0)
            {
                var channels = HeadControl1.Channel1.ChannelValues;
                QuDao = true;
                if (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay || Period == net91com.Stat.Core.PeriodOptions.Hours)
                {
                    for (int i = 0; i < channels.Count; i++)
                    {

                        //List<SoftUser> users =
                        //    Sjqd_StatUsersByChannelsService.GetInstance().GetChannelUsersByHoursDataCache(
                        //        BeginTime, EndTime, SelectedSofts[0].ID, channels[i].Platform, Period,
                        //        channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue),
                        //        channels[i].ChannelText, loginService,CacheTimeOption.TenMinutes)
                        //                                   .OrderBy(p => p.StatDate)
                        //                                   .ToList();

                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                        ssUsers = suService.GetStatUsersByHour(SelectedSofts[0].ID, channels[i].Platform, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), (int)Period, BeginTime, EndTime);
                        List<SoftUser> users = new List<SoftUser>();
                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                        {
                            SoftUser softuser = new SoftUser();
                            softuser.StatDate = u.StatDate;
                            softuser.SoftId = SelectedSofts[0].ID;
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
                            ListAll.Add(users);
                            //添加里程碑列表集(若ListAll添加了，里程碑也要添加)
                            //mileStoneDate.Add(Sjqd_MileStoneConfigService.GetInstance()
                            //                                             .GetMileStoneDatesByCache(SelectedSofts[0].ID,
                            //                                                                       channels[i].Platform,
                            //                                                                       BeginTime, EndTime));

                            QuDaoList.Add(i);
                        }
                        //获取对比曲线
                        if (HeadControl1.CompareTimeType > 0)
                        {
                            //List<SoftUser> usersCompare =
                            //    Sjqd_StatUsersByChannelsService.GetInstance().GetChannelUsersByHoursDataCache(
                            //        BeginCompareTime, EndCompareTime, SelectedSofts[0].ID, channels[i].Platform, Period,
                            //        channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue),
                            //        channels[i].ChannelText, loginService,CacheTimeOption.TenMinutes)
                            //                                   .OrderBy(p => p.StatDate)
                            //                                   .ToList();

                            List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsersComp;
                            ssUsersComp = suService.GetStatUsersByHour(SelectedSofts[0].ID, channels[i].Platform, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), (int)Period, BeginCompareTime, EndCompareTime);
                            List<SoftUser> usersCompare = new List<SoftUser>();
                            foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsersComp)
                            {
                                SoftUser softuser = new SoftUser();
                                softuser.StatDate = u.StatDate;
                                softuser.SoftId = SelectedSofts[0].ID;
                                softuser.Platform = channels[i].Platform;
                                softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                                softuser.Hour = u.StatHour;
                                softuser.NewNum = u.NewUserCount;
                                softuser.UseNum = u.ActiveUserCount;
                                usersCompare.Add(softuser);
                            }
                            usersCompare = usersCompare.OrderBy(p => p.StatDate).ToList();

                            if (usersCompare != null && usersCompare.Count != 0)
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

                        List<SoftUser> users =
                            Sjqd_StatUsersByChannelsService.GetInstance()
                                                           .GetSoftUserChanelListCache(BeginTime, EndTime,
                                                                                       SelectedSofts[0].ID,
                                                                                       channels[i].Platform, Period,
                                                                                       channels[i].ChannelType,
                                                                                       Convert.ToInt32(
                                                                                           channels[i].ChannelValue),
                                                                                       channels[i].ChannelText, false,
                                                                                       loginService,
                                                                                      CacheTimeOption.TenMinutes)
                                                           .OrderBy(p => p.StatDate)
                                                           .ToList();
                        if (users != null && users.Count != 0)
                        {
                            ListAll.Add(users);
                            ////添加里程碑列表集(若ListAll添加了，里程碑也要添加)
                            //mileStoneDate.Add(Sjqd_MileStoneConfigService.GetInstance()
                            //                        .GetMileStoneDatesByCache(SelectedSofts[0].ID, channels[i].Platform, BeginTime, EndTime));
                            QuDaoList.Add(i);

                            //if (Period == net91com.Stat.Core.PeriodOptions.Daily)
                            //{
                            //    ForecastSoftUser softUser =
                            //        Sjqd_StatUsersByChannelsService.GetInstance()
                            //                                       .GetStatUsersForRealTime(SelectedSofts[0].ID,
                            //                                                                channels[i].Platform,
                            //                                                                channels[i].ChannelType,
                            //                                                                Convert.ToInt32(
                            //                                                                    channels[i].ChannelValue),
                            //                                                                loginService,
                            //                                                                CacheTimeOption.TenMinutes);
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
                                                                                           EndCompareTime,
                                                                                           SelectedSofts[0].ID,
                                                                                           channels[i].Platform, Period,
                                                                                           channels[i].ChannelType,
                                                                                           Convert.ToInt32(channels[i].ChannelValue),
                                                                                           channels[i].ChannelText,
                                                                                           false, loginService,
                                                                                          CacheTimeOption.TenMinutes)
                                                               .OrderBy(p => p.StatDate)
                                                               .ToList();

                            if (usersCompare != null && usersCompare.Count != 0)
                            {
                                ListAllForCompare.Add(usersCompare);

                            }
                        }

                    }
                }

            }
            else
            {
                //选择所筛选的所有软件
                for (int softid = 0; softid < SelectedSofts.Count; softid++)
                {
                    for (int i = 0; i < SelectedPlatforms.Count; i++)
                    {
                        foreach (MobileOption tempplat in SelectedSofts[softid].Platforms)
                        {
                            //他选出的平台加上和自己支持平台的交集
                            if (tempplat == SelectedPlatforms[i] || SelectedPlatforms[i] == 0)
                            {

                                List<SoftUser> users = null;
                                if (Period != net91com.Stat.Core.PeriodOptions.TimeOfDay && Period != net91com.Stat.Core.PeriodOptions.Hours)
                                {
                                    users =
                                        Sjqd_StatUsersService.GetInstance()
                                                             .GetSoftUserListCache(BeginTime, EndTime,
                                                                                   SelectedSofts[softid].ID,
                                                                                   (int)SelectedPlatforms[i], Period,
                                                                                   loginService,
                                                                                  CacheTimeOption.TenMinutes)
                                                             .OrderBy(p => p.StatDate)
                                                             .ToList();
                                    if (Period == net91com.Stat.Core.PeriodOptions.Daily)
                                    {
                                        //ForecastSoftUser softUser =
                                        //    Sjqd_StatUsersService.GetInstance()
                                        //                         .GetStatUsersForRealTime(SelectedSofts[softid].ID,
                                        //                                                  (int)SelectedPlatforms[i],
                                        //                                                  loginService,
                                        //                                                  CacheTimeOption.TenMinutes);
                                        //if (softUser != null)
                                        //    ForeCastUserDatas.Add(softUser);
                                        //else
                                            ForeCastUserDatas.AddRange(
                                                Sjqd_StatUsersService.GetInstance()
                                                                     .GetForecastSoftUserCache(
                                                                         SelectedSofts[softid].ID,
                                                                         (int)SelectedPlatforms[i], Period,
                                                                         loginService, CacheTimeOption.TenMinutes));
                                    }
                                    else
                                    {
                                        //获取计算预测数据
                                        ForeCastUserDatas.AddRange(
                                            Sjqd_StatUsersService.GetInstance()
                                                                 .GetForecastSoftUserCache(SelectedSofts[softid].ID,
                                                                                           (int)SelectedPlatforms[i],
                                                                                           Period, loginService,
                                                                                          CacheTimeOption
                                                                                               .TenMinutes));
                                    }
                                }
                                else
                                {
                                    //users =
                                    //    StatUsersByHourService.GetInstance()
                                    //                          .GetHourUserDataCache(SelectedSofts[softid].ID,
                                    //                                                (int) SelectedPlatforms[i],
                                    //                                                BeginTime, EndTime, Period,
                                    //                                                loginService,
                                    //                                               CacheTimeOption.TenMinutes)
                                    //                          .OrderBy(p => p.StatDate)
                                    //                          .ToList();

                                    net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                                    List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                                    ssUsers = suService.GetStatUsersByHour(SelectedSofts[softid].ID, (int)SelectedPlatforms[i], ChannelTypeOptions.Category, 0, (int)Period, BeginTime, EndTime);
                                    users = new List<SoftUser>();
                                    foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                                    {
                                        SoftUser softuser = new SoftUser();
                                        softuser.StatDate = u.StatDate;
                                        softuser.SoftId = SelectedSofts[softid].ID;
                                        softuser.Platform = (int)SelectedPlatforms[i];
                                        softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                                        softuser.Hour = u.StatHour;
                                        softuser.NewNum = u.NewUserCount;
                                        softuser.UseNum = u.ActiveUserCount;
                                        users.Add(softuser);
                                    }
                                    users = users.OrderBy(p => p.StatDate).ToList();
                                }


                                if (users != null && users.Count != 0)
                                {
                                    ListAll.Add(users);
                                    //添加里程碑列表集(若ListAll添加了，里程碑也要添加)
                                    //mileStoneDate.Add(Sjqd_MileStoneConfigService.GetInstance()
                                    //                                             .GetMileStoneDatesByCache(
                                    //                                                 SelectedSofts[softid].ID,
                                    //                                                 (int)SelectedPlatforms[i],
                                    //                                                 BeginTime, EndTime));
                                    RealSoftLine.Add((int)SelectedPlatforms[i]);
                                }
                                //获取对比曲线
                                if (HeadControl1.CompareTimeType > 0)
                                {
                                    List<SoftUser> usersCompare = null;
                                    if (Period != net91com.Stat.Core.PeriodOptions.TimeOfDay && Period != net91com.Stat.Core.PeriodOptions.Hours)
                                        usersCompare =
                                            Sjqd_StatUsersService.GetInstance()
                                                                 .GetSoftUserListCache(BeginCompareTime, EndCompareTime,
                                                                                       SelectedSofts[softid].ID,
                                                                                       (int)SelectedPlatforms[i],
                                                                                       Period, loginService,
                                                                                      CacheTimeOption.TenMinutes)
                                                                 .OrderBy(p => p.StatDate)
                                                                 .ToList();
                                    else
                                    {
                                        //usersCompare =
                                        //    StatUsersByHourService.GetInstance()
                                        //                          .GetHourUserDataCache(SelectedSofts[softid].ID,
                                        //                                                (int)SelectedPlatforms[i],
                                        //                                                BeginCompareTime, EndCompareTime,
                                        //                                                Period, loginService,
                                        //                                               CacheTimeOption.TenMinutes)
                                        //                          .OrderBy(p => p.StatDate)
                                        //                          .ToList();

                                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                                        ssUsers = suService.GetStatUsersByHour(SelectedSofts[softid].ID, (int)SelectedPlatforms[i], ChannelTypeOptions.Category, 0, (int)Period, BeginCompareTime, EndCompareTime);
                                        usersCompare = new List<SoftUser>();
                                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                                        {
                                            SoftUser softuser = new SoftUser();
                                            softuser.StatDate = u.StatDate;
                                            softuser.SoftId = SelectedSofts[softid].ID;
                                            softuser.Platform = (int)SelectedPlatforms[i];
                                            softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                                            softuser.Hour = u.StatHour;
                                            softuser.NewNum = u.NewUserCount;
                                            softuser.UseNum = u.ActiveUserCount;
                                            usersCompare.Add(softuser);
                                        }
                                        usersCompare = usersCompare.OrderBy(p => p.StatDate).ToList();

                                    }

                                    if (usersCompare != null && usersCompare.Count != 0)
                                    {
                                        ListAllForCompare.Add(usersCompare);
                                    }
                                }
                                break;

                            }

                        }
                    }

                }
            }

            if (ListAll.Count == 0)
                return;


            //生成所有曲线的JSON
            GetAllLineJson();

            //生成Table的HTML
            GetAllTableHtml();

        }

        /// <summary>
        /// 生成所有曲线的JSON
        /// </summary>
        protected void GetAllLineJson()
        {
            LineChart chart = new LineChart(BeginTime, EndTime);
            chart.Period = Period;
            chart.NeedForecastPoint = (Period == net91com.Stat.Core.PeriodOptions.Daily || Period == net91com.Stat.Core.PeriodOptions.Weekly || Period == net91com.Stat.Core.PeriodOptions.Monthly);
            for (int i = 0; i < ListAll.Count; i++)
            {
                string softName = GetSoft(ListAll[i][0].SoftId).Name;
                string platform = "_" + ((MobileOption)ListAll[i][0].Platform).GetDescription();
                if (platform == "_None") platform = "_不区分平台";
                string channelName = QuDao ? "_" + ListAll[i][0].ChannelName : string.Empty;
                LineChartLine line = new LineChartLine
                {
                    Name = softName + platform + channelName,
                    Show = true,
                    XIntervalDays = 0,
                    Points = ListAll[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.NewNum, DataContext = a }).ToList()
                };

                if (ForeCastUserDatas.Count > 0)
                {
                    List<ForecastSoftUser> tempMyForecastUser =
                        ForeCastUserDatas.Where(p => p.Period == (int)Period)
                                         .Where(
                                             p =>
                                             p.SoftId == ListAll[i][0].SoftId && p.Platform == ListAll[i][0].Platform &&
                                             ListAll[i][0].ChannelID == p.ChannelID)
                                         .OrderByDescending(p => p.StatDate)
                                         .ToList();
                    SoftUser user = GetForeCastSoftUser(tempMyForecastUser);
                    if (user != null)
                        line.ForecastPoint = new LineChartPoint { YValue = user.NewNum, DataContext = user };
                }
                //mileStoneDate[i].ForEach(a => line.AddMarker(a.MileStoneDate, Period, a.Remarks));
                chart.Y.Add(line);
                ////增加新增用户未修正的线
                //if (!QuDao && checkForNotUpdated.Checked && Period != PeriodOptions.TimeOfDay && Period != PeriodOptions.Hours)
                //{
                //    LineChartLine line2 = new LineChartLine
                //    {
                //        Name = softName + platform + channelName + "_未修正数据",
                //        Show = true,
                //        XIntervalDays = 0,
                //        Points = ListAll[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.FirstNewUserCount + a.NewUserCount_Shanzhai, DataContext = a, Type = -1 }).ToList()
                //    };
                //    chart.Y.Add(line2);
                //}
            }
            //对比线前后时间差值
            int intervalDays = 0;
            if (ListAll[0].Count != 0 && ListAllForCompare.Count != 0 && ListAllForCompare[0].Count != 0)
            {
                intervalDays = ListAll[0][0].StatDate.Subtract(ListAllForCompare[0][0].StatDate).Days;
            }
            foreach (var item in ListAllForCompare)
            {
                string softName = GetSoft(item[0].SoftId).Name;
                string platform = "_" + ((MobileOption)item[0].Platform).GetDescription();
                if (platform == "_None") platform = "_不区分平台";
                string channelName = QuDao ? "_" + item[0].ChannelName : string.Empty;
                chart.Y.Add(
                    new LineChartLine
                    {
                        Name = softName + platform + channelName + "_对比线",
                        Show = true,
                        XIntervalDays = intervalDays,
                        Points = item.Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.NewNum, DataContext = a }).ToList()
                    });
            }
            AxisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140{1},step:{2}}}}}"
                , chart.GetXJson()
                , Period == net91com.Stat.Core.PeriodOptions.Hours ? ",rotation:-45,x:-40,y:60" : (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay ? ",x:-5" : ",rotation:-45,x:-30,y:45")
                , chart.Step);

            SeriesJsonStr = chart.GetYJson(
                delegate(LineChartPoint point)
                {
                    SoftUser user = (SoftUser)point.DataContext;
                    if (point.Type != -1)
                        return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", user.Growth, user.TotalNum - user.NewNum);
                    else
                        return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", "", "0");
                });
        }

        /// <summary>
        /// 生成所有的TABLE
        /// </summary>
        protected void GetAllTableHtml()
        {
            StringBuilder sb = new StringBuilder();
            //传入的tab 序列值
            int tabindex = 0;
            if (QuDao)
            {
                var channels = HeadControl1.Channel1.ChannelValues;
                //形成tablehtml 
                for (int j = 0; j < ListAll.Count; j++)
                {
                    string plat = ((MobileOption)ListAll[j][0].Platform).GetDescription();
                    if (plat == "None")
                        plat = "不区分平台";

                    TabStr.Add(SelectedSofts.FirstOrDefault(a => a.ID == ListAll[j][0].SoftId).Name + "_" + plat + "_" + channels[QuDaoList[j]].ChannelText);
                    sb.Append(GetOneTableHtml(ListAll[j], tabindex));
                    tabindex++;
                }
            }
            else
            {
                //形成tablehtml 
                for (int j = 0; j < RealSoftLine.Count; j++)
                {
                    if (ListAll[j].Count != 0)
                    {
                        string plat = ((MobileOption)ListAll[j][0].Platform).GetDescription();
                        if (plat == "None")
                            plat = "不区分平台";
                        TabStr.Add(SelectedSofts.FirstOrDefault(a => a.ID == ListAll[j][0].SoftId).Name + "_" + plat);
                        sb.Append(GetOneTableHtml(ListAll[j], tabindex));
                        tabindex++;
                    }
                }
            }
            TableStr = sb.ToString();
        }

        /// <summary>
        /// 形成每一个tab
        /// </summary>
        /// <param name="orderUsers"></param>
        /// <param name="tableindex"></param>
        /// <returns></returns>
        protected string GetOneTableHtml(List<SoftUser> users, int tableindex)
        {
            List<SoftUser> orderUsers = users.OrderByDescending(p => p.StatDate).ToList();
            var channels = HeadControl1.Channel1.ChannelValues;
            string tableName = string.Empty;
            if (QuDao)
            {
                tableName = orderUsers[0].SoftId + "_" + orderUsers[0].Platform + "_" + (int)Period + "_" + BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_"
                    + channels[QuDaoList[tableindex]].ChannelValue + "_" + (int)channels[QuDaoList[tableindex]].ChannelType;
            }
            else
            {
                tableName = orderUsers[0].SoftId + "_" + orderUsers[0].Platform + "_" + (int)Period + "_" + BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_" + "-1" + "_" + "-1";
            }
            //是否是内部软件
            bool isInternalSoft = SelectedSofts.FirstOrDefault(a => a.SoftType == SoftTypeOptions.InternalSoft && a.ID == orderUsers[0].SoftId) != null;
            return TableTemplateHelper.BuildStatUsersTable(orderUsers[0].SoftId, (MobileOption)orderUsers[0].Platform, isInternalSoft, QuDao, Period, "new", false, orderUsers, false, tableindex, tableName);
        }

        //计算预估时间
        protected SoftUser GetForeCastSoftUser(List<ForecastSoftUser> softUser)
        {
            if (softUser.Count == 0)
                return null;
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
    }
}