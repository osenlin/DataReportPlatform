using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd.Entity;

namespace net91com.Stat.Web.Reports.Services
{
    /// <summary>
    /// 曲线图对应的类
    /// </summary>
    public class LineChart
    {
        /// <summary>
        /// 不允许使用默认构造函数
        /// </summary>
        private LineChart()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xBeginTime"></param>
        /// <param name="xEndTime"></param>
        public LineChart(DateTime xBeginTime, DateTime xEndTime)
        {
            XBeginTime = xBeginTime;
            XEndTime = xEndTime;
            Period = net91com.Stat.Core.PeriodOptions.Daily;
        }

        /// <summary>
        /// 获取Y轴JSON时，自定义处理方法委托
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public delegate string GetYJsonHandler(LineChartPoint point);        

        /// <summary>
        /// X轴开始时间
        /// </summary>
        public DateTime XBeginTime { get; private set; }

        /// <summary>
        /// X轴结束时间
        /// </summary>
        public DateTime XEndTime { get; private set; }

        /// <summary>
        /// 是否需要预测点
        /// </summary>
        public bool NeedForecastPoint { get; set; }

        /// <summary>
        /// 显示步长(即间隔多少个点显示数据）
        /// </summary>
        public int Step
        {
            get
            {
                int maxShowPointCount = 20;
                return X.Count <= maxShowPointCount ? 1 : (X.Count % maxShowPointCount == 0 ? X.Count / maxShowPointCount : X.Count / maxShowPointCount + 1);
            }
        }

        /// <summary>
        /// 统计周期
        /// </summary>
        public net91com.Stat.Core.PeriodOptions Period { get; set; }

        private List<DateTime> x = null;
        /// <summary>
        /// X轴
        /// </summary>
        public List<DateTime> X
        {
            get
            {
                if (x == null)
                {
                    x = new List<DateTime>();
                    switch (Period)
                    {
                        case net91com.Stat.Core.PeriodOptions.TimeOfDay:
                            for (int i = 0; i < 24; i++)
                                x.Add(XBeginTime.AddHours(i));
                            break;
                        case net91com.Stat.Core.PeriodOptions.Hours:
                            for (DateTime d = XBeginTime; d <= XEndTime; d = d.AddDays(1))
                            {
                                for (int i = 0; i < 24; i++)
                                    x.Add(d.AddHours(i));
                            }
                            break;
                        case net91com.Stat.Core.PeriodOptions.Daily:
                        case net91com.Stat.Core.PeriodOptions.LatestOneMonth:
                            for (DateTime d = XBeginTime; d <= XEndTime; d = d.AddDays(1))
                                x.Add(d);
                            if (NeedForecastPoint && x.Count > 0)
                                x.Add(XEndTime.AddDays(1));
                            break;
                        case net91com.Stat.Core.PeriodOptions.Weekly:
                            DateTime btWeekly = XBeginTime;
                            for (; btWeekly <= XEndTime; btWeekly = btWeekly.AddDays(1))
                            {
                                if (btWeekly.DayOfWeek == DayOfWeek.Sunday) break;
                            }
                            if (btWeekly.DayOfWeek == DayOfWeek.Sunday)
                            {
                                for (; btWeekly <= XEndTime; btWeekly = btWeekly.AddDays(7))
                                    x.Add(btWeekly);
                            }
                            if (NeedForecastPoint && x.Count > 0)
                                x.Add(x[x.Count - 1].AddDays(7));
                            break;
                        case net91com.Stat.Core.PeriodOptions.Of2Weeks:
                            DateTime btOf2Weeks = XBeginTime;
                            for (; btOf2Weeks <= XEndTime; btOf2Weeks = btOf2Weeks.AddDays(1))
                            {
                                if (btOf2Weeks.Subtract(new DateTime(2009, 5, 10)).Days % 14 == 0) break;
                            }
                            if (btOf2Weeks.Subtract(new DateTime(2009, 5, 10)).Days % 14 == 0)
                            {
                                for (; btOf2Weeks <= XEndTime; btOf2Weeks = btOf2Weeks.AddDays(14))
                                    x.Add(btOf2Weeks);
                            }
                            if (NeedForecastPoint && x.Count > 0)
                                x.Add(x[x.Count - 1].AddDays(14));
                            break;
                        case net91com.Stat.Core.PeriodOptions.Monthly:
                            DateTime btMonthly = XBeginTime;
                            for (; btMonthly <= XEndTime; btMonthly = btMonthly.AddDays(1))
                            {
                                if (btMonthly.Day == 20) break;
                            }
                            if (btMonthly.Day == 20)
                            {
                                for (; btMonthly <= XEndTime; btMonthly = btMonthly.AddMonths(1))
                                    x.Add(btMonthly);
                            }
                            if (NeedForecastPoint && x.Count > 0)
                                x.Add(x[x.Count - 1].AddMonths(1));
                            break;
                        case net91com.Stat.Core.PeriodOptions.NaturalMonth:
                            DateTime btNaturalMonth = XBeginTime;
                            for (; btNaturalMonth <= XEndTime; btNaturalMonth = btNaturalMonth.AddDays(1))
                            {
                                if (btNaturalMonth.AddDays(1).Day == 1) break;
                            }
                            if (btNaturalMonth.AddDays(1).Day == 1)
                            {
                                for (; btNaturalMonth <= XEndTime; btNaturalMonth = btNaturalMonth.AddDays(1).AddMonths(1).AddDays(-1))
                                    x.Add(btNaturalMonth);
                            }
                            if (NeedForecastPoint && x.Count > 0)
                                x.Add(x[x.Count - 1].AddDays(1).AddMonths(1).AddDays(-1));
                            break;
                        default:
                            break;
                    }
                }
                return x;
            }
        }

        /// <summary>
        /// 获取X轴JSON
        /// </summary>
        public string GetXJson()
        {
            StringBuilder jsonBuilder = new StringBuilder("categories:[");
            foreach (DateTime d in X)
            {
                jsonBuilder.AppendFormat(@"""{0}"",", GetXFormatValue(d));
            }
            string json = jsonBuilder.ToString();
            if (json.EndsWith(","))
                return json.Substring(0, json.Length - 1) + "]";
            return json + "]";
        }

        /// <summary>
        /// 获取X轴格式化后的值
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns></returns>
        private string GetXFormatValue(DateTime xValue)
        {
            if (Period == net91com.Stat.Core.PeriodOptions.TimeOfDay)
                return xValue.ToString("HH");
            else if (Period == net91com.Stat.Core.PeriodOptions.Hours)
                return xValue.ToString("yy/MM/dd HH").Replace('-', '/');
            return xValue.ToString("yy/MM/dd").Replace('-', '/');
        }

        private List<LineChartLine> y = new List<LineChartLine>();
        /// <summary>
        /// Y轴
        /// </summary>
        public List<LineChartLine> Y 
        { 
            get { return y; } 
        }        

        /// <summary>
        /// 获取Y轴JSON
        /// </summary>
        /// <param name="getYJsonCallback"></param>
        /// <returns></returns>
        public string GetYJson(GetYJsonHandler getYJsonCallback)
        {
            StringBuilder json = new StringBuilder("[");
            for (int l = 0; l < Y.Count; l++)
            {
                json.Append("{\"data\":[");
                List<LineChartPoint> points = Y[l].Points.OrderBy(a => a.XValue).ToList();
                int xCount = NeedForecastPoint ? X.Count - 1 : X.Count;
                for (int i = 0, j = 0; i < xCount; i++)
                {
            
                    if (j < points.Count && ((Period == net91com.Stat.Core.PeriodOptions.TimeOfDay && X[i].Hour == points[j].XValue.Hour)
                            || X[i] == points[j].XValue.AddDays(Y[l].XIntervalDays)))
                    {
                        json.Append("{");
                        json.AppendFormat("\"y\":{0:0.0}", points[j].YValue);
                        json.AppendFormat(",\"dataLabels\":{{\"enabled\":{0}}}", (i % Step == 0).ToString().ToLower());
                        json.AppendFormat(",\"Datestr\":\"{0}\"", GetXFormatValue(points[j].XValue));
                        string marker = Y[l].GetMarker(points[j].XValue);
                        string markerJson = string.IsNullOrEmpty(marker) ? null : string.Format(",\"marker\":{{\"symbol\":\"url(/Images/sun.png)\",\"marks\":\"{0}\"}}", marker);
                        json.Append(markerJson);
                        json.AppendFormat("{0}", getYJsonCallback(points[j]));
                        json.Append("}");
                        if (i < xCount - 1) json.Append(",");
                        j++;
                    }
                    else
                    {
                        json.Append("null");
                        if (i < xCount - 1) json.Append(",");
                    }
                }
                //增加一个预估点，如果没有一个点不需要加这个点
                if (NeedForecastPoint && Y[l].Points.Count > 0 && Y[l].ForecastPoint != null)
                {
                    json.Append(",{");
                    json.AppendFormat("\"y\":{0:0.0}", Y[l].ForecastPoint.YValue);
                    json.Append(",\"dataLabels\":{\"enabled\":false}");
                    json.AppendFormat(",\"Datestr\":\"{0}\"", GetXFormatValue(X[X.Count - 1]));
                    json.Append(",\"marker\":{\"enabled\":true,\"fillColor\":\"#FFFFFF\",\"lineColor\":\"#FF0000\",\"radius\":6,\"lineWidth\":0.5,\"symbol\":null,\"marks\":\"预估值\"}");
                    json.AppendFormat("{0}", getYJsonCallback(Y[l].ForecastPoint));
                    json.Append("}");
                }
                json.AppendFormat("],\"visible\":{0},\"name\":\"{1}\"}}", Y[l].Show.ToString().ToLower(), Y[l].Name);
                if (l < Y.Count - 1) json.Append(","); 
            }
            json.Append("]");
            return json.ToString();
        }
    }

    /// <summary>
    /// 点的信息实体
    /// </summary>
    public class LineChartPoint
    {
        /// <summary>
        /// X轴值
        /// </summary>
        public DateTime XValue { get; set; }        

        /// <summary>
        /// Y轴值
        /// </summary>
        public double YValue { get; set; }

        /// <summary>
        /// 分母值
        /// </summary>
        public double Denominator { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public string Percent { get; set; }

        /// <summary>
        /// 数据引用实例
        /// </summary>
        public object DataContext { get; set; }
        /// <summary>
        /// 为特殊状态定义的值
        /// </summary>
        public int Type { get; set; }

        
    }

    /// <summary>
    /// 线的信息实体
    /// </summary>
    public class LineChartLine
    {
        /// <summary>
        /// 曲线名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 跟基准时间比较差多少天，比对时会用到
        /// </summary>
        public int XIntervalDays { get; set; }

        private Dictionary<DateTime, string> markers = new Dictionary<DateTime, string>();
        /// <summary>
        /// 添加里程碑
        /// </summary>
        /// <param name="date"></param>
        /// <param name="period"></param>
        /// <param name="remark"></param>
        public void AddMarker(DateTime date, net91com.Stat.Core.PeriodOptions period, string remark)
        {
            DateTime d = date;
            switch (period)
            {
                case net91com.Stat.Core.PeriodOptions.Weekly:
                    for (; ; d = d.AddDays(1))
                    {
                        if (d.DayOfWeek == DayOfWeek.Sunday) break;
                    }
                    break;
                case net91com.Stat.Core.PeriodOptions.Of2Weeks:
                    for (; ; d = d.AddDays(1))
                    {
                        if (d.Subtract(new DateTime(2009, 5, 10)).Days % 14 == 0) break;
                    }
                    break;
                case net91com.Stat.Core.PeriodOptions.Monthly:
                    for (; ; d = d.AddDays(1))
                    {
                        if (d.Day == 20) break;
                    }
                    break;
                case net91com.Stat.Core.PeriodOptions.NaturalMonth:
                    for (; ; d = d.AddDays(1))
                    {
                        if (d.AddDays(1).Day == 1) break;
                    }
                    break;
                default:
                    break;
            }
            if (!markers.ContainsKey(d))
                markers.Add(d, remark);
            else if (period != net91com.Stat.Core.PeriodOptions.Daily && period != net91com.Stat.Core.PeriodOptions.LatestOneMonth)
                markers[d] = markers[d] + remark;
        }

        /// <summary>
        /// 获取里程碑
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetMarker(DateTime date)
        {
            //要求每8个字符换行一次
            if (markers.ContainsKey(date))
                return Regex.Replace(markers[date], @"([\s\S]{8})", "$1<br/>");
            return null;
        }

        /// <summary>
        /// 线上点的信息
        /// </summary>
        public List<LineChartPoint> Points { get; set; }
        /// <summary>
        /// 预测点值
        /// </summary>
        public LineChartPoint ForecastPoint { get; set; }
    }
}