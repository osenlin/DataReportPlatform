using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using net91com.Core.Extensions;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class UserRoleManage : URBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<UserSystem> userSystems = new URRightsService().GetUserSystems(UserID);
                ddlSystems.DataSource = loginService.AdminSystems.Where(a => userSystems.Exists(b => a.ID == b.SystemID));
                ddlSystems.DataTextField = "Name";
                ddlSystems.DataValueField = "ID";
                ddlSystems.DataBind();
                ddlSystems.SelectedValue = Request["sysId"];
                ddlSystems.AutoPostBack = true;
                DisplayData();
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void DisplayData()
        {
            try
            {
                URBasicInfoService biService = new URBasicInfoService();
                //特定用户类型要限定特定角色
                List<Role> rangeRoles = biService.GetRoles(SelectedSysID);
                User user = biService.GetUser(UserID);
                if (user.AccountType == UserTypeOptions.Channel)
                    rangeRoles = rangeRoles.Where(a => a.RoleType == RoleTypeOptions.Channel).ToList();
                else if (user.AccountType == UserTypeOptions.ChannelPartner)
                    rangeRoles = rangeRoles.Where(a => a.RoleType == RoleTypeOptions.ChannelPartner).ToList();
                cbkListrole.DataSource = rangeRoles;
                cbkListrole.DataTextField = "Name";
                cbkListrole.DataValueField = "ID";
                cbkListrole.DataBind();
                List<int> selectedRoleIds = new URRightsService().GetUserRoles(SelectedSysID, UserID);
                foreach (ListItem item in cbkListrole.Items)
                {
                    if (selectedRoleIds.Exists(a => int.Parse(item.Value) == a))
                        item.Selected = true;
                }
            }
            catch (NotRightException)
            {
                AlertBack("您没有权限执行此操作");
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        private int UserID
        {
            get { return GetQueryString("uid").ToInt32(0); }
        }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get { return string.IsNullOrEmpty(Request["ReturnUrl"]) ? "UserManage.aspx" : Request["ReturnUrl"]; }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //获取当前选择的角色列表
            List<int> checkedRoleIds = new List<int>();
            foreach (ListItem item in cbkListrole.Items)
            {
                if (item.Selected)
                    checkedRoleIds.Add(Convert.ToInt32(item.Value));
            }
            try
            {
                new URRightsService().AddUserRoles(SelectedSysID, UserID, checkedRoleIds);
                Response.Redirect(ReturnUrl);
            }
            catch (NotRightException)
            {
                AlertBack("您没有权限执行此操作");
            }
        }

        /// <summary>
        /// 当前选择的系统ID
        /// </summary>
        protected int SelectedSysID
        {
            get { return string.IsNullOrEmpty(ddlSystems.SelectedValue) ? 0 : Convert.ToInt32(ddlSystems.SelectedValue); }
        }

        protected void ddlSystems_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayData();
        }
    }
}