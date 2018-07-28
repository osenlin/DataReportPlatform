using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Threading;
using System.Web.SessionState;

using net91com.Core;
using net91com.Core.Util;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web
{
    /// <summary>
    /// Summary description for LoginCheck
    /// </summary>
    public class LoginCheck : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = HttpContext.Current.Request;
            URLoginService loginService = new URLoginService();
            try
            {
                string logoutString = request.QueryString["logout"];
                bool logout = false;
                if (!string.IsNullOrEmpty(logoutString) && bool.TryParse(logoutString, out logout) && logout)
                {
                    loginService.Logout();
                    if (!string.IsNullOrEmpty(request.QueryString["ReturnUrl"]))
                    {
                        HttpContext.Current.Response.Redirect(request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect(FormsAuthentication.DefaultUrl);
                    }
                }
                else
                {
                    loginService.Login();
                }
            }
            catch (ThreadAbortException) { }
            catch (ToUserException ex)
            {
                HttpContext.Current.Response.Write("alert('" + ex.Message.Replace("'", "\\'") + "');location='" + request.Url + "&ReturnUrl=" + HttpContext.Current.Server.UrlEncode(request.QueryString["ReturnUrl"]) + "';");
            }            
            catch (Exception ex)
            {
                LogHelper.WriteException("net91com.Stat.Web", ex);
                HttpContext.Current.Response.Write("alert('系统错误,请稍候再试...');location='" + request.Url + "&ReturnUrl=" + HttpContext.Current.Server.UrlEncode(request.QueryString["ReturnUrl"]) + "';");
            }
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}