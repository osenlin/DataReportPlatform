using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Stat.Web.Reports.Services;
using Newtonsoft.Json;
using net91com.Core.Extensions;
using net91com.Core;
using net91com.Stat.Web.Base;
using net91com.Stat.Services.Entity;
//using BY.AccessControlCore;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports
{
    public partial class Default : ReportBasePage
    {
        /// 结束时间，前台绑定了的
        //最近30天的开始时间(用于前台显示)
        protected string DateTimeFor30Begin {get;set;}
        //最近30天的时间(用于前台显示)
        protected string DateTimeFor30 { get; set; }
        //时段分析的时间(用于前台显示)
        protected string DateTimeSpan {get;set;} 

        //这里有两个基准时间，按小时和按天基准是不一样的
        protected DateTime EndTimeForHour;
        /// 单个软件id
        public int SoftID{get;set;}

       
        //设置概况的tabstring
        public String TabStrGaiKuang { get; set; }

        //设置基础信息的tabString
        public String TabStrJcXx { get; set; }
        /// 用户选择平台 
        public int PlatID{get;set;}
        protected Sjqd_StatUsersService Ds = Sjqd_StatUsersService.GetInstance();
        
        /// <summary>
        /// 多个地方将要用到的数据
        /// </summary>
        List<SoftUser> _usersfor80Days;
        /// 转换为坐标点json字符的中间类

        public string Activityurl { get; set; }
        public string Newuserurl { get; set; }
        public string Activityparenturl { get; set; }
        public string Newuserparenturl { get; set; }
        

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            GetQueryString();
            BindData();
            

        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID > 0).ToList(); }
        }

        protected void GetQueryString()
        {

            
          
            HeadControl1.IsPlatSingle = true;
            HeadControl1.IsHasNoPlat = false;
            HeadControl1.IsSoftSingle = true;
 
            HeadControl1.HiddenTime = true;
            HeadControl1.MySupportSoft = AvailableSofts;
            //其他页面传递过来
            if ( HeadControl1.IsFirstLoad  && string.IsNullOrEmpty(Request["inputsoftselect"]))
            {
                 
                SoftID = CookieSoftid;
                PlatID = CookiePlatid;
                EndTimeForHour = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.SoftUserForHour, CacheTimeOption.TenMinutes);
                EndTime = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.UserUseNewActivity,CacheTimeOption.TenMinutes);
                BeginTime = EndTime.AddDays(-30);
          
               

            }
 
 
            //用户选择模式 
            else
            {

                if (!string.IsNullOrEmpty(Request["inputsoftselect"]) && !string.IsNullOrEmpty(Request["inputplatformselect"]))
                {
                    PlatID = Convert.ToInt32(Request["inputplatformselect"]);
                    SoftID = Convert.ToInt32(Request["inputsoftselect"]);
                }
                else
                {
                    PlatID = Convert.ToInt32(HeadControl1.PlatID);
                    SoftID = Convert.ToInt32(HeadControl1.SoftID);
                }
                EndTimeForHour = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.SoftUserForHour, CacheTimeOption.TenMinutes);
                EndTime = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.UserUseNewActivity,CacheTimeOption.TenMinutes);
                BeginTime = EndTime.AddDays(-30);
                SetRequestCookie(SoftID, PlatID);
            }

            HeadControl1.SoftID = SoftID.ToString();
            HeadControl1.PlatID = PlatID.ToString();
            //设置时段的时间和 最近30日的时间
            DateTimeFor30 = EndTime.Date.ToShortDateString();
            DateTimeSpan = EndTimeForHour.Date.ToShortDateString();
            DateTimeFor30Begin = BeginTime.ToShortDateString();


        }

        ///获取数据加上绑定数据
        protected void BindData()
        {
            //获取基础信息
            GetTableJcXx();
           
            GetTableGaiKuang();
            //设置跳转地址
            GetUrl();
        }

        private void GetUrl()
        {
            Right activeRight = loginService.FindRight("reports/ActivityUserReport.aspx");
            Right newRight = loginService.FindRight("reports/NewUserReport.aspx");
            if (activeRight != null)
            {
                Activityurl = activeRight.ID.ToString();
                Activityparenturl = activeRight.ParentID.ToString();
            }            
            if (newRight != null)
            {
                Newuserurl = newRight.ID.ToString();
                Newuserparenturl = newRight.ParentID.ToString();
            }
        }
       
        ///获取统计慨况
        public void GetTableGaiKuang()
        {
            DateTime hourendtime = EndTimeForHour.Date;
            //预计今日使用的数据,昨天之前的5天数据
            List<SoftUser> users = _usersfor80Days.Where(p => p.StatDate >= EndTime.AddDays(-5) && p.Period == (int)net91com.Stat.Core.PeriodOptions.Daily).ToList();

            if (users.Count != 0  )
            {
                StringBuilder  sb = new StringBuilder( "<table   class=\" tablesorter \"   cellspacing=\"1\">" );
                sb.Append(@" <thead><tr>
                     <th>指标</th>
                     <th>新增用户</th>     
                     <th>活跃用户</th>   
                     <th>新用户占比</th>
                     <th>日活跃度</th>
                     </tr></thead>");
                sb.Append("<tbody>");
                
                sb.Append("<tr class=\"tableover\">");
                int totalNumyes = users[0].NewNum + users[0].ActiveNum;
                sb.Append(string.Format(@"<td>昨日</td><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td> ",
                   Utility.SetNum(users[0].NewNum), Utility.SetNum(totalNumyes), (users[0].NewNum / (double)totalNumyes * 100).ToString("0.00") + "%", (totalNumyes / (double)users[0].TotalNum * 100).ToString("0.00") + "%"));
                sb.Append("</tr>");

                SoftUser forecast = GetForeCastSoftUser(users.ToList());
              
                sb.Append("<tr class=\"tableover\">");
                int totalNumforeLast = forecast.NewNum + forecast.ActiveNum;
                sb.Append(string.Format(@"<td>预计今日</td><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td> ",
                   Utility.SetNum(forecast.NewNum), Utility.SetNum( forecast.ActiveNum), (forecast.NewNum / (double)totalNumforeLast * 100).ToString("0.00") + "%", ((forecast.ActiveNum + forecast.NewNum) / (double)forecast.TotalNum * 100).ToString("0.00") + "%"));
                sb.Append("</tr>");

                //历史最高
                List<SoftUser> tempHigh = Sjqd_StatUsersService.GetInstance().GetMaxNumCache(EndTime.AddDays(-1000), EndTime, SoftID, PlatID, net91com.Stat.Core.PeriodOptions.Daily, CacheTimeOption.OneHour);
                
                sb.Append("<tr class=\"tableover\">");
                if (tempHigh.Count == 1)
                {
                    sb.Append(string.Format(@"<td>历史最高</td><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td> ",
                    Utility.SetNum( tempHigh[0].NewNum )+ "(" + tempHigh[0].StatDate.ToShortDateString() + ")",
                    Utility.SetNum(tempHigh[0].ActiveNum + tempHigh[0].NewNum) + "(" + tempHigh[0].StatDate.ToShortDateString() + ")", "--", "--"));
                    sb.Append("</tr>");
                }
                else if (tempHigh.Count == 2)
                {
                    sb.Append(string.Format(@"<td>历史最高</td><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td> ",
                    Utility.SetNum( tempHigh[0].NewNum) + "(" + tempHigh[0].StatDate.ToShortDateString() + ")",
                    Utility.SetNum(tempHigh[1].ActiveNum + tempHigh[1].NewNum) + "(" + tempHigh[1].StatDate.ToShortDateString() + ")", "--", "--"));
                    sb.Append("</tr>");

                }
                else
                {
                    sb.Append(string.Format(@"<td>历史最高</td><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td> ",
                   "--","--", "--", "--")); 
                    sb.Append("</tr>");
                }
               
                sb.Append("</tbody></table>");
                TabStrGaiKuang = sb.ToString();
            }

            _usersfor80Days.Clear();
            _usersfor80Days = null;
           
        }
        ///预估算法（慨况需要的）
        protected SoftUser GetForeCastSoftUser(List<SoftUser> softUser)
        {
            double frontNewGroupth = 0;
            double activityPercent = 0;
           
            foreach (var item in softUser)
            {
                //我的growth 加入了百分号
                frontNewGroupth += Convert.ToDouble(item.Growth.Substring(0,item.Growth.Length-1))/100;
                activityPercent += item.ActiveNum/(double)item.TotalNum;
            }
            frontNewGroupth = frontNewGroupth / softUser.Count;
            activityPercent = activityPercent / softUser.Count;
            SoftUser su = new SoftUser();
            su.NewNum =Convert.ToInt32( softUser[0].TotalNum * frontNewGroupth);
            su.TotalNum=  su.NewNum+softUser[0].TotalNum ;
            su.ActiveNum = Convert.ToInt32(activityPercent * su.TotalNum);
            su.ActivityPercent = activityPercent.ToString("0.00")+"%";
             
            return su;
        }

        //获取基础信息统计
        public void GetTableJcXx()
        {
            //已经按照时间倒叙了 取得范围是80天里面选
            _usersfor80Days = Ds.GetUsersWithNoPeriodCache(EndTime.AddDays(-80), EndTime, SoftID, PlatID, CacheTimeOption.HalfDay).OrderByDescending(p => p.StatDate).ToList();
            List<SoftUser> users = _usersfor80Days;
            if (users.Count != 0)
            {
                int totalNum = users.Max(p => p.TotalNum);
                //周用户数据
                SoftUser weekUser = users.Where(p => p.Period == (int)net91com.Stat.Core.PeriodOptions.Weekly).OrderByDescending(p => p.StatDate).FirstOrDefault();
                int weekActivity=0;
                string weekPercent = "--";
                if (weekUser != null)
                {
                    weekActivity = weekUser.ActiveNum;
                    weekPercent = weekUser.ActivityPercent;
                }
                SoftUser monthUser=users.Where(p=>p.Period==(int)net91com.Stat.Core.PeriodOptions.Monthly).OrderByDescending(p=>p.StatDate).FirstOrDefault();
                ///获取月留存数据
                var list = new RetainedUsersService(true)
                    .GetStatRetainedUsersCache(SoftID, PlatID, -1, net91com.Stat.Core.PeriodOptions.Monthly, EndTime.AddDays(-80), EndTime,CacheTimeOption.TenMinutes, ChannelTypeOptions.Category, loginService);
                Sjqd_StatChannelRetainedUsers lastretainuser =list.Count==0? null:list.OrderBy(p => p.OriginalDate).Last();
                //数据库还 没有值
                int monthActivity = 0;
                string monthPercent = "--";
                int monthnew = 0;
                
                if (monthUser != null)
                {
                     monthActivity = monthUser.ActiveNum;
                     monthPercent = monthUser.ActivityPercent;
                     monthnew = monthUser.NewNum;
                     
                }
                string percent = lastretainuser == null ? "--" : (lastretainuser.RetainedUserCount*100 / (decimal)lastretainuser.OriginalNewUserCount).ToString("0.00")+"%";
               
               
                StringBuilder sb = new StringBuilder("<table   class=\" tablesorter \"   cellspacing=\"1\">");
                sb.Append(@" <thead><tr>
                     <th>累计用户</th>
                     <th>周活跃用户</th>     
                     <th>周跃率</th>   
                     <th>月活跃用户</th>
                     <th>月活跃率</th>
                     <th>月新增用户</th>
                     <th>上月留存率</th>
                     </tr></thead>");
                sb.Append("<tbody>");
                sb.Append("<tr class=\"tableover\">");
                sb.AppendFormat(@"<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td>",
                    Utility.SetNum(totalNum),Utility.SetNum(weekActivity),weekPercent,Utility.SetNum(monthActivity),monthPercent,
                    monthnew, percent);
                  
                sb.Append("</tr>");
                sb.Append("</tbody></table>");
                TabStrJcXx = sb.ToString();

            }
        }
    }
}