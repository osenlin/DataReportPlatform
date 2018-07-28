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

using net91com.Reports.UserRights;

namespace net91com.Stat.Web
{
    public partial class ReportStat : ReportBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            //权限验证
            loginService.HaveUrlRight();

            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            { 
                foreach (ProjectSource item in AvailableProjectSources)
                {
                    ddlProjectSource.Items.Add(new ListItem(item.Name, item.ProjectSourceID.ToString()));
                }
                foreach (MobileOption platform in Enum.GetValues(typeof(MobileOption)))
                {
                    if (platform != MobileOption.None && platform != MobileOption.All)
                    {
                        ddlPlatform.Items.Add(new ListItem(platform.ToString(), ((int)platform).ToString()));
                    }
                }
            }
            //string md5 =Util.CryptoHelper.MD5_Encrypt("Reports/ReportStat.aspx".ToLower());

            //System.Data.DataTable dt = BY.AccessControlCore.RightManager.FindTableByPk(md5).Tables[0];

            Right right = AvailableRights.FirstOrDefault(a => a.PageUrl.ToLower() == "Reports/ReportStat.aspx".ToLower());
            if (right != null)
            {
                menuPannelKey.Value = right.ParentID.ToString();
            }
        }
    }
}