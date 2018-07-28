using net91com.Core.Util;
using net91com.Reports.DataAccess.OtherDataAccess;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.ServicesBaseEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;


namespace net91com.Reports.Services.HttpServices.R_Other_Service
{
    public class HttpLinkAdStat : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "LinkAdStat"; }
        }

        public override string ServiceName
        {
            get { return "广告跳转统计"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/R_Other/LinkAdStat.aspx"; }
        }

        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                    case "get_list":
                        myresult = GetList(context);
                        return myresult;
                    case "get_excel":
                        myresult = GetExcel(context);
                        return myresult;
                    case "get_list_hours":
                        myresult = GetList_Hours(context);
                        return myresult;
                    case "get_excel_hours":
                        myresult = GetExcel_Hours(context);
                        return myresult;
                    case "get_list_area":
                        myresult = GetList_Area(context);
                        return myresult;
                    case "get_excel_area":
                        myresult = GetExcel_Area(context);
                        return myresult;
                    case "get_list_keyword":
                        myresult = GetList_Keyword(context);
                        return myresult;
                    case "get_excel_keyword":
                        myresult = GetExcel_Keyword(context);
                        return myresult;
                    case "get_list_retained":
                        myresult = GetList_Retained(context);
                        return myresult;
                    case "get_excel_retained":
                        myresult = GetExcel_Retained(context);
                        return myresult;
                }
            }
            return myresult;
        }

        public Result GetList(HttpContext context)
        {
            Result result = null;
            DataTablesRequest param = new DataTablesRequest(context.Request);
            List<List<string>> data = new List<List<string>>();
            try
            {
                List<LinkAdCount> list = GetData(context);
                if (list != null && list.Count > 0)
                {
                    foreach (LinkAdCount obj in list)
                    {
                        List<string> values = new List<string>();
                        values.Add(obj.PlanName);
                        values.Add(obj.UnionName);
                        values.Add(obj.Keyword);
                        values.Add(obj.MapMode);
                        values.Add(obj.LinkCount.ToString());
                        values.Add(obj.StatCount_15.ToString());
                        values.Add(obj.StatCount_New_15.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                        values.Add(obj.StatCount_30.ToString());
                        values.Add(obj.StatCount_New_30.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount)); values.Add(obj.AdID.ToString());
                        values.Add(obj.AdID.ToString());
                        values.Add(obj.AdID.ToString());
                        values.Add(obj.AdID.ToString());
                        data.Add(values);
                    }
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("获取广告跳转数据失败：", ex);
            }
            finally
            {
                JQueryDataTableData dt = new JQueryDataTableData();
                dt.sEcho = param.sEcho;
                dt.iTotalRecords = dt.iTotalDisplayRecords = data.Count;
                dt.aaData = data;
                result = Result.GetSuccessedResult(dt, true);
            }
            return result;
        }

        private Result GetExcel(HttpContext context)
        {
            SetDownHead(context.Response, "广告跳转统计.xls");
            List<List<string>> data = new List<List<string>>();
            var list = GetData(context);
            if (list != null && list.Count > 0)
            {
                foreach (LinkAdCount obj in list)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.PlanName);
                    values.Add(obj.UnionName);
                    values.Add(obj.Keyword);
                    values.Add(obj.MapMode);
                    values.Add(obj.LinkCount.ToString());
                    values.Add(obj.StatCount_15.ToString());
                    values.Add(obj.StatCount_New_15.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                    values.Add(obj.StatCount_30.ToString());
                    values.Add(obj.StatCount_New_30.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount));
                    data.Add(values);
                }
            }
            list.Clear();
            string html = string.Empty;
            html = GetTableHtml(new string[] { "广告计划", "广告单元", "关键词", "匹配形式", "跳转数", "活跃数（15分钟）", "激活数（15分钟）", "激活转化率", "活跃数（30分钟）", "激活数（30分钟）", "激活转化率" }, data);
            return Result.GetSuccessedResult(html, false, true);
        }

        private List<LinkAdCount> GetData(HttpContext context)
        {
            int softid = 0;
            if (!string.IsNullOrEmpty(context.Request["softid"]))
            {
                softid = Convert.ToInt32(context.Request["softid"]);
            }
            int platform = 0;
            if (!string.IsNullOrEmpty(context.Request["platform"]))
            {
                platform = Convert.ToInt32(context.Request["platform"]);
            }
            DateTime begintime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["begintime"]))
            {
                begintime = Convert.ToDateTime(context.Request["begintime"]);
            }
            DateTime endtime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["endtime"]))
            {
                endtime = Convert.ToDateTime(context.Request["endtime"]);
            }
            return LinkAdStat_DataAccess.Instance.GetList(softid, platform, begintime, endtime);
        }

        public Result GetList_Hours(HttpContext context)
        {
            Result result = null;
            DataTablesRequest param = new DataTablesRequest(context.Request);
            List<List<string>> data = new List<List<string>>();
            try
            {
                List<LinkAdCount> list = GetData_Hours(context);
                if (list != null && list.Count > 0)
                {
                    foreach (LinkAdCount obj in list)
                    {
                        List<string> values = new List<string>();
                        values.Add(obj.Hours.ToString());
                        values.Add(obj.LinkCount.ToString());
                        values.Add(obj.StatCount_15.ToString());
                        values.Add(obj.StatCount_New_15.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                        values.Add(obj.StatCount_30.ToString());
                        values.Add(obj.StatCount_New_30.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount));
                        data.Add(values);
                    }
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("获取广告跳转时段数据失败：", ex);
            }
            finally
            {
                JQueryDataTableData dt = new JQueryDataTableData();
                dt.sEcho = param.sEcho;
                dt.iTotalRecords = dt.iTotalDisplayRecords = data.Count;
                dt.aaData = data;
                result = Result.GetSuccessedResult(dt, true);
            }
            return result;
        }

        private Result GetExcel_Hours(HttpContext context)
        {
            SetDownHead(context.Response, "广告跳转时段统计.xls");
            List<List<string>> data = new List<List<string>>();
            var list = GetData_Hours(context);
            if (list != null && list.Count > 0)
            {
                foreach (LinkAdCount obj in list)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.Hours.ToString());
                    values.Add(obj.LinkCount.ToString());
                    values.Add(obj.StatCount_15.ToString());
                    values.Add(obj.StatCount_New_15.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                    values.Add(obj.StatCount_30.ToString());
                    values.Add(obj.StatCount_New_30.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount));
                    data.Add(values);
                }
            }
            list.Clear();
            string html = string.Empty;
            html = GetTableHtml(new string[] { "时段", "跳转数", "活跃数（15分钟）", "激活数（15分钟）", "激活转化率", "活跃数（30分钟）", "激活数（30分钟）", "激活转化率" }, data);
            return Result.GetSuccessedResult(html, false, true);
        }
        private List<LinkAdCount> GetData_Hours(HttpContext context)
        {
            int softid = 0;
            if (!string.IsNullOrEmpty(context.Request["softid"]))
            {
                softid = Convert.ToInt32(context.Request["softid"]);
            }
            int platform = 0;
            if (!string.IsNullOrEmpty(context.Request["platform"]))
            {
                platform = Convert.ToInt32(context.Request["platform"]);
            }
            int adId = 0;
            if (!string.IsNullOrEmpty(context.Request["adid"]))
            {
                adId = Convert.ToInt32(context.Request["adid"]);
            }
            DateTime begintime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["begintime"]))
            {
                begintime = Convert.ToDateTime(context.Request["begintime"]);
            }
            DateTime endtime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["endtime"]))
            {
                endtime = Convert.ToDateTime(context.Request["endtime"]);
            }
            return LinkAdStat_DataAccess.Instance.GetList_Hours(softid, platform, begintime, endtime, adId);
        }

        public Result GetList_Area(HttpContext context)
        {
            Result result = null;
            DataTablesRequest param = new DataTablesRequest(context.Request);
            List<List<string>> data = new List<List<string>>();
            try
            {
                List<LinkAdCount> list = GetData_Area(context);
                if (list != null && list.Count > 0)
                {
                    foreach (LinkAdCount obj in list)
                    {
                        List<string> values = new List<string>();
                        values.Add(obj.Area);
                        values.Add(obj.LinkCount.ToString());
                        values.Add(obj.StatCount_15.ToString());
                        values.Add(obj.StatCount_New_15.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                        values.Add(obj.StatCount_30.ToString());
                        values.Add(obj.StatCount_New_30.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount));
                        data.Add(values);
                    }
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("获取广告跳转地区数据失败：", ex);
            }
            finally
            {
                JQueryDataTableData dt = new JQueryDataTableData();
                dt.sEcho = param.sEcho;
                dt.iTotalRecords = dt.iTotalDisplayRecords = data.Count;
                dt.aaData = data;
                result = Result.GetSuccessedResult(dt, true);
            }
            return result;
        }

        private Result GetExcel_Area(HttpContext context)
        {
            SetDownHead(context.Response, "广告跳转地区分布.xls");
            List<List<string>> data = new List<List<string>>();
            var list = GetData_Area(context);
            if (list != null && list.Count > 0)
            {
                foreach (LinkAdCount obj in list)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.Area);
                    values.Add(obj.LinkCount.ToString());
                    values.Add(obj.StatCount_15.ToString());
                    values.Add(obj.StatCount_New_15.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                    values.Add(obj.StatCount_30.ToString());
                    values.Add(obj.StatCount_New_30.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount));
                    data.Add(values);
                }
            }
            list.Clear();
            string html = string.Empty;
            html = GetTableHtml(new string[] { "地区", "跳转数", "活跃数（15分钟）", "激活数（15分钟）", "激活转化率", "活跃数（30分钟）", "激活数（30分钟）", "激活转化率" }, data);
            return Result.GetSuccessedResult(html, false, true);
        }
        private List<LinkAdCount> GetData_Area(HttpContext context)
        {
            int softid = 0;
            if (!string.IsNullOrEmpty(context.Request["softid"]))
            {
                softid = Convert.ToInt32(context.Request["softid"]);
            }
            int platform = 0;
            if (!string.IsNullOrEmpty(context.Request["platform"]))
            {
                platform = Convert.ToInt32(context.Request["platform"]);
            }
            int adId = 0;
            if (!string.IsNullOrEmpty(context.Request["adid"]))
            {
                adId = Convert.ToInt32(context.Request["adid"]);
            }
            DateTime begintime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["begintime"]))
            {
                begintime = Convert.ToDateTime(context.Request["begintime"]);
            }
            DateTime endtime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["endtime"]))
            {
                endtime = Convert.ToDateTime(context.Request["endtime"]);
            }
            return LinkAdStat_DataAccess.Instance.GetList_Area(softid, platform, begintime, endtime, adId);
        }

        public Result GetList_Keyword(HttpContext context)
        {
            Result result = null;
            DataTablesRequest param = new DataTablesRequest(context.Request);
            List<List<string>> data = new List<List<string>>();
            try
            {
                List<LinkAdCount> list = GetData_Keyword(context);
                if (list != null && list.Count > 0)
                {
                    foreach (LinkAdCount obj in list)
                    {
                        List<string> values = new List<string>();
                        values.Add(obj.SearchWord);
                        values.Add(obj.LinkCount.ToString());
                        values.Add(obj.StatCount_15.ToString());
                        values.Add(obj.StatCount_New_15.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                        values.Add(obj.StatCount_30.ToString());
                        values.Add(obj.StatCount_New_30.ToString());
                        values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount));
                        data.Add(values);
                    }
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("获取广告跳转实际搜索词数据失败：", ex);
            }
            finally
            {
                JQueryDataTableData dt = new JQueryDataTableData();
                dt.sEcho = param.sEcho;
                dt.iTotalRecords = dt.iTotalDisplayRecords = data.Count;
                dt.aaData = data;
                result = Result.GetSuccessedResult(dt, true);
            }
            return result;
        }

        private Result GetExcel_Keyword(HttpContext context)
        {
            SetDownHead(context.Response, "广告跳转实际搜索词分布.xls");
            List<List<string>> data = new List<List<string>>();
            var list = GetData_Keyword(context);
            if (list != null && list.Count > 0)
            {
                foreach (LinkAdCount obj in list)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.SearchWord);
                    values.Add(obj.LinkCount.ToString());
                    values.Add(obj.StatCount_15.ToString());
                    values.Add(obj.StatCount_New_15.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_15 * 1.0 / obj.LinkCount));
                    values.Add(obj.StatCount_30.ToString());
                    values.Add(obj.StatCount_New_30.ToString());
                    values.Add(string.Format("{0:P}", obj.StatCount_New_30 * 1.0 / obj.LinkCount));
                    data.Add(values);
                }
            }
            list.Clear();
            string html = string.Empty;
            html = GetTableHtml(new string[] { "实际搜索词", "跳转数", "活跃数（15分钟）", "激活数（15分钟）", "激活转化率", "活跃数（30分钟）", "激活数（30分钟）", "激活转化率" }, data);
            return Result.GetSuccessedResult(html, false, true);
        }
        private List<LinkAdCount> GetData_Keyword(HttpContext context)
        {
            int softid = 0;
            if (!string.IsNullOrEmpty(context.Request["softid"]))
            {
                softid = Convert.ToInt32(context.Request["softid"]);
            }
            int platform = 0;
            if (!string.IsNullOrEmpty(context.Request["platform"]))
            {
                platform = Convert.ToInt32(context.Request["platform"]);
            }
            int adId = 0;
            if (!string.IsNullOrEmpty(context.Request["adid"]))
            {
                adId = Convert.ToInt32(context.Request["adid"]);
            }
            DateTime begintime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["begintime"]))
            {
                begintime = Convert.ToDateTime(context.Request["begintime"]);
            }
            DateTime endtime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["endtime"]))
            {
                endtime = Convert.ToDateTime(context.Request["endtime"]);
            }
            return LinkAdStat_DataAccess.Instance.GetList_Keyword(softid, platform, begintime, endtime, adId);
        }

        public Result GetList_Retained(HttpContext context)
        {
            Result result = null;
            DataTablesRequest param = new DataTablesRequest(context.Request);
            List<List<string>> data = new List<List<string>>();
            try
            {
                List<LinkAdStatRetainedUsers> list = GetData_Retained(context);
                if (list != null && list.Count > 0)
                {
                    foreach (LinkAdStatRetainedUsers obj in list)
                    {
                        List<string> values = new List<string>();
                        values.Add(obj.OriginalDate.ToString("yyyy-MM-dd"));
                        values.Add(obj.NewUsers.ToString());
                        values.Add(obj.C_1.ToString());
                        values.Add(obj.C_2.ToString());
                        values.Add(obj.C_3.ToString());
                        values.Add(obj.C_4.ToString());
                        values.Add(obj.C_5.ToString());
                        values.Add(obj.C_6.ToString());
                        values.Add(obj.C_7.ToString());
                        data.Add(values);
                    }
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("获取广告跳转留存率失败：", ex);
            }
            finally
            {
                JQueryDataTableData dt = new JQueryDataTableData();
                dt.sEcho = param.sEcho;
                dt.iTotalRecords = dt.iTotalDisplayRecords = data.Count;
                dt.aaData = data;
                result = Result.GetSuccessedResult(dt, true);
            }
            return result;
        }

        private Result GetExcel_Retained(HttpContext context)
        {
            SetDownHead(context.Response, "广告跳转留存率.xls");
            List<List<string>> data = new List<List<string>>();
            var list = GetData_Retained(context);
            if (list != null && list.Count > 0)
            {
                foreach (LinkAdStatRetainedUsers obj in list)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.OriginalDate.ToString("yyyy-MM-dd"));
                    values.Add(obj.NewUsers.ToString());
                    values.Add(obj.C_1.ToString());
                    values.Add(obj.C_2.ToString());
                    values.Add(obj.C_3.ToString());
                    values.Add(obj.C_4.ToString());
                    values.Add(obj.C_5.ToString());
                    values.Add(obj.C_6.ToString());
                    values.Add(obj.C_7.ToString());
                    data.Add(values);
                }
            }
            list.Clear();
            string html = string.Empty;
            html = GetTableHtml(new string[] { "日期", "激活用户", "第1天", "第2天", "第3天", "第4天", "第5天", "第6天", "第7天" }, data);
            return Result.GetSuccessedResult(html, false, true);
        }
        private List<LinkAdStatRetainedUsers> GetData_Retained(HttpContext context)
        {
            int softid = 0;
            if (!string.IsNullOrEmpty(context.Request["softid"]))
            {
                softid = Convert.ToInt32(context.Request["softid"]);
            }
            int platform = 0;
            if (!string.IsNullOrEmpty(context.Request["platform"]))
            {
                platform = Convert.ToInt32(context.Request["platform"]);
            }
            int adId = 0;
            if (!string.IsNullOrEmpty(context.Request["adid"]))
            {
                adId = Convert.ToInt32(context.Request["adid"]);
            }
            DateTime begintime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["begintime"]))
            {
                begintime = Convert.ToDateTime(context.Request["begintime"]);
            }
            DateTime endtime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(context.Request["endtime"]))
            {
                endtime = Convert.ToDateTime(context.Request["endtime"]);
            }
            return LinkAdStat_DataAccess.Instance.GetList_RetainedUsers(softid, platform, begintime, endtime, adId);
        }
    }
}
