using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.R_Other
{
    public partial class LinkTagManager : net91com.Stat.Web.Base.ReportBasePage
    {
        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID > 0).ToList(); }
        }

        public string SoftPlatformJson =string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            StringBuilder sb = new StringBuilder("{");
            foreach (Soft soft in AvailableSofts)
            {
                ListItem item = new ListItem(soft.Name, soft.ID.ToString());
                if (soft.ID == CookieSoftid)
                {
                    item.Selected = true;
                }
                sltSofts.Items.Add(item);

                sb.AppendFormat("'{0}':[{1}],", soft.ID,
                       string.Join(",", soft.Platforms.Select(p => ((int)p).ToString()).ToArray()));
            }
            softPlatJson.Value = sb.ToString().TrimEnd(',') + "}";
        }
    }
}