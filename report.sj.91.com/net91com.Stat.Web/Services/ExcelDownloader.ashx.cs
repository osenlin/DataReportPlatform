using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Stat.Web.Base;
using net91com.Reports.UserRights;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Stat.Web.Reports.Services;
using net91com.Reports.Entities;

namespace net91com.Stat.Web.Services
{
    /// <summary>
    /// EXCEL文件下载接口
    /// </summary>
    public class ExcelDownloader : HandlerBase
    {
        /// <summary>
        /// 接口主入口
        /// </summary>
        /// <param name="context"></param>
        public override void ProcessRequest(HttpContext context)
        {
            #region 获取公共参数

            int softId = string.IsNullOrEmpty(ThisRequest["SoftID"]) ? -1 : Convert.ToInt32(ThisRequest["SoftID"]);
            int platform = string.IsNullOrEmpty(ThisRequest["Platform"]) ? -1 : Convert.ToInt32(ThisRequest["Platform"]);
            int period = string.IsNullOrEmpty(ThisRequest["Period"]) ? -1 : Convert.ToInt32(ThisRequest["Period"]);
            DateTime startDate = string.IsNullOrEmpty(ThisRequest["StartDate"]) ? DateTime.MinValue : Convert.ToDateTime(ThisRequest["StartDate"]);
            DateTime endDate = string.IsNullOrEmpty(ThisRequest["EndDate"]) ? DateTime.MinValue : Convert.ToDateTime(ThisRequest["EndDate"]);
            int[] chlIds = string.IsNullOrEmpty(ThisRequest["ChannelIds"]) ? new int[] { -1 } : ThisRequest["ChannelIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToInt32(a)).ToArray();
            ChannelTypeOptions[] chlTypes = string.IsNullOrEmpty(ThisRequest["ChannelTypes"]) ? new ChannelTypeOptions[] { ChannelTypeOptions.Category } : ThisRequest["ChannelTypes"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => (ChannelTypeOptions)Convert.ToInt32(a)).ToArray();
            string[] chlTexts = string.IsNullOrEmpty(ThisRequest["ChannelTexts"]) ? new string[] { string.Empty } : ThisRequest["ChannelTexts"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (chlIds.Length != chlTypes.Length || chlIds.Length != chlTexts.Length)
            {
                chlIds = new int[] { -1 };
                chlTypes = new ChannelTypeOptions[] { ChannelTypeOptions.Category };
                chlTexts = new string[] { string.Empty };
            }

            #endregion

            //使用GB2312输出
            ThisResponse.ContentEncoding = Encoding.GetEncoding("GB2312");

            switch (ThisRequest["Action"])
            {
                case "GetStatRetainedUsers":
                    GetStatRetainedUsers(softId, platform, period, startDate, endDate, chlIds, chlTypes, chlTexts);
                    break;
                case "GetStatRetainedActiveUsers":
                    GetStatRetainedActiveUsers(softId, platform, startDate, endDate, chlIds, chlTypes, chlTexts);
                    break;
                case "GetStatUsersByArea":
                    GetStatUsersByArea(softId, platform, period, startDate, endDate, chlIds, chlTypes, chlTexts);
                    break;
                case "GetStatRetainedUsersByArea":
                    GetStatRetainedUsersByArea(softId, platform, period, startDate, endDate, chlIds, chlTypes, chlTexts);
                    break;
                case "GetStatRetainedActiveUsersByArea":
                    GetStatRetainedActiveUsersByArea(softId, platform, startDate, endDate, chlIds, chlTypes, chlTexts);
                    break;
                case "GetStatUsersByVersion":
                    GetStatUsersByVersion(softId, platform, period, startDate, endDate, chlIds, chlTypes, chlTexts);
                    break;
                case "GetStatUsersByCountryByVersionEn":
                    GetStatUsersByCountryByVersionEn(softId, platform, startDate, endDate);
                    break;
                case "GetStatRetainedUsersByVersion":
                    GetStatRetainedUsersByVersion(softId, platform, period, startDate, endDate, chlIds, chlTypes, chlTexts);
                    break;
                case "GetStatUsersByVersionTransverse":
                    GetStatUsersByVersionTransverse(softId, platform, period, startDate, endDate, chlIds, chlTypes);
                    break;
                case "GetStatUsersByAreaTransverse":
                    GetStatUsersByAreaTransverse(softId, platform, period, startDate, endDate, chlIds, chlTypes);
                    break;
            }
            ThisResponse.Flush();
        }

        #region 新增用户留存率数据EXCEL文件下载(GetStatRetainedUsers)

        /// <summary>
        /// 新增用户留存率数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        /// <param name="channelTexts"></param>
        private void GetStatRetainedUsers(int softId, int platform, int period, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes, string[] channelTexts)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatRetainedUsers.aspx");
            if (channelIds.Length == 1)
            {
                string fileName = string.Format("新增用户留存率{0}.xls", channelIds[0] > 0 ? "(" + channelTexts[0] + ")" : string.Empty);
                AddHead(fileName);
                List<Sjqd_StatRetainedUsers> users = new StatUsersService().GetStatRetainedUsers(softId, platform, channelIds[0], channelTypes[0], period, startDate, endDate);
                ThisResponse.Write(TableTemplateHelper.BuildStatRetainedUsersTable(period, users, true, string.Empty));
            }
        }

        #endregion

        #region 活跃用户留存率数据EXCEL文件下载(GetStatRetainedActiveUsers)

        /// <summary>
        /// 活跃用户留存率数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        /// <param name="channelTexts"></param>
        private void GetStatRetainedActiveUsers(int softId, int platform, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes, string[] channelTexts)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatRetainedActiveUsers.aspx");
            if (channelIds.Length == 1)
            {
                string fileName = string.Format("活跃用户留存率{0}.xls", channelIds[0] > 0 ? "(" + channelTexts[0] + ")" : string.Empty);
                AddHead(fileName);
                List<Sjqd_StatRetainedUsers> users = new StatUsersService().GetStatRetainedActiveUsers(softId, platform, channelIds[0], channelTypes[0], startDate, endDate);
                ThisResponse.Write(TableTemplateHelper.BuildStatRetainedUsersTable((int)net91com.Stat.Core.PeriodOptions.Daily, users, true, string.Empty, true));
            }
        }

        #endregion

        #region 版本分布数据EXCEL文件下载(GetStatUsersByVersionTransverse)

        /// <summary>
        /// 版本分布数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        private void GetStatUsersByVersionTransverse(int softId, int platform, int period, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatUsersByVersionTransverse.aspx");

            AddHead("版本分布.xls");

            StatUsersService suService = new StatUsersService();
            List<Sjqd_StatUsers> users = suService.GetRankOfVersions(softId, platform, channelIds[0], channelTypes[0], period, ref startDate, ref endDate);
            users = users.OrderByDescending(a => a.NewUserCount).ToList();
            StringBuilder sb = new StringBuilder();
            sb.Append("<table border=\"1\">");
            sb.Append(@"<thead><tr style=""text-align:center;""><th>排名</th><th>版本</th><th>新增用户</th><th>新增占比</th><th>涨跌量</th><th>活跃用户</th><th>活跃占比</th><th>留存率</th></tr></thead><tbody>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.AppendFormat(@"<tr style=""text-align:right;""><td>{0}</td><td style=""text-align:left;"">{1}</td><td>{2:N0}</td>
                                  <td>{3:0.00}%</td><td>{4:N0}</td><td>{5:N0}</td><td>{6:0.00}%</td><td>{7:0.00}</td></tr>", i + 1
                                                                                                               , users[i].Name
                                                                                                               , users[i].NewUserCount
                                                                                                               , users[i].NewUserPercent * 100
                                                                                                               , users[i].NewUserCount - users[i].LastNewUserCount
                                                                                                               , users[i].ActiveUserCount
                                                                                                               , users[i].ActiveUserPercent * 100
                                                                                                               , users[i].OriginalNewUserCount > 0 ? (((double)users[i].RetainedUserCount) / users[i].OriginalNewUserCount).ToString("0.00") + "%" : "");
            }
            sb.Append("</tbody></table>");
            ThisResponse.Write(sb.ToString());
        }

        #endregion

        #region 地区分布数据EXCEL文件下载(GetStatUsersByAreaTransverse)

        /// <summary>
        /// 地区分布数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        private void GetStatUsersByAreaTransverse(int softId, int platform, int period, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatUsersByAreaTransverse.aspx");

            AddHead("地区分布.xls");

            int areaType = string.IsNullOrEmpty(ThisRequest["AreaType"]) ? 1 : Convert.ToInt32(ThisRequest["AreaType"]);
            StatUsersService suService = new StatUsersService();
            List<Sjqd_StatUsers> users;
            switch (areaType)
            {
                case 2:
                    users = suService.GetRankOfProvinces(softId, platform, channelTypes[0], channelIds[0], period, ref startDate, ref endDate);
                    break;
                case 3:
                    users = suService.GetRankOfCities(softId, platform, channelTypes[0], channelIds[0], period, ref startDate, ref endDate);
                    break;
                default:
                    users = suService.GetRankOfCountries(softId, platform, channelTypes[0], channelIds[0], period, ref startDate, ref endDate);
                    break;
            }            
            users = users.OrderByDescending(a => a.NewUserCount).ToList();
            StringBuilder sb = new StringBuilder();
            sb.Append("<table border=\"1\">");
            sb.Append(@"<thead><tr style=""text-align:center;""><th>排名</th><th>地区</th><th>新增用户</th><th>新增占比</th><th>涨跌量</th><th>活跃用户</th><th>活跃占比</th><th>留存率</th></tr></thead><tbody>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.AppendFormat(@"<tr style=""text-align:right;""><td>{0}</td><td style=""text-align:left;"">{1}</td><td>{2:N0}</td>
                                  <td>{3:0.00}%</td><td>{4:N0}</td><td>{5:N0}</td><td>{6:0.00}%</td><td>{7:0.00}</td></tr>", i + 1
                                                                                                               , users[i].Name
                                                                                                               , users[i].NewUserCount
                                                                                                               , users[i].NewUserPercent * 100
                                                                                                               , users[i].NewUserCount - users[i].LastNewUserCount
                                                                                                               , users[i].ActiveUserCount
                                                                                                               , users[i].ActiveUserPercent * 100
                                                                                                               , users[i].OriginalNewUserCount > 0 ? (((double)users[i].RetainedUserCount) / users[i].OriginalNewUserCount).ToString("0.00") + "%" : "");
            }
            sb.Append("</tbody></table>");
            ThisResponse.Write(sb.ToString());
        }

        #endregion

        #region 分版本用户数据EXCEL文件下载(GetStatUsersByVersion)

        /// <summary>
        /// 分版本用户数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        /// <param name="channelTexts"></param>
        private void GetStatUsersByVersion(int softId, int platform, int period, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes, string[] channelTexts)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatUsersByVersion.aspx");
            //版本ID
            string[] versionIds = string.IsNullOrEmpty(ThisRequest["VersionIds"]) ? new string[0] : ThisRequest["VersionIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] versionNames = string.IsNullOrEmpty(ThisRequest["VersionNames"]) ? new string[0] : ThisRequest["VersionNames"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (versionIds.Length > 0 && versionIds.Length == versionNames.Length)
            {
                AddHead("分版本统计.xls");

                StringBuilder firstRowBuilder = new StringBuilder("<tr style=\"text-align:center\"><th rowspan=\"2\">日期</th>");
                StringBuilder secondRowBuilder = new StringBuilder("<tr style=\"text-align:center\">");
                Dictionary<string, StringBuilder> dataRowBuilders = new Dictionary<string, StringBuilder>();
                List<DateTime> dates = GetDateList((net91com.Stat.Core.PeriodOptions)period, startDate, endDate);
                for (int i = 0; i < dates.Count; i++)
                {
                    dataRowBuilders.Add(dates[i].ToString("yyyy-MM-dd"), new StringBuilder("<tr style=\"text-align:right\"><td>" + dates[i].ToString("yyyy-MM-dd") + "</td>"));
                }
                StringBuilder totalRowBuilder = new StringBuilder("<tr style=\"text-align:right\"><td>总计</td>");
                StringBuilder avgRowBuilder = new StringBuilder("<tr style=\"text-align:right\"><td>平均</td>");
                //获取分地区用户数据并输出结果
                StatUsersService suService = new StatUsersService();
                for (int i = 0; i < channelIds.Length; i++)
                {
                    for (int j = 0; j < versionIds.Length; j++)
                    {
                        firstRowBuilder.AppendFormat("<th colspan=\"2\">{0}{1}</th>", string.IsNullOrEmpty(channelTexts[i]) ? "" : channelTexts[i] + "-", versionNames[j]);
                        secondRowBuilder.Append("<th>新增用户</th><th>活跃用户</th>");
                        List<Sjqd_StatUsers> statUsersList = suService.GetStatUsersByVersion(softId, platform, channelTypes[i], channelIds[i], versionIds[j], period, startDate, endDate).OrderByDescending(a => a.StatDate).ToList();
                        for (int k = 0, l = 0; l < dates.Count; l++)
                        {
                            if (k < statUsersList.Count && dates[l] == statUsersList[k].StatDate)
                            {
                                dataRowBuilders[dates[l].ToString("yyyy-MM-dd")].AppendFormat("<td>{0:N0}</td><td>{1:N0}</td>", statUsersList[k].NewUserCount, statUsersList[k].ActiveUserCount);
                                k++;
                            }
                            else
                            {
                                dataRowBuilders[dates[l].ToString("yyyy-MM-dd")].Append("<td></td><td></td>");
                            }
                        }
                        totalRowBuilder.AppendFormat("<td>{0:N0}</td><td></td>", statUsersList.Sum(a => a.NewUserCount));
                        avgRowBuilder.AppendFormat("<td>{0}</td><td>{1}</td>", statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.NewUserCount)).ToString("N0"), statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.ActiveUserCount)).ToString("N0"));
                    }
                }
                ThisResponse.Write("   <table name=\"分版本统计\" border=\"1\">");
                ThisResponse.Write(firstRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                ThisResponse.Write(secondRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                for (int i = 0; i < dates.Count; i++)
                {
                    ThisResponse.Write(dataRowBuilders[dates[i].ToString("yyyy-MM-dd")].ToString());
                    ThisResponse.Write("</tr>");
                }
                ThisResponse.Write(totalRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                ThisResponse.Write(avgRowBuilder.ToString());
                ThisResponse.Write("</tr></table>");
            }
        }

        #endregion

        #region 分国家分版本(海外)用户数据EXCEL文件下载(GetStatUsersByCountryByVersionEn)

        /// <summary>
        /// 分版本用户数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        private void GetStatUsersByCountryByVersionEn(int softId, int platform, DateTime startDate, DateTime endDate)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatUsersByCountryByVersion.aspx");
            //版本
            string[] versionIds = string.IsNullOrEmpty(ThisRequest["VersionIds"]) ? new string[0] : ThisRequest["VersionIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] versionNames = string.IsNullOrEmpty(ThisRequest["VersionNames"]) ? new string[0] : ThisRequest["VersionNames"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            //国家
            string[] countryIds = string.IsNullOrEmpty(ThisRequest["CountryIds"]) ? new string[0] : ThisRequest["CountryIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] countries = string.IsNullOrEmpty(ThisRequest["Countries"]) ? new string[0] : ThisRequest["Countries"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (versionIds.Length > 0 && countryIds.Length > 0 
                && versionIds.Length == versionNames.Length
                && countryIds.Length == countries.Length)
            {
                AddHead("分国家分版本(海外)统计.xls");

                StringBuilder firstRowBuilder = new StringBuilder("<tr style=\"text-align:center\"><th rowspan=\"2\">日期</th>");
                StringBuilder secondRowBuilder = new StringBuilder("<tr style=\"text-align:center\">");
                Dictionary<string, StringBuilder> dataRowBuilders = new Dictionary<string, StringBuilder>();
                List<DateTime> dates = GetDateList(net91com.Stat.Core.PeriodOptions.Daily, startDate, endDate);
                for (int i = 0; i < dates.Count; i++)
                {
                    dataRowBuilders.Add(dates[i].ToString("yyyy-MM-dd"), new StringBuilder("<tr style=\"text-align:right\"><td>" + dates[i].ToString("yyyy-MM-dd") + "</td>"));
                }
                StringBuilder totalRowBuilder = new StringBuilder("<tr style=\"text-align:right\"><td>总计</td>");
                StringBuilder avgRowBuilder = new StringBuilder("<tr style=\"text-align:right\"><td>平均</td>");
                //获取分地区用户数据并输出结果
                StatUsersService suService = new StatUsersService();
                Dictionary<string, List<Sjqd_StatUsers>> users = suService.GetStatUsersByCountryByVersionEn(softId, platform, versionIds.ToList(), countryIds.ToList(), startDate, endDate);
                for (int i = 0; i < versionIds.Length; i++)
                {
                    for (int j = 0; j < countryIds.Length; j++)
                    {
                        firstRowBuilder.AppendFormat("<th colspan=\"2\">{0}-{1}</th>", versionNames[i], countries[j]);
                        secondRowBuilder.Append("<th>新增用户</th><th>活跃用户</th>");
                        B_BaseTool_DataAccess bt = new B_BaseTool_DataAccess();
                        string key = string.Format("{0}-{1}", versionIds[i], countryIds[j]);
                        List<Sjqd_StatUsers> statUsersList = users.ContainsKey(key) ? users[key].OrderByDescending(a => a.StatDate).ToList() : new List<Sjqd_StatUsers>();
                        for (int k = 0, l = 0; l < dates.Count; l++)
                        {
                            if (k < statUsersList.Count && dates[l] == statUsersList[k].StatDate)
                            {
                                dataRowBuilders[dates[l].ToString("yyyy-MM-dd")].AppendFormat("<td>{0:N0}</td><td>{1:N0}</td>", statUsersList[k].NewUserCount, statUsersList[k].ActiveUserCount);
                                k++;
                            }
                            else
                            {
                                dataRowBuilders[dates[l].ToString("yyyy-MM-dd")].Append("<td></td><td></td>");
                            }
                        }
                        totalRowBuilder.AppendFormat("<td>{0:N0}</td><td></td>", statUsersList.Sum(a => a.NewUserCount));
                        avgRowBuilder.AppendFormat("<td>{0}</td><td>{1}</td>", statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.NewUserCount)).ToString("N0"), statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.ActiveUserCount)).ToString("N0"));
                    }
                }
                ThisResponse.Write("   <table name=\"分国家分版本(海外)统计\" border=\"1\">");
                ThisResponse.Write(firstRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                ThisResponse.Write(secondRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                for (int i = 0; i < dates.Count; i++)
                {
                    ThisResponse.Write(dataRowBuilders[dates[i].ToString("yyyy-MM-dd")].ToString());
                    ThisResponse.Write("</tr>");
                }
                ThisResponse.Write(totalRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                ThisResponse.Write(avgRowBuilder.ToString());
                ThisResponse.Write("</tr></table>");
            }
        }

        #endregion

        #region 新增用户留存率分版本数据EXCEL文件下载(GetStatRetainedUsersByVersion)

        /// <summary>
        /// 新增用户留存率分版本数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        /// <param name="channelTexts"></param>
        private void GetStatRetainedUsersByVersion(int softId, int platform, int period, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes, string[] channelTexts)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatRetainedUsersByVersion.aspx");
            //版本ID
            string[] versionIds = string.IsNullOrEmpty(ThisRequest["VersionIds"]) ? new string[0] : ThisRequest["VersionIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] versionNames = string.IsNullOrEmpty(ThisRequest["VersionNames"]) ? new string[0] : ThisRequest["VersionNames"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (versionIds.Length == 1 && versionIds.Length == versionNames.Length && channelIds.Length == 1)
            {
                string fileName = string.Format("新增用户留存率({0}{1}).xls", channelIds[0] > 0 ? channelTexts[0] + "-" : string.Empty, versionNames[0]);
                AddHead(fileName);
                List<Sjqd_StatRetainedUsers> users = new StatUsersService().GetStatRetainedUsersByVersion(softId, platform, versionIds[0], channelIds[0], channelTypes[0], period, startDate, endDate);
                ThisResponse.Write(TableTemplateHelper.BuildStatRetainedUsersTable(period, users, true, string.Empty));
            }
        }

        #endregion

        #region 分地区用户数据EXCEL文件下载(GetStatUsersByArea)

        /// <summary>
        /// 分地区用户数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        /// <param name="channelTexts"></param>
        private void GetStatUsersByArea(int softId, int platform, int period, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes, string[] channelTexts)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatUsersByArea.aspx");
            //地区ID
            string[] areaIds = string.IsNullOrEmpty(ThisRequest["AreaIds"]) ? new string[0] : ThisRequest["AreaIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] areaNames = string.IsNullOrEmpty(ThisRequest["AreaNames"]) ? new string[0] : ThisRequest["AreaNames"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (areaIds.Length > 0 && areaIds.Length == areaNames.Length)
            {
                AddHead("分地区统计.xls");

                StringBuilder firstRowBuilder = new StringBuilder("<tr style=\"text-align:center\"><th rowspan=\"2\">日期</th>");
                StringBuilder secondRowBuilder = new StringBuilder("<tr style=\"text-align:center\">");
                Dictionary<string, StringBuilder> dataRowBuilders = new Dictionary<string, StringBuilder>();
                List<DateTime> dates = GetDateList((net91com.Stat.Core.PeriodOptions)period, startDate, endDate);
                for (int i = 0; i < dates.Count; i++)
                {
                    dataRowBuilders.Add(dates[i].ToString("yyyy-MM-dd"), new StringBuilder("<tr style=\"text-align:right\"><td>" + dates[i].ToString("yyyy-MM-dd") + "</td>"));
                }
                StringBuilder totalRowBuilder = new StringBuilder("<tr style=\"text-align:right\"><td>总计</td>");
                StringBuilder avgRowBuilder = new StringBuilder("<tr style=\"text-align:right\"><td>平均</td>");
                //获取分地区用户数据并输出结果
                StatUsersService suService = new StatUsersService();
                for (int i = 0; i < channelIds.Length; i++)
                {
                    for (int j = 0; j < areaIds.Length; j++)
                    {
                        firstRowBuilder.AppendFormat("<th colspan=\"4\">{0}{1}</th>", string.IsNullOrEmpty(channelTexts[i]) ? "" : channelTexts[i] + "-", areaNames[j]);
                        secondRowBuilder.Append("<th>新增用户</th><th>新增价值用户</th><th>活跃用户</th><th>活跃价值用户</th>");
                        List<Sjqd_StatUsers> statUsersList = suService.GetStatUsersByArea(softId, platform, channelTypes[i], channelIds[i], areaIds[j], period, startDate, endDate).OrderByDescending(a => a.StatDate).ToList();
                        for (int k = 0, l = 0; l < dates.Count; l++)
                        {
                            if (k < statUsersList.Count && dates[l] == statUsersList[k].StatDate)
                            {
                                dataRowBuilders[dates[l].ToString("yyyy-MM-dd")].AppendFormat("<td>{0:N0}</td><td>{1:N0}</td><td>{2:N0}</td><td>{3:N0}</td>", statUsersList[k].NewUserCount, statUsersList[k].DownNewUserCount, statUsersList[k].ActiveUserCount, statUsersList[k].DownActiveUserCount);
                                k++;
                            }
                            else
                            {
                                dataRowBuilders[dates[l].ToString("yyyy-MM-dd")].Append("<td></td><td></td><td></td><td></td>");
                            }
                        }
                        totalRowBuilder.AppendFormat("<td>{0:N0}</td><td>{1:N0}</td><td></td><td></td>", statUsersList.Sum(a => a.NewUserCount), statUsersList.Sum(a => a.DownNewUserCount));
                        avgRowBuilder.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>"
                            , statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.NewUserCount)).ToString("N0")
                            , statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.DownNewUserCount)).ToString("N0")
                            , statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.ActiveUserCount)).ToString("N0")
                            , statUsersList.Count == 0 ? "" : ((int)statUsersList.Average(a => a.DownActiveUserCount)).ToString("N0"));
                    }
                }
                ThisResponse.Write("<table border=\"1\">");
                ThisResponse.Write(firstRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                ThisResponse.Write(secondRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                for (int i = 0; i < dates.Count; i++)
                {
                    ThisResponse.Write(dataRowBuilders[dates[i].ToString("yyyy-MM-dd")].ToString());
                    ThisResponse.Write("</tr>");
                }
                ThisResponse.Write(totalRowBuilder.ToString());
                ThisResponse.Write("</tr>");
                ThisResponse.Write(avgRowBuilder.ToString());
                ThisResponse.Write("</tr></table>");
            }
        }

        #endregion

        #region 新增用户留存率分地区数据EXCEL文件下载(GetStatRetainedUsersByArea)

        /// <summary>
        /// 新增用户留存率分地区数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        /// <param name="channelTexts"></param>
        private void GetStatRetainedUsersByArea(int softId, int platform, int period, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes, string[] channelTexts)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatRetainedUsersByArea.aspx");
            //地区ID
            string[] areaIds = string.IsNullOrEmpty(ThisRequest["AreaIds"]) ? new string[0] : ThisRequest["AreaIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] areaNames = string.IsNullOrEmpty(ThisRequest["AreaNames"]) ? new string[0] : ThisRequest["AreaNames"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (areaIds.Length == 1 && areaIds.Length == areaNames.Length && channelIds.Length == 1)
            {
                string fileName = string.Format("新增用户留存率({0}{1}).xls", channelIds[0] > 0 ? channelTexts[0] + "-" : string.Empty, areaNames[0]);
                AddHead(fileName);
                List<Sjqd_StatRetainedUsers> users = new StatUsersService().GetStatRetainedUsersByArea(softId, platform, areaIds[0], channelIds[0], channelTypes[0], period, startDate, endDate);
                ThisResponse.Write(TableTemplateHelper.BuildStatRetainedUsersTable(period, users, true, string.Empty));
            }
        }

        #endregion

        #region 活跃用户留存率分地区数据EXCEL文件下载(GetStatRetainedActiveUsersByArea)

        /// <summary>
        /// 活跃用户留存率分地区数据EXCEL文件下载
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channelIds"></param>
        /// <param name="channelTypes"></param>
        /// <param name="channelTexts"></param>
        private void GetStatRetainedActiveUsersByArea(int softId, int platform, DateTime startDate, DateTime endDate, int[] channelIds, ChannelTypeOptions[] channelTypes, string[] channelTexts)
        {
            //权限验证
            CheckHasRight(softId, "Reports/StatRetainedActiveUsersByArea.aspx");
            //地区ID
            string[] areaIds = string.IsNullOrEmpty(ThisRequest["AreaIds"]) ? new string[0] : ThisRequest["AreaIds"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] areaNames = string.IsNullOrEmpty(ThisRequest["AreaNames"]) ? new string[0] : ThisRequest["AreaNames"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (areaIds.Length == 1 && areaIds.Length == areaNames.Length && channelIds.Length == 1)
            {
                string fileName = string.Format("活跃用户留存率({0}{1}).xls", channelIds[0] > 0 ? channelTexts[0] + "-" : string.Empty, areaNames[0]);
                AddHead(fileName);
                List<Sjqd_StatRetainedUsers> users = new StatUsersService().GetStatRetainedActiveUsersByArea(softId, platform, areaIds[0], channelIds[0], channelTypes[0], startDate, endDate);
                ThisResponse.Write(TableTemplateHelper.BuildStatRetainedUsersTable((int)net91com.Stat.Core.PeriodOptions.Daily, users, true, string.Empty, true));
            }
        }

        #endregion

        #region 辅助方法

        #region 获取日期范围列表(GetDateList)

        /// <summary>
        /// 获取日期范围列表
        /// </summary>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private List<DateTime> GetDateList(net91com.Stat.Core.PeriodOptions period, DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = new List<DateTime>();
            for (DateTime dt = endDate; dt >= startDate; )
            {
                switch (period)
                {
                    case net91com.Stat.Core.PeriodOptions.Daily:
                        dates.Add(dt);
                        break;
                    case net91com.Stat.Core.PeriodOptions.Weekly:
                        if (dt.DayOfWeek == DayOfWeek.Sunday)
                            dates.Add(dt);
                        break;
                    case net91com.Stat.Core.PeriodOptions.NaturalMonth:
                        if (dt.AddDays(1).Day == 1)
                            dates.Add(dt);
                        break;
                    case net91com.Stat.Core.PeriodOptions.Monthly:
                        if (dt.Day == 20)
                            dates.Add(dt);
                        break;

                }
                dt = dt.AddDays(-1);
            }
            return dates;
        }

        #endregion

        #endregion
    }
}