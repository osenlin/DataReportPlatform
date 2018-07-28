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
    public class HttpLinkTagManager : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "LinkTagManager"; }
        }

        public override string ServiceName
        {
            get { return "跳转标签管理页面"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.UrlAndSoftRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/R_Other/LinkTagManager.aspx"; }
        }

        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                    case "getlinktagtree":
                        myresult = GetListTagTree(context);
                        return myresult;
                    case "getlinktaglist":
                        myresult = GetLinkTagList(context);
                        return myresult;
                    case "changecate":
                        myresult = ChangeCate(context);
                        return myresult;
                    case "editcate2":
                        myresult = EditCate(context);
                        return myresult;
                    case "deletecate":
                        myresult = DeleteCate(context);
                        return myresult;
                    case "edittag":
                        myresult = EditTag(context);
                        return myresult;
                    case "deletetag":
                        myresult = DeleteTag(context);
                        return myresult;
                    case "gettag":
                        myresult = GetTag(context);
                        return myresult;
                }
            }
            return myresult;
        }

        public Result GetListTagTree(HttpContext context)
        {
            int softid = Convert.ToInt32(context.Request["softid"]);
            List<LinkTagLog> tags = LinkTagLog_DataAccess.Instance.GetTags(softid, false);
            StringBuilder nodes = new StringBuilder().Append("[");
            nodes.Append("{\"id\":\"0\",\"pId\":\"0\",\"name\":\"根分类\",\"open\":true,\"checked\":true}");
            if (tags != null && tags.Count > 0)
            {
                foreach (LinkTagLog tag in tags)
                {
                    nodes.AppendFormat(",{{\"id\":\"{0}\",\"pId\":\"{1}\",\"name\":\"{2}\",\"open\":false,\"checked\":false}}"
                        , tag.ID, tag.PID, tag.Name);
                }
            }
            nodes.Append("]");
            return Result.GetSuccessedResult(nodes.ToString(), true);
        }

        private Result GetLinkTagList(HttpContext context)
        {
            Result result = null;
            DataTablesRequest param = new DataTablesRequest(context.Request);
            List<List<string>> data = new List<List<string>>();
            try
            {
                int softid = Convert.ToInt32(context.Request["softid"]);
                int platform = Convert.ToInt32(context.Request["platform"]);
                int cid = Convert.ToInt32(context.Request["cid"]);

                List<LinkTagInfo> dataList = LinkTagLog_DataAccess.Instance.GetTagInfoList(softid, platform, cid);
                if (dataList != null && dataList.Count > 0)
                {
                    data = new List<List<string>>();
                    foreach (LinkTagInfo obj in dataList)
                    {
                        List<string> values = new List<string>();
                        values.Add(obj.LinkName);
                        values.Add(obj.LinkType.ToString());
                        values.Add(obj.LinkTag);
                        values.Add(obj.LinkUrl);
                        values.Add(obj.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        values.Add(obj.ChannelId.ToString());
                        values.Add(obj.ID.ToString());
                        data.Add(values);
                    }
                    dataList.Clear();
                }
                dataList = null;
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("获取跳转链接列表失败：", ex);
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

        public Result ChangeCate(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["id"]);
            int pid = Convert.ToInt32(context.Request["pid"]);
            bool isOK = LinkTagLog_DataAccess.Instance.ChangeCate(id, pid);
            if (isOK)
            {
                return Result.GetSuccessedResult(null, true);
            }
            else
            {
                return Result.GetFailedResult("更新失败！");
            }
        }

        private Result DeleteCate(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["id"]);
            bool isOK = LinkTagLog_DataAccess.Instance.DeleteCate(id);
            if (isOK)
            {
                return Result.GetSuccessedResult(null, true);
            }
            else
            {
                return Result.GetFailedResult("删除失败!");
            }
        }

        private Result EditCate(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["id"]);
            int pid = Convert.ToInt32(context.Request["pid"]);
            int softid = Convert.ToInt32(context.Request["softid"]);
            string name = context.Request["name"].ToString();
            bool result = false;
            if (id == 0)
            {
                result = LinkTagLog_DataAccess.Instance.AddCate(softid, pid, name);
            }
            else
            {
                result = LinkTagLog_DataAccess.Instance.UpdateCate(id, name);
            }
            if (result)
            {
                return Result.GetSuccessedResult(null, true);
            }
            else
            {
                return Result.GetFailedResult("保存失败！");
            }
        }

        private Result DeleteTag(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["id"]);
            bool isOK = LinkTagLog_DataAccess.Instance.DeleteTag(id);
            if (isOK)
            {
                return Result.GetSuccessedResult(null, true);
            }
            else
            {
                return Result.GetFailedResult("删除失败!");
            }
        }

        private Result EditTag(HttpContext context)
        {
            string value = context.Request["obj"];
            LinkTagInfo obj = JsonConvert.DeserializeObject<LinkTagInfo>(value);
            obj.AppStoreUrlEncode();
            bool result = false;
            if (obj.ID == 0)
            {
                result = LinkTagLog_DataAccess.Instance.AddTag(obj);
            }
            else
            {
                result = LinkTagLog_DataAccess.Instance.UpdateTag(obj);
            }
            if (result)
            {
                // 刷新缓存
                UpdateCache(obj);
                return Result.GetSuccessedResult(null, true);
            }
            else
            {
                return Result.GetFailedResult("保存失败！");
            }
        }

        private Result GetTag(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["id"]);
            LinkTagInfo obj = LinkTagLog_DataAccess.Instance.GetTag(id);
            return Result.GetSuccessedResult(obj, true);
        }

        private static string url91comIPs = net91com.Core.Util.ConfigHelper.GetSetting("Url91ComIPs");
        private void UpdateCache(LinkTagInfo obj)
        {
            if (!string.IsNullOrEmpty(obj.LinkUrl) && !string.IsNullOrEmpty(url91comIPs))
            {
                try
                {
                    string url = string.Format(@"http://url.felinkapps.com/{0}?pid={1}&mt={2}&url={3}&update_cache=1",
                        obj.LinkTag, obj.SoftID, obj.Platform, System.Web.HttpUtility.UrlEncode(obj.LinkUrl));
                    string[] ips = url91comIPs.Split(',');
                    foreach (string ip in ips)
                    {
                        if (string.IsNullOrEmpty(ip))
                        {
                            continue;
                        }
                        net91com.Core.Web.WebHelper.GetPage(url, proxy: ip);
                        LogHelper.WriteDebug(string.Format("更新缓存：IP={0}，{1}", ip, obj.LinkUrl));
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteException("更新url缓存异常，url91comIPs=" + url91comIPs, ex);
                }
            }
        }
    }
}