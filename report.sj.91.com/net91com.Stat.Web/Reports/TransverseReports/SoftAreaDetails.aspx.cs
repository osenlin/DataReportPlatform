using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Reports.UserRights;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Web.Base;
using net91com.Core.Extensions;
using net91com.Core;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services;


namespace net91com.Stat.Web.Reports.TransverseReports
{
   
    public partial class SoftAreaDetails : System.Web.UI.Page
    {
        public List<Sjqd_StatUsersByArea> allSoftArea;
        public string TableString = string.Empty;
        public DateTime BeginTime;
        public DateTime EndTime;
        public int Softid;
        public int Platform;
        public int ChannelCate;
        public string ChannelIDStrs;
        public string AreaName;
        public int Period;
        protected void Page_Load(object sender, EventArgs e)
        {
            ChannelIDStrs = Request["channelids"];
            AreaName = Request["area"];
            List<int> channelIds = Request["channelids"].Split(new char[] {','},System.StringSplitOptions.RemoveEmptyEntries).Select(p=>Convert.ToInt32(p)).ToList();
            ChannelCate = Convert.ToInt32(Request["channelcate"]); 
            BeginTime = Convert.ToDateTime(Request["begintime"]);
            EndTime = Convert.ToDateTime(Request["endtime"]); 
            Softid = Convert.ToInt32(Request["softid"]);
            Platform = Convert.ToInt32(Request["platform"]);
            Period = Convert.ToInt32(Request["period"]);
            //这里周期不是每天的概念，具体要踩到那一天，所以两个参数都是endtime
            allSoftArea = new StatUsersByAreaService(true).GetSoftAreaTransverseWithChinaByCitys(
                    Softid, (MobileOption)Platform, (PeriodOptions)Period, EndTime, (ChannelTypeOptions)ChannelCate,
                    channelIds, AreaName).OrderByDescending(p=>p.UseCount).ToList();
            TableString = GetTableString();


        }
        /// <summary>
        /// 形成每一个tab
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tableindex"></param>
        /// <returns></returns>
        protected string GetTableString()
        {
            StringBuilder sb = new StringBuilder();
            //string tableName = softStr + "_" + platformStr + "_" + Period.GetDescription();
            sb = new StringBuilder("<table  class=\" tablesorter \""  + "cellspacing=\"1\">");
             sb.Append(@" <thead><tr><th>地区</th>
                <th>用户数</th>     
                <th>百分比</th> 
                <th></th>  
                </tr></thead>");
            decimal usercount = 0;
            int all = allSoftArea.Sum(p => p.UseCount);
            for (int i = 0; i < allSoftArea.Count; i++)
            {
                if (i < 100)
                {
                    sb.Append("<tr   class=\"tableover\"  >");
                    sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td>" +
                                     "<td><span class=\"find\" onclick=\"showLine({3},'{0}')\" id=\"lbl{3}\">{4}</span></td>" ,
                        (string.IsNullOrEmpty(allSoftArea[i].AreaName) ? "未知区域" : allSoftArea[i].AreaName), allSoftArea[i].UseCount,
                        Math.Round(Convert.ToDecimal(allSoftArea[i].UseCount) / all * 100, 2) + "%", i, string.IsNullOrEmpty(allSoftArea[i].AreaName)?"":"查看每天");
                    sb.Append("</tr>");
                    sb.AppendFormat("<tr class=\"divtr\" id=\"tr{0}\" style=\"display:none;height:400px;\"><td colspan=\"4\"><div id=\"div{0}\"> </div>" + "</td></tr>", i);
                }
                else
                {
                    usercount += allSoftArea[i].UseCount;
                }
            }
            if (usercount != 0)
            {
                sb.Append("<tr   class=\"tableover\"  >");
                sb.AppendFormat("<td style=\"width:35%\">{0}</td><td>{1}</td><td>{2}</td> <td></td> ", "其他", usercount, Math.Round((usercount / all * 100), 2) + "%");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
            return sb.ToString();


        }
    }
}