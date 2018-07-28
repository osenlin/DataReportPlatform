using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd.Entity;



namespace net91com.Stat.Web.Reports.Services
{
    /// <summary>
    /// 所有表格及EXCEL表格生成辅助类
    /// </summary>
    public class TableHelper
    {
        private static int[] pcSoftList = { 68, 69, -9, 9 };

        #region 构建分地区用户表格(BuildStatUsersByAreaTable)

        /// <summary>
        /// 构建分地区用户表格
        /// </summary>
        /// <param name="users"></param>
        /// <param name="byChannel">按渠道查询</param>
        /// <param name="period">统计周期</param>
        /// <param name="forDown">是否用于下载(EXCEL)</param>
        /// <param name="tableIndex">多个表格时,用于指明第几张表格</param>
        /// <param name="tableName">表格名称</param>
        /// <returns></returns>
        public static string BuildStatUsersByAreaTable(List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> users, int period, bool forDown, int tableIndex, string tableName)
        {
            bool formatNumber = !forDown;
            StringBuilder sb = new StringBuilder();
            if (forDown)
                sb.Append("<table border=\"1\">");
            else
                sb.AppendFormat("<table id=\"tab{0}\"  class=\"tablesorter\" name=\"{1}\" {2} cellspacing=\"1\">", tableIndex, tableName, tableIndex != 0 ? "style=\"display:none\"" : "");

            //表头
            sb.Append("<thead><tr style=\"text-align:center\">");
            sb.Append("<th>日期</th><th>新增用户</th><th>活跃用户</th></tr></thead><tbody>");
            //表体
            foreach (var item in users)
            {
                string colorStyle = period != (int)net91com.Stat.Core.PeriodOptions.Weekly && (item.StatDate.DayOfWeek == DayOfWeek.Sunday || item.StatDate.DayOfWeek == DayOfWeek.Saturday) ? " style=\"color:Red;\"" : "";
                sb.AppendFormat("<tr style=\"text-align:right;\"><td{0}>{1:yyyy-MM-dd}</td><td{0}>{2}</td><td{0}>{3}</td></tr>"
                    , colorStyle
                    , item.StatDate
                    , SetNum(item.NewUserCount, formatNumber)                    
                    , SetNum(item.ActiveUserCount, formatNumber)
                    );
            }
            //表尾
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>总计</td><td>{0}</td><td>--</td></tr>"
                , SetNum(users.Sum(p => p.NewUserCount), formatNumber)
                );
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>平均</td><td>{0}</td><td>{1}</td></tr>"
                , users.Count == 0 ? "--" : SetNum((int)users.Average(p => p.NewUserCount), formatNumber)
                , users.Count == 0 ? "--" : SetNum((int)users.Average(p => p.ActiveUserCount), formatNumber));

            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        #endregion

        #region 构建分版本用户表格(BuildStatUsersByVersionTable)

        /// <summary>
        /// 构建分版本用户表格
        /// </summary>
        /// <param name="users"></param>
        /// <param name="byChannel">按渠道查询</param>
        /// <param name="period">统计周期</param>
        /// <param name="forDown">是否用于下载(EXCEL)</param>
        /// <param name="tableIndex">多个表格时,用于指明第几张表格</param>
        /// <returns></returns>
        public static string BuildStatUsersByVersionTable(List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> users, int period, bool forDown, int tableIndex)
        {
            bool formatNumber = !forDown;
            StringBuilder sb = new StringBuilder();
            if (forDown)
                sb.Append("<table border=\"1\">");
            else
                sb.AppendFormat("<table id=\"tab{0}\"  class=\"tablesorter\" {1} cellspacing=\"1\">", tableIndex, tableIndex != 0 ? "style=\"display:none\"" : "");

            //表头
            sb.Append("<thead><tr style=\"text-align:center\">");
            sb.Append("<th>日期</th><th>新增用户</th><th>活跃用户</th></tr></thead><tbody>");
            //表体
            foreach (var item in users)
            {
                string colorStyle = period != (int)net91com.Stat.Core.PeriodOptions.Weekly && (item.StatDate.DayOfWeek == DayOfWeek.Sunday || item.StatDate.DayOfWeek == DayOfWeek.Saturday) ? " style=\"color:Red;\"" : "";
                sb.AppendFormat("<tr style=\"text-align:right;\"><td{0}>{1:yyyy-MM-dd}</td><td{0}>{2}</td><td{0}>{3}</td></tr>"
                    , colorStyle
                    , item.StatDate
                    , SetNum(item.NewUserCount, formatNumber)                    
                    , SetNum(item.ActiveUserCount, formatNumber));
            }
            //表尾
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>总计</td><td>{0}</td><td>--</td></tr>"
                , SetNum(users.Sum(p => p.NewUserCount), formatNumber)
                );
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>平均</td><td>{0}</td><td>{1}</td></tr>"
                , users.Count == 0 ? "--" : SetNum((int)users.Average(p => p.NewUserCount), formatNumber)               
                , users.Count == 0 ? "--" : SetNum((int)users.Average(p => p.ActiveUserCount), formatNumber));

            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        #endregion

        #region 构建分国家分版本(海外)用户表格(BuildStatUsersByCountryByVersionEnTable)

        /// <summary>
        /// 构建分版本用户表格
        /// </summary>
        /// <param name="users"></param>
        /// <param name="byChannel">按渠道查询</param>
        /// <param name="forDown">是否用于下载(EXCEL)</param>
        /// <param name="tableIndex">多个表格时,用于指明第几张表格</param>
        /// <returns></returns>
        public static string BuildStatUsersByCountryByVersionEnTable(List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers> users, bool forDown, int tableIndex)
        {
            bool formatNumber = !forDown;
            StringBuilder sb = new StringBuilder();
            if (forDown)
                sb.Append("<table border=\"1\">");
            else
                sb.AppendFormat("<table id=\"tab{0}\"  class=\"tablesorter\" {1} cellspacing=\"1\">", tableIndex, tableIndex != 0 ? "style=\"display:none\"" : "");

            //表头
            sb.Append("<thead><tr style=\"text-align:center\">");
            sb.Append("<th>日期</th><th>新增用户</th><th>活跃用户</th></tr></thead><tbody>");
            //表体
            foreach (var item in users)
            {
                string colorStyle = item.StatDate.DayOfWeek == DayOfWeek.Sunday || item.StatDate.DayOfWeek == DayOfWeek.Saturday ? " style=\"color:Red;\"" : "";
                sb.AppendFormat("<tr style=\"text-align:right;\"><td{0}>{1:yyyy-MM-dd}</td><td{0}>{2}</td><td{0}>{3}</td></tr>"
                    , colorStyle
                    , item.StatDate
                    , SetNum(item.NewUserCount, formatNumber)
                    , SetNum(item.ActiveUserCount, formatNumber));
            }
            //表尾
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>总计</td><td>{0}</td><td>--</td></tr>"
                , SetNum(users.Sum(p => p.NewUserCount), formatNumber));
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>平均</td><td>{0}</td><td>{1}</td></tr>"
                , users.Count == 0 ? "--" : SetNum((int)users.Average(p => p.NewUserCount), formatNumber)
                , users.Count == 0 ? "--" : SetNum((int)users.Average(p => p.ActiveUserCount), formatNumber));

            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        #endregion

        #region 构建跳转链接统计表格(BuildLinkTagTable)


        #endregion

        #region 创建用户流失表格
        public static string BuildUserLostUsers(List<SoftUser> users, string tableName, int tableIndex, net91com.Stat.Core.PeriodOptions period, bool forDown)
        {

            StringBuilder sb = new StringBuilder();
            if (forDown)
                sb.Append("<table border=\"1\">");
            else
                sb.AppendFormat("<table id=\"tab{0}\"  class=\"tablesorter\" name=\"{1}\" {2} cellspacing=\"1\">", tableIndex, tableName, tableIndex != 0 ? "style=\"display:none\"" : "");
            //表头
            sb.Append("<thead><tr style=\"text-align:center\">");
            sb.AppendFormat("<th>{0}</th><th>活跃用户</th><th>流失用户(下一周期)</th><th>流失率</th></tr></thead><tbody>", period == net91com.Stat.Core.PeriodOptions.Monthly ? "活跃月份" : "活跃日期");
            //表体
            foreach (SoftUser item in users)
            {
                sb.AppendFormat("<tr style=\"text-align:right;\"><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>",
                     period == net91com.Stat.Core.PeriodOptions.Monthly ? item.StatDate.ToString("yyyy-MM") : item.StatDate.ToString("yyyy-MM-dd"), item.UseNum, item.LostNum, item.LostPercent);
            }
            //表尾
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>平均</td><td>{0}</td><td>{1}</td><td>{2:0.00%}</td></tr>"
               , SetNum((int)users.Average(p => p.UseNum), true), SetNum((int)users.Average(p => p.LostNum), true), users.Average(p => (double)p.LostNum / p.UseNum));


            sb.Append("</tbody></table>");
            return sb.ToString();
        }
        #endregion

        #region 功能使用量 表格

        public static string GetFunctionUseV5Table(List<FunctionNode> funcUser, long total, bool isDownLoad)
        {
            StringBuilder sb = new StringBuilder()
                .AppendFormat("<table id=\"mytreetable\" class=\"tablesorter\" cellspacing=\"1\" {0}>",
                              isDownLoad ? "\"border:1px\"" : "")
                .Append("<tbody>")
                .AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", "名称", "次数", "占比");
            foreach (var node in funcUser)
            {
                sb.AppendFormat(
                    "<tr data-tt-id=\"{0}\" {1}><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                    node.Code,
                    node.Code == node.ParentCode ? "" : string.Format("data-tt-parent-id=\"{0}\"", node.ParentCode),
                    node.CodeName, node.Count, GetPercent(node.Count, total));
            }
            return sb.ToString() + "</tbody></table>";
        }

        protected static string GetPercent(long molecule, long denominator)
        {
            if (denominator == 0)
                return "--";
            else
            {
                return (molecule / (denominator * 1.0)).ToString("0.00%");
            }
        }

        #endregion

        #region 构建跳转链接统计表格(BuildLinkTagTable)

        /// <summary>
        /// 构建跳转链接统计表格
        /// </summary>
        /// <returns></returns>
        public static string BuildLinkTagTable(List<LinkTagCount> users, bool forDown, int tableIndex, string tableName)
        {
            bool formatNumber = !forDown;
            StringBuilder sb = new StringBuilder();
            if (forDown)
                sb.Append("<table border=\"1\">");
            else
                sb.AppendFormat("<table id=\"tab{0}\" class=\"tablesorter\" name=\"{1}\" {2} cellspacing=\"1\">", tableIndex, tableName, tableIndex != 0 ? "style=\"display:none\"" : "");

            //表头
            sb.Append("<thead><tr style=\"text-align:center\">");
            sb.Append("<th width='20%'>日期</th><th width='80%'>跳转量</th></tr></thead><tbody>");
            //表体
            foreach (LinkTagCount item in users)
            {
                string colorStyle = (item.StatDate.DayOfWeek == DayOfWeek.Sunday || item.StatDate.DayOfWeek == DayOfWeek.Saturday) ? " style=\"color:Red;\"" : "";
                sb.AppendFormat("<tr style=\"text-align:right;\"><td{0}>{1:yyyy-MM-dd}</td><td{0}>{2}</td></tr>"
                    , colorStyle, item.StatDate, item.StatCount);
            }
            //表尾
            sb.AppendFormat("<tr style=\"text-align:right;\"><td>总计</td><td>{0}</td></tr>"
                , SetNum(users.Sum(p => p.StatCount), formatNumber));

            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        #endregion

        private static string SetNum(int m, bool formatNumber)
        {
            if (m == 0)
                return "--";
            if (formatNumber)
                return string.Format("{0:N0}", m);
            return m.ToString();
        }

        #region 渠道商管理 查看所有渠道量构造table

        public static string GetTableStrForAllCustomers(DataTable dt, bool isfordown = false)
        {

            Dictionary<int, int> dic = new Dictionary<int, int>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table  class=\" tablesorter \" border=\"{0}\" cellspacing=\"1\"><thead><tr>", isfordown ? "1" : "0");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.AppendFormat("<th>{0}</th>", dt.Columns[i].ColumnName);
                dic.Add(i, 0);
            }
            sb.Append("</thead><tbody>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("<tr>");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.AppendFormat("<td>{0}</td>", dt.Rows[i][j]);
                    if (j != 0)
                        dic[j] += Convert.ToInt32(dt.Rows[i][j]);
                }
                sb.Append("</tr>");
            }
            sb.Append("<tr>");
            for (int i = 0; i < dic.Keys.Count; i++)
            {
                sb.AppendFormat("<td>{0}</td>", i == 0 ? "汇总" : dic[i].ToString());
            }
            sb.Append("</tr>");
            sb.Append("</tbody></table>");
            return sb.ToString();
        }
        #endregion
    }
}