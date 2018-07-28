using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.R_UserAnalysis
{
    public partial class ChannelCustomerManager : net91com.Stat.Web.Base.ReportBasePage
    {
        public string SoftPlatformJson =string.Empty;
        public string SoftHtml = string.Empty;
        public string SjqdUrl;
        public string SjqdParentUrl;
        public string SjqdUrlName;
        public bool HasEditRight=false;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();
            HasEditRight = loginService.CheckUrlRight("reports_new/r_useranalysis/channelcustomermanager.aspx", "?act=editaddcustomer");
            SoftHtml = GetSoftHtml(AvailableSofts);
            SoftPlatformJson = GetSoftPlatHtmlJson();
            
            GetUrl();
          

        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID > 0).ToList(); }
        }
       
        protected string GetSoftPlatHtmlJson()
        {
            StringBuilder sb = new StringBuilder("{");

            foreach (Soft item in AvailableSofts)
            {
                sb.AppendFormat("'{0}':[{1}],", item.ID,
                       string.Join(",", item.Platforms.Select(p => ((int)p).ToString()).ToArray()));
            }
            return sb.ToString().TrimEnd(',') + "}";
        }

        private void GetUrl()
        {
            Right right = AvailableRights.FirstOrDefault(a => a.PageUrl.ToLower() == "Reports/SoftVersionSjqd.aspx".ToLower());
            if (right != null)
            {
                SjqdUrl = right.ID.ToString();
                SjqdParentUrl = right.ParentID.ToString();
                SjqdUrlName = right.Name;
            }

        }
    }
}