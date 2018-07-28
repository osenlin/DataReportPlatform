using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class Log : URBasePage
    {
        protected URBasicInfoService biService = new URBasicInfoService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                txtEndTime.Value= DateTime.Now.ToString("yyyy-MM-dd");
                txtStartTime.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                ddlSystems.DataSource = loginService.AdminSystems;
                ddlSystems.DataTextField = "Name";
                ddlSystems.DataValueField = "ID";
                ddlSystems.DataBind();
                ddlSystems.AutoPostBack = false;
                ddlSystems.SelectedIndex = 0;
                BindData();
            }
        }

        private void BindData()
        {
            int count = 0;
            int sysId = string.IsNullOrEmpty(ddlSystems.SelectedValue) ? 0 : Convert.ToInt32(ddlSystems.SelectedValue);
            List<AdminLog> logs = biService.GetAdminLogs(sysId, txtKeyword.Text.Trim(), Convert.ToDateTime(txtStartTime.Value), Convert.ToDateTime(txtEndTime.Value), AspNetPager1.CurrentPageIndex, AspNetPager1.PageSize, ref count);
            repeaData.DataSource = logs;
            repeaData.DataBind();
            AspNetPager1.RecordCount = count;
        }

        protected void btnSreach_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}