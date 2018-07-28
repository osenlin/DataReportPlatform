using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.R_UserAnalysis_Service
{
    public class HttpISPUserAnalysis : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "ISPUserAnalysis"; }
        }

        public override string ServiceName
        {
            get { return "运营商用户统计"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.UrlAndSoftRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/R_UserAnalysis/ISPUsersAnalysis.aspx"; }
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
            return Result.GetSuccessedResult(GetTableString(context), true, false);
        }

        /// <summary>
        ///     获取运营商数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<List<Sjqd_StatUsersByISP>> GetData(HttpContext context)
        {
            var list = new List<List<Sjqd_StatUsersByISP>>();

            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int soft = Convert.ToInt32(context.Request["softs"]);
            int period = Convert.ToInt32(context.Request["period"]);
            string platforms = context.Request["platform"];
            string nettype = context.Request["nettype"];
            string isps = context.Request["selectisps"];

            List<string> isps_List = isps.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(p => p)
                                         .ToList();
            List<string> nettype_List = nettype.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(p => p)
                                               .ToList();
            List<int> platform_List = platforms.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(p => Convert.ToInt32(p))
                                               .ToList();
            for (int i = 0; i < platform_List.Count; i++)
            {
                for (int j = 0; j < nettype_List.Count; j++)
                {
                    for (int k = 0; k < isps_List.Count; k++)
                    {
                        list.Add(Sjqd_StatUsersByISPService.Instance.GetStatUsersByISPCache(
                            begintime, endtime, soft, platform_List[i], period, nettype_List[j], isps_List[k])
                            );
                    }
                }
            }


            return list;
        }

        /// <summary>
        ///     形成table字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected List<List<string>> GetStringList(List<Sjqd_StatUsersByISP> list)
        {
            var tempList = new List<List<string>>();
            for (int i = 0; i < list.Count; i++)
            {
                var values = new List<string>();
                values.Add(list[i].StatDate.ToString("yyyy-MM-dd"));
                values.Add(list[i].UserCount.ToString());
                tempList.Add(values);
            }
            return tempList;
        }

        public Result GetChart(HttpContext context)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            var lines = new List<LineChartLine>();
            List<List<Sjqd_StatUsersByISP>> list = GetData(context);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Count != 0)
                {
                    List<LineChartPoint> points = list[i].Select(p => new LineChartPoint
                        {
                            XValue = p.StatDate.ToString("yyyyMMdd"),
                            DataContext = "",
                            NumberType = 1,
                            Description = "",
                            YValue = p.UserCount.ToString()
                        }).ToList();
                    lines.Add(new LineChartLine
                        {
                            Name = (list[i][0].IspName) + "_" + (list[i][0].NetMode),
                            Points = points,
                            Show = true
                        });
                }
            }
            var line = new LineChart(GetXLine(begintime, endtime), lines);
            line.Step = 2;
            string result = "{ x:" +
                            line.GetXJson(
                                p =>
                                    {
                                        return
                                            ",align:'left',tickLength:80,tickPixelInterval:140,rotation:-45,x:-30,y:45";
                                    }) + ",y:" + line.GetYJson(p => { return ""; }) + ",title:'" + "分运营商统计" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        /// <summary>
        ///     获取用户量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result GetExcel(HttpContext context)
        {
            SetDownHead(context.Response, "分运营商统计.xls");
            return Result.GetSuccessedResult(GetTableString(context, true), false, true);
        }

        #region 获取table 字符串

        private string GetTableString(HttpContext context, bool fordown = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int soft = Convert.ToInt32(context.Request["softs"]);
            int period = Convert.ToInt32(context.Request["period"]);
            string platforms = context.Request["platform"];
            string nettype = context.Request["nettype"];
            string isps = context.Request["selectisps"];

            List<string> isps_List = isps.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(p => p)
                                         .ToList();
            List<string> nettype_List = nettype.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(p => p)
                                               .ToList();
            List<int> platform_List = platforms.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(p => Convert.ToInt32(p))
                                               .ToList();

            DataTable dt = Sjqd_StatUsersByISPService.Instance.GetStatUsersByISPTableCache(begintime, endtime, soft,
                                                                                           platform_List[0], period,
                                                                                           nettype_List, isps_List);

            DataColumnCollection coll = dt.Columns;
            var sb = new StringBuilder();
            sb.AppendFormat(
                "<table id=\"table1\" cellpadding=\"0\" cellspacing=\"0\" border=\"{0}\" class=\"display dataTable\">",
                fordown ? "1" : "0");
            sb.Append(" <thead> <tr>");
            for (int i = 0; i < coll.Count; i++)
            {
                sb.AppendFormat(" <th>{0}</th>", coll[i]);
            }
            sb.Append(" </tr></thead><tbody> ");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.AppendFormat("<tr class='{0}'>", i%2 == 0 ? "odd" : "even");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.AppendFormat(" <td class='center'>{0}</td>", dt.Rows[i][j]);
                }
                sb.Append("</tr>");
            }
            sb.Append(" </tbody></table>");

            return sb.ToString();
        }

        #endregion
    }
}