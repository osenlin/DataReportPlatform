using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
//using BY.AccessControlCore;
using net91com.Stat.Web.Reports.Services;
using System.Text;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services;
using Res91com.ResourceDataAccess;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Base
{
    public class HandlerBase : IHttpHandler, IRequiresSessionState
    {
        protected URLoginService loginService = new URLoginService();

        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        protected User CurrentUser
        {
            get { return loginService.LoginUser; }
        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts; }
        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected List<Soft> AvailableInternalSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.SoftType == SoftTypeOptions.InternalSoft).ToList(); }
        }

        /// <summary>
        /// 可访问的项目来源列表
        /// </summary>
        protected List<ProjectSource> AvailableProjectSources
        {
            get { return loginService.AvailableProjectSources; }
        }

        /// <summary>
        /// 可访问的操作权限
        /// </summary>
        protected List<Right> AvailableRights
        {
            get { return loginService.AvailableRights; }
        }

        /// <summary>
        /// 可访问的资源列表
        /// </summary>
        protected List<int> AvailableResIds
        {
            get { return loginService.AvailableResIds; }
        }

        /// <summary>
        /// 获取指定名称的软件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Soft GetSoft(string name)
        {
            Soft soft = loginService.AvailableSofts.FirstOrDefault(a => a.Name == name);
            return soft;
        }

        /// <summary>
        /// 获取指定ID的软件
        /// </summary>
        /// <param name="softId"></param>
        /// <returns></returns>
        protected Soft GetSoft(int softId)
        {
            Soft soft = loginService.AvailableSofts.FirstOrDefault(a => a.ID == softId);
            return soft;
        }

        /// <summary>
        /// Request
        /// </summary>
        protected HttpRequest ThisRequest
        {
            get {
                return HttpContext.Current.Request;
            }
        }
        /// <summary>
        /// Response
        /// </summary>
        protected HttpResponse ThisResponse
        {
            get
            {
                return HttpContext.Current.Response;
            }
        }

        /// <summary>
        /// 检查权限(检查软件权限)
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="url"></param>
        protected void CheckHasRight(int softid, string url)
        {
            if (!AvailableSofts.Exists(a => a.ID == softid) || !loginService.CheckUrlRight(url))
                throw new NotRightException(); 
        }

        /// <summary>
        /// 检查权限(检查项目来源权限)
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="url"></param>
        protected bool CheckHasRightForProject(int project, string url)
        {
            if (!AvailableProjectSources.Exists(a => a.ProjectSourceID == project) || !loginService.CheckUrlRight(url))
            {
                return false;
            }
            return true;
        }

        protected bool CheckUrl(string url)
        {
            return loginService.CheckUrlRight(url);
        }

        /// <summary>
        /// 检查是否有产品权限
        /// </summary>
        /// <param name="softstrs"></param>
        /// <returns></returns>
        public bool CheckSoft(string softstrs)
        {
            if (string.IsNullOrEmpty(softstrs))
                return false;
            var softids = softstrs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
            var result = from a in AvailableSofts
                         join b in softids
                         on a.ID equals b
                         select a;
            return result.Count() == softids.Count();
        }

        /// <summary>
        /// 检查是否有项目权限
        /// </summary>
        /// <param name="projectsource"></param>
        /// <param name="projSrcType"></param>
        /// <returns></returns>
        protected bool CheckProject(string projectsource)
        {
            if (string.IsNullOrEmpty(projectsource))
                return false;
            var projects = projectsource.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToList();
            var result = from a in AvailableProjectSources.Select(a => a.ProjectSourceID).Distinct()
                         join b in projects
                         on a equals b
                         select a;
            return result.Count() == projects.Count();
        }

        /// <summary>
        /// 获取Request.QueryString参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected string GetQueryString(string key, string defaultValue)
        {
            return string.IsNullOrEmpty(ThisRequest.QueryString[key]) ? defaultValue : ThisRequest.QueryString[key];
        }

        /// <summary>
        /// 获取Request.QueryString参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetQueryString(string key)
        {
            return GetQueryString(key, string.Empty);
        }

        /// <summary>
        /// 文件头浏览器兼容性
        /// </summary>
        /// <param name="filename"></param>
        protected void AddHead(string filename)
        {

            if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("msie") > -1)
            {
                filename = HttpUtility.UrlPathEncode(filename);
            }

            if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                //ThisResponse.AddHeader("Cache-Control", "public");
                ThisResponse.AddHeader("Content-Type", "application/vnd.ms-excel");
                ThisResponse.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                
                 
            }
            else
            {
                //ThisResponse.AddHeader("Cache-Control", "public");
                ThisResponse.AddHeader("Content-Type", "application/vnd.ms-excel");
                ThisResponse.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                
            }
        }
        protected DateTime ToDateTimeByInt(object obj)
        {
            if (obj != null)
            {
                int intDate = Convert.ToInt32(obj);
                return Convert.ToDateTime((intDate / 10000) + "-" + (intDate % 10000 / 100) + "-" + (intDate % 100));
            }
            return new DateTime(1970, 1, 1);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {            
        }

       protected void WriteString(string result)
       {
           ThisResponse.HeaderEncoding = Encoding.UTF8;
           ThisResponse.Write(result);
           ThisResponse.End();
       }

        //异步绘线需要的方法
       /// 获取x轴时间
       protected void SetxAxisJson(List<DateTime> times, ref string AxisJsonStr1)
       {
           //间隔个数
           int spanInt = -(times.Count / 25 + 1);
           StringBuilder sb = new StringBuilder();
           foreach (DateTime item in times)
           {
               sb.Append("\"" + item.ToString("yy-MM-dd").Replace("-", "/") + "\"" + ",");
           }
           string str = sb.ToString().Substring(0, sb.ToString().Length - 1);
           AxisJsonStr1 = "{" + string.Format("categories:[{0}] ", str);
           //加逗号和右括号
           AxisJsonStr1 += ",labels:{ align:'left', rotation: -45, tickLength:80,tickPixelInterval:140 ,x:-30,y:45";
           //设置系数 间隔两个显示一个日期
           AxisJsonStr1 += ",step:" + spanInt;

           AxisJsonStr1 += "}}";
       }
    }
}