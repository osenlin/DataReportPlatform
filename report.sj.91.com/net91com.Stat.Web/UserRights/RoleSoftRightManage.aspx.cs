using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using net91com.Core.Extensions;
using System.Text;
using net91com.Core;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class RoleSoftRightManage : URBasePage 
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
            PageTitle = string.Format("为{0}【{1}】设置权限", UserID > 0 ? "用户" : "角色", GetQueryString("name"));

            if (!IsPostBack)
            {
                BindSofts();
            }
        }

        /// <summary>
        /// 加载产品权限
        /// </summary>
        private void BindSofts()
        {
            URBasicInfoService biService = new URBasicInfoService();
            URRightsService rightService = new URRightsService();
            List<Soft> softs = biService.GetSofts();
            List<ProjectSource> projectSources = biService.GetProjectSources();
            List<RightItem> selectedSofts = UserID > 0
                ? rightService.GetUserSoftRights(UserID)
                : rightService.GetRoleSoftRights(RoleID);
            List<RightItem> selectedProjectSources = UserID > 0
                ? rightService.GetUserProjectSourceRights(UserID)
                : rightService.GetRoleProjectSourceRights(RoleID);
            List<RightItem> resIds = new List<RightItem>();
            //产品管理员不能设置资源查看权限
            if (loginService.LoginUser.AccountType != UserTypeOptions.ProductAdmin && UserID > 0)
                resIds = rightService.GetUserResRights(UserID);

            softs.ForEach(soft =>
            {
                var item = new ListItem("[" + soft.OutID.ToString().PadLeft(6, '0') + "]" + soft.Name, soft.ID.ToString());
                RightItem ri = selectedSofts.FirstOrDefault(f => f.RightID == soft.ID);
                item.Selected = ri != null;
                item.Enabled = ri == null || !ri.FromRole;
                item.Attributes.Add("stype", soft.SoftType.ToString());
                item.Attributes.Add("intype", "checkbox");
                if (soft.SoftType == SoftTypeOptions.InternalSoft || soft.SoftType == SoftTypeOptions.MultiSofts)
                    cbkListSoft.Items.Add(item);
                else
                    cbkListSoftout.Items.Add(item);
            });
            if (resIds.Count > 0)
            {
                txtResIdList.Text = string.Join(", ", resIds.Select(a => a.RightID.ToString()).ToArray());
            }
            projectSources.ForEach(projectSource =>
            {
                var item = new ListItem("[" + projectSource.ProjectSourceID + "]" + projectSource.Name, projectSource.ProjectSourceID.ToString());
                RightItem ri = selectedProjectSources.FirstOrDefault(f => f.RightID == projectSource.ProjectSourceID);
                item.Selected = ri != null;
                item.Enabled = ri == null || !ri.FromRole;
                item.Attributes.Add("intype", "checkbox");
                cbkProjectSource.Items.Add(item);
            });
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<int> softIds = new List<int>();
            foreach (ListItem item in cbkListSoft.Items)
            {
                if (item.Selected)
                    softIds.Add(int.Parse(item.Value));                    
            }
            foreach (ListItem item in cbkListSoftout.Items)
            {
                if (item.Selected)
                    softIds.Add(int.Parse(item.Value));                    
            }
            List<int> projectSources = new List<int>();
            foreach (ListItem item in cbkProjectSource.Items)
            {
                if (item.Selected)
                    projectSources.Add(int.Parse(item.Value));            
            }
            List<int> resIds = new List<int>();
            if (txtResIdList.Text.Length > 0)
            {
                string[] resids = txtResIdList.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);                
                foreach (string resid in resids)
                {
                    int id = 0;
                    if (int.TryParse(resid.Trim(), out id))
                    {
                        resIds.Add(id);
                    }
                }
            }
            URRightsService rightService = new URRightsService();
            if (UserID > 0)
            {
                rightService.AddUserSoftRights(UserID, softIds);
                rightService.AddUserProjectSourceRights(UserID, projectSources);
                if (loginService.LoginUser.AccountType != UserTypeOptions.ProductAdmin)
                  rightService.AddUserResRights(UserID, resIds);
            }
            else if (RoleID > 0)
            {
                rightService.AddRoleSoftRights(RoleID, softIds);
                rightService.AddRoleProjectSourceRights(RoleID, projectSources);
            }

            Response.Redirect(ReturnUrl);
        }
    }
}