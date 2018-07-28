using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class RoleManage : URBasePage 
    {
        /// <summary>
        /// 返回页面地址
        /// </summary>
        protected string ReturnUrl;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlSystems.DataSource = loginService.AdminSystems;
                ddlSystems.DataTextField = "Name";
                ddlSystems.DataValueField = "ID";
                ddlSystems.DataBind();
                ddlSystems.SelectedValue = Request["sysId"];
                ddlSystems.AutoPostBack = true;
            }
            ReturnUrl = HttpUtility.UrlEncode("RoleManage.aspx?sysId=" + SelectedSysID.ToString());

            BindData();
        }
        private void BindData()
        {
            URBasicInfoService biService = new URBasicInfoService();
            int pageIndex = string.IsNullOrEmpty(Request["page"]) ? 1 : Convert.ToInt32(Request["page"]);
            int pageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 15 : Convert.ToInt32(Request["pagesize"]);
            int count = 0;
            List<Role> list = biService.GetRoles(SelectedSysID, pageIndex, pageSize, ref count);
            repeaData.DataSource = list;
            repeaData.DataBind();
            AspNetPager1.RecordCount = count;
            AspNetPager1.PageSize = pageSize;
            AspNetPager1.CurrentPageIndex = pageIndex;
        }

        /// <summary>
        /// 当前选择的系统ID
        /// </summary>
        protected int SelectedSysID
        {
            get { return string.IsNullOrEmpty(ddlSystems.SelectedValue) ? 0 : Convert.ToInt32(ddlSystems.SelectedValue); }
        }

        /// <summary>
        /// 当前选择的系统ID
        /// </summary>
        protected string SelectedSysName
        {
            get { return ddlSystems.SelectedItem.Text; }
        }

        #region 编辑角色HTML构造(GetOpHtml)

        /// <summary>
        /// 编辑角色HTML构造
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        protected string GetOpHtml(Role role)
        {
            if (loginService.CheckAdminRightForRole(role))
            {
                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.AppendFormat(@"<a href=""javascript:eidt({0})"" name=""upd"">修改</a>", role.ID);
                htmlBuilder.AppendFormat(@"<a href=""javascript:dele({0})"" name=""dele"">删除</a>", role.ID);
                htmlBuilder.AppendFormat(@"<a href=""javascript:eidtright({0},'{1}',{2})"" name=""right"">操作权限</a>", role.ID, role.Name, SelectedSysID);
                //是REPORT平台才显示该项
                if (SelectedSysID == URBasicInfoService.REPORT_SYS_ID)
                    htmlBuilder.AppendFormat(@"<a href=""javascript:eidtsoftright({0},'{1}',{2})"" name=""right"">产品权限</a>", role.ID, role.Name, role.SystemID);
                htmlBuilder.AppendFormat(@"<input type=""hidden"" id=""input{0}"" value=""{1}"" />", role.ID, role.Name);
                return htmlBuilder.ToString();
            }
            return string.Empty;
        }

        #endregion
    }
}