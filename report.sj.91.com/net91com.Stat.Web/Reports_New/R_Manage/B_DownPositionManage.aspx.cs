using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Admin
{
    public partial class B_DownPositionManage : net91com.Stat.Web.Base.ReportBasePage
    {
        public string projecthtml;
        public string restypehtml;
        public string projectdichtml;
        public string projecthtml_en;
        public string projectdichtml_en;


        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();
            if (AvailableProjectSources.Count == 0)
                throw new NotRightException();

            projecthtml = GetProjectSourceHtml(false, ref projectdichtml);
            projecthtml_en = GetProjectSourceHtml(false, ref projectdichtml_en, ProjectSourceTypeOptions.Oversea);
            restypehtml = GetResTypeHtml();
        }
    }
}