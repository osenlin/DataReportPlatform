using net91com.Core.Util;
using net91com.Stat.Web.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace net91com.Stat.Web
{
    public partial class LogList : ReportBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            long ticks = net91com.Stat.Core.UtilHelper.GetTimeTicks(DateTime.Now.AddMinutes(50));
            string sign = CryptoHelper.MD5_Encrypt(ticks + "!!)@)^@$");
            Response.Redirect("http://funcstatic.sj.91.com/wcf/loglist.aspx?account=" + ticks + "&sign=" + sign);
        }
    }
}