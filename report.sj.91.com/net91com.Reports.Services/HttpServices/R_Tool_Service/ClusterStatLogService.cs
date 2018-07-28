using System;
using System.Collections.Generic;
using System.Web;
using net91com.Reports.Services.CommonServices.Tool;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.R_Tool_Service
{
    public class ClusterStatLogService : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "clusterstatlogservice"; }
        }

        public override string ServiceName
        {
            get { return "海外资源下载量查询"; }
        }

        //?
        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "/Tools/ClusterStatLog.aspx"; }
        }

        public override Result Process(HttpContext context)
        {
            String strHtml = GetData(context);
            return Result.GetSuccessedResult(strHtml, true, true);
        }

        public String GetData(HttpContext context)
        {
            string statdate = context.Request["begintime"];
            string enddate = context.Request["endtime"];

            int modulutype = Convert.ToInt32(context.Request["type"]);

            List<Dictionary<string, string>> lstmap = ToolService.Instance.GetClusterStatLog(statdate,
                                                                                             DateTime.Parse(enddate)
                                                                                                     .AddDays(1)
                                                                                                     .ToString(
                                                                                                         "yyyy-MM-dd"),
                                                                                             modulutype);

            return GetListForExcel(lstmap);
        }

        private string GetListForExcel(List<Dictionary<string, string>> errorcontext)
        {
            string str =
                "<table cellspacing=0' border='1' width='100%'><tr><td>序号</td><td>时间</td><td>任务名</td><td>详细</td><td>类别1：user,2:down</td></tr>";
            foreach (var dictionary in errorcontext)
            {
                str += "<tr>";
                foreach (var item in dictionary)
                {
                    str += "<td>" + item.Value + "</td>";
                }
                str += "</tr>";
            }
            return str + "</table>";
        }
    }
}