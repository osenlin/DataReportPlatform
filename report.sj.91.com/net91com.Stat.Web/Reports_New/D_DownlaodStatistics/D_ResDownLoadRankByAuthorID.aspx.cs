using System;
using System.Web;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadRankByAuthorID : net91com.Stat.Web.Base.ReportBasePage
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
            PlatHtml = GetPlatformHtml(true, 0, true);

            SoftHtml  = GetSoftHtml(AvailableSofts,6);

            SoftAreaJson = GetSoftAreaJson();
            BeginTime = DateTime.Now.AddDays(-1);

        }
    }
}