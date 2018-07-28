using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace net91com.Stat.Web.Reports_New.R_UserAnalysis
{
    public partial class ChannelCustomerManager_Iframe2 : net91com.Stat.Web.Base.ReportBasePage
    {
        public bool HasEditRight = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight("Reports_New/R_UserAnalysis/ChannelCustomerManager.aspx");
            HasEditRight = loginService.CheckUrlRight("reports_new/r_useranalysis/channelcustomermanager.aspx", "?act=editaddcustomer");
        }
    }
}