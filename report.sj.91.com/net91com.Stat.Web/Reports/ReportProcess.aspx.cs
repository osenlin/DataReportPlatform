using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using net91com.Core;
using net91com.Core.Extensions;
using Res91com.ResourceDataAccess;
using net91com.Stat.Web.Base;

namespace net91com.Stat.Web
{
    public partial class ReportProcess : ReportBasePage
    {
        protected override void OnInit(EventArgs e)
        {
       
            //权限验证
            loginService.HaveUrlRight("Reports/ReportStat.aspx");

            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}