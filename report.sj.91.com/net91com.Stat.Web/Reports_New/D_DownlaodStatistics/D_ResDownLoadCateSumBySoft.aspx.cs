using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.ViewEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.Tool;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadCateSumBySoft : net91com.Stat.Web.Base.ReportBasePage
    {
        public string SoftPlatformJson;
        public string ProjectJson;
        public string RestypeHtml;
        public string SoftHtml;
        public string PlatHtml;
        public string SoftAreaJson;
        public string ParentCategoryHtml;
        public string SubCategoryHtml;

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl);
            //SoftPlatformJson = GetSoftPlatHtmlJson();
            SoftPlatformJson = "{'-2':[1,7,4,9],'-46':[4,9] }";
            RestypeHtml = GetResTypeHtml();
            PlatHtml = GetPlatformHtml(true, -1, false);
            SoftAreaJson = GetSoftAreaJson();

            StringBuilder sb = new StringBuilder();
            var softs = AvailableSofts.Where(a => a.SoftType == SoftTypeOptions.InternalSoft && (a.ID != 85 && a.ID != 58 && a.ID != 46)).ToList();
            if (softs.Count > 0)
            {
                sb.AppendFormat("<option value='{0}' {2} >{1}</option>", -2, "国内产品", -2 == CookieSoftid ? "selected='selected'" : "");
            }

            var soft2 = AvailableSofts.Where(a => a.SoftType == SoftTypeOptions.InternalSoft && a.ID == 85 || a.ID == 58 || a.ID == 46).ToList();
            if (soft2.Count > 0)
            { 
                sb.AppendFormat("<option value='{0}' {2} >{1}</option>", -46, "海外产品", -46 == CookieSoftid ? "selected='selected'" : "");
            }
            SoftHtml = sb.ToString();
            if (softs.Count == 0 && soft2.Count == 0)
            {
                throw new NotRightException();
            }
            sb = new StringBuilder();
            List<KeyValueModel> list = B_BaseToolService.Instance.GetResCateCache(2)
                                 .Where(p =>  p.PCID == 0 && p.ResType==1)
                                 .Select(p => new KeyValueModel { ID = p.CID.ToString(), Value = p.CName })
                                 .ToList();
            sb.AppendFormat("<option  value='{0}' selected='selected'>{1}</option>", 0, "不区分大分类");
            for (int i = 0; i < list.Count; i++)
            {
                sb.AppendFormat("<option  value='{0}'>{1}</option>", list[i].ID, list[i].Value);
            }
            ParentCategoryHtml = sb.ToString();

            EndTime = DateTime.Now.Date.AddDays(-1);
     
            if (EndTime.Year != 1 )
            {
                BeginTime = EndTime.AddMonths(-1);
            }


        }
    }
}