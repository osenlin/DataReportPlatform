﻿using System;
using System.Web.UI;

namespace net91com.Stat.Web.Reports_New.D_DownlaodStatistics
{
    public partial class D_ResDownLoadPositionDistributionDetail : Page
    {

        protected int softid;
        protected int platform;
        protected int restype;
        protected string version;
        protected int period;
        protected int downtype;
        protected int projectsource;
        protected string positionid;
        protected DateTime BeginTime;
        protected DateTime EndTime;
        protected int isTagClassDetail;
        protected string pagename;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //loginService.HaveUrlRight(HttpContext.Current.Request.RawUrl);


            BeginTime = Convert.ToDateTime(Request["begintime"]);
            EndTime = Convert.ToDateTime(Request["endtime"]);
            version = (Request["version"]);
            restype = Convert.ToInt32(Request["restype"]);
            softid = Convert.ToInt32(Request["softs"]);
            platform = Convert.ToInt32(Request["plat"]);
            period = Convert.ToInt32(Request["period"]);
            downtype = Convert.ToInt32(Request["downtype"]);
            projectsource = Convert.ToInt32(Request["projectsource"]);
            positionid = Request["positionid"];
            isTagClassDetail= Convert.ToInt32(Request["isTagClassDetail"]);
            pagename = Request["pagename"];

            myhidden.Value = BeginTime.ToString("yyyy-MM-dd") + "_" + EndTime.ToString("yyyy-MM-dd") + "_" + version
                             + "_" + restype + "_" + softid + "_" + platform
                             + "_" + -20 + "_" + period + "_" + downtype
                             + "_" + projectsource + "_" + positionid + "_" + isTagClassDetail + "_" 
                             + pagename+"_"+Request["isdiffpagetype"]+"_"+Request["stattype"]+"_"+Request["areaid"];
        }
    }
}