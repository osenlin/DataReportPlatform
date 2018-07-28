using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public class URBasePage : Page 
    {
        protected URLoginService loginService = new URLoginService();

        protected void AlertBack(string message)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "", string.Format("<script>alert(\"{0}\");</script>", message));
        }

        /// <summary>
        /// 获取Request.QueryString参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected string GetQueryString(string key, string defaultValue)
        {
            return string.IsNullOrEmpty(Request.QueryString[key]) ? defaultValue : Request.QueryString[key];
        }

        /// <summary>
        /// 获取Request.QueryString参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetQueryString(string key)
        {
            return string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
        }
    }
}