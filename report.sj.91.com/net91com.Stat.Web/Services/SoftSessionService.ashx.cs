using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Stat.Web.Reports.Services;
using net91com.Core.Extensions;
using Newtonsoft.Json;
using System.Web.SessionState;

using System.Text;
using net91com.Core;
using net91com.Stat.Web.Base;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;

namespace net91com.Stat.Web.Services
{
    /// <summary>
    /// Summary description for HandlerForSession
    /// </summary>
    public class SoftSessionService : HandlerBase
    {
        ///x轴上控制系数 
        public int MaxNumCoef = -2;
        ///x轴上坐标(即时间)超过多少个开始利用控制系数
        public const int MaxNum = 20;

        /// 水平轴
        public string AxisJsonStr = "{}";
        /// <summary>
        /// 显示数组
        /// </summary>
        public string SeriesJsonStr = "[]";
        public override void ProcessRequest(HttpContext context)
        {

            switch (context.Request["action"])
            { 
                case "avgperiodlength":
                    GetAvgPeriodLength();
                    break;
                case "oncelength":
                    GetOnceUseLength();
                    break;
                case "uselength":
                    GetUseLength();
                    break;
                case "excelsessionlength":
                    GetDownSessionLengthExcel();
                    break;
                case "sessionperuser":
                    GetSessionPerUser();
                    break;
                case "sessions":
                    GetSessions();
                    break;

            }


            

        }
        /// <summary>
        /// 对应周期平均每个用户启动次数
        /// </summary>
        private void GetSessionPerUser()
        {
            string myzhouqi = HttpContext.Current.Request["period"];
            net91com.Stat.Core.PeriodOptions Period;
            switch (myzhouqi)
            {
                case "一天":
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
                case "一周":
                    Period = net91com.Stat.Core.PeriodOptions.Weekly;
                    break;
                case "一月":
                    Period = net91com.Stat.Core.PeriodOptions.Monthly;
                    break;
                default:
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
            }
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            int platid = Convert.ToInt32(HttpContext.Current.Request["plat"]);
            int versionid = Convert.ToInt32(HttpContext.Current.Request["version"]);
            DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["endtime"]);
            CheckHasRight(softid, "Reports/SoftSessionlenReport.aspx");
            ULSessionService sessionService = new ULSessionService(true);
            List<DateTime> X_DateTime = new List<DateTime>();
            List<Sjqd_ULSessionAvgUsers> avgUsers = sessionService.GetULSessionAvgUsersCache(softid, platid, (int)Period, begintime, endtime, CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
            if (avgUsers.Count != 0)
            {
                X_DateTime = avgUsers.Select(p => p.StatDate).Distinct().ToList();
                MaxNumCoef = X_DateTime.Count / MaxNum + 1;
                SetxAxisJson(X_DateTime);
                string result = "{ x:" + SetxAxisJson(X_DateTime) + "," + "y:" + GetDataJsonList(avgUsers, X_DateTime, false) + "}";
                HttpContext.Current.Response.Write(result);
            }
            else
            {
                string result = "{ x:{},y:[]}";
                HttpContext.Current.Response.Write(result);
            }
        }
        /// <summary>
        /// 启动次数分布
        /// </summary>
        private void GetSessions()
        {
            string myzhouqi = HttpContext.Current.Request["period"];
            net91com.Stat.Core.PeriodOptions Period;
            switch (myzhouqi)
            {
                case "一天":
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
                case "一周":
                    Period = net91com.Stat.Core.PeriodOptions.Weekly;
                    break;
                case "一月":
                    Period = net91com.Stat.Core.PeriodOptions.Monthly;
                    break;
                default:
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
            }
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            int platid = Convert.ToInt32(HttpContext.Current.Request["plat"]);
            var version = HttpContext.Current.Request["version"];

            DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["endtime"]);
            CheckHasRight(softid, "Reports/SoftSessionlenReport.aspx");
            ULSessionService sessionService = new ULSessionService(true);
            List<Sjqd_ULSessionsCount> users = sessionService.GetULSessionsCountCache(softid, platid, version, (int)Period, begintime, endtime, CacheTimeOption.TenMinutes);
            if (users.Count != 0)
            {
                StringBuilder sbYWeek;
                StringBuilder sbX = new StringBuilder();
                if (Period == net91com.Stat.Core.PeriodOptions.Daily)
                { 
                    sbX.Append("{ categories:['1次用户','2-3次用户','4-6次用户','7-9次用户','10-12用户','13-17次用户','18-22次用户','22次以上用户']}");
                    sbYWeek = new StringBuilder("[{ name:'累计启动天数',");
                     
                }
                else if (Period == net91com.Stat.Core.PeriodOptions.Weekly)
                {
                    
                    sbX.Append("{ categories:['1天用户','2天用户','3天用户','4天用户','5天用户','6天用户','7天用户']}");
                    sbYWeek = new StringBuilder("[{ name:'累计启动天数',");
                    
                }
                else
                {
                    sbX.Append("{ categories:['1-2天用户','3-4天用户','5-8天用户','9-12天用户','13-16天用户','17-20天用户','21-25天用户','25天以上用户']}");
                     sbYWeek = new StringBuilder("[{ name:'累计启动天数',");
                  
                     
                }
                
                sbYWeek.Append("data:[");
                sbYWeek.Append(users[0].Percent * 100 + ",");
                sbYWeek.Append(users[1].Percent * 100 + ",");
                sbYWeek.Append(users[2].Percent * 100 + ",");
                sbYWeek.Append(users[3].Percent * 100 + ",");
                sbYWeek.Append(users[4].Percent * 100 + ",");
                sbYWeek.Append(users[5].Percent * 100 + ",");
                if (Period != net91com.Stat.Core.PeriodOptions.Daily)
                    sbYWeek.Append(users[6].Percent * 100 + "]}]");
                else
                {
                    sbYWeek.Append(users[6].Percent * 100 + ",");
                    sbYWeek.Append(users[7].Percent * 100 + "]}]");
                }
                string resultWeek = "{ x:" + sbX.ToString() + "," + " y:" + sbYWeek.ToString() + "}";
                HttpContext.Current.Response.Write(resultWeek);
                return;
                
            }
            else
            {
                string result = "{ x:{},y:[]}";
                HttpContext.Current.Response.Write(result);
            }
        }
        /// <summary>
        /// 使用时长分布excel导出
        /// </summary>
        private void GetDownSessionLengthExcel()
        {
            string myzhouqi = HttpContext.Current.Request["period"];
            net91com.Stat.Core.PeriodOptions Period;
            switch (myzhouqi)
            {
                case "一天":
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
                case "一周":
                    Period = net91com.Stat.Core.PeriodOptions.Weekly;
                    break;
                case "一月":
                    Period = net91com.Stat.Core.PeriodOptions.Monthly;
                    break;
                default:
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
            }
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            int platid = Convert.ToInt32(HttpContext.Current.Request["plat"]);
            int versionid = Convert.ToInt32(HttpContext.Current.Request["version"]);
            DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["endtime"]);
            CheckHasRight(softid, "Reports/SoftSessionlenReport.aspx");
            ULSessionService sessionService = new ULSessionService(true);

            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            string headername = "使用时长分布" + ".xls";
            AddHead(headername);
            List<Sjqd_ULSessionAvgUsers> avgUsers = sessionService.GetULSessionAvgUsersCache(softid, platid, (int)Period, begintime, endtime, CacheTimeOption.TenMinutes).OrderByDescending(p => p.StatDate).ToList();
            
            ///文件标题+内容
            string colHeaders = "", ls_item = "";
            //列数
            int cl = 7;
            colHeaders += "日期" + "\t" + "活跃用户" + "\t" + "累计启动次数" +"\t"+ "人均启动次数" + "\t" + "累计使用时长(分)" + "\t" + "人均每次使用时长(秒)"+"\t" + "人均使用时长(分)"  + "\t\n";
            resp.Write(colHeaders);
            //向HTTP输出流中写入取得的数据信息 

            //逐行处理数据   
            foreach (Sjqd_ULSessionAvgUsers row in avgUsers)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据     
                for (int i = 0; i < cl; i++)
                {
                    if (i == 0)
                        ls_item += row.StatDate.ToString("yyyy-MM-dd") + "\t";
                    else if (i == 1)
                        ls_item += row.UseUsers + "\t";
                    else if (i == 2)
                        ls_item += row.AllSessions + "\t";
                    else if (i == 3)
                        ls_item += row.AvgSessions + "\t";
                    else if (i == 4)
                        ls_item += row.AllSessionLength/60 + "\t";
                    else if (i == 5)
                        ls_item += row.AvgLengthPerSession + "\t";
                    else if (i == 6)
                        ls_item += row.AvgSessionLength/60 + "\n";

                }
                resp.Write(ls_item);
                ls_item = "";

            }
            resp.End();
         
        }
        /// <summary>
        /// 获取x轴的数据
        /// </summary>
        protected string SetxAxisJson(List<DateTime> times)
        {
            string AxisJsonStr = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (DateTime item in times)
            {
                sb.Append("\"" + item.ToString("yy-MM-dd").Replace("-", "/") + "\"" + ",");
            }
            string str = sb.ToString().Substring(0, sb.ToString().Length - 1);
            AxisJsonStr = "{" + string.Format("categories:[{0}] ", str);
            ///加逗号和右括号
            AxisJsonStr += ",labels:{ align:'left', rotation: -45, tickLength:80,tickPixelInterval:140 ,x:-30,y:45";
            ///设置系数

            if (times.Count > MaxNum)
                ///如果大于 一定数目的话就隔 MaxNumCoef显示x 轴上lable
                AxisJsonStr += ",step:" + MaxNumCoef.ToString();
            AxisJsonStr += "}}";

            return AxisJsonStr;
        }
        /// <summary>
        /// 按照档次使用时长分布人数
        /// </summary>
        private void GetUseLength()
        {
            string myzhouqi = HttpContext.Current.Request["period"];
            net91com.Stat.Core.PeriodOptions Period;
            switch (myzhouqi)
            {
                case "一天":
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
                case "一周":
                    Period = net91com.Stat.Core.PeriodOptions.Weekly;
                    break;
                case "一月":
                    Period = net91com.Stat.Core.PeriodOptions.Monthly;
                    break;
                default:
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
            }
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            int platid = Convert.ToInt32(HttpContext.Current.Request["plat"]);
            var version = HttpContext.Current.Request["version"];
            DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["endtime"]);
            CheckHasRight(softid, "Reports/SoftSessionlenReport.aspx"); 
            ULSessionService sessionService = new ULSessionService(true);
            List<Sjqd_ULSessionLength> users = sessionService.GetULSessionsLengthCache(softid, platid, version, (int)Period, begintime, endtime, CacheTimeOption.TenMinutes);
            if (users.Count != 0)
            { 
                StringBuilder sbX = new StringBuilder();
                if (Period == net91com.Stat.Core.PeriodOptions.Daily)
                {
                    sbX.Append("{ categories:['0-10秒用户','10-60秒用户','1-4分钟用户','4-20分钟用户','20-60分钟用户','1-2小时用户','2-5小时用户','5小时以上用户']}");
                }
                else if (Period == net91com.Stat.Core.PeriodOptions.Weekly)
                {
                    sbX.Append("{ categories:['0-20秒用户','20-60秒用户','1-60分钟用户','1-3小时用户','3-8小时用户','8-16小时用户','16-24小时用户','24小时以上用户']}");
                }
                else
                {
                    sbX.Append("{ categories:['0-60秒用户','1-60分钟用户','1-8小时用户','8-24小时用户','24-48小时用户','48-72小时用户','72-96小时用户','96小时以上用户']}");
                }
                StringBuilder sbY;
                sbY = new StringBuilder("[{ name:'累计使用时长',");
                sbY.Append("data:[");
                sbY.Append(users[0].Percent * 100 + ",");
                sbY.Append(users[1].Percent * 100 + ",");
                sbY.Append(users[2].Percent * 100 + ",");
                sbY.Append(users[3].Percent * 100 + ",");
                sbY.Append(users[4].Percent * 100 + ",");
                sbY.Append(users[5].Percent * 100 + ",");
                sbY.Append(users[6].Percent * 100 + ",");
                sbY.Append(users[7].Percent * 100 + "]}]");
                string result = "{ x:" + sbX.ToString() + "," + " y:" + sbY.ToString() + "}";
                HttpContext.Current.Response.Write(result);
            }
            else
            {
                string result = "{ x:{},y:[]}";
                HttpContext.Current.Response.Write(result);
            }

            

        }
        /// <summary>
        /// 一次使用时长各档位分布人数
        /// </summary>
        private void GetOnceUseLength()
        {
            string myzhouqi = HttpContext.Current.Request["period"];
            net91com.Stat.Core.PeriodOptions Period;
            switch (myzhouqi)
            {
                case "一天":
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
                case "一周":
                    Period = net91com.Stat.Core.PeriodOptions.Weekly;
                    break;
                case "一月":
                    Period = net91com.Stat.Core.PeriodOptions.Monthly;
                    break;
                default:
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
            }
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            int platid = Convert.ToInt32(HttpContext.Current.Request["plat"]);
            string version = HttpContext.Current.Request["version"];

            DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["endtime"]);
            CheckHasRight(softid, "Reports/SoftSessionlenReport.aspx");
            ///单次是没有周期的概念
            Period = net91com.Stat.Core.PeriodOptions.Daily;
            ULSessionService sessionService = new ULSessionService(true);
            List<Sjqd_ULSessionsSingle> users =null;
            users = sessionService.GetSingleULSessionsLengthCache(softid, platid, version, (int)Period, begintime, endtime, CacheTimeOption.TenMinutes);
            if (users.Count != 0)
            {
                StringBuilder sbY; 
                sbY = new StringBuilder("[{ name:'单次使用时长',");
                sbY.Append("data:[");
                sbY.Append(users[0].Percent * 100 + ",");
                sbY.Append(users[1].Percent * 100 + ",");
                sbY.Append(users[2].Percent * 100 + ",");
                sbY.Append(users[3].Percent * 100 + ",");
                sbY.Append(users[4].Percent * 100 + ",");
                sbY.Append(users[5].Percent * 100 + ",");
                sbY.Append(users[6].Percent * 100 + ",");
                sbY.Append(users[7].Percent * 100 + "]}]");
                StringBuilder sbX = new StringBuilder("{ categories:['1-3秒用户','3-60秒用户','1-5分钟用户','5-10分钟用户','10-30分钟用户','30-60分钟用户','1-2小时用户','2小时以上用户']}");
                string result = "{ x:" + sbX.ToString() + "," + " y:" + sbY.ToString() + "}";
                HttpContext.Current.Response.Write(result);
            }
            else
            {
                string result = "{ x:{},y:[]}";
                HttpContext.Current.Response.Write(result);
            }
            


            

        }

        public void SetTime(net91com.Stat.Core.PeriodOptions period,ref DateTime begintime,ref DateTime endtime)
        {
            DateTime dt = endtime;
            while (true)
            {
                if (period == net91com.Stat.Core.PeriodOptions.Weekly && dt.DayOfWeek == DayOfWeek.Sunday)
                    break;
                else if (period == net91com.Stat.Core.PeriodOptions.Monthly && dt.Day == 20)
                    break;
                else if (period == net91com.Stat.Core.PeriodOptions.Daily)
                    break;
                else
                    dt = dt.AddDays(-1);
            }
            endtime = dt;
            if (period == net91com.Stat.Core.PeriodOptions.Monthly)
            {
                
                begintime = dt.AddMonths(-1);
            }
            if (period == net91com.Stat.Core.PeriodOptions.Weekly)
            {
                begintime = dt.AddDays(-6);
            }
        }


        /// <summary>
        /// 每天平均使用时长
        /// </summary>
        private void GetAvgPeriodLength()
        {
            string myzhouqi = HttpContext.Current.Request["period"];
            net91com.Stat.Core.PeriodOptions Period;
            switch (myzhouqi)
            {
                case "一天":
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
                case "一周":
                    Period = net91com.Stat.Core.PeriodOptions.Weekly;
                    break;
                case "一月":
                    Period = net91com.Stat.Core.PeriodOptions.Monthly;
                    break;
                default:
                    Period = net91com.Stat.Core.PeriodOptions.Daily;
                    break;
            }
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            int platid = Convert.ToInt32(HttpContext.Current.Request["plat"]);
            int versionid = Convert.ToInt32(HttpContext.Current.Request["version"]);
            DateTime begintime = Convert.ToDateTime(HttpContext.Current.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(HttpContext.Current.Request["endtime"]);
            CheckHasRight(softid, "Reports/SoftSessionlenReport.aspx");
            ULSessionService sessionService = new ULSessionService(true);
            List<DateTime> X_DateTime = new List<DateTime>();
            List<Sjqd_ULSessionAvgUsers> avgUsers = sessionService.GetULSessionAvgUsersCache(softid, platid, (int)Period, begintime, endtime, CacheTimeOption.TenMinutes).OrderBy(p => p.StatDate).ToList();
            if (avgUsers.Count != 0)
            {
                X_DateTime = avgUsers.Select(p => p.StatDate).Distinct().ToList();
                MaxNumCoef = X_DateTime.Count / MaxNum + 1;
                SetxAxisJson(X_DateTime);
                string result = "{ x:" + SetxAxisJson(X_DateTime) + "," + "y:" + GetDataJsonList(avgUsers, X_DateTime, true) + "}";
                HttpContext.Current.Response.Write(result);
            }
            else
            {
                string result = "{ x:{},y:[]}";
                HttpContext.Current.Response.Write(result);
            }
        }

        protected string GetDataJsonList(List<Sjqd_ULSessionAvgUsers> users, List<DateTime> X_DateTime, bool isSessionLength)
        {
            List<SeriesJsonModel> seriesJsonStr = new List<SeriesJsonModel>();
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            if(isSessionLength)
                sjModel2.name = "人均日使用时长";
            else
                sjModel2.name = "人均启动次数(不区分版本)";
            if (X_DateTime.Count <= MaxNum)
            {
                for (int ii = 0; ii < X_DateTime.Count; ii++)
                {
                    DataLabels dl = null;
                    sjModel2.data.Add(dl);
                }

                for (int j = 0; j < X_DateTime.Count; j++)
                {
                    foreach (Sjqd_ULSessionAvgUsers item in users)
                    {

                        if (item.StatDate == X_DateTime[j])
                        {
                            DataLabels dl = new DataLabels();
                            SmallDataLabels smalldata = new SmallDataLabels();
                            if(isSessionLength)
                                dl.y = (double)item.AvgSessionLength;
                            else
                                dl.y = (double)item.AvgSessions;
                            smalldata.enabled = true;
                            dl.dataLabels = smalldata;
                            ///替换掉以前的null
                            sjModel2.data[j] = dl;
                            break;
                        }
                    }


                }


            }
            else
            {
                for (int ii = 0; ii < X_DateTime.Count; ii++)
                {
                    DataLabels dl = null;
                    sjModel2.data.Add(dl);
                }

                for (int j = 0; j < X_DateTime.Count; j++)
                {
                    foreach (Sjqd_ULSessionAvgUsers item in users)
                    {

                        if (item.StatDate == X_DateTime[j])
                        {
                            DataLabels dl = new DataLabels();
                            SmallDataLabels smalldata = new SmallDataLabels();
                            if (isSessionLength)
                                dl.y = (double)item.AvgSessionLength;
                            else
                                dl.y = (double)item.AvgSessions;
                            if (j % (MaxNumCoef) == 0)
                                smalldata.enabled = true;
                            dl.dataLabels = smalldata;
                            ///替换掉以前的null
                            sjModel2.data[j] = dl;
                            break;
                        }
                    }


                }



            }
            seriesJsonStr.Add(sjModel2);
            
            return JsonConvert.SerializeObject(seriesJsonStr);

        }


       
    }
}