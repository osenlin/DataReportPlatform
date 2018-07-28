using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using net91com.Core.Util;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {

        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码
 
        }

        void Application_Error(object sender, EventArgs e)
        { 
            Exception ex = Server.GetLastError().InnerException ?? Server.GetLastError();
            try
            {
                if (ex is net91com.Reports.UserRights.NotRightException)
                {
                    Response.Redirect("/Reports/NoRight.aspx");
                    return;
                }
                if (ex is System.Threading.ThreadAbortException)
                {
                    System.Threading.Thread.ResetAbort();
                    return;
                }
                string ip1 = net91com.Core.Web.WebHelper.GetRealIP();
                string ip2 = Request.Headers.Get("HTTP_NDUSER_FORWARDED_FOR_HAPROXY");
                string serverIp = Request.ServerVariables["LOCAL_ADDR"];
                string msg = string.Format("\r\nGlobal异常: 客户IP:{0};{1};  服务器ip:{2}\r\nPost数据:{3}",
                    ip1, ip2, serverIp, Request.Form);
                URLoginService loginService = new URLoginService();
                string account = string.Empty;
                try
                {
                    account = loginService.LoginUser.Account;
                }
                catch { }
                LogHelper.WriteException(msg + ", Account=" + account, ex);
            }
            finally
            {
                Server.ClearError();
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码

        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。

        }

    }
}
