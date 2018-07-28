using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using net91com.Core.Util;

namespace net91com.Reports.Tools
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
    }
}