using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Services.sjqd;
using net91com.Reports.Services.CommonServices.SjqdUserStat;

namespace net91com.Stat.Web.Reports_New.R_UserAnalysis
{
    public partial class RetainUsersForExternalChannels : System.Web.UI.Page
    {
        public string CustomerName=string.Empty;
        public string PlatName = string.Empty;
        public string P = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                P = Request["p"];
                var p_Result = net91com.Common.CryptoHelper.DES_Decrypt(Request["p"], "$retain^");
                string[] strs = p_Result.Split(new char[] { '&' }, StringSplitOptions.None);
                int channelId = Convert.ToInt32(strs[0]);
                int platform = Convert.ToInt32(strs[1]);
                var node = new CfgChannelService().GetChannelCustomer(channelId);
                CustomerName = node.Name;
                PlatName = GetPlatName(platform);
                //渠道商留存要开放出去,并且对外可看
                if (!(node.ReportType == 1 && node.ShowType == 2))
                {
                    Response.End();
                }
            }
            catch (Exception)
            {
                    
                Response.Write("无权访问");
                Response.End();
            }
            

        }

        protected string GetPlatName(int plat)
        {
            switch (plat)
            {
                case 1:
                    return "iPhone";
                case 4:
                    return "Android";
                case 7:
                    return "iPad";
                case 9:
                    return "AndroidPad";
                case 255:
                    return "PC";
                default:
                    return "未知";

            }
        }
    }
}