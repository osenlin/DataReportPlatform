using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.R_UserAnalysis 
{
    public partial class ISPUsersAnalysis : net91com.Stat.Web.Base.ReportBasePage
    {
        protected string softplatjson { get; set; }
        protected string selectsofthtml { get; set; }
        public string isphtml = "<option value='所有运营商' selected='selected'>所有运营商</option>";
        public string ipsJson { get; set; }
        public string netModeJson { get; set; }
        public string netModeHtml = "<option value='所有网络' selected='selected' >所有网络</option>";
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(p => p.ID > 0).ToList(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();
            softplatjson = GetSoftPlatHtml();
            for (int i = 0; i < AvailableSofts.Count; i++)
            {
                selectsofthtml += string.Format("<option value='{0}' >{1}</option>", AvailableSofts[i].ID, AvailableSofts[i].Name);
            }
            var ipsList = Sjqd_ISPService.Instance.GetSjqd_ISPCache();

            for (int i = 0; i < ipsList.Count; i++)
            {
                isphtml += string.Format("<option value='{0}'  >{1}</option>", ipsList[i].Name, ipsList[i].Name);
            }
            ipsJson = GetIPSJsonHtml(ipsList);

            List<string> netModeList = Sjqd_NetModesService.Instance.GetSjqd_NetModesCache().Select(p=>p.E_Name).Distinct().ToList();
            for (int i = 0; i < netModeList.Count(); i++)
            {
                netModeHtml += string.Format("<option value='{0}'  >{1}</option>", netModeList[i], netModeList[i]);
            }

            netModeJson=GetNetModesJson(netModeList);
        }
        protected string GetSoftPlatHtml()
        {
            StringBuilder sb = new StringBuilder("{"); 
            foreach (Soft item in AvailableSofts)
            {
                sb.AppendFormat("'{0}':[0,{1}],", item.ID,
                       string.Join(",", item.Platforms.Select(p => ((int)p).ToString()).ToArray()));
            }
            return sb.ToString().TrimEnd(',') + "}";
        }

        protected string GetIPSJsonHtml(List<Sjqd_ISP> list)
        {
            StringBuilder sb = new StringBuilder("{ '0':'所有运营商',");
            foreach (Sjqd_ISP item in list)
            {
                sb.AppendFormat("'{0}':'{1}',", item.Name,item.Name);
            }
            return sb.ToString().TrimEnd(',') + "}";
        }
        /// <summary>
        /// 获取网络类型的json串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected string GetNetModesJson(List<string>  list)
        {
            StringBuilder sb = new StringBuilder("{ '-1':'所有网络',");
            foreach (string item in list)
            {
                sb.AppendFormat("'{0}':'{1}',", item, item);
            }
            return sb.ToString().TrimEnd(',') + "}";
        }
    }
}