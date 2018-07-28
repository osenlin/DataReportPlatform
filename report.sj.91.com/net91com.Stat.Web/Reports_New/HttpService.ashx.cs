using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json; 
using net91com.Core.Util;
using net91com.Reports.Services;
using net91com.Reports.Services.ServicesBaseEntity;

using System.Text;

using net91com.Reports.UserRights;
using net91com.Stat.Web.Base;

namespace net91com.Stat.Web.Reports_New
{
    /// <summary>
    /// HttpService 的摘要说明
    /// </summary>
    public class HttpService : HandlerBase
    { 
        

        private static Dictionary<string, HttpServiceBase> services;
        static HttpService()
        {
            services = new Dictionary<string, HttpServiceBase>(50);
            Type[] types = typeof(HttpServiceBase).Assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsAbstract && typeof(HttpServiceBase).IsAssignableFrom(types[i]))
                {
                    HttpServiceBase obj = Activator.CreateInstance(types[i]) as HttpServiceBase;
                    if (obj != null)
                    {
                        services.Add(obj.ServiceCategory.ToLower(), obj);
                        
                    }
                }
            }
        }

        public override void ProcessRequest(HttpContext context)
        {
            try
            {
                string serviceCategory = context.Request["service"];

                if (!string.IsNullOrEmpty(serviceCategory))
                {
                    serviceCategory = serviceCategory.Trim().ToLower();
                    HttpServiceBase service = null;
                    if (services.TryGetValue(serviceCategory, out service))
                    {
                        service.LoginService = loginService;
                        

                        //try
                        //{
                            SetDefaultProjectAndPlat(context);
                            SetDefaultSofts(context);
                            Result result = service.Process(context);
                            if (result != null)
                            {
                                if (result.resultCode == 0)
                                {
                                    if (result.OriginalString)
                                    {
                                        context.Response.Write(result.data);
                                    }
                                    else
                                    {
                                        context.Response.Write(
                                            JsonConvert.SerializeObject(result.m_Showstate ? result : result.data));
                                    }
                                }
                                else if (result.resultCode == 2)
                                {
                                    context.Response.Write(JsonConvert.SerializeObject(result));
                                }
                                else
                                {
                                    context.Response.Write(
                                        JsonConvert.SerializeObject(Result.GetFailedResult(service.ServiceName + "处理失败！")));
                                }
                            }
                        //}
                        //catch (NotRightException ex)
                        //{
                        //    LogHelper.WriteException("Http服务发生异常：" + service.ServiceName, ex);
                        //    context.Response.Write(JsonConvert.SerializeObject(Result.GetFailedResult("该页面权限不足！", ex)));
                        //}
                        //catch (CheckException ex)
                        //{
                        //    context.Response.Write(JsonConvert.SerializeObject(Result.GetFailedResult(ex.Mes, ex)));
                        //}
                        //catch (Exception ex)
                        //{
                        //    LogHelper.WriteException("Http服务发生异常：" + service.ServiceName, ex);
                        //    context.Response.Write(JsonConvert.SerializeObject(Result.GetFailedResult("该页面处理异常请反馈！", ex)));
                        //}
                    }
                    else
                    {
                        context.Response.Write(
                            JsonConvert.SerializeObject(
                            Result.GetDeniedResult("客户端发出错误的服务请求类型：" + serviceCategory)));
                    }
                }
            }
            finally
            {
                try
                {
                    if (context.Response.IsClientConnected)
                    {
                        context.Response.Flush();

                    }
                }
                catch { }
            }
        }

        public bool Check(HttpServiceBase service,HttpRequest req)
        { 
            switch (service.RightType)
            {
                case RightEnum.NoCheck:
                    return true;
                case RightEnum.OnlyUrlRight:
                    return loginService.CheckUrlRight(service.RightUrl, service.GetQueryString(req));
                case RightEnum.UrlAndSoftRight:
                    return loginService.CheckUrlRight(service.RightUrl, service.GetQueryString(req)) && CheckSoft(req["softs"]); 
                case RightEnum.UrlAndProjectSourceRight:
                    return loginService.CheckUrlRight(service.RightUrl, service.GetQueryString(req)) && CheckProject(req["projectsource"]);
                case RightEnum.DefinedByYourself:
                    return service.Check(req);
                case RightEnum.DefinedByYourselfAndUrlAndSoftRight:
                    return loginService.CheckUrlRight(service.RightUrl, service.GetQueryString(req)) && CheckSoft(req["softs"])&& service.Check(req);
                default:
                    return true; 
            }
        }
        /// <summary>
        /// 设置默认选择的项目和平台
        /// </summary>
        /// <param name="context"></param>
        private void SetDefaultProjectAndPlat(HttpContext context)
        {
            var req = context.Request;
            string projectsouce = "";
            string platform = "4";//默认android
            if (req["projectsource"] != null)
                projectsouce = req["projectsource"];
            if (req["project"] != null)
                projectsouce = req["project"];
            if (req["platform"] != null)
                platform = req["platform"];
            if(req["plat"] != null)
                platform = req["plat"];
            if (projectsouce!="")
                UtilityHelp.SetDefaultProjectAndPlat(context, projectsouce, platform);
        }

        private void SetDefaultSofts(HttpContext context)
        {
            var req = context.Request;
            string soft = string.Empty;
            if (req["softs"] != null)
                soft = req["softs"];
            UtilityHelp.SetDefaultSoft(context,soft);
        }

        /// <summary>
        /// 获取进来参数
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetParasStr(HttpRequest req)
        {
            return "softs:" + req["softs"] + ",projectsource:" + req["projectsource"];
        }
    }
}