using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using net91com.Core;
using net91com.Core.Web;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.UserRights;
using net91com.Stat.Core;

namespace net91com.Reports.Services
{
    public class UtilityHelp
    {

        public static List<MobileOption> GetMobileList(bool filter)
        {
            var list = new List<MobileOption>();
            Array array = Enum.GetValues(typeof (MobileOption));
            foreach (MobileOption item in array)
            {
                if (filter)
                {
                    if (item != MobileOption.None && item != MobileOption.All)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    list.Add(item);
                }
            }
            return list.OrderBy(p => p.GetPlatIndex()).ToList();
        }

        /// <summary>
        ///     根据项目ID获取缓存名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetProjectSoureNameByID(int id)
        {
            if (id == -1)
                return "不区分来源";
            else
            {
                //缓存一个小时的
                List<Config_ProjectSource> allSoures =
                    Sjqd_ProjectSource_DataAccess.Instance.GetConfig_ProjectSourceByIDCache(CacheTimeOption.OneHour);
                //allSoures.Add(new Config_ProjectSource());
                Config_ProjectSource source = allSoures
                    .Find(p => p.ProjectSourceID == id);
                if (source == null)
                {
                    return "未知来源";
                }
                else
                {
                    return source.ProjectSourceName;
                }
            }
        }

        /// <summary>
        ///     获取所有项目来源
        /// </summary>
        /// <returns></returns>
        public static List<Config_ProjectSource> GetAllProjectSources()
        {
            return Sjqd_ProjectSource_DataAccess.Instance.GetConfig_ProjectSourceByIDCache(CacheTimeOption.OneHour);
        }

        /// <summary>
        ///     编辑推荐标准时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetTime(DateTime standartTime, DateTime time, bool starttime)
        {
            if (standartTime.Year == time.Year && standartTime.Month == time.Month && standartTime.Day == time.Day)
            {
                return time;
            }
            else if (starttime)
            {
                return standartTime;
            }
            else
            {
                return standartTime.AddDays(1).AddSeconds(-1);
            }
        }

        /// <summary>
        ///     不同数值不同颜色
        /// </summary>
        /// <param name="needSpeacilStyple"></param>
        /// <param name="doublevalue"></param>
        /// <param name="wholeStyle">是否是整个样式风格字符串</param>
        /// <returns></returns>
        public static string GetStyle(int needSpeacilStyple, double doublevalue, bool wholeStyle = true)
        {
            string style = string.Empty;
            if (needSpeacilStyple > 0 && doublevalue > 0)
            {
                style = wholeStyle ? "style=\"color:red\"" : "color:red\"";
            }
            else if (needSpeacilStyple > 0 && doublevalue < 0)
            {
                style = wholeStyle ? "style=\"color:green\"" : "color:green\"";
            }
                //下面就是相反的颜色
            else if (needSpeacilStyple < 0 && doublevalue > 0)
            {
                style = wholeStyle ? "style=\"color:green\"" : "color:green\"";
            }
            else if (needSpeacilStyple < 0 && doublevalue < 0)
            {
                style = wholeStyle ? "style=\"color:red\"" : "color:red\"";
            }
            else
            {
                style = wholeStyle ? "style=\"\"" : "\"";
            }
            return style;
        }

        /// <summary>
        ///     格式化数字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string FormatNum<T>(T a, bool zeroshow = true)
        {
            try
            {
                if (zeroshow)
                {
                    return string.Format("{0:N0}", a);
                }
                else
                {
                    if (Convert.ToInt64(a) == 0)
                    {
                        return "--";
                    }
                    else
                        return string.Format("{0:N0}", a);
                }
            }
            catch (Exception)
            {
                return "--";
            }
        }

        /// <summary>
        ///     获取字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="formata"></param>
        /// <returns></returns>
        public static string GetDecimalDataString<T>(T a, string format = "0.00")
        {
            try
            {
                decimal b = Convert.ToDecimal(a);
                if (b == 0)
                    return "--";
                return b.ToString(format);
            }
            catch (Exception)
            {
                return "--";
            }
        }

        /// <summary>
        ///     格式化百分比
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="allownegative">允许负数</param>
        /// <returns></returns>
        public static string FormatPercent<T>(T a, bool allownegative = false, string format = "0.00%")
        {
            try
            {
                decimal b = Convert.ToDecimal(a);
                if (allownegative)
                {
                    return b.ToString(format);
                }
                else
                {
                    if (b <= 0)
                        return "--";
                    return b.ToString(format);
                }
            }
            catch (Exception)
            {
                return "--";
            }
        }

        /// <summary>
        ///     得出均值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fenmu"></param>
        /// <param name="fenzi"></param>
        /// <returns></returns>
        public static int GetAvg<T>(T fenmu, T fenzi)
        {
            try
            {
                decimal a = Convert.ToDecimal(fenmu);
                decimal b = Convert.ToDecimal(fenzi);
                if (Convert.ToInt64(fenzi) == 0)
                    return 0;
                return (int) (a/b);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        ///     获取百分比例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fenmu"></param>
        /// <param name="fenzi"></param>
        /// <returns></returns>
        public static string GetPercent<T>(T fenmu, T fenzi)
        {
            try
            {
                decimal a = Convert.ToDecimal(fenmu);
                decimal b = Convert.ToDecimal(fenzi);
                if (a == 0)
                    return "0.00%";
                return (b/a).ToString("0.00%");
            }
            catch (Exception)
            {
                return "--";
            }
        }

        /// <summary>
        ///     获取百分比数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fenmu"></param>
        /// <param name="fenzi"></param>
        /// <returns></returns>
        public static decimal GetPercent_Decimal<T>(T fenmu, T fenzi)
        {
            decimal a = Convert.ToDecimal(fenmu);
            decimal b = Convert.ToDecimal(fenzi);
            if (a == 0)
                return 0;
            return Math.Round((b/a), 6);
        }

        public static string GetDownStatTypeString(int i)
        {
            switch (i)
            {
                case 1:
                    return "下载点击";
                case 4:
                    return "下载成功";
                case 8:
                    return "下载失败";
                case 5:
                    return "安装成功";
                case 6:
                    return "安装失败";
                case 2:
                    return "下载浏览";
                default:
                    return "";
            }
        }


        public static string GetMessageStatTypeString(int i)
        {
            switch (i)
            {
                case 11:
                    return "已到达";
                case 12:
                    return "已阅读";
                case 13:
                    return "已触发";
                default:
                    return "";
            }
        }

        /// <summary>
        ///     ip 转 long
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long ConvertIpToLong(string ip)
        {
            string[] arr = ip.Split('.');
            if (arr.Length != 4)
                return 0L;
            try
            {
                return (long.Parse(arr[0]) << 24)
                       | (long.Parse(arr[1]) << 16)
                       | (long.Parse(arr[2]) << 8)
                       | long.Parse(arr[3]);
            }
            catch
            {
                return 0L;
            }
        }

        /// <summary>
        ///     规范时间
        /// </summary>
        public static void SpecificateTime(ref DateTime begintime, ref DateTime endtime, int period)
        {
            //周
            if (period == (int) PeriodOptions.Weekly)
            {
                while (!(begintime.DayOfWeek == DayOfWeek.Monday))
                {
                    begintime = begintime.AddDays(-1);
                }
                while (!(endtime.DayOfWeek == DayOfWeek.Sunday))
                {
                    endtime = endtime.AddDays(-1);
                }
            } //月
            if (period == (int) PeriodOptions.Monthly)
            {
                while (!(begintime.Day == 21))
                {
                    begintime = begintime.AddDays(-1);
                }
                while (!(endtime.Day == 20))
                {
                    endtime = endtime.AddDays(-1);
                }
            }
        }

        /// <summary>
        ///     获取标准的统计时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="period"></param>
        public static void SpecificateSingleTime(ref DateTime time, PeriodOptions period)
        {
            //周
            if (period == PeriodOptions.Weekly)
            {
                while (!(time.DayOfWeek == DayOfWeek.Sunday))
                {
                    time = time.AddDays(-1);
                }
            }
                //月
            else if (period == PeriodOptions.Monthly)
            {
                while (!(time.Day == 20))
                {
                    time = time.AddDays(-1);
                }
            }
            else if (period == PeriodOptions.NaturalMonth)
            {
                while (!(time.AddDays(1).Day == 1))
                {
                    time = time.AddDays(-1);
                }
            }
        }

        /// <summary>
        ///     32位(utf-8)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StrMd5By32(string str)
        {
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            byte[] s = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(str));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + string.Format("{0:x2}", s[i]);
            }
            return pwd;
        }

        /// <summary>
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="title"></param>
        /// <param name="needAppendHeadHtml">是否添加兼容性的html</param>
        public static void SetDownHead(HttpResponse resp, string title, bool needAppendHeadHtml = false,
                                       string encoding = "utf-8")
        {
            resp.ContentEncoding = Encoding.GetEncoding(encoding);
            if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("msie") > -1)
            {
                title = HttpUtility.UrlPathEncode(title);
            }

            if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                resp.AddHeader("Content-Type", "application/vnd.ms-excel");
                resp.AddHeader("Content-Disposition", "attachment;filename=\"" + title + "\"");
            }
            else
            {
                resp.AddHeader("Content-Type", "application/vnd.ms-excel");
                resp.AddHeader("Content-Disposition", "attachment;filename=" + title);
            }
            if (needAppendHeadHtml)
                resp.Write("<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=UTF-8\"/>");
        }

        /// <summary>
        ///     向客户端输出EXCEL文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void WriteExcel(string fileName, string content)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            SetDownHead(HttpContext.Current.Response, fileName);
            HttpContext.Current.Response.Write(
                "<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=UTF-8\"/>");
            HttpContext.Current.Response.Write(content);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        ///     向客户端输出本地文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <param name="deletefile"></param>
        public static void ResposeAttachFile(string filename, string path, bool deletefile = true)
        {
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            SetDownHead(HttpContext.Current.Response, filename);
            HttpContext.Current.Response.WriteFile(path);
            HttpContext.Current.Response.Flush();
            if (deletefile)
                File.Delete(path);
            HttpContext.Current.Response.End();
        }


        /// <summary>
        ///     对外获取默认项目来源
        /// </summary>
        /// <param name="context"></param>
        /// <param name="availableProjectSource"></param>
        /// <returns></returns>
        public static List<int> GetDefaultProjectSources(HttpContext context, List<ProjectSource> availableProjectSource)
        {
            if (availableProjectSource.Count == 0)
            {
                return new List<int>();
            }
            List<int> defaultProjectsource = GetDefaultListFromCookie(context, 1);
            if (defaultProjectsource.Count == 0 ||
                !availableProjectSource.Select(p => p.ProjectSourceID).Contains(defaultProjectsource[0]))
            {
                defaultProjectsource.Add(availableProjectSource[0].ProjectSourceID);
                SetDefaultValueToCookie(context,
                                        string.Join(",", defaultProjectsource.Select(p => p.ToString()).ToArray()), "");
            }
            return defaultProjectsource;
        }

        /// <summary>
        ///     获取默认平台
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<int> GetDefaultPlatform(HttpContext context)
        {
            List<int> defaultPlats = GetDefaultListFromCookie(context, 2);
            if (defaultPlats.Count == 0)
            {
                //默认android
                defaultPlats.Add(4);
                SetDefaultValueToCookie(context, "", string.Join(",", defaultPlats.Select(p => p.ToString()).ToArray()));
            }
            return defaultPlats;
        }

        /// <summary>
        ///     设置选择的项目来源和平台
        /// </summary>
        public static void SetDefaultProjectAndPlat(HttpContext context, string projectsource, string platforms)
        {
            string[] projectsources = projectsource.Split(new[] {','}, StringSplitOptions.None);
            string[] platform = platforms.Split(new[] {','}, StringSplitOptions.None);
            if (projectsources.Length > 0 && projectsources[0] != "null")
                SetDefaultValueToCookie(context, projectsources[0], platform.Length > 0 ? platform[0] : "");
        }

        /// <summary>
        ///     设置软件cookie
        /// </summary>
        /// <param name="context"></param>
        /// <param name="softs"></param>
        public static void SetDefaultSoft(HttpContext context, string softs)
        {
            string[] soft = softs.Split(new[] {','}, StringSplitOptions.None);
            if (soft.Length != 0)
            {
                int softid = 0;
                if (int.TryParse(soft[0], out softid))
                {
                    SetDefaultSoftsToCookie(new List<Soft> {new Soft {ID = softid}}, new List<MobileOption> {});
                }
            }
        }

        public static List<int> GetDefaultSoftsFromCookie(out List<MobileOption> defaultPlatforms)
        {
            var defaultSoftIds = new List<int>();
            defaultPlatforms = new List<MobileOption>();
            HttpCookie cookie = HttpContext.Current.Request.Cookies["DefaultSofts"];
            if (cookie != null)
            {
                string softIdsString = cookie.Values["SoftIds"];
                if (!string.IsNullOrEmpty(softIdsString))
                {
                    int[] softIds =
                        softIdsString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(a => int.Parse(a))
                                     .ToArray();
                    foreach (int sid in softIds)
                        defaultSoftIds.Add(sid);
                }
                string platformsString = cookie.Values["Platforms"];
                if (defaultSoftIds.Count > 0 && !string.IsNullOrEmpty(platformsString))
                {
                    MobileOption[] platforms =
                        platformsString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(a => (MobileOption) int.Parse(a))
                                       .ToArray();
                    foreach (MobileOption p in platforms)
                        defaultPlatforms.Add(p);
                }
            }
            return defaultSoftIds;
        }

        /// <summary>
        ///     设置默认的软件和平台到Cookie
        /// </summary>
        /// <param name="selectedSofts"></param>
        /// <param name="selectedPlatforms"></param>
        public static void SetDefaultSoftsToCookie(List<Soft> selectedSofts, List<MobileOption> selectedPlatforms)
        {
            var cookie = new HttpCookie("DefaultSofts");
            cookie.Values.Add("SoftIds", string.Join(",", selectedSofts.Select(a => a.ID.ToString()).ToArray()));
            if (selectedPlatforms.Count != 0)
                cookie.Values.Add("Platforms",
                                  string.Join(",", selectedPlatforms.Select(a => ((int) a).ToString()).ToArray()));
            cookie.Expires = DateTime.MaxValue;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        /// <summary>
        ///     从cookie中获取默认项目来源或平台
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type">1 是项目来源 2 是平台</param>
        /// <returns></returns>
        private static List<int> GetDefaultListFromCookie(HttpContext context, int type)
        {
            var defaultList = new List<int>();
            HttpCookie cookie = context.Request.Cookies["DefaultProjectSourcePlatform"];
            string key = "ProjectSources";
            if (type == 2)
                key = "Platforms";
            if (cookie != null)
            {
                string stringValues = cookie.Values[key];
                if (!string.IsNullOrEmpty(stringValues))
                {
                    string[] sources =
                        stringValues.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    foreach (string pid in sources)
                    {
                        int s = 0;
                        if (int.TryParse(pid, out s))
                        {
                            defaultList.Add(s);
                        }
                    }
                }
            }
            return defaultList;
        }


        /// <summary>
        ///     设置默认项目来源和平台
        /// </summary>
        /// <param name="selectedSofts"></param>
        /// <param name="selectedPlatforms"></param>
        private static void SetDefaultValueToCookie(HttpContext context, string listProjectValues, string platForms)
        {
            HttpCookie cookie = context.Request.Cookies["DefaultProjectSourcePlatform"];
            if (cookie == null)
                cookie = new HttpCookie("DefaultProjectSourcePlatform");
            if (listProjectValues.Trim() != "")
                cookie.Values["ProjectSources"] = listProjectValues;
            //-1这个平台有的地方用不到 担心兼容性问题，不存入cookie
            if (platForms.Trim() != "" && platForms.Trim() != "-1" && platForms.Trim() != "0")
                cookie.Values["Platforms"] = platForms;
            cookie.Expires = new DateTime(2020, 1, 30);
            context.Response.Cookies.Add(cookie);
        }


        /// <summary>
        ///     获取table 表格
        /// </summary>
        /// <param name="ths"></param>
        /// <param name="trs"></param>
        /// <returns></returns>
        public static string GetTableHtml(string[] ths, List<List<string>> trs)
        {
            var head = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"1\" >");
            var body = new StringBuilder();
            var bottom = new StringBuilder("</table>");
            if (trs.Count == 0)
            {
                return head + "<tr><td></td></tr>" + bottom;
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

        public static bool SendMessage(string title, string content, string to, bool isHtml)
        {
            string hh = WebHelper.GetPage("http://pc.sj.91.com/mail.aspx",
                                          "title=" + title + "&content=" + content + "&to=" + to +
                                          "&sms=0" + "&isHtml=" + (isHtml ? "1" : "0"), "POST",
                                          proxy: "10.1.20.181");
            return hh.StartsWith("OK", StringComparison.OrdinalIgnoreCase);
        }

        #region 模拟post 数据

        public static string WebRequesetWithPost(string url, string postData)
        {
            HttpWebRequest webRequest = null;

            string responseData = "";
            webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "post";
            webRequest.ContentType = "text/json";
            Stream stream = webRequest.GetRequestStream();
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream.Close(); //记住这里一定要close 掉，不然后面会获取不到返回
                stream = null;
            }
            StreamReader responseReader = null;
            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream(),
                                                  Encoding.GetEncoding("utf-8"));
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }
            return responseData;
        }

        #endregion
    }


    public static class PlatExtend
    {
        public static int GetPlatIndex(this MobileOption plat)
        {
            switch (plat)
            {
                case MobileOption.Android:
                    return 2;
                case MobileOption.AndroidPad:
                    return 5;
                case MobileOption.AndroidTV:
                    return 6;
                case MobileOption.IPAD:
                    return 3;
                case MobileOption.PC:
                    return 4;
                case MobileOption.S60:
                    return 9;
                case MobileOption.WM:
                    return 8;
                case MobileOption.WP7:
                    return 5;
                case MobileOption.WebGame:
                    return 7;
                case MobileOption.Win8:
                    return 6;
                case MobileOption.iPhone:
                    return 1;
                default:
                    return 20;
            }
        }
    }
}