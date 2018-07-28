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
    public partial class ActivityUserReport : ReportBasePage
    {
        /// <summary>
        /// 判断是否为渠道
        /// </summary>
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

        public List<int> QuDaoList = new List<int>();
        /// <summary>
        /// htmltable 字符串
        /// </summary>
        public string TableStr { get; set; }


        ///每一条线一个List<SoftNewUser> 这里多条线
        public List<List<SoftUser>> ListAll = new List<List<SoftUser>>();
        /// <summary>
        /// 比较的对比曲线
        /// </summary>
        public List<List<SoftUser>> ListAllForCompare = new List<List<SoftUser>>();

        /// 转换为坐标点json字符的中间类
        public List<SeriesJsonModel> seriesJsonStr = new List<SeriesJsonModel>();

        ///渠道分类的记录
        public List<int> ChannelCatesList = new List<int>();
        /// <summary>
        /// table 展示列的类型
        /// </summary>
        public bool SuperPeriod { get; set; }

        /// tab 切换的字符
        public List<string> TabStr = new List<string>();

        /// <summary>
        /// 排除pc助手5.x（去重）
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID != -11).ToList(); }
        }

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
        /// 初始化控件部分属性值
        /// </summary>
        private void InitControls()
        {

            HeadControl1.Channel1.PeriodCheck = false;
            HeadControl1.MySupportSoft = AvailableSofts;
            //让弹出的渠道商下拉控件也支持不区分平台
            HeadControl1.Channel1.IsHasNoPlat = true;
            HeadControl1.IsHasNoPlat = true;
            HeadControl1.IsSoftSingle = false;
            HeadControl1.IsPlatSingle = false;
            HeadControl1.ShowCheckBox = true;
            HeadControl1.HiddenCompareTime = false;
            HeadControl1.CheckBoxText = "仅老用户";
            HeadControl1.CheckboxWidth = "8%";
            HeadControl1.PlatWidth = "12%";

            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[] {
                    net91com.Stat.Core.PeriodOptions.NaturalMonth,
                    net91com.Stat.Core.PeriodOptions.Of2Weeks,
                    net91com.Stat.Core.PeriodOptions.Weekly,
                    net91com.Stat.Core.PeriodOptions.Daily,
                    net91com.Stat.Core.PeriodOptions.Hours, },
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
                ? (HeadControl1.IsFirstLoad ? new List<int>() : HeadControl1.SoftID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList())
                : Request["inputsoftselect"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            List<MobileOption> selectedPlatforms = string.IsNullOrEmpty(Request["inputplatformselect"])
                ? (HeadControl1.IsFirstLoad ? new List<MobileOption>() : HeadControl1.PlatID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (MobileOption)int.Parse(p)).ToList())
                : Request["inputplatformselect"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (MobileOption)int.Parse(p)).ToList();

            //验证参数
            CheckParams(selectedSoftIds, selectedPlatforms, period, beginTime, endTime, ReportType.UserUseNewActivity);

            //根据周期显示不同的列
            SuperPeriod = (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay || Period == net91com.Stat.Core.PeriodOptions.Hours);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        protected void SetParams()
        {
            //初次加载一些参数设置成默认值
            ReportTitle = "活跃用户";
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
            if (HeadControl1.IsFirstLoad)
            {
                HeadControl1.isChecked = false;

            }
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
            //进入分渠道统计
            if (HeadControl1.Channel1.ChannelValues.Count != 0)
            {
                QuDao = true;
                var channels = HeadControl1.Channel1.ChannelValues;
                if (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay || Period == net91com.Stat.Core.PeriodOptions.Hours)
                {
                    for (int i = 0; i < channels.Count; i++)
                    {

                        //小时和每小时将其和到一条曲线中去了
                        //List<SoftUser> users = Sjqd_StatUsersByChannelsService.GetInstance().GetChannelUsersByHoursDataCache(
                        //    BeginTime, EndTime, SelectedSofts[0].ID, channels[i].Platform, Period, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), channels[i].ChannelText, loginService,CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();

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
                            QuDaoList.Add(i);
                        }
                        //获取对比曲线
                        if (HeadControl1.CompareTimeType > 0)
                        {
                            //List<SoftUser> usersCompare = Sjqd_StatUsersByChannelsService.GetInstance().GetChannelUsersByHoursDataCache(
                            //BeginCompareTime, EndCompareTime, SelectedSofts[0].ID, channels[i].Platform, Period, channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), channels[i].ChannelText, loginService,CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();

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
                        List<SoftUser> users = Sjqd_StatUsersByChannelsService.GetInstance().GetSoftUserChanelListCache(BeginTime, EndTime, SelectedSofts[0].ID, channels[i].Platform, Period,
                             channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), channels[i].ChannelText, false, loginService, CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();

                        if (users != null && users.Count != 0)
                        {
                            ListAll.Add(users);
                            QuDaoList.Add(i);

                        }
                        //获取对比曲线
                        if (HeadControl1.CompareTimeType > 0)
                        {
                            List<SoftUser> usersCompare = Sjqd_StatUsersByChannelsService.GetInstance().GetSoftUserChanelListCache(BeginCompareTime, EndCompareTime, SelectedSofts[0].ID, channels[i].Platform, Period,
                                 channels[i].ChannelType, Convert.ToInt32(channels[i].ChannelValue), channels[i].ChannelText, false, loginService, CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();

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
                    Soft SingleSoftInfo = SelectedSofts[softid];
                    for (int i = 0; i < SelectedPlatforms.Count; i++)
                    {
                        foreach (MobileOption tempplat in SingleSoftInfo.Platforms)
                        {
                            ///他选出的平台加上和自己支持平台的交集
                            if (tempplat == SelectedPlatforms[i] || SelectedPlatforms[i] == 0)
                            {
                                List<SoftUser> users = null;
                                if (!SuperPeriod)
                                    users = Sjqd_StatUsersService.GetInstance().GetSoftUserListCache(BeginTime, EndTime, SingleSoftInfo.ID, (int)SelectedPlatforms[i], Period, loginService, CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
                                else
                                {
                                    //users = StatUsersByHourService.GetInstance().GetHourUserDataCache(SingleSoftInfo.ID, (int)SelectedPlatforms[i], BeginTime, EndTime, Period, loginService,CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
                                    net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                                    List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                                    ssUsers = suService.GetStatUsersByHour(SingleSoftInfo.ID, (int)SelectedPlatforms[i], ChannelTypeOptions.Category, 0, (int)Period, BeginTime, EndTime);
                                    users = new List<SoftUser>();
                                    foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                                    {
                                        SoftUser softuser = new SoftUser();
                                        softuser.StatDate = u.StatDate;
                                        softuser.SoftId = SingleSoftInfo.ID;
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
                                    RealSoftLine.Add((int)SelectedPlatforms[i]);

                                }
                                //获取对比曲线
                                if (HeadControl1.CompareTimeType > 0)
                                {
                                    List<SoftUser> usersCompare = null;
                                    if (!SuperPeriod)
                                        usersCompare = Sjqd_StatUsersService.GetInstance().GetSoftUserListCache(BeginCompareTime, EndCompareTime, SingleSoftInfo.ID, (int)SelectedPlatforms[i], Period, loginService, CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
                                    else
                                    {
                                        //usersCompare = StatUsersByHourService.GetInstance().GetHourUserDataCache(SingleSoftInfo.ID, (int)SelectedPlatforms[i], BeginCompareTime, EndCompareTime, Period, loginService,CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
                                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                                        ssUsers = suService.GetStatUsersByHour(SingleSoftInfo.ID, (int)SelectedPlatforms[i], ChannelTypeOptions.Category, 0, (int)Period, BeginCompareTime, EndCompareTime);
                                        usersCompare = new List<SoftUser>();
                                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                                        {
                                            SoftUser softuser = new SoftUser();
                                            softuser.StatDate = u.StatDate;
                                            softuser.SoftId = SingleSoftInfo.ID;
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
                HeadControl1.Channel1.SelectedText = "选择渠道";
                HeadControl1.Channel1.SelectedValue = "";
            }
            //一条线数据都没有
            if (ListAll.Count == 0)
                return;
            //生成所有的Table
            GetAllTableHtml();
            //生成所有曲线的JSON
            GetAllLineJson();
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
            var channels = HeadControl1.Channel1.ChannelValues;
            string tableName = string.Empty;
            if (QuDao)
            {
                tableName = users[0].SoftId + "_" + users[0].Platform + "_" + (int)Period + "_" + BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_" + channels[QuDaoList[tableindex]].ChannelValue + "_" + (int)channels[QuDaoList[tableindex]].ChannelType + "_" + (HeadControl1.isChecked ? 1 : 0);
            }
            else
            {
                tableName = users[0].SoftId + "_" + users[0].Platform + "_" + (int)Period + "_" + BeginTime.ToShortDateString() + "_" + EndTime.ToShortDateString() + "_" + "-1" + "_" + "-1" + "_" + (HeadControl1.isChecked ? 1 : 0);
            }
            StringBuilder sb;
            if (tableindex == 0)
                sb = new StringBuilder(string.Format("<table id=\"tab{0}\" name=\"" + tableName + "\" class=\" tablesorter \" cellspacing=\"1\">", tableindex));
            else
                sb = new StringBuilder(string.Format("<table id=\"tab{0}\" name=\"" + tableName + "\"  class=\" tablesorter \" style=\"display:none\"  cellspacing=\"1\">", tableindex));

            //是否是内部软件
            bool isInternalSoft = AvailableSofts.FirstOrDefault(a => a.SoftType == SoftTypeOptions.InternalSoft && a.ID == users[0].SoftId) != null;
            return TableTemplateHelper.BuildStatUsersTable(users[0].SoftId, (MobileOption)users[0].Platform, isInternalSoft, QuDao, Period, "active", HeadControl1.isChecked, users, false, tableindex, tableName);
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
        /// 生成所有曲线的JSON
        /// </summary>
        protected void GetAllLineJson()
        {
            LineChart chart = new LineChart(BeginTime, EndTime);
            chart.Period = Period;

            int lineType = HeadControl1.isChecked ? 1 : 2;
            for (int i = 0; i < ListAll.Count; i++)
            {
                string softName = GetSoft(ListAll[i][0].SoftId).Name;
                string platform = "_" + ((MobileOption)ListAll[i][0].Platform).GetDescription();
                if (platform == "_None") platform = "_不区分平台";
                string channelName = QuDao ? "_" + ListAll[i][0].ChannelName : string.Empty;
                //1 是ActiveNum-ActivityPercent-TotalNum 2 是UseNum-UsePercent-TotalNum，3 是LostNum-LostPercent-UseNum 

                List<LineChartPoint> MyPoints = null;
                List<LineChartPoint> MyPoints2 = null;
                if (lineType == 1)
                {
                    MyPoints = ListAll[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.ActiveNum, DataContext = a, Denominator = a.TotalNum, Percent = a.ActivityPercent }).ToList();
                    MyPoints2 = ListAll[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.FirstActiveUserCount + a.ActiveUserCount_Shanzhai, DataContext = a, Denominator = a.TotalNum }).ToList();
                }
                else
                {
                    MyPoints = ListAll[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.UseNum, DataContext = a, Denominator = a.TotalNum, Percent = a.UsePercent }).ToList();
                    MyPoints2 = ListAll[i].Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.FirstUseUserCount + a.ActiveUserCount_Shanzhai + a.NewUserCount_Shanzhai, DataContext = a, Denominator = a.TotalNum }).ToList();
                }
                LineChartLine line = new LineChartLine
                {
                    Name = softName + platform + channelName,
                    Show = true,
                    XIntervalDays = 0,
                    Points = MyPoints
                };
                chart.Y.Add(line);
                //增加新增用户未修正的线
                if (!QuDao && checkForNotUpdated.Checked && Period != net91com.Stat.Core.PeriodOptions.TimeOfDay && Period != net91com.Stat.Core.PeriodOptions.Hours && !QuDao)
                {
                    LineChartLine line2 = new LineChartLine
                    {
                        Name = softName + platform + channelName + "_未修正数据",
                        Show = true,
                        XIntervalDays = 0,
                        Points = MyPoints2
                    };
                    chart.Y.Add(line2);
                }
            }
            //增加对比线
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
                List<LineChartPoint> ComparePoints = null;
                if (lineType == 1)
                {
                    ComparePoints = item.Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.ActiveNum, DataContext = a, Denominator = a.TotalNum, Percent = a.ActivityPercent }).ToList();
                }
                else
                {
                    ComparePoints = item.Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.UseNum, DataContext = a, Denominator = a.TotalNum, Percent = a.UsePercent }).ToList();
                }
                chart.Y.Add(
                        new LineChartLine
                        {
                            Name = softName + platform + channelName + "_对比线",
                            Show = true,
                            XIntervalDays = intervalDays,
                            Points = ComparePoints
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
                    return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", point.Percent, point.Denominator);

                });

        }

        /// <summary>
        /// 生成所有的TABLE
        /// </summary>
        protected void GetAllTableHtml()
        {
            StringBuilder sb = new StringBuilder();
            ///传入的tab 序列值
            int tabindex = 0;
            if (QuDao)
            {
                var channels = HeadControl1.Channel1.ChannelValues;
                ///形成tablehtml 
                for (int j = 0; j < ListAll.Count; j++)
                {
                    string plat = ((MobileOption)ListAll[j][0].Platform).GetDescription();
                    if (plat == "None")
                        plat = "不区分平台";

                    TabStr.Add(GetSoft(ListAll[j][0].SoftId).Name + "_" + plat + "_" + channels[QuDaoList[j]].ChannelText);
                    sb.Append(GetOneTableHtml(ListAll[j], tabindex));
                    tabindex++;
                }
            }
            else
            {
                ///形成tablehtml 
                for (int j = 0; j < RealSoftLine.Count; j++)
                {
                    if (ListAll[j].Count != 0)
                    {
                        string plat = ((MobileOption)ListAll[j][0].Platform).GetDescription();
                        if (plat == "None")
                            plat = "不区分平台";
                        TabStr.Add(GetSoft(ListAll[j][0].SoftId).Name + "_" + plat);
                        sb.Append(GetOneTableHtml(ListAll[j], tabindex));
                        tabindex++;
                    }
                }
            }
            TableStr = sb.ToString();
        }
    }
}