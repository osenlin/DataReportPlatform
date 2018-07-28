using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.Services.CommonServices.Tool;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownloadStatistics
{
    public partial class D_ResDownloadAvgCountByChannelType : net91com.Stat.Web.Base.ReportBasePage
    {
        public string SoftPlatformJson;
        public string ProjectJson; 
        public string RestypeHtml; 
        public string SoftHtml;
        public string PlatHtml;
        public string ChannelType="1";

        protected override List<Soft> AvailableSofts
        {
            get
            {

                if (ChannelType == "1")
                {
                    return loginService.AvailableSofts.Where(a => a.ID == 9 || a.ID == 105550).ToList();
                }
                else 
                {
                    return loginService.AvailableSofts.Where(a => a.ID == 2 || a.ID == 46).ToList();
                }
            }
        }
 
        protected void Page_Load(object sender, EventArgs e)
        {
            ChannelType = Request["channeltype"];
            string querystr = "?channeltype=" + ChannelType;
            loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl, querystr);
            SoftHtml = GetSoftHtml();
            SoftPlatformJson = GetSoftPlatHtmlJson();
            RestypeHtml = GetResTypeHtml();
            PlatHtml = GetPlatformHtml(true, 0, false);

            EndTime = DateTime.Now.Date.AddDays(-1);
            BeginTime = EndTime.AddDays(-15);
             
        }
        
    }
}