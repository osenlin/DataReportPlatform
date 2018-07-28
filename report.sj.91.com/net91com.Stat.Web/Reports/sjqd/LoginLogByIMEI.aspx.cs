using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using net91com.Core.Extensions;
using net91com.Stat.Web.Base;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.DownloadReports
{
    public partial class LoginLogByIMEI : ReportBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        private void BindData()
        {
            string imei = txtIMEI.Value;
            if (string.IsNullOrEmpty(imei))
            {
                return;
            }
            List<Sjqd_LoginLog> log = Sjqd_LoginLogService.GetList(imei);
            if (log != null && log.Count > 0)
            {
                List<Soft> softs = AvailableSofts;
                Repeater1.DataSource = from obj in log
                                       from soft in softs
                                       where obj.SoftID == soft.ID
                                       orderby obj.LoginTime descending
                                       select new Sjqd_LoginLog
                                       {
                                           SoftID = obj.SoftID,
                                           SoftName = soft.Name,
                                           SoftVersion = obj.SoftVersion,
                                           Fromway = obj.Fromway,
                                           LoginTime = obj.LoginTime
                                       };
                Repeater1.DataBind();
            }
        }
    }
}