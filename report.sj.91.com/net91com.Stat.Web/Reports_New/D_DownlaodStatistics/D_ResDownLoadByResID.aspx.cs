using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Reports.Services;
using net91com.Core.Extensions;

using Res91com.ResourceDataAccess;
using net91com.Core;
using net91com.Reports.UserRights;


namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadByResID : net91com.Stat.Web.Base.ReportBasePage
    {
        public string plathtml;
        public string projecthtml;
        public string restypehtml;
        public string projectdichtml;
        public string SoftHtml;
        public string SoftPlatformJson;
        public string ProjectJson;
        public string SoftAreaJson;
        public string AreaJson;

        protected override List<Soft> AvailableSofts
        {
            get
            {
                //return loginService.AvailableSofts.Where(p=>p.ID==46 ||p.ID==85||p.ID==58).ToList();
                return loginService.AvailableSofts.Where(a => a.ID > 0 && a.SoftType == SoftTypeOptions.InternalSoft).ToList();
            }
        }

        
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();
            if (AvailableProjectSources.Count == 0)
                throw new NotRightException();
            plathtml = GetPlatformHtml(true);
            SoftHtml = GetSoftHtml();
            ProjectJson = GetProjectJson();
            SoftPlatformJson = GetSoftPlatHtmlJson();
            SoftAreaJson = GetSoftAreaJson();
            AreaJson = GetAreaJsonForSpecial(1);

            restypehtml = GetResTypeHtml();
        }
    }
}