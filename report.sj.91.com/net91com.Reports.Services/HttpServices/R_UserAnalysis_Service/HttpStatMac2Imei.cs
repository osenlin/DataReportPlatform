using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.R_UserAnalysis_Service
{
    public class HttpStatMac2Imei : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "StatMac2Imei"; }
        }

        public override string ServiceName
        {
            get { return "新MAC带来的新用户统计"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/R_UserAnalysis/StatMac2Imei.aspx"; }
        }

        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                    case "get_list":
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

        private Result GetExcel(HttpContext context)
        {
            SetDownHead(context.Response, "PC助手新增MAC.xls");
            List<Sjqd_StatMAC2IMEI> list = GetData(context);
            List<List<string>> result = GetDataStrList(list);
            string html = string.Empty;
            html = GetTableHtml(new[] {"日期", "新增MAC", "当天", "7天", "14天", "当天占比(%)", "7天占比(%)", "14天占比(%)"},
                                result);
            return Result.GetSuccessedResult(html, false, true);
        }

        private Result GetChart(HttpContext context)
        {
            List<Sjqd_StatMAC2IMEI> list = GetData(context);
            var chart = new LineChart();
            if (list != null && list.Count > 0)
            {
                var serie1 = new Serie("当天");
                var serie7 = new Serie("7天");
                var serie14 = new Serie("14天");
                chart.Series.Add(serie1);
                chart.Series.Add(serie7);
                chart.Series.Add(serie14);
                var data = new Dictionary<DateTime, Sjqd_StatMAC2IMEI>();
                foreach (Sjqd_StatMAC2IMEI obj in list)
                {
                    data[obj.StatDate] = obj;
                }
                DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
                DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
                while (begintime <= endtime)
                {
                    chart.Categories.Add(begintime.ToString("yyyy-MM-dd"));

                    double user1 = 0;
                    double user7 = 0;
                    double user14 = 0;
                    Sjqd_StatMAC2IMEI obj;
                    if (data.TryGetValue(begintime, out obj))
                    {
                        user1 = Math.Round(obj.NewUsers1*100.0/obj.NewMacs, 2);
                        user7 = Math.Round(obj.NewUsers7*100.0/obj.NewMacs, 2);
                        user14 = Math.Round(obj.NewUsers14*100.0/obj.NewMacs, 2);
                    }
                    serie1.Data.Add(user1);
                    serie7.Data.Add(user7);
                    serie14.Data.Add(user14);

                    begintime = begintime.AddDays(1);
                }
            }
            return Result.GetSuccessedResult(JsonConvert.SerializeObject(chart), true);
        }

        private Result GetPage(HttpContext context)
        {
            var param = new DataTablesRequest(context.Request);
            List<Sjqd_StatMAC2IMEI> list = GetData(context);
            var dt = new JQueryDataTableData();
            dt.sEcho = param.sEcho;
            dt.iTotalRecords = dt.iTotalDisplayRecords = list.Count;
            int pageIndex = (param.iDisplayStart + 1)/param.iDisplayLength + 1;
            int pageSize = param.iDisplayLength;
            List<Sjqd_StatMAC2IMEI> resultList = list; // list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            dt.aaData = GetDataStrList(resultList);
            return Result.GetSuccessedResult(dt, true);
        }

        private List<Sjqd_StatMAC2IMEI> GetData(HttpContext context)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            string channelid = context.Request["channelid"];
            List<Sjqd_StatMAC2IMEI> list = Sjqd_StatMac2Imei_Service.Instance.GetSjqd_StatMAC2IMEI(channelid, begintime,
                                                                                                   endtime);
            return list;
        }

        public List<List<string>> GetDataStrList(List<Sjqd_StatMAC2IMEI> data)
        {
            var tempList = new List<List<string>>();
            foreach (Sjqd_StatMAC2IMEI obj in data)
            {
                var values = new List<string>();
                values.Add(obj.StatDate.ToString("yyyy-MM-dd"));
                values.Add(obj.NewMacs.ToString());
                values.Add(obj.NewUsers1.ToString());
                values.Add(obj.NewUsers7.ToString());
                values.Add(obj.NewUsers14.ToString());
                values.Add(string.Format("{0:P}", ((double) obj.NewUsers1/obj.NewMacs)));
                values.Add(string.Format("{0:P}", ((double) obj.NewUsers7/obj.NewMacs)));
                values.Add(string.Format("{0:P}", ((double) obj.NewUsers14/obj.NewMacs)));
                tempList.Add(values);
            }
            return tempList;
        }

        private class LineChart
        {
            public LineChart()
            {
                Categories = new List<string>();
                Series = new List<Serie>();
            }

            public List<string> Categories { get; set; }
            public List<Serie> Series { get; set; }
        }

        [DataContract]
        private class Serie
        {
            public Serie()
            {
                Name = string.Empty;
                Data = new List<double>();
            }

            public Serie(string name)
                : this()
            {
                Name = name;
            }

            [DataMember(Order = 1, Name = "name")]
            public string Name { get; set; }

            [DataMember(Order = 2, Name = "data")]
            public List<double> Data { get; set; }
        }
    }
}