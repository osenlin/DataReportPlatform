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
    public partial class RightManage : URBasePage
    {
        protected URBasicInfoService biService = new URBasicInfoService();

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
            ReturnUrl = HttpUtility.UrlEncode("RightManage.aspx?sysId=" + SelectedSysID.ToString());
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        protected string GetRightTree()
        {
            StringBuilder jsonBuilder = new StringBuilder("[");
            List<Right> rights = biService.GetRights(SelectedSysID);
            for (int i = 0; i < rights.Count(); i++)
            {
                Right right = rights.ElementAt(i);
                jsonBuilder.AppendFormat("{{id:\"{0}\",pId:\"{1}\",name:\"{2}\",open:{3},_url:\"{4}\",type:{5},status:{6},descript:\"{7}\",onlyinternal:\"{8}\"}},",
                    right.ID, right.ParentID, right.Name, "true", right.PageUrl, (int)right.RightType, (int)right.Status, right.Description, right.OnlyInternal ? "是" : "否");
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

        /// <summary>
        /// 当前选择的系统ID
        /// </summary>
        protected string SelectedSysName
        {
            get { return ddlSystems.SelectedItem.Text; }
        }

    }
}