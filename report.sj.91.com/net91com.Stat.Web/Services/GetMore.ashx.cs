using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using Newtonsoft.Json;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Stat.Web.Base;
using net91com.Reports.UserRights;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Stat.Web.Reports.Services;
using net91com.Reports.Entities;

namespace net91com.Stat.Web.Services
{
    /// <summary>
    /// 页面异步获取数据接口
    /// </summary>
    public class GetMore : HandlerBase
    {
        /// <summary>
        /// 接口主入口
        /// </summary>
        /// <param name="context"></param>
        public override void ProcessRequest(HttpContext context)
        {
            #region 获取公共参数

            int[] softIds = string.IsNullOrEmpty(ThisRequest["SoftIds"]) ? new int[] { -1 } : ThisRequest["SoftIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
            int[] platforms = string.IsNullOrEmpty(ThisRequest["Platforms"]) ? new int[] { -1 } : ThisRequest["Platforms"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
            int period = string.IsNullOrEmpty(ThisRequest["Period"]) ? -1 : Convert.ToInt32(ThisRequest["Period"]);
            DateTime startDate = string.IsNullOrEmpty(ThisRequest["StartDate"]) ? DateTime.MinValue : Convert.ToDateTime(ThisRequest["StartDate"]);
            DateTime endDate = string.IsNullOrEmpty(ThisRequest["EndDate"]) ? DateTime.MinValue : Convert.ToDateTime(ThisRequest["EndDate"]);
            int[] chlIds = string.IsNullOrEmpty(ThisRequest["ChannelIds"]) ? new int[] { -1 } : ThisRequest["ChannelIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
            ChannelTypeOptions[] chlTypes = string.IsNullOrEmpty(ThisRequest["ChannelTypes"]) ? new ChannelTypeOptions[] { ChannelTypeOptions.Category } : ThisRequest["ChannelTypes"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => (ChannelTypeOptions)Convert.ToInt32(a)).ToArray();
            if (chlIds.Length != chlTypes.Length)
            {
                chlIds = new int[] { -1 };
                chlTypes = new ChannelTypeOptions[] { ChannelTypeOptions.Category };
            }

            #endregion

            //使用GB2312输出
            ThisResponse.ContentEncoding = Encoding.GetEncoding("GB2312");

            switch (ThisRequest["Action"])
            {
                case "GetRankOfSubChannels":
                    GetRankOfSubChannels(softIds[0], platforms[0], period, startDate, endDate, chlIds[0]);
                    break;
                case "GetRankOfVersions":
                    GetRankOfVersions(softIds[0], platforms[0], period, startDate, endDate, chlIds[0], chlTypes[0]);
                    break;
                case "GetRankOfAreas":
                    GetRankOfAreas(softIds[0], platforms[0], period, startDate, endDate, chlIds[0], chlTypes[0]);
                    break;
                case "GetStatUsersByVersionLine":
                    GetStatUsersByVersionLine(softIds[0], platforms[0], period, startDate, endDate, chlIds[0], chlTypes[0]);
                    break;
                case "GetStatUsersByAreaLine":
                    GetStatUsersByAreaLine(softIds[0], platforms[0], period, startDate, endDate, chlIds[0], chlTypes[0]);
                    break;
            }

            ThisResponse.Flush();
        }

        #region 获取子渠道排行(GetRankOfSubChannels)

        /// <summary>
        /// 获取子渠道排行
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelId"></param>
        private void GetRankOfSubChannels(int softId, int platform, int period, DateTime startDate, DateTime endDate, int channelId)
        {
            //权限验证
            CheckHasRight(softId, "Reports/RankOfChannels.aspx");

            int orderby = Convert.ToInt32(ThisRequest["OrderBy"]);
            bool desc = Convert.ToBoolean(ThisRequest["Desc"]);
            List<Sjqd_StatUsers> users = new StatUsersService().GetRankOfChannels(softId, platform, channelId, period, ref startDate, ref endDate);
            if (desc)
                users = (orderby == 0 ? users.OrderByDescending(a => a.NewUserCount) : users.OrderByDescending(a => a.NewUserCount - a.LastNewUserCount)).ToList();
            else
                users = (orderby == 0 ? users.OrderBy(a => a.NewUserCount) : users.OrderBy(a => a.NewUserCount - a.LastNewUserCount)).ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table id=\"tbl{0}\" class=\"tablesorter\" cellspacing=\"1\"><thead><tr style=\"text-align:center;\">", channelId);
            sb.Append("<th>排名</th><th>子渠道</th><th>新增用户</th><th>涨跌量</th><th>累计用户</th><th>活跃用户</th><th>活跃度</th><th>留存率(上周期)</th><th>操作</th></tr></thead><tbody>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.Append("<tr style=\"text-align:right;\">");
                sb.AppendFormat("<td>{0}</td>", i + 1);
                sb.AppendFormat("<td style=\"text-align:left;\"><a href=\"javascript:linkDetail({1},2)\">{0}</a></td>", users[i].Name, users[i].ID);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].NewUserCount);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].NewUserCount - users[i].LastNewUserCount);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].TotalUserCount);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].ActiveUserCount);
                sb.AppendFormat("<td>{0}</td>", users[i].TotalUserCount == 0 ? "" : ((decimal)users[i].ActiveUserCount / users[i].TotalUserCount * 100).ToString("0.00") + "%");
                sb.AppendFormat("<td>{0}</td>", users[i].LastNewUserCount == 0 ? "" : ((decimal)users[i].RetainedUserCount / users[i].LastNewUserCount * 100).ToString("0.00") + "%");
                sb.AppendFormat(@"<td style=""text-align:left;""><span class=""caozuo"" onclick=""getRankOfSubChannels(this,{0});"">子渠道</span>&nbsp;&nbsp;&nbsp;&nbsp;
                                  <span class=""caozuo"" onclick=""getRankOfVersions(this,{0});"">版本</span>&nbsp;&nbsp;&nbsp;&nbsp;
                                  <span class=""caozuo"" onclick=""getRankOfCountries(this,{0});"">国家</span></td></tr>", users[i].ID);
                sb.AppendFormat(@"<tr style=""display:none;"" id=""tr_{0}""><td colspan=""10""><div id=""div_{0}""></div></td></tr>", users[i].ID);
            }
            sb.Append("</tbody></table>");
            object b;
            if (users.Count == 0)
            {
                b = new { code = -1, data = "无数据" };
            }
            else
            {
                b = new { code = 1, data = sb.ToString() };

            }
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(b));
        }

        #endregion

        #region 获取版本排行(GetRankOfVersions)

        /// <summary>
        /// 获取版本排行
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        private void GetRankOfVersions(int softId, int platform, int period, DateTime startDate, DateTime endDate, int channelId, ChannelTypeOptions channelType)
        {
            //权限验证
            CheckHasRight(softId, "Reports/RankOfChannels.aspx");

            int orderby = Convert.ToInt32(ThisRequest["OrderBy"]);
            List<Sjqd_StatUsers> users = new StatUsersService().GetRankOfVersions(softId, platform, channelId, channelType, period, ref startDate, ref endDate);
            users = (orderby == 0 ? users.OrderByDescending(a => a.NewUserCount) : users.OrderByDescending(a => a.NewUserCount - a.LastNewUserCount)).ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table id=\"tbl{0}\" class=\"tablesorter\" cellspacing=\"1\"><thead><tr style=\"text-align:center;\">", channelId);
            sb.Append("<th>排名</th><th>版本</th><th>新增用户</th><th>涨跌量</th><th>活跃用户</th><th>留存率(上周期)</th></tr></thead><tbody>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.Append("<tr style=\"text-align:right;\">");
                sb.AppendFormat("<td>{0}</td>", i + 1);
                sb.AppendFormat("<td style=\"text-align:left;\">{0}</td>", users[i].Name);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].NewUserCount);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].NewUserCount - users[i].LastNewUserCount);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].ActiveUserCount);
                sb.AppendFormat("<td>{0}</td></tr>", users[i].LastNewUserCount == 0 ? "" : ((decimal)users[i].RetainedUserCount / users[i].LastNewUserCount * 100).ToString("0.00") + "%");
            }
            sb.Append("</tbody></table>");
            object b;
            if (users.Count == 0)
            {
                b = new { code = -1, data = "无数据" };
            }
            else
            {
                b = new { code = 1, data = sb.ToString() };

            }
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(b));
        }

        #endregion

        #region 获取地区排行(GetRankOfAreas)

        /// <summary>
        /// 获取地区排行
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        private void GetRankOfAreas(int softId, int platform, int period, DateTime startDate, DateTime endDate, int channelId, ChannelTypeOptions channelType)
        {
            //权限验证
            CheckHasRight(softId, "Reports/RankOfChannels.aspx");

            int areaType = string.IsNullOrEmpty(ThisRequest["AreaType"]) ? 1 : Convert.ToInt32(ThisRequest["AreaType"]);
            int orderby = Convert.ToInt32(ThisRequest["OrderBy"]);
            StatUsersService suService = new StatUsersService();
            List<Sjqd_StatUsers> users;
            switch (areaType)
            {
                case 2:
                    users = suService.GetRankOfProvinces(softId, platform, channelType, channelId, period, ref startDate, ref endDate);
                    break;
                case 3:
                    users = suService.GetRankOfCities(softId, platform, channelType, channelId, period, ref startDate, ref endDate);
                    break;
                default:
                    users = suService.GetRankOfCountries(softId, platform, channelType, channelId, period, ref startDate, ref endDate);
                    break;
            }
            users = (orderby == 0 ? users.OrderByDescending(a => a.NewUserCount) : users.OrderByDescending(a => a.NewUserCount - a.LastNewUserCount)).ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table id=\"tbl{0}\" class=\"tablesorter\" cellspacing=\"1\"><thead><tr style=\"text-align:center;\">", channelId);
            sb.Append("<th>排名</th><th>国家</th><th>新增用户</th><th>涨跌量</th><th>活跃用户</th><th>留存率(上周期)</th></tr></thead><tbody>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.Append("<tr style=\"text-align:right;\">");
                sb.AppendFormat("<td>{0}</td>", i + 1);
                sb.AppendFormat("<td style=\"text-align:left;\">{0}</td>", users[i].Name);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].NewUserCount);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].NewUserCount - users[i].LastNewUserCount);
                sb.AppendFormat("<td>{0:N0}</td>", users[i].ActiveUserCount);
                sb.AppendFormat("<td>{0}</td></tr>", users[i].LastNewUserCount == 0 ? "" : ((decimal)users[i].RetainedUserCount / users[i].LastNewUserCount * 100).ToString("0.00") + "%");
            }
            sb.Append("</tbody></table>");
            object b;
            if (users.Count == 0)
            {
                b = new { code = -1, data = "无数据" };
            }
            else
            {
                b = new { code = 1, data = sb.ToString() };

            }
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(b));
        }

        #endregion

        #region 获取分版本曲线数据(GetStatUsersByVersionLine)

        /// <summary>
        /// 获取分版本曲线数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        public void GetStatUsersByVersionLine(int softId, int platform, int period, DateTime startDate, DateTime endDate, int channelId, ChannelTypeOptions channelType)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatUsersByVersionTransverse.aspx");

            //版本ID
            string[] versionIds = string.IsNullOrEmpty(ThisRequest["VersionIds"]) ? new string[0] : ThisRequest["VersionIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (versionIds.Length > 0)
            {
                StatUsersService suService = new StatUsersService();
                List<Sjqd_StatUsers> statUsersList = suService.GetStatUsersByVersion(softId, platform, channelType, channelId, versionIds[0], period, startDate, endDate);
                LineChart chart = new LineChart(startDate, endDate);
                chart.Period = (net91com.Stat.Core.PeriodOptions)period;
                LineChartLine newUserLine = new LineChartLine
                {
                    Name = "新增",
                    Show = true,
                    XIntervalDays = 0,
                    Points = statUsersList.Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.NewUserCount, DataContext = a }).ToList()
                };
                chart.Y.Add(newUserLine);
                LineChartLine activeUserLine = new LineChartLine
                {
                    Name = "活跃",
                    Show = true,
                    XIntervalDays = 0,
                    Points = statUsersList.Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.ActiveUserCount, DataContext = a }).ToList()
                };
                chart.Y.Add(activeUserLine);
                string axisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140,rotation:-45,x:-30,y:45,step:{1}}}}}"
                    , chart.GetXJson()
                    , chart.Step);

                string seriesJsonStr = chart.GetYJson(
                    delegate(LineChartPoint point)
                    {
                        return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", "", "0");
                    });
                string result = "{ x:" + axisJsonStr + "," + "y:" + seriesJsonStr + "}";
                HttpContext.Current.Response.Write(result);
            }
        }

        #endregion

        #region 获取分地区曲线数据(GetStatUsersByAreaLine)

        /// <summary>
        /// 获取分地区曲线数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        public void GetStatUsersByAreaLine(int softId, int platform, int period, DateTime startDate, DateTime endDate, int channelId, ChannelTypeOptions channelType)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatUsersByAreaTransverse.aspx");

            string[] areaIds = string.IsNullOrEmpty(ThisRequest["AreaIds"]) ? new string[0] : ThisRequest["AreaIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (areaIds.Length > 0)
            {
                StatUsersService suService = new StatUsersService();
                List<Sjqd_StatUsers> statUsersList = suService.GetStatUsersByArea(softId, platform, channelType, channelId, areaIds[0], period, startDate, endDate);
                LineChart chart = new LineChart(startDate, endDate);
                chart.Period = (net91com.Stat.Core.PeriodOptions)period;
                LineChartLine newUserLine = new LineChartLine
                {
                    Name = "新增",
                    Show = true,
                    XIntervalDays = 0,
                    Points = statUsersList.Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.NewUserCount, DataContext = a }).ToList()
                };
                chart.Y.Add(newUserLine);
                LineChartLine activeUserLine = new LineChartLine
                {
                    Name = "活跃",
                    Show = true,
                    XIntervalDays = 0,
                    Points = statUsersList.Select(a => new LineChartPoint { XValue = a.StatDate, YValue = a.ActiveUserCount, DataContext = a }).ToList()
                };
                chart.Y.Add(activeUserLine);
                string axisJsonStr = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140,rotation:-45,x:-30,y:45,step:{1}}}}}"
                    , chart.GetXJson()
                    , chart.Step);

                string seriesJsonStr = chart.GetYJson(
                    delegate(LineChartPoint point)
                    {
                        return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":null", "", "0");
                    });
                string result = "{ x:" + axisJsonStr + "," + "y:" + seriesJsonStr + "}";
                HttpContext.Current.Response.Write(result);
            }
        }

        #endregion
    }
}