using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadSumBySoft : net91com.Stat.Web.Base.ReportBasePage
     
    {
        public string SoftPlatformJson;
        public string ProjectJson;
        public string RestypeHtml;
        public string SoftHtml;
        public string PlatHtml;
        public string SoftAreaJson;

        public string AreaJson;
        public string AreaJsonForEnShortName;
        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<ProjectSource> AvailableProjectSources
        {
            get { return loginService.AvailableProjectSources.Where(a => a.ProjectSourceID >= 2).ToList(); }
        }

        /// <summary>
        /// 去掉没有下载的产品
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get
            {
                return loginService.AvailableSofts.Where(a => a.SoftType == SoftTypeOptions.InternalSoft ).ToList();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl);
            SoftPlatformJson = GetSoftPlatHtmlJson();
            ProjectJson = GetProjectJson();
            RestypeHtml = GetResTypeHtml(1);
            PlatHtml = GetPlatformHtml(true, 0, true);
            SoftHtml = GetSoftHtml();
            SoftAreaJson=GetSoftAreaJson();
            AreaJson = GetAreaJson();
            AreaJsonForEnShortName = GetAreaJsonForEnShortName(1);
            //CurStatDate_DownLogRank_New
            EndTime = DateTime.Now.Date.AddDays(-1);
            
            BeginTime = EndTime.AddDays(-15);
        }
    }
}