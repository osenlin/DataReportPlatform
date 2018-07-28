using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadPositionDistribution : net91com.Stat.Web.Base.ReportBasePage
    {
        public string SoftPlatformJson;
        public string ProjectJson;
        public string RestypeHtml;
        public string SoftHtml;
        public string PlatHtml;
        public string SoftAreaJson;
        public string ParentCategoryHtml;
        public string SubCategoryHtml;
        public string AreaJson;


        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl);
            SoftPlatformJson = GetSoftPlatHtmlJson();
            
            RestypeHtml = GetResTypeHtml();
            PlatHtml = GetPlatformHtml(true, 0, false);

            ProjectJson = GetProjectJson();
            SoftHtml =GetSoftHtml();


            SoftAreaJson = GetSoftAreaJson();
            AreaJson = GetAreaJson();

            // key = string.Format("N_StatDownRank_{0}_StatDate{1}", period, ProjectSourceType == 2 ? "_46" : "");
            EndTime = DateTime.Now.Date.AddDays(-1);

            BeginTime = EndTime.AddDays(-30);
        }
    }
}