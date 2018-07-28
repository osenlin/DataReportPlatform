using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.R_UserAnalysis_Service
{
    public class HttpChannelCustomUsers : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "ChannelCustomUsers"; }
        }

        public override string ServiceName
        {
            get { return "外部渠道数据"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.UrlAndSoftRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/R_UserAnalysis/ChannelCustomUsers.aspx"; }
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
                    case "get_excel":
                        myresult = GetExcel(context);
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
            DataTable dtresult = GetTable(context);
            var dt = new JQueryDataTableData();
            var param = new DataTablesRequest(context.Request);
            dt.sEcho = param.sEcho;
            dt.iTotalRecords = dt.iTotalDisplayRecords = dtresult.Rows.Count;

            int channelid = Convert.ToInt32(context.Request["channelid"]);
            Sjqd_ChannelCustomers customer = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(channelid);
            int showType = customer == null ? 0 : customer.ShowType;
            dt.aaData = GetStringList(dtresult, showType);
            return Result.GetSuccessedResult(dt, false);
        }

        protected DataTable GetTable(HttpContext context)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int soft = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["platform"]);
            int channelid = Convert.ToInt32(context.Request["channelid"]);

            DataTable dtresult = null;
            //没有此软件权限
            List<Channel> rights = LoginService.GetAvailableChannels(soft, null, false);
            Sjqd_ChannelCustomers node = new CfgChannelService().GetChannelCustomer(channelid);
            if (node == null || !LoginService.AvailableSofts.Exists(a => a.ID == soft) ||
                !rights.Exists(p => p.ChannelType == ChannelTypeOptions.Customer && p.ID == channelid))
            {
                dtresult = new DataTable();
            }
            else
            {
                DateTime stattime =
                    EtlStates_DataAccess.Instance.GetNowStatTimeByKey("StatUsers_Daily_StatDate", 0).AddDays(-1);
                //下午两点才能开放出去
                if (endtime >= stattime && (DateTime.Now < stattime.AddHours(38)))
                {
                    endtime = stattime.AddDays(-1);
                }
                if (begintime < node.MinViewTime)
                {
                    begintime = node.MinViewTime;
                }

                dtresult = ChannelCustomUsers_DataAccess.Instance.GetChannelCustomTables(channelid, soft, platform,
                                                                                         begintime, endtime);
            }
            return dtresult;
        }

        /// <summary>
        ///     形成table字符串
        /// </summary>
        /// <param name="dtresult"></param>
        /// <param name="showtype">1 展示新增活跃，0 展示新增</param>
        /// <returns></returns>
        protected List<List<string>> GetStringList(DataTable dtresult, int showtype)
        {
            var tempList = new List<List<string>>();
            for (int i = 0; i < dtresult.Rows.Count; i++)
            {
                var values = new List<string>();
                int temp = Convert.ToInt32(dtresult.Rows[i]["statdate"]);
                values.Add(new DateTime(temp/10000, temp%10000/100, temp%100).ToString("yyyy-MM-dd"));
                values.Add(Convert.ToInt32(dtresult.Rows[i]["newnum"]).ToString());
                if (showtype == 1)
                    values.Add(Convert.ToInt32(dtresult.Rows[i]["activenum"]).ToString());
                else //不显示活跃
                    values.Add("");
                tempList.Add(values);
            }
            return tempList;
        }

        public Result GetChart(HttpContext context)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            DataTable dtresult = GetTable(context);
            var lines = new List<LineChartLine>();
            var points = new List<LineChartPoint>();
            for (int i = 0; i < dtresult.Rows.Count; i++)
            {
                points.Add(new LineChartPoint
                    {
                        DataContext = "",
                        NumberType = 1,
                        Description = "",
                        YValue = Convert.ToInt32(dtresult.Rows[i]["newnum"]).ToString(),
                        XValue = dtresult.Rows[i]["statdate"].ToString()
                    });
            }
            lines.Add(new LineChartLine {Name = "新增用户", Points = points, Show = true});
            //若可以支持活跃
            int channelid = Convert.ToInt32(context.Request["channelid"]);
            Sjqd_ChannelCustomers customer = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(channelid);
            int showType = customer == null ? 0 : customer.ShowType;
            if (showType == 1)
            {
                var points2 = new List<LineChartPoint>();
                for (int i = 0; i < dtresult.Rows.Count; i++)
                {
                    points2.Add(new LineChartPoint
                        {
                            DataContext = "",
                            NumberType = 1,
                            Description = "",
                            YValue = Convert.ToInt32(dtresult.Rows[i]["activenum"]).ToString(),
                            XValue = dtresult.Rows[i]["statdate"].ToString()
                        });
                }
                lines.Add(new LineChartLine {Name = "活跃用户", Points = points2, Show = true});
            }
            var line = new LineChart(GetXLine(begintime, endtime), lines);
            string result = "{ x:" + line.GetXJson(p => { return ""; }) + ",y:" + line.GetYJson(p => { return ""; }) +
                            ",title:'" + "渠道新增" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        /// <summary>
        ///     资源下载excel
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result GetExcel(HttpContext context)
        {
            DataTable dtresult = GetTable(context);
            int channelid = Convert.ToInt32(context.Request["channelid"]);
            Sjqd_ChannelCustomers customer = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(channelid);
            int showType = customer == null ? 0 : customer.ShowType;
            List<List<string>> result = GetStringList(dtresult, showType);
            SetDownHead(context.Response, "新增量.xls");
            string html = string.Empty;
            string[] str = null;
            if (showType == 1)
            {
                str = new[] {"日期", "新增量", "活跃量"};
            }
            else
                str = new[] {"日期", "新增量", ""};
            html = GetTableHtml(str, result);
            return Result.GetSuccessedResult(html, false, true);
        }
    }
}