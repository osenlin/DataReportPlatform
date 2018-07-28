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
    public partial class UserSystemManage : URBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                cbkSystems.DataSource = loginService.AdminSystems;
                cbkSystems.DataTextField = "Name";
                cbkSystems.DataValueField = "ID";
                cbkSystems.DataBind();
                cbkAdminSystems.DataSource = loginService.AdminSystems;
                cbkAdminSystems.DataTextField = "Name";
                cbkAdminSystems.DataValueField = "ID";
                cbkAdminSystems.DataBind();
                List<UserSystem> selectedUserSystems = new URRightsService().GetUserSystems(UserID);
                foreach (ListItem item in cbkSystems.Items)
                {
                    if (selectedUserSystems.Exists(a => int.Parse(item.Value) == a.SystemID))
                        item.Selected = true;
                }
                foreach (ListItem item in cbkAdminSystems.Items)
                {
                    if (selectedUserSystems.Exists(a => int.Parse(item.Value) == a.SystemID && a.Admin))
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
            List<UserSystem> userSystems = new List<UserSystem>();
            foreach (ListItem item in cbkSystems.Items)
            {
                if (item.Selected)
                {
                    userSystems.Add(
                            new UserSystem
                            {
                                UserID = UserID,
                                Admin = false,
                                AddTime = DateTime.Now,
                                LastLoginTime = DateTime.Now,
                                SystemID = Convert.ToInt32(item.Value)
                            });
                }
            }
            foreach (ListItem item in cbkAdminSystems.Items)
            {
                if (item.Selected)
                {
                    bool exists = false;
                    foreach (UserSystem uSys in userSystems)
                    {
                        if (uSys.SystemID == Convert.ToInt32(item.Value))
                        {
                            exists = true;
                            uSys.Admin = true;
                            break;
                        }  
                    }
                    if (!exists)
                    {
                        userSystems.Add(
                            new UserSystem
                            {
                                UserID = UserID,
                                Admin = true,
                                AddTime = DateTime.Now,
                                LastLoginTime = DateTime.Now,
                                SystemID = Convert.ToInt32(item.Value)
                            });
                    }
                }
            }
            try
            {
                new URRightsService().AddUserSystems(UserID, userSystems);
                Response.Redirect(ReturnUrl);
            }
            catch (NotRightException)
            {
                AlertBack("您没有权限执行此操作");
            }
        }
    }
}