using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownPositionDistributionByTagDetailImpl : Abs_HttpDownLoad<D_StatDownPositionDistribution>
    {
        private int diffdays = 1;
        public override Result GetChart<T>(HttpContext context)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            var lists = GetData<T>(context, false) as List<List<D_StatDownPositionDistribution>>;
            List<LineChartLine> lines = new List<LineChartLine>();
            List<D_StatDownPositionDistribution> lstEntity = lists.Count != 0 ? lists[0] : new List<D_StatDownPositionDistribution>();
            List<LineChartPoint> points = lstEntity.Select(p => new LineChartPoint
            {
                XValue = p.StatDate.ToString("yyyyMMdd"),
                DataContext = "",
                NumberType = 1,
                Description = "",
                YValue = p.DownCount.ToString()
            }).ToList();
            lines.Add(new LineChartLine { Name = "下载位置分布按专辑中位置", Points = points, Show = true });

            LineChart chart = new LineChart(GetXLine(begintime, endtime), lines);
            string result = "{ x:" + chart.GetXJson(p => { return ""; }) + ",y:" + chart.GetYJson(p => { return ""; }) +
                            ",title:'" + "下载位置分布按专辑中位置" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        protected override List<List<T>> GetData<T>(HttpContext context, bool isfortable = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            TimeSpan dt = endtime.Subtract(begintime);
            diffdays = dt.Days+1;
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);

            string version = context.Request["version"];
            int positionid = Convert.ToInt32(context.Request["positionid"]);

            int modetype = Convert.ToInt32(context.Request["modetype"]);
            int period = Convert.ToInt32(context.Request["period"]);
            int downtype = Convert.ToInt32(context.Request["downtype"]);

            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            string singlevalue = context.Request["singlehiddenvalue"] ?? "";
            int isdiffpagetype=Convert.ToInt32(context.Request["isdiffpagetype"]);
            int stattype = Convert.ToInt32(context.Request["stattype"]);
            string areaid = context.Request["areaid"];

            List<List<D_StatDownPositionDistribution>> lists = new List<List<D_StatDownPositionDistribution>>();

            var result = D_StatDownPositionDistribution_Service.Instance.GetD_StatDownPositionDistributionByTagDetailCache(restype, softs,
                                                                                                         platform,
                                                                                                         projectsource,
                                                                                                         version,
                                                                                                         downtype,
                                                                                                         begintime,
                                                                                                         endtime,
                                                                                                         period, 
                                                                                                         positionid,
                                                                                                         areaid,
                                                                                                         isdiffpagetype,stattype);
            if (result.Count != 0)
                lists.Add(result);


            return lists as List<List<T>>;
        }

        protected override string DownloadExcelName
        {
            get { return "下载位置分布按专辑中位置.xls"; }
        }

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "时间","下载量"
                    };
            }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list,bool DetailEqChart=true)
        {
            var tempList = new List<List<string>>();
            if (list is List<D_StatDownPositionDistribution>)
            {
                var temp = list as List<D_StatDownPositionDistribution>;
                foreach (D_StatDownPositionDistribution item in temp)
                {
                    List<string> values = new List<string>();
                    values.Add(item.StatDate.ToString("yyyy-MM-dd"));
                    values.Add(UtilityHelp.FormatNum<long>(item.DownCount,true));
                    tempList.Add(values);
                }
            }
            return tempList;
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownPositionDistributionByTagDetailImpl"; }
        }

        public override string ServiceName
        {
            get { return "新载位置分布按专辑中位置"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.DefinedByYourself; }
        }


        public override bool Check(object obj)
        {
            return new URLoginService().FindRight(
                "reports_new/d_downlaodstatistics/d_resdownloadpositiondistribution.aspx")!=null?true:false;
        }

        public override string RightUrl
        {
            get { return "reports_new/d_downlaodstatistics/d_resdownloadpositiondistribution.aspx"; }
        }
    }
}
