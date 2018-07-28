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
    public class D_HttpDownStatDownCountSumByExtendAttrLstImpl : Abs_HttpDownLoad<D_StatDownCountsByExtendAttrLst_SUM>
    {

        /// <summary>
        /// 获取曲线名称
        /// </summary>
        /// <returns></returns>
        private string GetLineName(int showtype)
        {
            return GetShowTypeString(showtype);
        }

        public override Result GetChart<T>(HttpContext context)
        {
            List<LineChartLine> lines = new List<LineChartLine>();
            int extendAttrLst = Convert.ToInt32(context.Request["ExtendAttrLst"]);
            int period = Convert.ToInt32(context.Request["period"]);
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            var lists = GetData<T>(context, false) as List<List<D_StatDownCountsByExtendAttrLst_SUM>>;
            for (int j = 0; j < lists.Count; j++)
            {
                var linename = GetLineName(extendAttrLst);
                List<LineChartPoint> points = lists[j].Select(p => new LineChartPoint
                {
                    XValue = p.StatDate.ToString("yyyyMMdd"),
                    DataContext = "",
                    NumberType = 1,
                    Description = "",
                    YValue = p.DownCount.ToString()
                }).ToList();
                lines.Add(new LineChartLine { Name = linename, Points = points, Show = true });
            }
 
            LineChart chart = new LineChart(GetXLine(begintime, endtime, period), lines);
            string result = "{ x:" + chart.GetXJson(p => { return ""; }) + ",y:" + chart.GetYJson(p => { return ""; }) +
                            ",title:'" + "下载量统计(扩展属性)" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        protected override List<List<T>> GetData<T>(HttpContext context, bool isfortable = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int stattype = Convert.ToInt32(context.Request["stattype"]);

            int extendAttrLst = Convert.ToInt32(context.Request["ExtendAttrLst"]);
            int period = Convert.ToInt32(context.Request["period"]);

            List<List<D_StatDownCountsByExtendAttrLst_SUM>> lists = new List<List<D_StatDownCountsByExtendAttrLst_SUM>>();
  
            var result = D_StatDownCountSum_Service.Instance.GetD_StatDownByExtendAttrLst_SUMByCache(
                                                                                            restype,
                                                                                            softs,
                                                                                            platform, 
                                                                                            begintime,
                                                                                            endtime,
                                                                                            period,
                                                                                            extendAttrLst,stattype);
            if (result.Count != 0)
                lists.Add(result);

            return lists as List<List<T>>;
        }

        protected override string DownloadExcelName
        {
            get { return "下载量统计(扩展属性).xls"; }
        }

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                        {
                            "日期","数据量"
                        };
            }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list, bool DetailEqChart = true)
        {
            var tempList = new List<List<string>>();
            if (list is List<D_StatDownCountsByExtendAttrLst_SUM>)
            {
                var temp = list as List<D_StatDownCountsByExtendAttrLst_SUM>;
                var list2 = temp.OrderByDescending(p => p.StatDate).ToList();
                foreach (D_StatDownCountsByExtendAttrLst_SUM obj in list2)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.StatDate.ToString("yyyy-MM-dd"));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownCount, false));

                    tempList.Add(values);
                }
            }

            return tempList;
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownCountSumByExtendAttrLstImpl"; }
        }

        public override string ServiceName
        {
            get { return "下载量统计(扩展属性)"; }
        }


        public override RightEnum RightType
        {
            get { return RightEnum.NoCheck; }
        }


        public override bool Check(object obj)
        {
            return new URLoginService().FindRight(
                "reports_new/d_downlaodstatistics/D_ResDownloadSumByExtendAttr.aspx".ToLower()) != null ? true : false;
        }

        public override string RightUrl
        {
            get { return "reports_new/d_downlaodstatistics/D_ResDownloadSumByExtendAttr.aspx"; }
        }


        /// <summary>
        /// 获取展示类型名称
        /// </summary>
        /// <param name="showType"></param>
        /// <returns></returns>
        private string GetShowTypeString(int showType)
        {
            string showTypeString = string.Empty;
            switch (showType)
            {
                case -1:
                    showTypeString = "所有";
                    break;
                case 1:
                    showTypeString = "彩虹计划";
                    break;
                case 2:
                    showTypeString = "付费";
                    break;
                case 3:
                    showTypeString = "彩虹计划&付费";
                    break;
                case 4:
                    showTypeString = "试用";
                    break;
                default:
                    showTypeString = "";
                    break;
            }
            return showTypeString;
        }
        private double GetIndexValue(D_StatDownCountsByExtendAttrLst_SUM down, int extendattrlst)
        {
            switch (extendattrlst)
            {
                case -1:
                    return down.DownCount;
                case 1:
                    return down.RainBowDownCount;
                case 2:
                    return down.PayDownCount;
                case 3:
                    return down.ProbationAndRainBowDownCount;
                case 4:
                    return down.ProbationDownCount;
                default:
                    return 0;
            }
        }



    }
}
