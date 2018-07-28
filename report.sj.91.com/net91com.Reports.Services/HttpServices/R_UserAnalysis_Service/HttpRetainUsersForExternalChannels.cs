using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Common;
using net91com.Core;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;
using net91com.Stat.Core;

namespace net91com.Reports.Services.HttpServices.R_UserAnalysis_Service
{
    public class HttpRetainUsersForExternalChannels : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "RetainUsersForExternalChannels"; }
        }

        public override string ServiceName
        {
            get { return "外部渠道留存数据"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.DefinedByYourself; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/R_UserAnalysis/RetainUsersForExternalChannels.aspx"; }
        }

        public override bool Check(object obj)
        {
            var req = (HttpRequest) obj;
            try
            {
                string customIdAndPlat = CryptoHelper.DES_Decrypt(req["p"], "$retain^");
                string[] strs = customIdAndPlat.Split(new[] {'&'}, StringSplitOptions.None);
                int channelId = Convert.ToInt32(strs[0]);
                int platform = Convert.ToInt32(strs[1]);
                DateTime begin = Convert.ToDateTime(req["begintime"]);
                DateTime end = Convert.ToDateTime(req["endtime"]);
                var platformList = new List<int> {0, 1, 4, 7, 9, 255};
                if (channelId <= 0 || !platformList.Contains(platform))
                    return false;
                Sjqd_ChannelCustomers node = new CfgChannelService().GetChannelCustomer(channelId);
                //渠道商要存在
                if (node == null)
                {
                    return false;
                }
                //渠道商留存要开放出去,并且对外可看
                if (!(node.ReportType == 1 && node.ShowType == 2))
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                    case "get_page":
                        myresult = GetPage(context);
                        return myresult;
                    case "get_chart":
                        myresult = GetChart(context);
                        return myresult;
                }
            }
            return myresult;
        }

        /// <summary>
        ///     获取table中数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Result GetPage(HttpContext context)
        {
            var x_DateTime = new List<DateTime>();
            List<List<Sjqd_StatRetainedUsers>> lists = GetData(context, ref x_DateTime);
            var dt = new JQueryDataTableData();
            var param = new DataTablesRequest(context.Request);
            dt.sEcho = param.sEcho;
            dt.iTotalRecords = dt.iTotalDisplayRecords = lists.Count;
            int period = Convert.ToInt32(context.Request["period"]);
            dt.aaData = GetStringList(lists, period);
            return Result.GetSuccessedResult(dt, false);
        }

        public Result GetChart(HttpContext context)
        {
            var x_times = new List<DateTime>();
            List<List<Sjqd_StatRetainedUsers>> lists = GetData(context, ref x_times);
            x_times = x_times.OrderBy(p => p).ToList();

            DateTime begin = Convert.ToDateTime(context.Request["begintime"]);
            DateTime end = Convert.ToDateTime(context.Request["endtime"]);
            int period = Convert.ToInt32(context.Request["period"]);


            var lines = new List<LineChartLine>();
            for (int i = 0; i < lists.Count; i++)
            {
                //增加起点,留存率都是100%，完了之后要把这个点给删掉
                var startPoint = new Sjqd_StatRetainedUsers();
                string name = lists[i][0].OriginalDate.ToString("yyyyMMdd");
                startPoint.StatDate = lists[i][0].OriginalDate;
                startPoint.RetainedUserCount = lists[i][0].OriginalNewUserCount;
                startPoint.OriginalNewUserCount = lists[i][0].OriginalNewUserCount;

                lists[i].Insert(0, startPoint);

                List<LineChartPoint> points = lists[i].OrderBy(p => p.StatDate).Select(p => new LineChartPoint
                    {
                        XValue = p.StatDate.ToString("yyyyMMdd"),
                        DataContext = "",
                        NumberType = 2,
                        Description = "",
                        YValue = Math.Round((p.RetainedUserCount/(double) p.OriginalNewUserCount)*100, 2).ToString()
                    }).ToList();
                lines.Add(new LineChartLine {Name = name, Points = points, Show = true});
                lists[i].RemoveAt(0);
            }
            var line = new LineChart(GetXLine(begin, end, period), lines);
            string result = "{ x:" + line.GetXJson(p => { return ""; }) + ",y:" + line.GetYJson(p => { return ""; }) +
                            ",title:'" + "新增用户留存" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        private List<List<string>> GetStringList(List<List<Sjqd_StatRetainedUsers>> lists, int peroid)
        {
            var tempList = new List<List<string>>();
            //输出表格
            foreach (var objs in lists)
            {
                var values = new List<string>();


                if (peroid != (int) PeriodOptions.Daily)
                {
                    values.Add(string.Format("{0:yyyy-MM-dd} ~ {1:yyyy-MM-dd}",
                                             peroid == (int) PeriodOptions.Weekly
                                                 ? objs[0].OriginalDate.AddDays(-6)
                                                 : objs[0].OriginalDate.AddMonths(-1).AddDays(1),
                                             objs[0].OriginalDate));
                    values.Add(UtilityHelp.FormatNum<long>(objs[0].OriginalNewUserCount));
                }
                else
                {
                    values.Add(objs[0].OriginalDate.ToString("yyyyMMdd"));
                    values.Add(UtilityHelp.FormatNum<long>(objs[0].OriginalNewUserCount));
                }
                List<Sjqd_StatRetainedUsers> objs2 = objs.OrderByDescending(p => p.StatDate).ToList();
                for (int index = objs2.Count - 1; index >= 0; index--)
                {
                    values.Add(UtilityHelp.FormatNum<long>(objs2[index].RetainedUserCount));
                    values.Add(
                        UtilityHelp.FormatPercent(objs2[index].RetainedUserCount/
                                                  (double) objs2[index].OriginalNewUserCount));
                }
                if (objs.Count < 6)
                {
                    for (int ii = 0; ii < 6 - objs.Count; ii++)
                    {
                        values.Add("");
                        values.Add("");
                    }
                }
                tempList.Add(values);
            }
            return tempList;
        }

        /// <summary>
        ///     获取留存数据(返回的每个list 都不会是空列表)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<List<Sjqd_StatRetainedUsers>> GetData(HttpContext context, ref List<DateTime> x_DateTime)
        {
            //int channelid = Convert.ToInt32(context.Request["channelid"]);
            //int softid = Convert.ToInt32(context.Request["softs"]);
            //int platform = Convert.ToInt32(context.Request["platform"]);


            string customIdAndPlat = CryptoHelper.DES_Decrypt(context.Request["p"], "$retain^");
            string[] strs = customIdAndPlat.Split(new[] {'&'}, StringSplitOptions.None);
            int channelid = Convert.ToInt32(strs[0]);
            int platform = Convert.ToInt32(strs[1]);

            int period = Convert.ToInt32(context.Request["period"]);
            DateTime begin = Convert.ToDateTime(context.Request["begintime"]);
            DateTime end = Convert.ToDateTime(context.Request["endtime"]);

            Sjqd_ChannelCustomers node = new CfgChannelService().GetChannelCustomer(channelid);
            if (begin > end || node == null || (end - begin).TotalDays > 93)
                return new List<List<Sjqd_StatRetainedUsers>>();
            //2点后才开放当天的
            DateTime stattime = EtlStates_DataAccess.Instance.GetNowStatTimeByKey("U_StatRetainedUsers_Export_Daily", 0).AddDays(-1);
            //下午两点才能开放出去
            if (end >= stattime && (DateTime.Now < stattime.AddHours(38)))
            {
                end = stattime.AddDays(-1);
            }
            //若渠道商设置了最小时间
            if (begin < node.MinViewTime)
            {
                begin = node.MinViewTime;
            }
            List<Sjqd_StatRetainedUsers> lists = Sjqd_StatRetainedUsersService.
                Instance.GetChannelRetainedUsersForChannelCustomerByCache(node.SoftID, (MobileOption) platform,
                                                                          ChannelTypeOptions.Customer, channelid, period,
                                                                          begin, end, node.MinViewTime)
                                                                              .ToList();

            x_DateTime = lists.Select(p => p.StatDate).Distinct().ToList();
            List<DateTime> tempOriginalDate = lists.Select(p => p.OriginalDate).Distinct().ToList();
            //增加一个起始点,最后一个点是 日期最小点
            if (tempOriginalDate.Count != 0)
            {
                x_DateTime.Insert(0, tempOriginalDate[tempOriginalDate.Count - 1]);
            }

            var listAll = new List<List<Sjqd_StatRetainedUsers>>();
            foreach (DateTime item in tempOriginalDate)
            {
                List<Sjqd_StatRetainedUsers> tempUserbyOriginalDate =
                    lists.Where(p => p.OriginalDate == item).OrderBy(p => p.StatDate).Take(6).ToList();
                if (tempUserbyOriginalDate.Count != 0)
                    listAll.Add(tempUserbyOriginalDate);
            }
            return listAll;
        }
    }
}