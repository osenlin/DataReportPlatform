using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.JsEntity
{
    public class LineChart
    {
        /// <summary>
        /// x 轴
        /// </summary>
        private List<string> X_Strings = null;

        /// <summary>
        /// 多条曲线
        /// </summary>
        private List<LineChartLine> Y_List = null;

        public LineChart(List<string> m, List<LineChartLine> y_List)
        {
            ///保证升序
            X_Strings = m.Distinct().OrderBy(p => p).ToList();
            Y_List = y_List;
            step = X_Strings.Count/15 + 1;
        }

        /// <summary>
        /// 获取Y轴JSON时，自定义处理方法委托
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public delegate string GetJsonAppHandler(object onedata);

        private int step;

        /// <summary>
        ///  间隔多少打点
        /// </summary>
        public int Step
        {
            set { step = value; }
            get { return step; }
        }

        /// <summary>
        /// 获取X轴JSON
        /// </summary>
        public string GetXJson(GetJsonAppHandler fun)
        {
            StringBuilder jsonBuilder = new StringBuilder("categories:[");
            foreach (string x in X_Strings)
            {
                jsonBuilder.AppendFormat(@"""{0}"",", x);
            }
            string json = jsonBuilder.ToString();
            if (json.EndsWith(","))
                json = json.Substring(0, json.Length - 1) + "]";
            if (X_Strings.Count == 0)
                json = json + "]";
            string labels = string.Format("labels:{{step:{0} {1} }}", Step, fun(null));
            return "{" + json + "," + labels + "}";
        }

        /// <summary>
        /// 获取Y轴JSON
        /// </summary>
        /// <param name="getYJsonCallback"></param>
        /// <returns></returns>
        public virtual string GetYJson(GetJsonAppHandler getYJsonCallback)
        {
            StringBuilder json = new StringBuilder("[");
            for (int l = 0; l < Y_List.Count; l++)
            {
                json.Append("{\"data\":[");
                List<LineChartPoint> points = Y_List[l].Points.OrderBy(a => a.XValue).ToList();
                int xCount = X_Strings.Count;
                for (int i = 0, j = 0; i < xCount; i++)
                {
                    if (j < points.Count && points[j].XValue == X_Strings[i])
                    {
                        json.Append("{");
                        json.AppendFormat("\"dataLabels\":{{\"enabled\":{0}}}", (i%Step == 0).ToString().ToLower());
                        json.AppendFormat(",\"Des\":\"{0}\"", points[j].Description);
                        json.AppendFormat(",\"NumType\":\"{0}\"", points[j].NumberType);
                        json.AppendFormat("{0}", getYJsonCallback(points[j]));
                        json.AppendFormat(",\"y\":{0}", points[j].YValue);
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
                json.AppendFormat("],\"visible\":{0},\"name\":\"{1}\"}}", Y_List[l].Show.ToString().ToLower(),
                                  Y_List[l].Name);
                if (l < Y_List.Count - 1) json.Append(",");
            }
            json.Append("]");
            return json.ToString();
        }

        /// <summary>
        /// 特别要求方法抽取出来
        /// </summary>
        /// <param name="getYJsonCallback"></param>
        /// <param name="isshow">是否展示所有点的功能</param>
        /// <returns></returns>
        public string GetYJson(GetJsonAppHandler getYJsonCallback, bool hasfunction)
        {
            StringBuilder json = new StringBuilder("[");
            for (int l = 0; l < Y_List.Count; l++)
            {
                json.Append("{\"data\":[");
                List<LineChartPoint> points = Y_List[l].Points.OrderBy(a => a.XValue).ToList();
                int xCount = X_Strings.Count;
                for (int i = 0, j = 0; i < xCount; i++)
                {
                    if (j < points.Count && points[j].XValue == X_Strings[i])
                    {
                        json.Append("{");
                        json.AppendFormat("\"dataLabels\":{{\"enabled\":{0}}}", hasfunction.ToString().ToLower());
                        json.AppendFormat(",\"Des\":\"{0}\"", points[j].Description);
                        json.AppendFormat(",\"NumType\":\"{0}\"", points[j].NumberType);
                        json.AppendFormat("{0}", getYJsonCallback(points[j]));
                        json.AppendFormat(",\"y\":{0}", points[j].YValue);
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
                json.AppendFormat("],\"visible\":{0},\"name\":\"{1}\"}}", Y_List[l].Show.ToString().ToLower(),
                                  Y_List[l].Name);
                if (l < Y_List.Count - 1) json.Append(",");
            }
            json.Append("]");
            return json.ToString();
        }

        /// <summary>
        /// 根据起始时间获取x轴对应值
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="endtime"></param>
        /// <param name="format"></param>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        public List<String> GetTimeString(DateTime begin, DateTime endtime, string format, int dayNum)
        {
            List<String> strList = new List<string>();
            while (begin <= endtime)
            {
                strList.Add(begin.ToString(format));
                begin = begin.AddDays(dayNum);
            }
            return strList;
        }
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
        /// 线上点的信息
        /// </summary>
        public List<LineChartPoint> Points { get; set; }

        /// <summary>
        /// 预测点值
        /// </summary>
        public LineChartPoint ForecastPoint { get; set; }
    }

    /// <summary>
    /// 点信息
    /// </summary>
    public class LineChartPoint
    {
        /// <summary>
        /// X轴值
        /// </summary>
        public string XValue { get; set; }

        /// <summary>
        /// Y轴值
        /// </summary>
        public string YValue { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据引用实例
        /// </summary>
        public object DataContext { get; set; }

        /// <summary>
        /// 1 的为整数，2 为小数
        /// </summary>
        public int NumberType { get; set; }
    }
}