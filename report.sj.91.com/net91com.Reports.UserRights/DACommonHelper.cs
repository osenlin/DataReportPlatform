using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using net91com.Core.Util;
using System.Web;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 连接字符串辅助类
    /// </summary>
    internal class DACommonHelper
    {
        /// <summary>
        /// 用户权限库连接串
        /// </summary>
        public static string ConnectionString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        /// <summary>
        /// REPORT平台的系统ID
        /// </summary>
        public const int REPORT_SYS_ID = 1;

        /// <summary>
        /// 获取客户端登录IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string l_ret = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                l_ret = Convert.ToString(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(l_ret))
                l_ret = Convert.ToString(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return l_ret;
        }
    }
}