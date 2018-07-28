using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownAvgCountByAreaImpl : D_HttpDownStatDownAvgCountByVersionImpl
    {


        public override Result GetChart<T>(HttpContext context)
        {
            List<LineChartLine> lines = new List<LineChartLine>();
            int modetype = Convert.ToInt32(context.Request["modetype"]);
            int showtype = Convert.ToInt32(context.Request["showtype"]);
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int period = Convert.ToInt32(context.Request["period"]);
            var lists = GetData<T>(context, false) as List<List<D_StatDownCountsBySoft_SUM>>;

            for (int j = 0; j < lists.Count; j++)
            {
                var linename = GetLineName(lists[j][0]);
                List<LineChartPoint> points = lists[j].Select(p => new LineChartPoint
                    {
                        XValue = p.StatDate.ToString("yyyyMMdd"),
                        DataContext = "",
                        NumberType = showtype == 3 ? 3 : 1,
                        Description = "",
                        YValue = (p.DownCount * 1.0 / (p.UserCount == 0 ? 1 : p.UserCount)).ToString()
                    }).ToList();
                lines.Add(new LineChartLine { Name = linename + "_所有人均分发", Points = points, Show = true });
            }

            for (int j = 0; j < lists.Count; j++)
            {
                var linename = GetLineName(lists[j][0]);
                List<LineChartPoint> points = lists[j].Select(p => new LineChartPoint
                {
                    XValue = p.StatDate.ToString("yyyyMMdd"),
                    DataContext = "",
                    NumberType = showtype == 3 ? 3 : 1,
                    Description = "",
                    YValue = (p.DownCountExceptAllUpdating * 1.0 /(p.UserCountExceptAllUpdating==0?1:p.UserCountExceptAllUpdating)).ToString()
                }).ToList();
                lines.Add(new LineChartLine { Name =linename+"_一次人均分发", Points = points, Show = true });
            }
            LineChart line = new LineChart(GetXLine(begintime, endtime, period), lines);
            string result = "{ x:" + line.GetXJson(p => { return ""; }) + ",y:" + line.GetYJson(p => { return ""; }) + ",title:'" + "人均下载统计" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        private string GetLineName(D_StatDownCountsBySoft_SUM down)
        {
                return down.ChannelName; 
        }

        protected override List<List<T>> GetData<T>(HttpContext context, bool isfortable = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);

            int areaid = Convert.ToInt32(context.Request["areaid"]);
            int areatype = Convert.ToInt32(context.Request["areatype"]);

            int modetype = Convert.ToInt32(context.Request["modetype"]);
            int period = Convert.ToInt32(context.Request["period"]);

            string channelNames = context.Request["channelnames"];
            string channelids = context.Request["channelids"];
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            string singlevalue = context.Request["singlehiddenvalue"] ?? "";
            string singlename = context.Request["singlehiddenname"] ?? "";
            int statype = Convert.ToInt32(context.Request["stattype"]);

            if (isfortable && !string.IsNullOrEmpty(singlevalue))
            {
                channelNames = singlename;
                channelids = singlevalue;
            }
     
            List<ChannelRight> list = new List<ChannelRight>();
            string[] channels = channelids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] channelarray = channelNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (channels.Length == 0)
            {
                channels = new string[] { "Customer_0" };
                channelarray = new string[] { "不区分渠道" };
            }
            //非渠道选项
            if (channels.Length != channelarray.Length && modetype == 1)
                return new List<List<D_StatDownCountsBySoft_SUM>>() as List<List<T>>;

            Dictionary<string, string> channelDic = new Dictionary<string, string>();
            for (int i = 0; i < channels.Count(); i++)
            {
                string[] strs = channels[i].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length == 2)
                {
                    list.Add(new ChannelRight()
                    {
                        ChannelType = (ChannelTypeOptions)Enum.Parse(typeof(ChannelTypeOptions), strs[0]),
                        ChannelID = Convert.ToInt32(strs[1])
                    });
                    channelDic.Add(channels[i], channelarray[i]);
                }
            }

            List<List<D_StatDownCountsBySoft_SUM>> lists = new List<List<D_StatDownCountsBySoft_SUM>>();
 
              
            for (int i = 0; i < list.Count; i++)
            {
                    var result = D_StatDownCountSum_Service.Instance.GetD_StatDownAvgCountByCache(restype, softs,
                                                                                                platform, begintime,
                                                                                                endtime,
                                                                                                period,
                                                                                                list[i].ChannelType,
                                                                                                channelDic[
                                                                                                    list[i]
                                                                                                        .ChannelType +
                                                                                                    "_" +
                                                                                                    list[i].ChannelID],
                                                                                                list[i].ChannelID,
                                                                                                projectsource,
                                                                                                new List<int>(), 
                                                                                                statype,
                                                                                                areaid,
                                                                                                modetype,
                                                                                                new URLoginService());
                    if (result.Count != 0)
                        lists.Add(result);
            }
            return lists as List<List<T>>;
        }


        protected override string DownloadExcelName
        {
            get { return "人均下载量统计.xls"; }
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownAvgCountByAreaImpl"; }
        }

        public override string ServiceName
        {
            get { return "人均下载量统计(地区)"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.NoCheck; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/D_DownlaodStatistics/D_ResDownloadAvgCountByArea.aspx"; }
        }
    }
}
