using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Stat.Web.Reports.Services;
using Newtonsoft.Json;
using net91com.Core.Extensions;
using System.Text;
using net91com.Stat.Web.Base;
using net91com.Core;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services;
using net91com.Reports.UserRights;
using net91com.Reports.Services.CommonServices.SjqdUserStat;

namespace net91com.Stat.Web.Reports
{
    /// <summary>
    /// Summary description for AllHandler
    /// </summary>
    public class AllHandler :HandlerBase
    {
        public string AxisJsonStr1 { get; set; }
        public string SeriesJsonStr1 { get; set; }
        public DateTime endtime { get; set; }
        public override void ProcessRequest(HttpContext context)
        {

            switch (context.Request["action"])
            {
                case "downloadexcelnewuser":
                    GetExcelNewUser();
                    break;
                case "downloadexcelactivity":
                    GetExcelActivity();
                    break;
                case "GetActivityfor30":
                    GetActivityFor30();
                    break;

                case "downloadforoutcustomen":
                    GetDownLoadForCustomEn();
                    break;
                case "getzjs":
                    GetDownZjs();
                    break;
                default:
                    break;
            }
        }

        private void GetDownLoadForCustomEn()
        {
            DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["inputtimestart"]);
            DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["inputtimeend"]);
            int excelsoft = Convert.ToInt32(HttpContext.Current.Request["excelsoft"]);
            int plat = Convert.ToInt32(HttpContext.Current.Request["excelplatform"]);
            string selectCountryId = ThisRequest["mycountry"] ?? string.Empty;
            int channelid = Convert.ToInt32(net91com.Common.CryptoHelper.DES_Decrypt(HttpContext.Current.Request["p"], "ndwebweb"));

            StatUsersService suService = new StatUsersService();
            List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> userList = null;
            if (selectCountryId == string.Empty)
            {
                userList = suService.GetStatUsersByChannel(excelsoft, plat, ChannelTypeOptions.Customer, channelid, (int)net91com.Stat.Core.PeriodOptions.Daily, begintime, endtime, false, true);
            }
            else
            {
                userList = suService.GetStatUsersByArea(excelsoft, plat, ChannelTypeOptions.Customer, channelid, selectCountryId, (int)net91com.Stat.Core.PeriodOptions.Daily, begintime, endtime, false, true);
            }
            List<SoftUser> users = new List<SoftUser>();
            foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in userList)
            {
                SoftUser softuser = new SoftUser();
                softuser.StatDate = u.StatDate;
                softuser.SoftId = excelsoft;
                softuser.Platform = plat;
                softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                softuser.Hour = u.StatHour;
                softuser.NewNum = u.NewUserCount;
                softuser.UseNum = u.ActiveUserCount;
                users.Add(softuser);
            }
            users = users.OrderBy(p => p.StatDate).ToList();

            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            string filename = "渠道统计.xls";
            //真正算出的时间
            DateTime dtrighttime = UtilityService.GetInstance().GetMaxChannelUserTimeCache(excelsoft, MobileOption.None, net91com.Stat.Core.PeriodOptions.Daily,CacheTimeOption.TenMinutes);
            //下午两点才能开放出去
            if (endtime >= dtrighttime && (DateTime.Now < dtrighttime.AddHours(38)))
            {
                endtime = dtrighttime.AddDays(-1);
            }
            AddHead(filename);
            //文件标题+内容
            string colHeaders = "", ls_item = "";
            //列数
            int cl = 2;
            colHeaders += "日期" + "\t" + "新增用户" + "\t\n";
            resp.Write(colHeaders);
            //向HTTP输出流中写入取得的数据信息 
            int total = users.Sum(p => p.NewNum);
            //逐行处理数据   
            foreach (SoftUser row in users)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据     
                for (int i = 0; i < cl; i++)
                {
                    if (i == 0)
                        ls_item += row.StatDate.ToString("yyyy-MM-dd") + "\t";

                    else if (i == 1)
                        ls_item += row.NewNum + "\n";
                }

                resp.Write(ls_item);
                ls_item = "";

            }
            ls_item += "总计" + "\t";
            ls_item += total + "\n";
            resp.Write(ls_item);
            resp.End();
        }

        /// <summary>
        /// 首页里最近三十天数据的活跃用户
        /// </summary>
        private void GetActivityFor30()
        {
            Sjqd_StatUsersService ds = Sjqd_StatUsersService.GetInstance();
            HttpRequest Request = HttpContext.Current.Request;
            int platformsid = Convert.ToInt32(Request["platform"]);
            int softsid = Convert.ToInt32(Request["soft"]);
            ///type 1 为新增，2 为活跃
            int type=Convert.ToInt32(Request["type"]);
            ///检查权限
            CheckHasRight(softsid, "Reports/Default.aspx");
            List<int> X_DateTime1 = new List<int>();
            endtime = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.UserUseNewActivity,CacheTimeOption.TenMinutes);

            List<SoftUser> users = ds.GetAllSoftUsersCache(endtime.AddDays(-30), endtime, softsid, platformsid, net91com.Stat.Core.PeriodOptions.Daily, net91com.Core.CacheTimeOption.HalfDay).OrderBy(p => p.StatDate).ToList();
            List<DateTime> X_DateTime = users.Select(p => p.StatDate).Distinct().ToList();
            if (users.Count != 0)
            {
                SetxAxisJson(X_DateTime);
                SeriesJsonStr1 = JsonConvert.SerializeObject(GetDataJsonList(X_DateTime, users, type));

            }
            else
            {
                AxisJsonStr1 = "{}";
                SeriesJsonStr1 = "[]";
            }
             
            
            string result = "{ x:" + AxisJsonStr1 + "," + "y:" + SeriesJsonStr1 + "}";
            HttpContext.Current.Response.Write(result);
                
        }
        //最近三十天数据
        private List<SeriesJsonModel> GetDataJsonList(List<DateTime> x_date, List<SoftUser> softs,int type)
        {
            
            List<SeriesJsonModel> result = new List<SeriesJsonModel>();
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            ///构造对应x轴上对应各个坐标点,一开始就是null ，先填充好
            for (int ii = 0; ii < x_date.Count; ii++)
            {
                DataLabels dl = null;
                sjModel2.data.Add(dl);
            }
            if (type == 2)
            {
                for (int i = 0; i < softs.Count; i++)
                {
                    sjModel2.name = "活跃";
                    for (int j = 0; j < x_date.Count; j++)
                    {
                        if (x_date[j] == softs[i].StatDate)
                        {
                            DataLabels dl = new DataLabels();
                            SmallDataLabels smalldata = new SmallDataLabels();

                            dl.y = softs[i].ActiveNum;


                            dl.dataLabels = smalldata;
                            if (j % (-3) == 0)//这个间隔和x轴设置是一样的
                                smalldata.enabled = true;
                            ///替换掉以前的null
                            sjModel2.data[j] = dl;

                        }
                    }

                }
            }
            else
            {
                for (int i = 0; i < softs.Count; i++)
                {
                    sjModel2.name = "新增";
                    for (int j = 0; j < x_date.Count; j++)
                    {
                        if (x_date[j] == softs[i].StatDate)
                        {
                            DataLabels dl = new DataLabels();
                            SmallDataLabels smalldata = new SmallDataLabels();

                            dl.y = softs[i].NewNum;


                            dl.dataLabels = smalldata;
                            if (j % (-3) == 0)//这个间隔和x轴设置是一样的
                                smalldata.enabled = true;
                            ///替换掉以前的null
                            sjModel2.data[j] = dl;

                        }
                    }

                }
                
            }
            result.Add(sjModel2);

            return result;

        }
        /// 获取x轴的数据（最近30日数据需要）
        protected void SetxAxisJson(List<DateTime> times)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DateTime item in times)
            {
                sb.Append("\"" + item.ToString("yy-MM-dd").Replace("-", "/") + "\"" + ",");
            }
            string str = sb.ToString().Substring(0, sb.ToString().Length - 1);
            AxisJsonStr1 = "{" + string.Format("categories:[{0}] ", str);
            ///加逗号和右括号
            AxisJsonStr1 += ",labels:{ align:'left', rotation: -45, tickLength:80,tickPixelInterval:140 ,x:-30,y:45";
            ///设置系数 间隔两个显示一个日期
            AxisJsonStr1 += ",step:-3";
            
            AxisJsonStr1 += "}}";
        }

        
        ///严格规范时间范围
        protected void SetStandardTime(net91com.Stat.Core.PeriodOptions Period,DateTime endtime,DateTime begintime)
        {
            TimeSpan dtSpan = endtime - begintime;
            int days = dtSpan.Days;
            if (Period == net91com.Stat.Core.PeriodOptions.Hours)
            {
                if (days > 5)
                    begintime = endtime.AddDays(-5);

            }


        }
        public void GetExcelActivity()
        {
            try
            { 
                net91com.Stat.Core.PeriodOptions Period = HttpContext.Current.Request["inputzhouqi"].ToEnum<net91com.Stat.Core.PeriodOptions>(net91com.Stat.Core.PeriodOptions.Daily);
                DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["inputtimestart"]);
                DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["inputtimeend"]);
                int excelsoft = Convert.ToInt32(HttpContext.Current.Request["excelsoft"]);
                int channelvalue=Convert.ToInt32(HttpContext.Current.Request["channelcate"]);
                int channeltype = Convert.ToInt32(HttpContext.Current.Request["channeltype"]);
                int isold = Convert.ToInt32(HttpContext.Current.Request["isold"]);
                ///检查权限
                CheckHasRight(excelsoft, "Reports/ActivityUserReport.aspx");
                
                int excelplatform = Convert.ToInt32(HttpContext.Current.Request["excelplatform"]);
                SetStandardTime(Period,begintime,endtime);
                List<SoftUser> users;
                ///没有渠道的
                if (channelvalue == -1)
                {
                    if (Period != net91com.Stat.Core.PeriodOptions.Hours && Period != net91com.Stat.Core.PeriodOptions.TimeOfDay)
                    {
                        users = Sjqd_StatUsersService.GetInstance().GetSoftUserListCache(begintime, endtime, excelsoft, (int)excelplatform, Period, loginService,CacheTimeOption.TenMinutes).OrderByDescending(p => p.StatDate).ToList();
                    }
                    else
                    {
                        //users = StatUsersByHourService.GetInstance().GetHourUserDataCache(excelsoft, excelplatform, begintime, endtime, Period, loginService,CacheTimeOption.TenMinutes).OrderByDescending(p => p.StatDate).ToList();
                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsersComp;
                        ssUsersComp = suService.GetStatUsersByHour(excelsoft, excelplatform, ChannelTypeOptions.Category, 0, (int)Period, begintime, endtime);
                        users = new List<SoftUser>();
                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsersComp)
                        {
                            SoftUser softuser = new SoftUser();
                            softuser.StatDate = u.StatDate;
                            softuser.SoftId = excelsoft;
                            softuser.Platform = excelplatform;
                            softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                            softuser.Hour = u.StatHour;
                            softuser.NewNum = u.NewUserCount;
                            softuser.UseNum = u.ActiveUserCount;
                            users.Add(softuser);
                        }
                        users = users.OrderBy(p => p.StatDate).ToList();
                    }
                }//有渠道
                else
                {
                    if (Period != net91com.Stat.Core.PeriodOptions.Hours && Period != net91com.Stat.Core.PeriodOptions.TimeOfDay)
                    {
                        users = Sjqd_StatUsersByChannelsService.GetInstance().GetSoftUserChanelListCache(begintime, endtime, excelsoft, excelplatform, Period,
                             (ChannelTypeOptions)channeltype, channelvalue, "", false, loginService,CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
                    }
                    else
                    {
                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsersComp;
                        ssUsersComp = suService.GetStatUsersByHour(excelsoft, excelplatform, (ChannelTypeOptions)channeltype, channelvalue, (int)Period, begintime, endtime);
                        users = new List<SoftUser>();
                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsersComp)
                        {
                            SoftUser softuser = new SoftUser();
                            softuser.StatDate = u.StatDate;
                            softuser.SoftId = excelsoft;
                            softuser.Platform = excelplatform;
                            softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                            softuser.Hour = u.StatHour;
                            softuser.NewNum = u.NewUserCount;
                            softuser.UseNum = u.ActiveUserCount;
                            users.Add(softuser);
                        }
                        users = users.OrderBy(p => p.StatDate).ToList();
                    }
                }
                HttpResponse resp;
                resp = HttpContext.Current.Response;
                resp.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                // excelsoftStr + "不区分平台" + "_用户活跃" + ".xls"
                string filename =string.Empty;
               
                filename = "活跃用户.xls";
                AddHead(filename);

                //是否是内部软件
                bool isInternalSoft = AvailableSofts.FirstOrDefault(a => a.ID == excelsoft) != null;
                resp.Write(TableTemplateHelper.BuildStatUsersTable(excelsoft, (MobileOption)excelplatform, isInternalSoft, channelvalue != -1, Period, "active", isold == 1, users, true, 0, ""));
                resp.End(); 
               
            }
            catch (Exception)
            {
                HttpContext.Current.Response.Write("");

            }
            
        }
     
       
        public void GetExcelNewUser()
        {
            try
            { 
                net91com.Stat.Core.PeriodOptions Period = HttpContext.Current.Request["inputzhouqi"].ToEnum<net91com.Stat.Core.PeriodOptions>(net91com.Stat.Core.PeriodOptions.Daily);
                DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["inputtimestart"]);
                DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["inputtimeend"]);
                int excelsoft = Convert.ToInt32(HttpContext.Current.Request["excelsoft"]);
                int channelcate = Convert.ToInt32(HttpContext.Current.Request["channelcate"]);
                int channeltype = Convert.ToInt32(HttpContext.Current.Request["channeltype"]);
                ///检查权限
                CheckHasRight(excelsoft, "Reports/NewUserReport.aspx");

                int excelplatform = Convert.ToInt32(HttpContext.Current.Request["excelplatform"]);
                SetStandardTime(Period, begintime, endtime);
                List<SoftUser> users;
                if (channelcate == -1)
                {
                    if (Period != net91com.Stat.Core.PeriodOptions.Hours && Period != net91com.Stat.Core.PeriodOptions.TimeOfDay)
                    {
                        users = Sjqd_StatUsersService.GetInstance().GetSoftUserListCache(begintime, endtime, excelsoft, (int)excelplatform, Period, loginService,CacheTimeOption.TenMinutes).OrderByDescending(p => p.StatDate).ToList();
                    }
                    else
                    {
                        //users = StatUsersByHourService.GetInstance().GetHourUserDataCache(excelsoft, excelplatform, begintime, endtime, Period, loginService,CacheTimeOption.TenMinutes).OrderByDescending(p => p.StatDate).ToList();
                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                        ssUsers = suService.GetStatUsersByHour(excelsoft, excelplatform, ChannelTypeOptions.Category, 0, (int)Period, begintime, endtime);
                        users = new List<SoftUser>();
                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                        {
                            SoftUser softuser = new SoftUser();
                            softuser.StatDate = u.StatDate;
                            softuser.SoftId = excelsoft;
                            softuser.Platform = excelplatform;
                            softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                            softuser.Hour = u.StatHour;
                            softuser.NewNum = u.NewUserCount;
                            softuser.UseNum = u.ActiveUserCount;
                            users.Add(softuser);
                        }
                        users = users.OrderBy(p => p.StatDate).ToList();
                    }
                }
                else
                {
                    if (Period != net91com.Stat.Core.PeriodOptions.Hours && Period != net91com.Stat.Core.PeriodOptions.TimeOfDay)
                    {
                        users = Sjqd_StatUsersByChannelsService.GetInstance().GetSoftUserChanelListCache(begintime, endtime, excelsoft, excelplatform, Period,
                                               (ChannelTypeOptions)channeltype, channelcate, "", false, loginService,CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
                    }
                    else
                    {
                        net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService suService = new net91com.Reports.Services.CommonServices.SjqdUserStat.StatUsersService();
                        List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> ssUsers;
                        ssUsers = suService.GetStatUsersByHour(excelsoft, excelplatform, (ChannelTypeOptions)channeltype, channelcate, (int)Period, begintime, endtime);
                        users = new List<SoftUser>();
                        foreach (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers u in ssUsers)
                        {
                            SoftUser softuser = new SoftUser();
                            softuser.StatDate = u.StatDate;
                            softuser.SoftId = excelsoft;
                            softuser.Platform = excelplatform;
                            softuser.ActiveNum = u.ActiveUserCount - u.NewUserCount;
                            softuser.Hour = u.StatHour;
                            softuser.NewNum = u.NewUserCount;
                            softuser.UseNum = u.ActiveUserCount;
                            users.Add(softuser);
                        }
                        users = users.OrderBy(p => p.StatDate).ToList();
                    }
                }               
                HttpResponse resp;
                resp = HttpContext.Current.Response;
                resp.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                string filename = "新增用户.xls";
                 
                AddHead(filename);

                //是否是内部软件
                bool isInternalSoft = AvailableSofts.FirstOrDefault(a => a.ID == excelsoft) != null;
                resp.Write(TableTemplateHelper.BuildStatUsersTable(excelsoft, (MobileOption)excelplatform, isInternalSoft, channelcate != -1, Period, "new", false, users, true, 0, ""));
                resp.End(); 
            }
            catch (Exception)
            {
                HttpContext.Current.Response.Write("");
                 
            }
            
         }



        public void GetDownZjs()
        {
            string tablename = HttpContext.Current.Request["tablename"];
            string[] paras = tablename.Split('_');
            int softid = Convert.ToInt32(paras[0]);
            MobileOption platid = (MobileOption)Convert.ToInt32(paras[1]);
            int statDate = Convert.ToInt32(paras[2]);
            DateTime dt = new DateTime(statDate / 10000, statDate / 100 % 100, statDate % 100);
            net91com.Stat.Core.PeriodOptions period = (net91com.Stat.Core.PeriodOptions)Convert.ToInt32(paras[3]);
            CheckHasRight(softid, "Reports/NewUserByMac.aspx");
            List<Sjqd_StatUsersByMAC> lists = new StatUsersByMACService(true).GetTop100MacUsersByCache(
                    softid,
                    platid, period, dt, CacheTimeOption.TenMinutes
                 );
            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            string filename = string.Empty;
            filename = "装机量.xls";
            AddHead(filename);
            ///文件标题+内容
            string colHeaders = "", ls_item = "";
            //列数
            int cl = 2;
            colHeaders += "Mac" + "\t" + "新增量" + "\t\n";
            resp.Write(colHeaders);
            //向HTTP输出流中写入取得的数据信息 
            //逐行处理数据   
            for (int j = 0; j < lists.Count; j++)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据     
                for (int i = 0; i < cl; i++)
                {
                     
                    if (i == 0)
                        ls_item +="'"+lists[j].Mac + "\t";
                    else if (i == 1)
                        ls_item += lists[j].NewUserCount + "\t\n";
                  

                }
                resp.Write(ls_item);
                ls_item = "";
            }

            resp.End();

        }

        

        
        
       
    }
}