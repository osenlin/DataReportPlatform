using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports_New.R_UserAnalysis
{
    public partial class ChannelCustomRetainUsers_Out : net91com.Stat.Web.Base.ReportBasePage
    {
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
            softplatjson = GetSoftPlatHtml();
            for (int i = 0; i < AvailableSofts.Count; i++)
            {
                selectsofthtml += string.Format("<option value='{0}' >{1}</option>", AvailableSofts[i].ID, AvailableSofts[i].Name);
            }
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