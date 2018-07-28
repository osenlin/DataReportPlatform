using System;
using System.Web.UI;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadByResIDDetail : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            myhidden.Value = Request["params"];
        }
    }
}