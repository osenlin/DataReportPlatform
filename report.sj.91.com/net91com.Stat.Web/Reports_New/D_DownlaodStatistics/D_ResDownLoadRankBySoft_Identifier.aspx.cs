using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadRankBySoft_Identifier : net91com.Stat.Web.Base.ReportBasePage
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

        /// <summary>
        /// 去掉没有下载的产品
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get
            {
                return loginService.AvailableSofts.Where(a => a.ID > 0 && a.SoftType == SoftTypeOptions.InternalSoft ).ToList();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl);
            SoftPlatformJson = GetSoftPlatHtmlJson();
            
            RestypeHtml = GetResTypeHtml();
            PlatHtml = GetPlatformHtml(true, 0, true);

            SoftHtml =GetSoftHtml();

            SoftAreaJson = GetSoftAreaJson();
            AreaJson = GetAreaJson();

            BeginTime = DateTime.Now.AddDays(-1);
        }
    }
}