using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Reports.Services.CommonServices.Tool;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadRankBySoft : net91com.Stat.Web.Base.ReportBasePage
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
                //这边做了一个临时处理，只有显示海外的数据 && (a.ID == 46 || a.ID == 58 || a.ID == 85)
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