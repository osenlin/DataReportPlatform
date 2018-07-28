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
    public partial class MsmqMonitor : ReportBasePage
    {
        /// 横轴显示的json字符
        public string AxisJsonStr { get; set; }
        /// <summary>
        /// 具体点坐标的json字符
        /// </summary>
        public string SeriesJsonStr { get; set; }
        ///用户绑定图的title
        public string ReportTitle { get; set; }

        /// 转换为坐标点json字符的中间类
        public List<SeriesJsonModel> SeriesJsonModels = new List<SeriesJsonModel>();

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            Dictionary<string, string> dic = MsmqMonitorService.GetMsmqNameDict();
            HeadControl1.LogNameSource = dic;;
            HeadControl1.ServerIpSingle = true;
            HeadControl1.HiddenDoubleTime = true;
            HeadControl1.HiddenSingleTime = false;
            ReportTitle = "消息队列监控";
            AxisJsonStr = "{}";
            SeriesJsonStr = "[]";
            if (HeadControl1.IsFirstLoad)
            {
                HeadControl1.LogName = @".\private$\sjchanneluserdata";
                HeadControl1.ServerIp = "121.207.242.100";
            }
            BindData();           
        }

        private void BindData()
        {
            //获取X与Y轴数据
            MsmqMonitorService mmService = new MsmqMonitorService();
            Dictionary<string, List<DateTime>> xData;
            Dictionary<string, List<int>> yData = mmService.GetMsmqMsgCountList(HeadControl1.LogName, HeadControl1.SingleTime, out xData);
            if (yData[HeadControl1.ServerIp].Count == 0)
            {
                ReportTitle = "无数据";
                return;
            }
            LineMaxNumCoef = xData[HeadControl1.ServerIp].Count / LineMaxNum + 1;
            SetxAxisJson(xData[HeadControl1.ServerIp]);
            GetDataJsonList(HeadControl1.ServerIp, yData[HeadControl1.ServerIp]);
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
                sb.Append("\"" + item.ToString("MM-dd HH:mm:ss").Replace("-", "/") + "\"" + ",");
            }
            string str = sb.ToString().Substring(0, sb.ToString().Length - 1);
            AxisJsonStr = "{" + string.Format("categories:[{0}] ", str);
            ///加逗号和右括号
            AxisJsonStr += ",labels:{ align:'left', rotation: -45, tickLength:80,tickPixelInterval:140 ,x:-45,y:70";
            ///设置系数
            if (times.Count > LineMaxNum)
                AxisJsonStr += ",step:" + LineMaxNumCoef.ToString();
            AxisJsonStr += "}}";

        }
        /// <summary>
        /// 获取y轴数据
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="yData"></param>
        protected void GetDataJsonList(string serverIp, List<int> yData)
        {
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            for (int ii = 0; ii < yData.Count; ii++)
            {
                sjModel2.data.Add(null);
            }
            sjModel2.name = serverIp;
            if (yData.Count <= LineMaxNum)
            {
                for (int j = 0; j < yData.Count; j++)
                {
                    DataLabels dl = new DataLabels();
                    SmallDataLabels smalldata = new SmallDataLabels();
                    dl.y = yData[j];
                    smalldata.enabled = true;
                    dl.dataLabels = smalldata;
                    ///替换掉以前的null
                    sjModel2.data[j] = dl;
                }
                SeriesJsonModels.Add(sjModel2);

            }
            else
            {
                for (int j = 0; j < yData.Count; j++)
                {
                    DataLabels dl = new DataLabels();
                    SmallDataLabels smalldata = new SmallDataLabels();
                    dl.y = yData[j];
                    if (j % (LineMaxNumCoef) == 0)
                        smalldata.enabled = true;
                    dl.dataLabels = smalldata;
                    ///替换掉以前的null
                    sjModel2.data[j] = dl;

                }
                SeriesJsonModels.Add(sjModel2);
            }
        }
    }
}