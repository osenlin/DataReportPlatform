using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.R_UserAnalysis
{
    public partial class ChannelCustomUsers : net91com.Stat.Web.Base.ReportBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            //权限验证
            loginService.HaveUrlRight();

            base.OnInit(e);
        }
        protected string softStr { get; set; }
        protected string softplatjson { get; set; }
        protected string channelCumstomStr { get; set; }
        protected string selectsofthtml { get; set; }
       
        protected int channelid { get; set; }
        /// 开始时间,前台也绑定了
        protected DateTime begintime;
        /// 结束时间，前台绑定了的
        protected DateTime endtime;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            //Dictionary<int, List<ChannelCustomers>> dic = Sjqd_StatUsersByChannelsService.GetInstance().GetChannelCustomsTable(CurrentUser);
            //softplatjson = GetSoftPlatHtml(CanViewSofts,dic);
            //if (dic.Keys.Count == 0&&CurrentUser.AccountType!=2)
            //    Response.Redirect("/Reports/NoRight.aspx");

            softplatjson = GetSoftPlatHtml();
            for (int i = 0; i < AvailableSofts.Count; i++)
            {
                selectsofthtml += string.Format("<option value='{0}' >{1}</option>", AvailableSofts[i].ID, AvailableSofts[i].Name);
            }
            //Dictionary<int, List<ChannelCustomers>> dic = Sjqd_StatUsersByChannelsService.GetInstance().GetChannelCustomsTable(CurrentUser.ID);
            //channelCumstomStr = GetChannelCustom(dic);;

            Dictionary<int, List<Channel>> channels = loginService.GetAvailableChannelsWithoutSubChannels();
            StringBuilder sb = new StringBuilder("{");
            foreach (var item in channels.Keys)
            {
                StringBuilder sbson = new StringBuilder("[");
                for (int i = 0; i < channels[item].Count; i++)
                {
                    sbson.AppendFormat("{{ 'id': '{0}','name':'{1}' }},", channels[item][i].ID, channels[item][i].Name);
                }
                sb.AppendFormat("'{0}':{1},", item, sbson.ToString().TrimEnd(',') + "]");
            }
            channelCumstomStr = sb.ToString().TrimEnd(',') + "}"; 
        }

        //protected string GetChannelCustom(Dictionary<int, List<ChannelCustomers>> dic)
        //{
        //    StringBuilder sb = new StringBuilder("{");
        //    foreach (var item in dic.Keys)
        //    {
        //        StringBuilder sbson = new StringBuilder("[");
        //        for (int i = 0; i < dic[item].Count; i++)
        //        { 
        //            sbson.AppendFormat("{{ 'id': '{0}','name':'{1}' }},", dic[item][i].ID, dic[item][i].Name);
        //        }
        //        sb.AppendFormat("'{0}':{1},", item, sbson.ToString().TrimEnd(',') + "]");
        //    }
        //    return sb.ToString().TrimEnd(',') + "}";

        //}

        protected string GetSoftPlatHtml()
        {
            StringBuilder sb = new StringBuilder("{");
            
            foreach (Soft item in AvailableSofts)
            {
                sb.AppendFormat("'{0}':[{1}],", item.ID,
                       string.Join(",", item.Platforms.Select(p => ((int)p).ToString()).ToArray()));
            }
            return sb.ToString().TrimEnd(',') + "}";
        }
    }
}