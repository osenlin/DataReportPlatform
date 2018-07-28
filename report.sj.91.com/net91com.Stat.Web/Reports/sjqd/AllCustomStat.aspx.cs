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
using net91com.Stat.Services.Entity;
using net91com.Stat.Services;
using net91com.Core;
using net91com.Stat.Web.Reports.sjqd;
using net91com.Stat.Services.sjqd;
using System.Data;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Common;

using net91com.Reports.UserRights;
using net91com.Stat.Web.Base;
using net91com.Reports.Services.CommonServices.SjqdUserStat;

namespace net91com.Stat.Web.Reports.sjqd
{
    public partial class AllCustomStat : ReportBasePage 
    {
        /// 开始时间,前台也绑定了
        protected DateTime begintime;
        /// 结束时间，前台绑定了的
        protected DateTime endtime;
        //这个是平台字符串，前台绑定了的，格式例如 iphone,ipad,wm
        
      
        /// <summary>
        /// htmltable 字符串
        /// </summary>
        public string tableStr { get; set; }

        public List<MobileOption> MySupportPlat { get; set; }
        public List<Soft> mySupportSoft;
        public string channelName;
        public int channelId
        {
            get;
            set;
        }

        public int customType
        {
            get;
            set;
        }
        public  List<SoftUser> listAll =new   List<SoftUser> ();

        public Dictionary<string, int> tableColumn = new Dictionary<string, int>();
        /// 软件列表
        public int softsid;
        //数据库交互对象
        protected UtilityService ds = UtilityService.GetInstance();
        /// 用户选择平台 
        protected int platformsid;
        ///x轴上控制系数 
        public int MaxNumCoef = -1;
        ///x轴上坐标(即时间)超过多少个开始利用控制系数
        public const int MaxNum = 20;
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight("Reports/SoftVersionSjqd.aspx");
            
            //设置周期及默认周期
            PeriodSelector1.AddPeriods(
                    new net91com.Stat.Core.PeriodOptions[] {
                            net91com.Stat.Core.PeriodOptions.NaturalMonth,
                            net91com.Stat.Core.PeriodOptions.Weekly,
                            net91com.Stat.Core.PeriodOptions.Daily },
                            net91com.Stat.Core.PeriodOptions.Daily);
            Period = PeriodSelector1.SelectedPeriod;
            //外部传递过来
            if (!string.IsNullOrEmpty(Request["Channelid"]) && !string.IsNullOrEmpty(Request["Channeltype"]))
            {
                ChannelID.Value = Request["Channelid"] ;
                ChannelType.Value = Request["Channeltype"] ;
            }

            channelId = Convert.ToInt32(ChannelID.Value);
            customType = Convert.ToInt32(ChannelType.Value);
            //所有产品
            mySupportSoft = AvailableSofts;
          
            if (customType == (int)ChannelTypeOptions.Category)
            {
                var node = new CfgChannelService().GetChannelCategory(channelId);
                softsid = node.SoftID;
                Name.InnerHtml = node.Name;
            }
            else
            {
                var node = new CfgChannelService().GetChannelCustomer(channelId);
                softsid = node.SoftID;
                Name.InnerHtml = node.Name;
            }            
            //没有权限
            var softinfo = mySupportSoft.Find(p => p.ID == softsid);
            if (softinfo == null)
            {
                RedirectNoright();
            }
            MySupportPlat = softinfo.Platforms;

            if (fromtime.Value == "" || totime.Value == "")
            {
                endtime = ds.GetMaxTimeCache( net91com.Stat.Core.PeriodOptions.Daily, ReportType.UserUseNewActivity,CacheTimeOption.TenMinutes);
                endtime = endtime.Date;
                begintime = endtime.AddDays(-30);
                fromtime.Value = begintime.ToString("yyyy-MM-dd");
                totime.Value = endtime.ToString("yyyy-MM-dd");
            }
            else
            {
                begintime = Convert.ToDateTime(fromtime.Value);
                endtime = Convert.ToDateTime(totime.Value);
                
            }
            

            //绑定数据
            BindData();
        }

        private void BindData()
        {
            listAll = Sjqd_StatUsersByChannelsService.GetInstance().GetAllSonChannelCustomUserCache((ChannelTypeOptions)customType, inputplatformselect.Value, channelId, begintime, endtime, Period, CacheTimeOption.TenMinutes);
            List<DateTime> times = listAll.Select(p => p.StatDate).Distinct().OrderByDescending(p=>p).ToList();
            var channels = listAll.Select(p =>new ChannelCustomers {Name= p.ChannelName,ID=p.ChannelID}).Distinct(new channelComparer()).ToList();
            for (int i = 0; i < channels.Count; i++)
            {
                if (!tableColumn.Keys.Contains(channels[i].Name)) 
                     tableColumn.Add(channels[i].Name,channels[i].ID);
                else
                     tableColumn.Add(channels[i].Name+"["+i+"]", channels[i].ID);
            }
            DataTable dt = MakeChannelIncDataTable();
            for (int i = 0; i < times.Count; i++)
            {
                DataRow dr= dt.NewRow();
                dr["日期"] = times[i].ToString("yyyy-MM-dd");
                foreach (var item in tableColumn.Keys)
                {
                   var temp=listAll.Find(p => p.ChannelID == tableColumn[item] && p.StatDate == times[i]);
                   if(temp!=null)
                    dr[item] = temp.NewNum;
                }
                dt.Rows.Add(dr);
            }
            tableStr= TableHelper.GetTableStrForAllCustomers(dt);
        }

        

        public DataTable MakeChannelIncDataTable()
        {
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn("日期", typeof(string)));
            foreach (var item in  tableColumn.Keys)
            {
                DataColumn dc = new DataColumn(item, typeof(string));
                dc.DefaultValue = "0";
                result.Columns.Add(dc);
            }

            return result;
        }
        public  void RedirectNoright()
        {
            Response.Redirect("../NoRight.aspx");
        }


    }

    public class channelComparer : IEqualityComparer<ChannelCustomers>
    {
        public bool Equals(ChannelCustomers x, ChannelCustomers y)
        {
            if (x == null)
                return y == null;
            return x.ID == y.ID;
        }

        public int GetHashCode(ChannelCustomers obj)
        {
            if (obj == null)
                return 0;
            return obj.ID.GetHashCode();
        }

    }

}