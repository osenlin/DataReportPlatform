using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Stat.Services.sjqd.Entity;
using System.Text;
using Newtonsoft.Json;
using net91com.Stat.Services.sjqd;
using net91com.Core.Web;

using net91com.Stat.Services.Entity;
using net91com.Stat.Web.Reports.Services;
using System.Data;
using net91com.Core;

using net91com.Reports.UserRights;
using net91com.Stat.Web.Base;

namespace net91com.Stat.Web.Reports.sjqd
{
    /// <summary>
    /// ChannelCustom 的摘要说明
    /// </summary>
    public class ChannelCustom : HandlerBase
    {
        public string GetPara(string key)
        {
            return HttpContext.Current.Request[key];
        }

        public override void  ProcessRequest(HttpContext context)
        {
            string code = GetPara("code");
            switch (code)
            {
                case "allcustomstatexcel":
                    GetCustomStatExcel();
                    break;
            }

        }

        public void GetCustomStatExcel()
        {
            CheckUrl("Reports/SoftVersionSjqd.aspx");

            Dictionary<string, int> tableColumn = new Dictionary<string, int>();
            int channelid = Convert.ToInt32(GetPara("Channelid"));
            int channeltype = Convert.ToInt32(GetPara("Channeltype"));
            string name = GetPara("channelname");
            string plat = GetPara("plat");
            net91com.Stat.Core.PeriodOptions period = (net91com.Stat.Core.PeriodOptions)Convert.ToInt32(GetPara("period"));
            DateTime begintime = Convert.ToDateTime(GetPara("begintime"));
            DateTime endtime = Convert.ToDateTime(GetPara("endtime"));
            var listAll = Sjqd_StatUsersByChannelsService.GetInstance().GetAllSonChannelCustomUserCache((ChannelTypeOptions)channeltype, plat, channelid, begintime, endtime, period, CacheTimeOption.TenMinutes);
            List<DateTime> times = listAll.Select(p => p.StatDate).Distinct().OrderByDescending(p => p).ToList();
            var channels = listAll.Select(p => new ChannelCustomers { Name = p.ChannelName, ID = p.ChannelID }).Distinct(new channelComparer()).ToList();
            for (int i = 0; i < channels.Count; i++)
            {
                tableColumn.Add(channels[i].Name, channels[i].ID);
            }
            DataTable dt = MakeChannelIncDataTable(tableColumn);
            for (int i = 0; i < times.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["日期"] = times[i].ToString("yyyy-MM-dd");
                foreach (var item in tableColumn.Keys)
                {
                    var temp = listAll.Find(p => p.ChannelID == tableColumn[item] && p.StatDate == times[i]);
                    if (temp != null)
                        dr[item] = temp.NewNum;
                }
                dt.Rows.Add(dr);
            }

            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.UTF8;
            string filename = string.Empty;
            filename = name + ".xls";
            AddHead(filename);
            string tablestr = TableHelper.GetTableStrForAllCustomers(dt, true);
            resp.Write(tablestr);
            resp.End();

        }

        /// <summary>
        /// 构造需要的table
        /// </summary>
        /// <returns></returns>
        public DataTable MakeChannelIncDataTable(Dictionary<string, int> tableColumn)
        {
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn("日期", typeof(string)));
            foreach (var item in tableColumn.Keys)
            {
                DataColumn dc = new DataColumn(item, typeof(string));
                dc.DefaultValue = "0";
                result.Columns.Add(dc);
            }
            return result;
        }
     
    }
}