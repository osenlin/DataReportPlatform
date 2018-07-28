using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    /// <summary>
    /// 提供给第三方系统使用的接口，获取用户的权限
    /// </summary>
    public class GetRights : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            int sysId = 0;
            int.TryParse(context.Request["sysId"], out sysId);
            string account = context.Request["account"];
            string sign = context.Request["sign"];
            context.Response.Write(new URRightsService().GetUserRightsJson(sysId, account, sign));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}