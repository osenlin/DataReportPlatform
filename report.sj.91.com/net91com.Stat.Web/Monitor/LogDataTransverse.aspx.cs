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
    public partial class LogDataTransverse : ReportBasePage
    {
        
        protected string SeriesJsonStr { get; set; }
        ///用户绑定图的title
        public string reportTitle { get; set; }
        public List<Monitor_DataLogs> allList;
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            //var list = new Monitor_DataLogsServer(true).GetLogNameAndServerIpCache(CacheTimeOption.TenMinutes).Select(p => p.DataLogName).Distinct().ToList();
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //list.ForEach(p => dic.Add(p, p));
            Dictionary<string, string> dic = Monitor_DataLogsServer.GetDataLogNameDirt();
            HeadControl1.LogNameSource = dic;
            HeadControl1.HiddenServerIp = true;
            reportTitle = "日志分布";
            SeriesJsonStr = "[]";
            allList = new List<Monitor_DataLogs>();
            if (HeadControl1.IsFirstLoad)
            {
                HeadControl1.LogName = "sjqd";
            }
            var temp = new Monitor_DataLogsServer(true).GetLogTransverseCache(HeadControl1.LogName, HeadControl1.BeginTime, HeadControl1.EndTime,CacheTimeOption.TenMinutes);
            if (temp.Count != 0)
            {
                SeriesJsonStr = GetYlineJson(temp);
            }
            else
                reportTitle = "无数据";
        }

        protected string GetYlineJson(List<Monitor_DataLogs> list)
        {
            int all = list.Sum(p => p.LogFileSize);
            StringBuilder sb = new StringBuilder();
            sb.Append("[{type: 'pie',name: '服务器日志分布',data: [");
            for (int i = 0; i <list.Count; i++)
            {
                 sb.AppendFormat("['{0}',{1}],",list[i].ServerIp, list[i].LogFileSize);
            }
            return sb.ToString().TrimEnd(',') + "]}]";
        }
    }
}