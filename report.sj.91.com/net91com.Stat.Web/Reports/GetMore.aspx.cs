using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Web.Reports
{
    public partial class GetMore : System.Web.UI.Page
    {
        protected string MyPeriod;
        protected string url;
        protected string softStr;
        protected string platformStr;
        protected DateTime begintime;
        protected DateTime endtime;
        protected string Channelid;
        protected string Channeltype;
        protected string UserCate;
        protected string Area;
        protected void Page_Load(object sender, EventArgs e)
        {


            endtime =Request["endtime"]!=null? Convert.ToDateTime(Request["endtime"]):DateTime.MinValue;
            begintime = Request["begintime"] != null ? Convert.ToDateTime(Request["begintime"]) : DateTime.MinValue;
            softStr = Request["soft"];
            platformStr = Request["plat"];
            if (Request["reporttype"] == "1")
            {
                url = "ActivityUserReport.aspx";
            }
            else if (Request["reporttype"] == ((int)ReportType.Default).ToString())
            {
                url = "Default.aspx";
            }
            else if (Request["reporttype"] ==((int)ReportType.SoftVersionSjqd).ToString() )
            {
                Channelid = Request["channelid"];
                Channeltype = Request["channeltype"];
                if (!string.IsNullOrEmpty(Request["cate"]))
                    UserCate = Request["cate"];
                url = "SoftVersionSjqd.aspx";
            }
            else if (Request["reporttype"] == ((int)ReportType.AllCustomStat).ToString())
            {
                Channelid = Request["channelid"];
                Channeltype = Request["channeltype"];
                url = "sjqd/AllCustomStat.aspx";
            }
            else if (Request["reporttype"] == ((int)ReportType.StatDownLoadSpeed).ToString())
            {
                platformStr = Request["areaname"];
                UserCate = "1";
                url = "DownloadReports/ResDownSpeedByResType.aspx";
            }
            ///下面这个else 不要动
            else if (Request["reporttype"] == "2")
            {
                url = "NewUserReport.aspx";
            }
            else if (Request["reporttype"] == ((int)ReportType.StatUsersByAreaByChannel).ToString())
            {
                Area = Request["area"];
                url = "StatUsersByArea.aspx";
            }
            else
            {
                url = "NoRight.aspx";
            }
            
            ///时段
            if (Request["smalltype"] == "1")
            {
                //7为时段
                MyPeriod="每小时";
            }
            //天
            else
            { 
                //1为天
                MyPeriod="一天";
            }

         
        }
    }
}