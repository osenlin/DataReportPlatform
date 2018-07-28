using System.Web;
using net91com.Stat.Web.Base;
using System.Text;
using net91com.Stat.Services.Monitor;

namespace net91com.Stat.Web.Monitor
{
    /// <summary>
    /// ServerLogService 的摘要说明
    /// </summary>
    public class MonitorService : HandlerBase
    {

        public override void ProcessRequest(HttpContext context)
        {
            switch (GetQueryString("act").ToLower())
            {
                case "getserverip":
                    GetServerIp();
                    break;
               
            }
        }
        public void GetServerIp()
        {
            //if (!CheckHasRight("Reports/ServerLogReports/ServiceLogDateReport.aspx"))
            //    return;
            //List<Monitor_DataLogs> logList = new Monitor_DataLogsServer(true).GetLogNameAndServerIpCache(CacheTimeOption.TenMinutes);
            //string logname = GetQueryString("logname");
            //if (logList != null)
            //{
            //    var list = logList.Where(p => p.DataLogName == logname).Select(p=>p.ServerIp).ToList();
            //    StringBuilder sb = new StringBuilder("[");
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        sb.Append("{").AppendFormat("\"ServerID\":\"{0}\",\"ServerIp\":\"{1}\"",list[i],list[i]).Append("},");
            //    }
            //    ThisResponse.Write( sb.ToString().TrimEnd(',') + "]");
                
            //}

            string[] serverIps = Monitor_DataLogsServer.GetServerIPList();
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < serverIps.Length; i++)
            {
                sb.Append("{").AppendFormat("\"ServerID\":\"{0}\",\"ServerIp\":\"{1}\"", serverIps[i], serverIps[i]).Append("},");
            }
            ThisResponse.Write(sb.ToString().TrimEnd(',') + "]");


        }
    }
}