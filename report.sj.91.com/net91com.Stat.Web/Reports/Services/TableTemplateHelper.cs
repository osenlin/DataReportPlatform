using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;

using net91com.Core;
using net91com.Stat.Services.sjqd.Entity;

namespace net91com.Stat.Web.Reports.Services
{
    /// <summary>
    /// 表格模板类
    /// </summary>
    internal class TableTemplateHelper
    {
        #region 构建用户量表格

        private static XmlDocument statUsersXmlDoc = null;
        /// <summary>
        /// 构建用户量表格
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>        
        /// <param name="isInternalSoft">是否是内部软件</param>
        /// <param name="byChannel">按渠道查询</param>
        /// <param name="period">统计周期</param>
        /// <param name="reportType"></param>
        /// <param name="onlyOldUser"></param>
        /// <param name="users"></param>
        /// <param name="forDown">是否用于下载(EXCEL)</param>
        /// <param name="tableIndex">多个表格时,用于指明第几张表格</param>
        /// <param name="tableName">表格名称</param>
        /// <returns></returns>
        public static string BuildStatUsersTable(int softId, MobileOption platform, bool isInternalSoft, bool byChannel, net91com.Stat.Core.PeriodOptions period, string reportType, bool onlyOldUser, List<SoftUser> users, bool forDown, int tableIndex, string tableName)
        {
            #region 表头开始

            StringBuilder htmlBuilder = new StringBuilder();
            if (forDown)
                htmlBuilder.Append("<table border=\"1\">");
            else
                htmlBuilder.AppendFormat("<table id=\"tab{0}\" class=\"tablesorter\" name=\"{1}\" {2} cellspacing=\"1\">", tableIndex, tableName, tableIndex != 0 ? "style=\"display:none\"" : "");

            if (statUsersXmlDoc == null)
            {
                statUsersXmlDoc = new XmlDocument();
                statUsersXmlDoc.Load(HttpContext.Current.Server.MapPath("~/DataTables/StatUsers.xml"));
            }
            TableTemplate.IsTrueHandler IsTrue = new TableTemplate.IsTrueHandler(IsTrue_StatUsers);
            NodeCondition_StatUsers nCodition = new NodeCondition_StatUsers
            {
                ByChannel = byChannel,
                SoftID = softId,
                Platform = platform,
                IsInternalSoft = isInternalSoft,
                Period = period,
                OnlyOldUser = onlyOldUser,
                ReportType = reportType
            };
            TableTemplate.ColumnNode rootNode = new TableTemplate.ColumnNode { CorrespondXmlNode = statUsersXmlDoc.DocumentElement };
            //生成表头，并返回数据绑定列
            List<TableTemplate.ColumnNode> dataColumns;
            htmlBuilder.Append(TableTemplate.BuildTableHead(rootNode, IsTrue, nCodition, out dataColumns));

            #endregion

            #region 表体开始

            htmlBuilder.Append("<tbody>");
            StringBuilder averageBuilder = new StringBuilder();
            StringBuilder sumBuilder = new StringBuilder();
            for (int i = 0; i < users.Count; i++)
            {
                bool red = period == net91com.Stat.Core.PeriodOptions.Daily && (users[i].StatDate.DayOfWeek == DayOfWeek.Sunday || users[i].StatDate.DayOfWeek == DayOfWeek.Saturday);
                htmlBuilder.Append("<tr style=\"text-align:right;\">");
                foreach (TableTemplate.ColumnNode col in dataColumns)
                {
                    htmlBuilder.Append(red ? "<td style=\"color:red;\">" : "<td>");
                    switch (col.Name)
                    {
                        case "StatDate":
                            htmlBuilder.AppendFormat("{0:yyyy-MM-dd}", users[i].StatDate);
                            if (i == 0)
                            {
                                averageBuilder.Append("<td>平均</td>");
                                sumBuilder.Append("<td>总计</td>");
                            }
                            break;
                        case "Hour":
                            htmlBuilder.Append(users[i].Hour);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0}</td>", averageBuilder.Length == 0 ? "平均" : "--");
                                sumBuilder.AppendFormat("<td>{0}</td>", sumBuilder.Length == 0 ? "总计" : "--");
                            }
                            break;
                        case "NewUserCount":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].NewNum);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.NewNum));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.NewNum));
                            }
                            break;
                        case "NewUserCount_NotFromCache":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].FirstNewUserCount);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.FirstNewUserCount));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.FirstNewUserCount));
                            }
                            break;
                        case "NewUserCount_Shanzhai":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].NewUserCount_Shanzhai);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.NewUserCount_Shanzhai));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.NewUserCount_Shanzhai));
                            }
                            break;
                        case "NewUserCount_SecAct":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].NewUserCount_SecAct);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.NewUserCount_SecAct));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.NewUserCount_SecAct));
                            }
                            break;
                        case "NewUserCount_SecAct2":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].NewUserCount_SecAct2);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.NewUserCount_SecAct2));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.NewUserCount_SecAct2));
                            }
                            break;
                        case "NewUserCount_Broken":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].NewNum_Broken);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.NewNum_Broken));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.NewNum_Broken));
                            }
                            break;
                        case "NewUserCount_NotBroken":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].NewNum_NotBroken);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.NewNum_NotBroken));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.NewNum_NotBroken));
                            }
                            break;
                        case "NewUserCount_ZJS":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].NewNum_ZJS);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.NewNum_ZJS));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => a.NewNum_ZJS));
                            }
                            break;
                        case "NewUserCount_ValuedUsers":
                            htmlBuilder.AppendFormat("{0:N0}", Math.Max(users[i].FuncValueUsersForNew, users[i].DownValueUsersForNew));
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => Math.Max(a.FuncValueUsersForNew, a.DownValueUsersForNew)));
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Sum(a => Math.Max(a.FuncValueUsersForNew, a.DownValueUsersForNew)));
                            }
                            break;
                        case "ActiveUserCount":
                            htmlBuilder.AppendFormat("{0:N0}", onlyOldUser ? users[i].ActiveNum : users[i].ActiveNum + users[i].NewNum);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", onlyOldUser ? (int)users.Average(a => a.ActiveNum) : (int)users.Average(a => a.ActiveNum + a.NewNum));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "OldUserCount":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].ActiveNum);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", (int)users.Average(a => a.ActiveNum));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "OldPercent":
                            htmlBuilder.Append((((double)users[i].ActiveNum) * 100 / (users[i].ActiveNum + users[i].NewNum)).ToString("0.00") + "%");
                            if (i == 0)
                            {
                                averageBuilder.Append("<td>--</td>");
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "ActiveUserCount_NotFromCache":
                            htmlBuilder.AppendFormat("{0:N0}", onlyOldUser ? users[i].FirstActiveUserCount : users[i].FirstActiveUserCount + users[i].FirstNewUserCount);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", onlyOldUser ? (int)users.Average(a => a.FirstActiveUserCount) : (int)users.Average(a => a.FirstActiveUserCount + a.FirstNewUserCount));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "ActiveUserCount_Shanzhai":
                            htmlBuilder.AppendFormat("{0:N0}", onlyOldUser ? users[i].ActiveUserCount_Shanzhai : users[i].ActiveUserCount_Shanzhai + users[i].NewUserCount_Shanzhai);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", onlyOldUser ? (int)users.Average(a => a.ActiveUserCount_Shanzhai) : (int)users.Average(a => a.ActiveUserCount_Shanzhai + a.NewUserCount_Shanzhai));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "ActiveUserCount_Broken":
                            htmlBuilder.AppendFormat("{0:N0}", onlyOldUser ? users[i].ActiveNum_Broken : users[i].ActiveNum_Broken + users[i].NewNum_Broken);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", onlyOldUser ? (int)users.Average(a => a.ActiveNum_Broken) : (int)users.Average(a => a.ActiveNum_Broken + a.NewNum_Broken));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "ActiveUserCount_NotBroken":
                            htmlBuilder.AppendFormat("{0:N0}", onlyOldUser ? users[i].ActiveNum_NotBroken : users[i].ActiveNum_NotBroken + users[i].NewNum_NotBroken);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", onlyOldUser ? (int)users.Average(a => a.ActiveNum_NotBroken) : (int)users.Average(a => a.ActiveNum_NotBroken + a.NewNum_NotBroken));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "ActiveUserCount_ZJS":
                            htmlBuilder.AppendFormat("{0:N0}", onlyOldUser ? users[i].ActiveNum_ZJS : users[i].ActiveNum_ZJS + users[i].NewNum_ZJS);
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", onlyOldUser ? (int)users.Average(a => a.ActiveNum_ZJS) : (int)users.Average(a => a.ActiveNum_ZJS + a.NewNum_ZJS));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "ActiveUserCount_ValuedUsers":
                            htmlBuilder.AppendFormat("{0:N0}", onlyOldUser ? Math.Max(users[i].FuncValueUsersForAct, users[i].DownValueUsersForAct) : Math.Max(users[i].FuncValueUsersForAct + users[i].FuncValueUsersForNew, users[i].DownValueUsersForAct + users[i].DownValueUsersForNew));
                            if (i == 0)
                            {
                                averageBuilder.AppendFormat("<td>{0:N0}</td>", onlyOldUser ? (int)users.Average(a => Math.Max(a.FuncValueUsersForAct, a.DownValueUsersForAct)) : (int)users.Average(a => Math.Max(a.FuncValueUsersForAct + a.FuncValueUsersForNew, a.DownValueUsersForAct + a.DownValueUsersForNew)));
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "TotalUserCount":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].TotalNum);
                            if (i == 0)
                            {
                                averageBuilder.Append("<td>--</td>");
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Max(a => a.TotalNum));
                            }
                            break;
                        case "TotalUserCount_Shanzhai":
                            htmlBuilder.AppendFormat("{0:N0}", users[i].TotalUserCount_Shanzhai);
                            if (i == 0)
                            {
                                averageBuilder.Append("<td>--</td>");
                                sumBuilder.AppendFormat("<td>{0:N0}</td>", users.Max(a => a.TotalUserCount_Shanzhai));
                            }
                            break;
                        case "Growth":
                            htmlBuilder.Append(users[i].Growth);
                            if (i == 0)
                            {
                                averageBuilder.Append("<td>--</td>");
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                        case "ActivePercent":
                            htmlBuilder.Append(onlyOldUser ? users[i].ActivityPercent : users[i].UsePercent);
                            if (i == 0)
                            {
                                averageBuilder.Append("<td>--</td>");
                                sumBuilder.Append("<td>--</td>");
                            }
                            break;
                    }
                    htmlBuilder.Append("</td>");
                }
                htmlBuilder.Append("</tr>");
            }

            #endregion

            #region 表尾开始

            htmlBuilder.Append("</tbody><tr style=\"text-align:right;\">");
            htmlBuilder.Append(averageBuilder.ToString());
            htmlBuilder.Append("</tr>");
            if (period != net91com.Stat.Core.PeriodOptions.LatestOneMonth)
            {
                htmlBuilder.Append("<tr style=\"text-align:right;\">");
                htmlBuilder.Append(sumBuilder.ToString());
                htmlBuilder.Append("</tr>");
            }
            htmlBuilder.Append("</tr></table>");

            #endregion

            return htmlBuilder.ToString();
        }

        /// <summary>
        /// 条件是否为true
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool IsTrue_StatUsers(TableTemplate.NodeCondition nCondition, XmlNode node)
        {
            NodeCondition_StatUsers condition = (NodeCondition_StatUsers)nCondition;
            return TableTemplate.IncludeString(node.Attributes["reportType"], condition.ReportType)
                && TableTemplate.IncludeInt(node.Attributes["period"], (int)condition.Period)
                && TableTemplate.CompareBool(node.Attributes["byChannel"], condition.ByChannel)
                && TableTemplate.CompareBool(node.Attributes["isInternalSoft"], condition.IsInternalSoft)
                && TableTemplate.CompareBool(node.Attributes["onlyOldUser"], condition.OnlyOldUser)
                && TableTemplate.IncludeInt(node.Attributes["platform"], (int)condition.Platform)
                && TableTemplate.IncludeInt(node.Attributes["softId"], (int)condition.SoftID)
                && TableTemplate.ExcludeInt(node.Attributes["excludeSoftId"], (int)condition.SoftID);
        }

        /// <summary>
        /// 遍历条件类
        /// </summary>
        private class NodeCondition_StatUsers : TableTemplate.NodeCondition
        {
            /// <summary>
            /// 报表类型(new新增报表，active活跃报表)
            /// </summary>
            public string ReportType { get; set; }

            /// <summary>
            /// 仅老用户
            /// </summary>
            public bool OnlyOldUser { get; set; }

            /// <summary>
            /// 软件ID
            /// </summary>
            public int SoftID { get; set; }

            /// <summary>
            /// 平台
            /// </summary>
            public MobileOption Platform { get; set; }

            /// <summary>
            /// 分渠道
            /// </summary>
            public bool ByChannel { get; set; }

            /// <summary>
            /// 是否是内部产品
            /// </summary>
            public bool IsInternalSoft { get; set; }

            /// <summary>
            /// 周期
            /// </summary>
            public net91com.Stat.Core.PeriodOptions Period { get; set; }
        }

        #endregion

        #region 构建用户留存率表格

        private static XmlDocument statRetainedUsersXmlDoc = null;
        /// <summary>
        /// 构建用户留存率表格
        /// </summary>
        /// <param name="period"></param>
        /// <param name="retainedUsers"></param>
        /// <param name="forDown"></param>
        /// <param name="tableName"></param>
        /// <param name="activeUsers"></param>
        /// <returns></returns>
        public static string BuildStatRetainedUsersTable(int period, List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatRetainedUsers> retainedUsers, bool forDown, string tableName, bool activeUsers = false)
        {
            #region 表头开始

            StringBuilder htmlBuilder = new StringBuilder();
            if (forDown)
                htmlBuilder.Append("<table border=\"1\">");
            else
                htmlBuilder.AppendFormat("<table id=\"tab0\" class=\"tablesorter\" name=\"{0}\" cellspacing=\"1\">", tableName);

            if (statRetainedUsersXmlDoc == null)
            {
                statRetainedUsersXmlDoc = new XmlDocument();
                statRetainedUsersXmlDoc.Load(HttpContext.Current.Server.MapPath("~/DataTables/StatRetainedUsers.xml"));
            }
            TableTemplate.IsTrueHandler IsTrue = new TableTemplate.IsTrueHandler(IsTrue_StatRetainedUsers);
            NodeCondition_StatRetainedUsers nCodition = new NodeCondition_StatRetainedUsers
            {
                Period = (net91com.Stat.Core.PeriodOptions)period
            };
            TableTemplate.ColumnNode rootNode = new TableTemplate.ColumnNode { CorrespondXmlNode = statRetainedUsersXmlDoc.DocumentElement };
            //生成表头，并返回数据绑定列
            List<TableTemplate.ColumnNode> dataColumns;
            string head = TableTemplate.BuildTableHead(rootNode, IsTrue, nCodition, out dataColumns);
            htmlBuilder.Append(activeUsers ? head.Replace("新增", "活跃") : head);

            #endregion

            #region 表体开始

            htmlBuilder.Append("<tbody>");
            var usersGrpByOrgDate = retainedUsers.GroupBy(a => a.OriginalDate).OrderByDescending(a => a.Key);
            var dataTable = new double[usersGrpByOrgDate.Count(), dataColumns.Count];

            for (int i = 0; i < usersGrpByOrgDate.Count(); i++)
            {
                htmlBuilder.Append("<tr style=\"text-align:right;\">");
                var columnIndex = 0;
                foreach (TableTemplate.ColumnNode col in dataColumns)
                {
                    htmlBuilder.Append("<td>");
                    DateTime key = usersGrpByOrgDate.ElementAt(i).Key;
                    var temp = usersGrpByOrgDate.ElementAt(i).ToList();

                    switch (col.Name)
                    {
                        case "OrignalDate":
                            htmlBuilder.AppendFormat("{0:yyyy-MM-dd}", key);
                            break;
                        case "OrignalNewUserCount":
                            htmlBuilder.AppendFormat("{0:N0}", temp.ElementAt(0).OriginalNewUserCount);
                            dataTable[i, columnIndex] = temp.ElementAt(0).OriginalNewUserCount;
                            break;
                        default:
                            var str = GetRetainedUserCountString(temp, col, (net91com.Stat.Core.PeriodOptions)period);
                            htmlBuilder.Append(GetRetainedUserCountString(temp, col, (net91com.Stat.Core.PeriodOptions)period));
                            var val = 0.0;
                            double.TryParse(str.Replace("%", ""), out val);
                            if (str.Contains("%"))
                                val = val / 100;

                            dataTable[i, columnIndex] = val;
                            break;
                    }
                    htmlBuilder.Append("</td>");
                    columnIndex++;
                }
                htmlBuilder.Append("</tr>");
            }

            var sum = new double[dataColumns.Count];
            for (int col = 0; col < dataColumns.Count; col++)
            {
                int counter = 0;
                for (int row = 0; row < usersGrpByOrgDate.Count(); row++)
                {
                    sum[col] += dataTable[row, col];
                    if (!dataTable[row, col].Equals(0))
                        counter++;
                }
                sum[col] = sum[col] / counter;
            }
            htmlBuilder.Append("<tr style='text-align:right;'><td>均值</td>");
            for (int i = 1; i < sum.Length; i++)
            {
                if (double.IsNaN(sum[i]))
                {
                    htmlBuilder.Append("<td></td>");
                    continue;
                }
                htmlBuilder.Append("<td>" + (sum[i] > 1 ? sum[i].ToString("N0") : sum[i].ToString("0.00%")) + "</td>");
            }
            htmlBuilder.Append("</tr>");


            #endregion

            #region 表尾开始

            htmlBuilder.Append("</tbody></table>");

            #endregion

            return htmlBuilder.ToString();
        }

        /// <summary>
        /// 获取留存量索引
        /// </summary>
        /// <param name="period"></param>
        /// <param name="orignalDate"></param>
        /// <param name="statDate"></param>
        private static string GetRetainedUserCountString(List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatRetainedUsers> retainedUsers, TableTemplate.ColumnNode col, net91com.Stat.Core.PeriodOptions period)
        {
            int index = int.Parse(col.Name.Replace("RetainedUserCount", string.Empty).Replace("RetainedRate", string.Empty));
            for (int t = 0; t < retainedUsers.Count; t++)
            {
                switch (period)
                {
                    case net91com.Stat.Core.PeriodOptions.Daily:
                        if (retainedUsers[t].StatDate.Subtract(retainedUsers[t].OriginalDate).Days != index) continue;
                        break;
                    case net91com.Stat.Core.PeriodOptions.Weekly:
                        if (retainedUsers[t].StatDate.Subtract(retainedUsers[t].OriginalDate).Days / 7 != index) continue;
                        break;
                    default: //月
                        if (retainedUsers[t].StatDate.Subtract(retainedUsers[t].OriginalDate).Days / 28 != index) continue;
                        break;
                }
                if (col.Name.StartsWith("RetainedUserCount"))
                    return String.Format("{0:N0}", retainedUsers[t].RetainedUserCount);
                //否则返回留存率
                return retainedUsers[t].OriginalNewUserCount == 0 ? "100.00%" : String.Format("{0:0.00}%", retainedUsers[t].RetainedUserCount / (double)retainedUsers[t].OriginalNewUserCount * 100);
            }
            return string.Empty;
        }

        /// <summary>
        /// 条件是否为true
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool IsTrue_StatRetainedUsers(TableTemplate.NodeCondition nCondition, XmlNode node)
        {
            NodeCondition_StatRetainedUsers condition = (NodeCondition_StatRetainedUsers)nCondition;
            return TableTemplate.IncludeInt(node.Attributes["period"], (int)condition.Period);
        }

        /// <summary>
        /// 遍历条件类
        /// </summary>
        private class NodeCondition_StatRetainedUsers : TableTemplate.NodeCondition
        {
            /// <summary>
            /// 周期
            /// </summary>
            public net91com.Stat.Core.PeriodOptions Period { get; set; }
        }

        #endregion
    }
}