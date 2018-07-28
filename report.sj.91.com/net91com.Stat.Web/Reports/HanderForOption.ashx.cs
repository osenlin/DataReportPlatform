using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using net91com.Core;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Services.CommonServices.Other;
using net91com.Stat.Web.Base;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;

using net91com.Reports.UserRights;


namespace net91com.Stat.Web.Reports
{
    /// <summary>
    /// Summary description for HanderForOption
    /// </summary>
    public class HanderForOption : HandlerBase
    {
        ///x轴上控制系数 
        public int MaxNumCoef = -1;
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
             
                case "getversionforsession":
                            GetVersionForSession();
                            break; 
                case "getfunctionsforstat":
                            GetFunctionForStat();
                            break;     
                case "channeltree":
                       GetChannelTreeJson();
                    break;
                case "linktagtree":
                    GetLinkTagTreeJson();
                    break;
             }

        }

        private void GetFunctionForStat()
        {
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            int platformid = Convert.ToInt32(HttpContext.Current.Request["plat"]);
            if (softid == 1 && platformid == 1)
            {
                HttpContext.Current.Response.Write("[{\"FunctionID\":14,\"FunctionName\":\"测试1\"},{\"FunctionID\":127,\"FunctionName\":\"测试2\"},{\"FunctionID\":154,\"FunctionName\":\"测试33\"},{\"FunctionID\":167,\"FunctionName\":\"测试44\"}]");

            }
            else if (platformid == 2)
            {
                HttpContext.Current.Response.Write("[{\"FunctionID\":19899,\"FunctionName\":\"测试23\"},{\"FunctionID\":190951,\"FunctionName\":\"测试55\"},{\"FunctionID\":1900051,\"FunctionName\":\"测试5225\"}]");
            }
            else
                HttpContext.Current.Response.Write("[]");
        }

        /// <summary>
        /// 获取跳转标签树形数据
        /// </summary>
        private void GetLinkTagTreeJson()
        {
            int softId = Convert.ToInt32(HttpContext.Current.Request["id"]);
            string platformsString = HttpContext.Current.Request["plat"];
            MobileOption[] platforms = platformsString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => (MobileOption)Convert.ToInt32(a)).ToArray();
            string idsString = HttpContext.Current.Request["ids"] ?? string.Empty;
            string typesString = HttpContext.Current.Request["types"] ?? string.Empty;
            int[] ids = idsString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
            bool[] types = typesString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a) == 1).ToArray();
            if (ids.Length != types.Length)
            {
                HttpContext.Current.Response.Write("[]");
                return;
            }
            List<LinkTagLog> tags = LinkTagService.Instance.GetTagTree(softId, platforms, true);
            StringBuilder jsonBuilder = new StringBuilder("[");
            foreach (LinkTagLog tag in tags)
            {
                bool selected = false;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (types[i] == tag.IsCategory && ids[i] == tag.ID)
                    {
                        selected = true;
                        break;
                    }
                }
                jsonBuilder.AppendFormat("{{\"id\":\"{0}_{1}\",\"pId\":\"1_{2}\",\"name\":\"{3}\",\"open\":false,\"val\":{1},\"type\":{0},\"checked\":{4},\"keyid\":\"{1}\",\"drag\":false}},",
                    tag.IsCategory ? "1" : "0", tag.ID, tag.PID, tag.Name,
                    selected.ToString().ToLower());
            }
            HttpContext.Current.Response.Write(jsonBuilder.ToString().TrimEnd(',') + "]");
        }
        
        

         
        /// <summary>
        /// 获取渠道树形数据
        /// </summary>
        private void GetChannelTreeJson()
        {
            int softId = Convert.ToInt32(HttpContext.Current.Request["id"]);
            string platformsString = HttpContext.Current.Request["plat"];
            MobileOption[] platforms = platformsString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => (MobileOption)Convert.ToInt32(a)).ToArray();
            string idsString = HttpContext.Current.Request["ids"] ?? string.Empty;
            string typesString = HttpContext.Current.Request["types"] ?? string.Empty;
            int[] ids = idsString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
            ChannelTypeOptions[] types = typesString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => (ChannelTypeOptions)Convert.ToInt32(a)).ToArray();
            if (ids.Length != types.Length)
            {
                HttpContext.Current.Response.Write("[]");
                return;
            }
            List<Channel> channels = loginService.GetAvailableChannels(softId, platforms, true);
            StringBuilder jsonBuilder = new StringBuilder("[");
            foreach (Channel chl in channels)
            {
                bool selected = false;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (types[i] == chl.ChannelType && ids[i] == chl.ID)
                    {
                        selected = true;
                        break;
                    }
                }
                jsonBuilder.AppendFormat("{{\"id\":\"{0}_{1}\",\"pId\":\"{2}_{3}\",\"name\":\"{4}\",\"open\":false,\"val\":{5},\"type\":{6},\"checked\":{7},\"keyid\":\"{8}\",\"drag\":false}},"
                    , chl.ChannelType, chl.ID, chl.ParentChannelType, chl.ParentID, (chl.Platform == 0 ? chl.Name : chl.Name + "(" + chl.Platform + ")").Replace("\"", ""), chl.ID, (int)chl.ChannelType, selected.ToString().ToLower(), chl.ID);
            }
            HttpContext.Current.Response.Write(jsonBuilder.ToString().TrimEnd(',') + "]");
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

        public void GetVersionForSession()
        {
            int softid = Convert.ToInt32(HttpContext.Current.Request["soft"]);
            
            int platformid = Convert.ToInt32(HttpContext.Current.Request["platform"]);
            int versiontype= Convert.ToInt32(HttpContext.Current.Request["versiontype"]);  
            if (platformid<=0)
            {
                HttpContext.Current.Response.Write("[]");
                return;
            }
            StatUsersByVersionService ss = StatUsersByVersionService.GetInstance();
            List<SoftVersion> listVersions = ss.GetVersionCacheStatDB(softid, platformid, versiontype,CacheTimeOption.TenMinutes);
            string jsonVersions = JsonConvert.SerializeObject(listVersions);
            HttpContext.Current.Response.Write(jsonVersions);
        }

         
    }
}