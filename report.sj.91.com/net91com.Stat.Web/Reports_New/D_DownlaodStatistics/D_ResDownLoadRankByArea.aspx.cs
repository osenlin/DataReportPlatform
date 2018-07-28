using System;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Services.CommonServices.Tool;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadRankByArea : net91com.Stat.Web.Base.ReportBasePage
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
            SoftPlatformJson = "{'-2':[1,7,4,9],'-46':[4,9] }";
            
            RestypeHtml = GetResTypeHtml();
            PlatHtml = GetPlatformHtml(true, 0, true);

            StringBuilder sb = new StringBuilder();
            var softs = AvailableSofts.Where(a => a.SoftType == SoftTypeOptions.InternalSoft && (a.ID != 85 && a.ID != 58 && a.ID != 46)).ToList();
            if (softs.Count > 0)
            {

                sb.AppendFormat("<option value='{0}' {2} >{1}</option>", -2, "国内产品", -2 == CookieSoftid ? "selected='selected'" : "");
            }

            var soft2 = AvailableSofts.Where(a => a.SoftType == SoftTypeOptions.InternalSoft && a.ID == 85 || a.ID == 58 || a.ID==46).ToList();
            if (soft2.Count > 0)
            {
               
                sb.AppendFormat("<option value='{0}' {2} >{1}</option>", -46, "海外产品", -46 == CookieSoftid ? "selected='selected'" : "");
            }
            SoftHtml = sb.ToString();
            if (softs.Count==0 && soft2.Count==0)
            {
                throw new NotRightException();
            }

            SoftAreaJson = GetSoftAreaJson();
            AreaJson = GetAreaJson();

            // key = string.Format("N_StatDownRank_{0}_StatDate{1}", period, ProjectSourceType == 2 ? "_46" : "");
            BeginTime = ToolService.Instance.GetEltStates("N_StatDownRank_Daily_StatDate_46");
            DateTime BegintTimeChina = ToolService.Instance.GetEltStates("N_StatDownRank_Daily_StatDate");

            if (BeginTime.Year != 1 || BegintTimeChina.Year != 1)
            {
                BeginTime = (BeginTime < BegintTimeChina ? BegintTimeChina : BeginTime).AddDays(-1);
            }
            else
            {
                BeginTime=new DateTime().AddYears(-20).AddDays(-1);
            }
        }
    }
}