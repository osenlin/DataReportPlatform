using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownAvgCountByChannelTypeImpl : Abs_HttpDownLoad<D_StatDownCountsBySoft_SUM>
    {
        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "日期", "下载量", "下载用户量", "人均", "下载量（新用户）", "下载用户量（新用户）", "人均（新用户）", "下载量（老用户）", "下载用户量（老用户）",
                        "人均（老用户）", "人均一次分发", "人均二次分发"
                    };
            }
        }

        public override Result GetChart<T>(System.Web.HttpContext context)
        {
            List<LineChartLine> lines = new List<LineChartLine>();
            int showtype = Convert.ToInt32(context.Request["showtype"]);
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);

            var lists = GetData<T>(context, false) as List<List<D_StatDownCountsBySoft_SUM>>;

            var lst = lists.Count == 0 ? new List<D_StatDownCountsBySoft_SUM>() : lists[0];


            var linename = GetLineName(showtype);
            List<LineChartPoint> points = lst.Select(p => new LineChartPoint
            {
                XValue = p.StatDate.ToString("yyyyMMdd"),
                DataContext = "",
                NumberType = showtype == 3 ? 3 : 1,
                Description = "",
                YValue = GetYValue(p,showtype)
            }).ToList();
            lines.Add(new LineChartLine { Name = linename, Points = points, Show = true });
       
            LineChart line = new LineChart(GetXLine(begintime, endtime, 1), lines);
            string result = "{ x:" + line.GetXJson(p => { return ""; }) + ",y:" + line.GetYJson(p => { return ""; }) + ",title:'" + "下载统计" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        private string GetYValue(D_StatDownCountsBySoft_SUM item,int showtype)
        {
            string value=item.DownCount.ToString();
            switch (showtype)
            {
                case 2:
                    value = item.UserCount.ToString();
                    break;
                case 3:
                    value = UtilityHelp.GetDecimalDataString(item.DownCount*1.0/(item.UserCount==0?1:item.UserCount));
                    break;
            }
            return value;
        }

        private string GetLineName(int showtype)
        {
            string strname = "总下载量";
            switch (showtype)
            {
                case 2:
                    strname = "下载用户量";
                    break;
                case 3:
                    strname = "人均下载量";
                    break;
            }
            return strname;

        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list, bool DetailEqChart = true)
        {
            var tempList = new List<List<string>>();
            if (list is List<D_StatDownCountsBySoft_SUM>)
            {
                var temp = list as List<D_StatDownCountsBySoft_SUM>;
                var list2 = temp.OrderByDescending(p => p.StatDate).ToList();
                foreach (D_StatDownCountsBySoft_SUM obj in list2)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.StatDate.ToString("yyyy-MM-dd"));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownCount));
                    values.Add(UtilityHelp.FormatNum<int>(obj.UserCount));
                    values.Add(UtilityHelp.GetDecimalDataString<double>(obj.DownCount * 1.0 / (obj.UserCount == 0 ? 1 : obj.UserCount)));

                    values.Add(UtilityHelp.FormatNum<int>(obj.NewUserDownCount));
                    values.Add(UtilityHelp.FormatNum<int>(obj.NewUserCount));
                    values.Add(UtilityHelp.GetDecimalDataString(obj.NewUserDownCount * 1.0 / (obj.NewUserCount == 0 ? 1 : obj.NewUserCount)));
                   
                    int user = obj.UserCount - obj.NewUserCount;
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownCount-obj.NewUserDownCount));
                    values.Add(UtilityHelp.FormatNum<int>(user));
                    values.Add(
                        UtilityHelp.GetDecimalDataString<double>((obj.DownCount - obj.NewUserDownCount) * 1.0 / (user == 0 ? 1 : user)));
                   
                    values.Add(UtilityHelp.GetDecimalDataString<double>(obj.DownCountExceptAllUpdating * 1.0 / (obj.UserCountExceptAllUpdating == 0 ? 1 : obj.UserCountExceptAllUpdating)));

                    values.Add(UtilityHelp.GetDecimalDataString<double>((obj.DownCount - obj.DownCountExceptAllUpdating) * 1.0 / obj.UserCountUpdateing));
                    tempList.Add(values);
                }
            }
            return tempList;
        }

        protected override List<List<T>> GetData<T>(System.Web.HttpContext context, bool flag = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int channeltype = Convert.ToInt32(context.Request["channeltype"]);

            List<List<D_StatDownCountsBySoft_SUM>> lists = new List<List<D_StatDownCountsBySoft_SUM>>();


            var result = D_StatDownCountSum_Service.Instance.GetD_StatDownAvgCountByChannelTypeCache(
                                                                                          restype, 
                                                                                          softs,
                                                                                          platform, 
                                                                                          begintime,
                                                                                          endtime,
                                                                                          channeltype);
            if (result.Count != 0)
                lists.Add(result);
            return lists as List<List<T>>;
        }

        protected override string DownloadExcelName
        {
            get { return "人均下载统计按渠道类型.xls"; }
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownAvgCountByChannelTypeImpl"; }
        }

        public override string ServiceName
        {
            get { return "人均下载统计按渠道类型"; }
        }

        public override ServicesBaseEntity.RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string GetQueryString(HttpRequest context)
        {
            string channeltype = context["channeltype"];
            return "?channeltype=" + channeltype;
        }



        public override string RightUrl
        {
            get { return "/Reports_New/D_DownlaodStatistics/D_ResDownloadAvgCountByChannelType.aspx"; }
        }
    }
}
