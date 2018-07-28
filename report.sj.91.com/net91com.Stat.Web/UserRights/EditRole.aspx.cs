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
    public partial class EditRole : URBasePage  
    {
        protected URBasicInfoService biService = new URBasicInfoService();

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
                //非超级管理无法定义非普通角色
                if (loginService.LoginUser.AccountType != UserTypeOptions.SuperAdmin
                    || GetQueryString("sysId").ToInt32(0) != URBasicInfoService.REPORT_SYS_ID)
                {
                    rblRoleTypes.Items[1].Enabled = false;
                    rblRoleTypes.Items[2].Enabled = false;
                }
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {
            if (RoleID > 0)
            {
                try
                {
                    Role role = biService.GetRole(RoleID);
                    if (role != null)
                    {
                        txtName.Text = role.Name;
                        rblstatus.SelectedValue = ((int)role.Status).ToString();
                        rblRoleTypes.SelectedValue = ((int)role.RoleType).ToString();
                        txtDescription.Text = role.Description;
                    }
                }
                catch (NotRightException)
                {
                    AlertBack("您没有权限执行此操作");
                }
            }
        }

        /// <summary>
        /// 角色ID
        /// </summary>
        private int RoleID
        {
            get { return GetQueryString("rid").ToInt32(0); }
        }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get { return string.IsNullOrEmpty(Request["ReturnUrl"]) ? "RoleManage.aspx" : Request["ReturnUrl"]; }
        }

        /// <summary>
        /// 添加/编辑角色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, System.EventArgs e)
        {
            string roleName = txtName.Text.Trim();
            if (roleName == "")
            {
                AlertBack("请输入角色名称！");
                return;
            }
            Role role = new Role
            {
                ID = RoleID,
                Description = this.txtDescription.Text,
                Name = roleName,
                RoleType = (RoleTypeOptions)Convert.ToInt32(rblRoleTypes.SelectedValue),
                Status = (StatusOptions)Convert.ToInt32(rblstatus.SelectedValue),
                SystemID = GetQueryString("sysId").ToInt32(0),
            };
            try
            {
                //添加
                if (RoleID == 0)
                {
                    biService.AddRole(role);
                    Response.Redirect(ReturnUrl);
                }
                else  //修改
                {
                    biService.UpdateRole(role);
                    Response.Redirect(ReturnUrl);
                }
            }
            catch (NotRightException)
            {
                AlertBack("您没有权限执行此操作");
            }
            catch (net91com.Core.ToUserException ex)
            {
                AlertBack(ex.Message);
            }
        }
    }
}