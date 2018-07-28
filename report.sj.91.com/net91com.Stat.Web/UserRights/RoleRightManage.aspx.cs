using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using net91com.Core;

using net91com.Core.Extensions;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class RoleRightManage : URBasePage 
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        protected string PageTitle;

        /// <summary>
        /// 用户ID
        /// </summary>
        protected int UserID { get { return GetQueryString("userId").ToInt32(0); } }

        /// <summary>
        /// 角色ID
        /// </summary>
        protected int RoleID { get { return GetQueryString("roleId").ToInt32(0); } }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(Request["ReturnUrl"]))
                    return Request["ReturnUrl"];
                if (UserID > 0)
                    return "UserManage.aspx";
                return "RoleManage.aspx";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (RoleID > 0)
                {
                    int sysId = GetQueryString("sysId").ToInt32(0);
                    if (sysId <= 0)
                        throw new NotRightException();
                    ddlSystems.DataSource = loginService.AdminSystems.Where(a => a.ID == sysId);
                }
                else
                {
                    List<UserSystem> userSystems = new URRightsService().GetUserSystems(UserID);
                    ddlSystems.DataSource = loginService.AdminSystems.Where(a => userSystems.Exists(b => a.ID == b.SystemID));
                }
                ddlSystems.DataTextField = "Name";
                ddlSystems.DataValueField = "ID";
                ddlSystems.DataBind();
                ddlSystems.SelectedValue = Request["sysId"];
                ddlSystems.AutoPostBack = true;
            }
            PageTitle = string.Format("为{0}【{1}】设置权限", UserID > 0 ? "用户" : "角色", GetQueryString("name"));
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        protected string GetRightTree()
        {
            List<Right> rights = new URBasicInfoService().GetRights(SelectedSysID);
            List <RightItem> selectedRights = UserID > 0 
                ? new URRightsService().GetUserRights(SelectedSysID, UserID) 
                : new URRightsService().GetRoleRights(RoleID);
            StringBuilder jsonBuilder = new StringBuilder("[");
            for (int i = 0; i < rights.Count(); i++)
            {
                Right right = rights.ElementAt(i);
                jsonBuilder.AppendFormat("{{\"id\":\"{0}\",\"pId\":\"{1}\",\"name\":\"{2}\",\"open\":{3}",
                        right.ID, right.ParentID, right.Name, "true");
                RightItem rr = selectedRights.FirstOrDefault(r => r.RightID == right.ID);
                if (rr != null)
                {
                    jsonBuilder.Append(",\"checked\":true");
                    if (rr.FromRole)
                        jsonBuilder.Append(",\"disabled\":true");
                }
                jsonBuilder.Append("},");
            }
            return jsonBuilder.ToString().TrimEnd(',') + "]";
        }

        /// <summary>
        /// 当前选择的系统ID
        /// </summary>
        protected int SelectedSysID
        {
            get { return string.IsNullOrEmpty(ddlSystems.SelectedValue) ? 0 : Convert.ToInt32(ddlSystems.SelectedValue); }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<int> selectedRightIds = Request["SelectedRightIds"]
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => int.Parse(a)).ToList();
            if (UserID > 0)
                new URRightsService().AddUserRights(SelectedSysID, UserID, selectedRightIds);
            else if (RoleID > 0)
                new URRightsService().AddRoleRights(RoleID, selectedRightIds);
            Response.Redirect(ReturnUrl);
        }
    }
}