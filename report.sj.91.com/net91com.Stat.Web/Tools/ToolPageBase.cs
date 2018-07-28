using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Tools
{
    public class ToolPageBase : System.Web.UI.Page 
    {
        protected URLoginService loginService = new URLoginService();

        protected override void OnInit(EventArgs e)
        {
            if (!CheckRight())
            {
                Response.Redirect("~/Error.html");
            }
            base.OnInit(e);
        }

        /// <summary>
        /// 工具使用权限
        /// </summary>
        /// <returns></returns>
        protected bool CheckRight()
        {
            return loginService.InternalRequest || (loginService.LoginUser != null
                && loginService.LoginUser.AccountType == UserTypeOptions.SuperAdmin);
        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected virtual List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts; }
        }

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