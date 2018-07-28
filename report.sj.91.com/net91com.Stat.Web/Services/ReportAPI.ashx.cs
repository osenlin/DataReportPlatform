using System;
using System.Web;
using System.Text;
using net91com.Core.Util;

using net91com.Reports.Services;
using ToolService = net91com.Reports.Services.CommonServices.Tool.ToolService;

namespace net91com.Stat.Web.Services
{
    /// <summary>
    /// 提供对外的接口
    /// </summary>
    public class ReportAPI : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string act = context.Request["act"];
            if (string.IsNullOrEmpty(act)) return;
            switch (act.ToLower())
            {
                case "getversioninfo":
                    GetVersionInfo();
                    break;
            }
        }

        private void GetVersionInfo()
        {
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.Charset = "UTF-8";
            string html = "";
            try
            {
                HttpRequest req = HttpContext.Current.Request;
                if (string.IsNullOrEmpty(req["sign"]))
                    throw new Exception("参数缺省异常");

                DateTime datetime = Convert.ToDateTime(req["stattime"]);
                string sign = req["sign"];
                string md5 = UtilityHelp.StrMd5By32("&getversioninfo$01$");
                if (sign == md5)
                {
                    var callback = req["callback"];
                    html = ToolService.Instance.GetVersionInfo();

                    if (!string.IsNullOrEmpty(callback))
                        html = string.Format(@"{0}({1})", callback, html);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("提供百度搜索kpi的接口异常", ex);

            }
            finally
            {

                HttpContext.Current.Response.Write(html);
                HttpContext.Current.Response.Flush();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}