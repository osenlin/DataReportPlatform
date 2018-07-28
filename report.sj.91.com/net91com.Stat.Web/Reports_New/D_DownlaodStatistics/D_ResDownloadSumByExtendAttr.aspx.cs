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
    public partial class D_ResDownloadSumByExtendAttr : net91com.Stat.Web.Base.ReportBasePage
    {
        public string SoftPlatformJson;
        public string ProjectJson; 
        public string RestypeHtml; 
        public string SoftHtml;
        public string ExtendAttrHtml;
        public string PlatHtml;
        public string restypebyrequest;

        protected override List<Soft> AvailableSofts
        {
            get
            {
                if (Convert.ToInt32(Request["stataloneid"]) == 0)
                {
                    return loginService.AvailableSofts.Where(p => p.ID == 117030 || p.ID == 117142 || p.ID == 117143 || p.ID == 117144 || p.ID == 109361 || p.ID == 109361 || p.ID == 89 || p.ID == 119 || p.ID == 2).ToList();
                }
                return loginService.AvailableSofts;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight(HttpContext.Current.Request.Path, "?" + HttpContext.Current.Request.QueryString.ToString());
            int stataloneid = Convert.ToInt32(Request["stataloneid"]);
            restypebyrequest = Request["restype"];
            List<int> lstrestype = Array.ConvertAll(restypebyrequest.Split('_'), s => int.Parse(s)).ToList();
            string resattr = Request["resattribute"];

            List<string> lst=resattr.Split('_').ToList();
            ExtendAttrHtml = GetExtendResAttrHtml(lstrestype[0]);

            SoftHtml = GetSoftHtml(AvailableSofts, stataloneid);

            ProjectJson = GetProjectJson();
            SoftPlatformJson = GetSoftPlatHtmlJson();
            RestypeHtml = GetResTypeHtml(lstrestype);
            PlatHtml = GetPlatformHtml(false, -1, false);
            EndTime = DateTime.Now.Date.AddDays(-1);
            BeginTime = EndTime.AddDays(-30);
             
        }
        
    }
}