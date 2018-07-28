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
using net91com.Stat.Services.Monitor.Entity;
using net91com.Stat.Services.Monitor;

namespace net91com.Stat.Web.Monitor
{
    public partial class ServiceLogDateReport : ReportBasePage 
    {
        /// 横轴显示的json字符
        public string AxisJsonStr { get; set; }
        /// <summary>
        /// 具体点坐标的json字符
        /// </summary>
        public string SeriesJsonStr { get; set; }
        ///用户绑定图的title
        public string ReportTitle { get; set; }
        /// <summary>
        /// x轴上的时间
        /// </summary>
        public List<DateTime> X_DateTime = new List<DateTime>();

        public List<List<Monitor_DataLogs>> allList;

        /// 转换为坐标点json字符的中间类
        public List<SeriesJsonModel> SeriesJsonModels = new List<SeriesJsonModel>();

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                new net91com.Stat.Core.PeriodOptions[] {
                    net91com.Stat.Core.PeriodOptions.Daily,
                    net91com.Stat.Core.PeriodOptions.Hours,
                    net91com.Stat.Core.PeriodOptions.TimeOfDay },
                    net91com.Stat.Core.PeriodOptions.Daily);

            Period = PeriodSelector1.SelectedPeriod;           
            inputzhouqi.Value = Period.GetDescription();
            //var list = new Monitor_DataLogsServer(true).GetLogNameAndServerIpCache(CacheTimeOption.TenMinutes).Select(p => p.DataLogName).Distinct().ToList();
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //list.ForEach(p => dic.Add(p, p));
            Dictionary<string, string> dic = Monitor_DataLogsServer.GetDataLogNameDirt();
            HeadControl1.LogNameSource = dic;
            HeadControl1.IsHasNoServerIp = true;
            ReportTitle = "日志曲线图";
            AxisJsonStr = "{}";
            SeriesJsonStr = "[]";
            allList = new List<List<Monitor_DataLogs>>();
            if (HeadControl1.IsFirstLoad)
            {
                HeadControl1.LogName = "sjqd";
                if (HeadControl1.IsHasNoServerIp)
                    HeadControl1.ServerIp = "0";
            }
            BindData();

        }

        private void BindData()
        {
            string[] serverip = HeadControl1.ServerIp.Split(',');
            for (int i = 0; i < serverip.Length; i++)
            {
                var temp = new Monitor_DataLogsServer(true).GetLogListByDateCache(HeadControl1.LogName, serverip[i], HeadControl1.BeginTime, HeadControl1.EndTime,Period, CacheTimeOption.TenMinutes);
                if (temp.Count != 0)
                {
                    allList.Add(temp);
                    X_DateTime.AddRange(temp.Select(p => p.LogDate).Distinct().ToList());
                }
            }
            if (allList.Count == 0)
            {
                ReportTitle = "无数据";
                return;
            }
            X_DateTime = X_DateTime.Distinct().OrderBy(p => p).ToList();
            LineMaxNumCoef = X_DateTime.Count / LineMaxNum + 1;
            SetxAxisJson(X_DateTime);
            GetDataJsonList(allList);
            SeriesJsonStr = JsonConvert.SerializeObject(SeriesJsonModels);  

        }
        /// <summary>
        /// 获取x轴的数据
        /// </summary>
        protected void SetxAxisJson(List<DateTime> times)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DateTime item in times)
            {
                 
                if (Period == net91com.Stat.Core.PeriodOptions.Hours)
                    sb.Append("\"" + item.ToString("MM-dd HH").Replace("-", "/") + "\"" + ",");
                else if(Period==net91com.Stat.Core.PeriodOptions.TimeOfDay)
                    sb.Append("\"" + item.Hour + "\"" + ",");
                else
                    sb.Append("\"" + item.ToString("yy-MM-dd").Replace("-", "/") + "\"" + ",");
            }
            string str = sb.ToString().Substring(0, sb.ToString().Length - 1);
            AxisJsonStr = "{" + string.Format("categories:[{0}] ", str);
            ///加逗号和右括号
            if(Period!=net91com.Stat.Core.PeriodOptions.TimeOfDay)
                AxisJsonStr += ",labels:{ align:'left', rotation: -45, tickLength:80,tickPixelInterval:140 ,x:-30,y:45";
            else
                AxisJsonStr += ",labels:{ align:'left',tickLength:80,tickPixelInterval:140 ,x:-5";
            ///设置系数
            if (times.Count > LineMaxNum)
                 AxisJsonStr += ",step:" + LineMaxNumCoef.ToString();
            AxisJsonStr += "}}";

        }
        /// <summary>
        /// 获取y轴数据
        /// </summary>
        /// <param name="temp"></param>
        protected void GetDataJsonList(List<List<Monitor_DataLogs>> temp)
        {
            foreach (var item in temp)
            {
                
                ///构造一个json模型
                SeriesJsonModel sjModel2 = new SeriesJsonModel();
                for (int ii = 0; ii < X_DateTime.Count; ii++)
                {
                     sjModel2.data.Add(null);
                }
                sjModel2.name = item[0].DataLogName + "_" + item[0].ServerIp;
                if (X_DateTime.Count <= LineMaxNum)
                {
                    for (int j = 0; j < X_DateTime.Count; j++)
                    {
                        for (int i = 0; i < item.Count; i++)
                        {
                            if (item[i].LogDate == X_DateTime[j])
                            {
                                DataLabels dl = new DataLabels();
                                SmallDataLabels smalldata = new SmallDataLabels();
                                dl.y = item[i].LogFileSize;
                                smalldata.enabled = true;
                                dl.dataLabels = smalldata;
                                ///替换掉以前的null
                                sjModel2.data[j] = dl;
                                break;
                            }
                        }

                    }
                    SeriesJsonModels.Add(sjModel2);

                }
                else
                {
                    for (int j = 0; j < X_DateTime.Count; j++)
                    {
                        for (int i = 0; i < item.Count; i++)
                        {
                            if (item[i].LogDate == X_DateTime[j])
                            {
                                DataLabels dl = new DataLabels();
                                SmallDataLabels smalldata = new SmallDataLabels();
                                dl.y = item[i].LogFileSize;
                                if (j % (LineMaxNumCoef) == 0)
                                    smalldata.enabled = true;
                                dl.dataLabels = smalldata;
                                ///替换掉以前的null
                                sjModel2.data[j] = dl;
                                break;
                            }
                        }

                    }
                    SeriesJsonModels.Add(sjModel2);

                    
                }
                
            }
        }
    }
}