using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.Services.CommonServices.Tool;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadSumByAreaDistributionl : net91com.Stat.Web.Base.ReportBasePage
     
    {
        public string SoftPlatformJson;
        public string ProjectJson;
        public string RestypeHtml;
        public string SoftHtml;
        public string PlatHtml;
        public string SoftAreaJson;

        public string AreaJson;
        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<ProjectSource> AvailableProjectSources
        {
            get { return loginService.AvailableProjectSources.Where(a => a.ProjectSourceID >= 100).ToList(); }
        }

        /// <summary>
        /// 去掉没有下载的产品
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get
            {
                //这边做了一个临时处理，只有显示海外的数据 &&(a.ID==46 || a.ID==58 || a.ID==85)
                return loginService.AvailableSofts.Where(a => a.ID > 0 && a.SoftType == SoftTypeOptions.InternalSoft ).ToList();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl);
            SoftPlatformJson = GetSoftPlatHtmlJson();
            ProjectJson = GetProjectJson();
            RestypeHtml = GetResTypeHtml(1);
            PlatHtml = GetPlatformHtml(true, 0, false);
            SoftHtml = GetSoftHtml();
            SoftAreaJson=GetSoftAreaJson();
            AreaJson = GetAreaJson();
            //CurStatDate_DownLogRank_New
            EndTime = DateTime.Now.Date.AddDays(-1);
            
            BeginTime = EndTime.AddDays(-30);
        }
    }
}