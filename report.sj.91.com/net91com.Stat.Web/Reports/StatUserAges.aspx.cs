using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Services.Entity;
using net91com.Stat.Web.Reports.Services;
using System.Text;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Core.Extensions;
using net91com.Core;
using net91com.Stat.Services;

using net91com.Reports.UserRights;
using net91com.Stat.Web.Base;

namespace net91com.Stat.Web.Reports
{
    public partial class StatUserAges : ReportBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            //权限验证
            loginService.HaveUrlRight();

            base.OnInit(e);
        }
        /// 开始时间,前台也绑定了
        protected DateTime begintime;
        /// 结束时间，前台绑定了的
        protected DateTime endtime;
        //标题
        protected string reportTitle = "";
        //曲线值
        protected string AxisJsonStr = "{}";
        protected string SeriesJsonStr = "[]";

        //列表
        public List<Sjqd_StatUsersByAge> listAll = new List<Sjqd_StatUsersByAge>();
        /// 平台id  
        protected int PlatformID { get; set; }
        /// 软件列表
        public int SoftID { get; set; } 
        protected void Page_Load(object sender, EventArgs e)
        {
            HeadControl1.MySupportSoft = AvailableSofts;
            HeadControl1.IsHasNoPlat = false;
            HeadControl1.IsSoftSingle = true;
            HeadControl1.IsPlatSingle = true;
            
            GetQueryString();
            BindData();
        }
        protected void GetQueryString()
        {
            reportTitle = "用龄分布(针对选择时间范围内的活跃用户进行统计)";
            AxisJsonStr = "{}";
            SeriesJsonStr = "[]";
            if (HeadControl1.IsFirstLoad)
            {

                SoftID = CookieSoftid;
                PlatformID = CookiePlatid;
                endtime = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.LatestOneWeek, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes);
                begintime = endtime.AddDays(-30);
                HeadControl1.BeginTime = begintime;
                HeadControl1.EndTime = endtime;
                HeadControl1.SoftID = SoftID.ToString();
                HeadControl1.PlatID = PlatformID.ToString();



            }
            ///用户选择模式
            else
            {
                begintime = HeadControl1.BeginTime;
                endtime = HeadControl1.EndTime;
                PlatformID = Convert.ToInt32(HeadControl1.PlatID);
                SoftID = Convert.ToInt32(HeadControl1.SoftID);
                SetRequestCookie(SoftID, PlatformID);


            }

        }
        protected void BindData()
        {

            listAll = StatUsersByAgeService.GetInstance().GetGetUsersAgesByCache(begintime, endtime, SoftID, PlatformID);
            if (listAll.Count != 0)
            {
                GetDataJsonList(listAll);
                ///设置x轴上的日期
                SetxAxisJson();
            }
        }
        protected void SetxAxisJson()
        {
            string str = "";
            foreach (StatUsersAgesEnum item in Enum.GetValues(typeof(StatUsersAgesEnum)))
            {
                str += "\"" + item.GetDescription() + "\",";
            }
            str = str.TrimEnd(',');

            AxisJsonStr = "{" + string.Format("categories:[{0}] ", str);

            AxisJsonStr += " }";

        }
        /// <summary>
        /// json转换函数
        /// </summary>
        /// <param name="temp"></param>
        protected void GetDataJsonList(List<Sjqd_StatUsersByAge> temp)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("[{ name:'人数比例',data:[");
            foreach (var item in temp)
            {
                sb.Append(Math.Round(item.Percent * 100, 2) + " ,");
            }
            string result = sb.ToString();
            result = result.TrimEnd(',');
            result += "]}]";
            SeriesJsonStr = result;

        }

    }
}