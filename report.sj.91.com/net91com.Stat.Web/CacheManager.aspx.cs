using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Web.Tools;

namespace net91com.Stat.Web
{
    public partial class CacheManager : ToolPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sign = Request["sign"];
            string action = Request["action"];

            string CheckSign =
                net91com.Core.Util.CryptoHelper.MD5_Encrypt(action + "&" + DateTime.Now.ToString("yyyyMMdd") + "&" +
                                                            "$clear123$");
            if (string.IsNullOrEmpty(action)||CheckSign != sign)
            {
                throw  new HttpException(404,"Page Not Found");
            }
            switch (action)
            {
                case "clearcache":
                    net91com.Core.Web.CacheHelper.Clear();
                    Response.Write("清除缓存成功!");
                    return;

            }
        }
    }
}