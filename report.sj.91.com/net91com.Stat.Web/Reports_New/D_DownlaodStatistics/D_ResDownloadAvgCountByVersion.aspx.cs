using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.Services.CommonServices.Tool;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownloadAvgCountByVersion : net91com.Stat.Web.Base.ReportBasePage
    {
        public string SoftPlatformJson;
        public string ProjectJson; 
        public string RestypeHtml; 
        public string SoftHtml;
        public string PlatHtml;
        public int IsEn = 0;
        public string AreaJson;
        public string SoftAreaJson;



        protected override List<Soft> AvailableSofts
        {
            get
            {
                //&&(a.ID==46 ||a.ID==85 ||a.ID==58)
                 return loginService.AvailableSofts.Where(a => a.ID > 0 && a.SoftType == SoftTypeOptions.InternalSoft ).ToList();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           // loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl);
            SoftHtml = GetSoftHtml();
            ProjectJson = GetProjectJson();
            SoftPlatformJson = GetSoftPlatHtmlJson();
            RestypeHtml = GetResTypeHtml(1);
            AreaJson = GetAreaJson();
            SoftAreaJson = GetSoftAreaJson();
            PlatHtml = GetPlatformHtml(false, -1, false);
            EndTime = DateTime.Now.Date.AddDays(-1);
            BeginTime = EndTime.AddDays(-30);
             
        }
        
    }
}