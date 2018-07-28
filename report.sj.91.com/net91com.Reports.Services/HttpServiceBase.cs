using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;
//using BY.AccessControlCore;

namespace net91com.Reports.Services
{
    public abstract class HttpServiceBase
    {
        /// <summary>
        ///     注意要和页面名称一一映射
        /// </summary>
        public abstract string ServiceCategory { get; }

        public abstract string ServiceName { get; }

        public abstract RightEnum RightType { get; }

        public abstract string RightUrl { get; }

        public URLoginService LoginService { get; set; }
        public abstract Result Process(HttpContext context);

        public virtual bool Check(object obj)
        {
            return true;
        }

        /// <summary>
        ///     获取附带参数
        /// </summary>
        /// <returns></returns>
        public virtual string GetQueryString(HttpRequest context)
        {
            return null;
        }

        /// <summary>
        ///     设置下载头部
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="title"></param>
        public virtual void SetDownHead(HttpResponse resp, string title, bool needAppendHeadHtml = false,
                                        string encoding = "utf-8")
        {
            UtilityHelp.SetDownHead(resp, title, needAppendHeadHtml, encoding);
        }

        /// <summary>
        ///     生成html 字符串
        /// </summary>
        /// <param name="ths"></param>
        /// <param name="trs"></param>
        /// <returns></returns>
        public virtual string GetTableHtml(string[] ths, List<List<string>> trs)
        {
            var head = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"1\" >");
            var body = new StringBuilder();
            var bottom = new StringBuilder("</table>");
            if (trs.Count == 0)
            {
                body.Append("<tr>");
                for (int i = 0; i < ths.Length; i++)
                {
                    body.AppendFormat("<td>{0}</td>", ths[i]);
                }
                body.Append("</tr>");

                return head + body.ToString() + bottom;
            }
            else if (ths.Length != trs[0].Count)
            {
                return head + "<tr><td></td></tr>" + bottom;
            }
            else
            {
                body.Append("<tr>");
                for (int i = 0; i < ths.Length; i++)
                {
                    body.AppendFormat("<td>{0}</td>", ths[i]);
                }
                body.Append("</tr>");
                for (int i = 0; i < trs.Count; i++)
                {
                    body.Append("<tr>");
                    for (int j = 0; j < trs[i].Count; j++)
                    {
                        body.AppendFormat("<td>{0}</td>", trs[i][j]);
                    }
                    body.Append("</tr>");
                }
                return head.ToString() + body + bottom;
            }
        }


        /// 每天的x轴线
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetXLine(DateTime begintime, DateTime endtime, int period = 1)
        {
            var list = new List<string>();
            if (period == 1)
            {
                DateTime dt = begintime;
                while (dt <= endtime)
                {
                    list.Add(dt.ToString("yyyyMMdd"));
                    dt = dt.AddDays(1);
                }
                return list;
            }
            else if (period == 3) //周
            {
                DateTime dt = begintime;
                while (dt.DayOfWeek != DayOfWeek.Sunday)
                {
                    dt = dt.AddDays(1);
                }
                while (dt <= endtime)
                {
                    list.Add(dt.ToString("yyyyMMdd"));
                    dt = dt.AddDays(7);
                }
                return list;
            }
            else if (period == 5) //月
            {
                DateTime dt = begintime;
                while (dt.Day != 20)
                {
                    dt = dt.AddDays(1);
                }
                while (dt <= endtime)
                {
                    list.Add(dt.ToString("yyyyMMdd"));
                    dt = dt.AddMonths(1);
                }
                return list;
            }
            else if (period == 12) //自然月
            {
                DateTime dt = begintime;
                while (dt.AddDays(1).Day != 1)
                {
                    dt = dt.AddDays(1);
                }
                while (dt <= endtime)
                {
                    list.Add(dt.ToString("yyyyMMdd"));
                    dt = dt.AddDays(1).AddMonths(1).AddDays(-1);
                }
                return list;
            }
            else
                return list;
        }

        public virtual void OutputOnSuccess(HttpContext context, object returnObj, bool showstate)
        {
            context.Response.Write(
                JsonConvert.SerializeObject(
                    Result.GetSuccessedResult(returnObj, showstate)));
        }

        public virtual void OutputOnDenied(HttpContext context, string message)
        {
            context.Response.Write(
                JsonConvert.SerializeObject(
                    Result.GetDeniedResult(message)));
        }

        public virtual void OutputOnFailure(HttpContext context, string message)
        {
            context.Response.Write(
                JsonConvert.SerializeObject(
                    Result.GetFailedResult(message, null)));
        }

        public virtual void OutputOnException(HttpContext context, Exception ex)
        {
            context.Response.Write(
                JsonConvert.SerializeObject(
                    Result.GetFailedResult(ex.Message, ex)));
        }

        public virtual void OutputOnException(HttpContext context, string message, Exception ex)
        {
            context.Response.Write(
                JsonConvert.SerializeObject(
                    Result.GetFailedResult(message, ex)));
        }
    }
}