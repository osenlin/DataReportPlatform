using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownAvgCountByVersionImpl : Abs_HttpDownLoad<D_StatDownCountsBySoft_SUM>
    {

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "日期", "新用户平均分发量", "老用户平均分发量", "人均分发量", "新用户平均分发量（一次分发）", "老用户平均分发量（一次分发）", "人均分发量（一次分发）", "静默平均分发（二次分发）", "人均分发量（二次分发）"
                    };
            }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list,bool DetailEqChart=true)
        {
            var tempList = new List<List<string>>();
            if (list is List<D_StatDownCountsBySoft_SUM>)
            {
                var temp = list as List<D_StatDownCountsBySoft_SUM>;
                var list2 = temp.OrderByDescending(p => p.StatDate).ToList();
                foreach (D_StatDownCountsBySoft_SUM obj in list2)
                {
                    List<string> values = new List<string>();
                    values.Add(obj.StatDate.ToString("yyyy-MM-dd"));
                    values.Add(UtilityHelp.GetDecimalDataString(obj.NewUserDownCount * 1.0 / (obj.NewUserCount == 0 ? 1 : obj.NewUserCount)));
                    int user = obj.UserCount - obj.NewUserCount;
                    values.Add(
                        UtilityHelp.GetDecimalDataString<double>((obj.DownCount - obj.NewUserDownCount)*1.0/(user == 0 ? 1 : user)));
                    values.Add(UtilityHelp.GetDecimalDataString<double>(obj.DownCount * 1.0 / (obj.UserCount == 0 ? 1 : obj.UserCount)));

                    //去除更新人均分发
                    int userupdateex = obj.UserCountExceptAllUpdating - obj.NewUserCountExceptAllUpdating;
                    values.Add(UtilityHelp.GetDecimalDataString<double>(obj.NewUserDownCountExceptAllUpdating * 1.0 / (obj.NewUserCountExceptAllUpdating == 0 ? 1 : obj.NewUserCountExceptAllUpdating)));
                    
                    values.Add(UtilityHelp.GetDecimalDataString<double>((obj.DownCountExceptAllUpdating - obj.NewUserDownCountExceptAllUpdating) * 1.0 / (userupdateex == 0 ? 1 : userupdateex)));
                    values.Add(UtilityHelp.GetDecimalDataString<double>(obj.DownCountExceptAllUpdating * 1.0 / (obj.UserCountExceptAllUpdating == 0 ? 1 : obj.UserCountExceptAllUpdating)));

                    values.Add(UtilityHelp.GetDecimalDataString<double>(obj.DownCountBySlienceUpdating * 1.0 / obj.UserCountSilenceUpdateing));
                    values.Add(UtilityHelp.GetDecimalDataString<double>((obj.DownCount - obj.DownCountExceptAllUpdating) * 1.0 / obj.UserCountUpdateing ));
                    tempList.Add(values);
                }
                if (list2.Count != 0)
                {
                    List<string> meavValues = new List<string>();
                    meavValues.Add("均值");
                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => (obj.NewUserDownCount * 1.0 / (obj.NewUserCount == 0 ? 1 : obj.NewUserCount))).Average()));
                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => (obj.DownCount - obj.NewUserDownCount) * 1.0 / (obj.UserCount - obj.NewUserCount == 0 ? 1 : obj.UserCount - obj.NewUserCount)).Average()));
                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => (obj.DownCount * 1.0 / (obj.UserCount == 0 ? 1 : obj.UserCount))).Average()));
                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => (obj.NewUserDownCountExceptAllUpdating * 1.0 / (obj.NewUserCountExceptAllUpdating == 0 ? 1 : obj.NewUserCountExceptAllUpdating))).Average()));
                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => ((obj.DownCountExceptAllUpdating - obj.NewUserDownCountExceptAllUpdating) * 1.0 / (obj.UserCountExceptAllUpdating - obj.NewUserCountExceptAllUpdating == 0 ? 1 : obj.UserCountExceptAllUpdating - obj.NewUserCountExceptAllUpdating))).Average()));

                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => (obj.DownCountExceptAllUpdating * 1.0 / (obj.UserCountExceptAllUpdating == 0 ? 1 : obj.UserCountExceptAllUpdating))).Average()));
                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => (obj.DownCountBySlienceUpdating * 1.0 / obj.UserCountSilenceUpdateing)).Average()));
                    meavValues.Add(UtilityHelp.GetDecimalDataString<double>(list2.Select(obj => (obj.DownCount - obj.DownCountExceptAllUpdating) * 1.0 / obj.UserCountUpdateing).Average()));
                    tempList.Add(meavValues);
                }
            }
            return tempList;
        }

        public override Result GetChart<T>(HttpContext context)
        {
            List<LineChartLine> lines = new List<LineChartLine>();
            int modetype = Convert.ToInt32(context.Request["modetype"]);
            int showtype = Convert.ToInt32(context.Request["showtype"]);
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int period = Convert.ToInt32(context.Request["period"]);

            string version = "";
            if (context.Request["version"] == "null" || string.IsNullOrEmpty(context.Request["version"]))
            {
                version = "-1";
            }
            else
            {
                version = context.Request["version"];
            }
            var lists = GetData<T>(context, false) as List<List<D_StatDownCountsBySoft_SUM>>;
            List<List<D_StatDownCountsBySoft_SUM>> lst;
            int mutitype = Convert.ToInt32(context.Request["mutitype"]);
            if (mutitype == 2)
            {
                lst = new List<List<D_StatDownCountsBySoft_SUM>>();
                string[] versions = version.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (lists.Count > 0)
                {
                    List<int> lsttmp = versions.Select(p => int.Parse(p)).ToList();
                    var entityversionlst=B_BaseToolService.Instance.GetVersionCache(lsttmp);
                    foreach (B_VersionEntity ver in entityversionlst)
                    {
                        var partlst = lists[0].Where(p => p.E_Version == ver.Version).ToList();
                        if (partlst.Count != 0)
                        {
                            lst.Add(partlst);
                        }
                    }
                }
            }
            else
            {
                lst = lists;
            }

            for (int j = 0; j < lst.Count; j++)
            {
                var linename = GetLineName(mutitype, lst[j][0]);
                List<LineChartPoint> points = lst[j].Select(p => new LineChartPoint
                    {
                        XValue = p.StatDate.ToString("yyyyMMdd"),
                        DataContext = "",
                        NumberType = showtype == 3 ? 3 : 1,
                        Description = "",
                        YValue = (p.DownCount * 1.0 / (p.UserCount == 0 ? 1 : p.UserCount)).ToString()
                    }).ToList();
                lines.Add(new LineChartLine { Name = linename + "_所有人均分发", Points = points, Show = true });
            }

            for (int j = 0; j < lst.Count; j++)
            {
                var linename = GetLineName(mutitype, lst[j][0]);
                List<LineChartPoint> points = lst[j].Select(p => new LineChartPoint
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

        private string GetLineName(int mutitype, D_StatDownCountsBySoft_SUM down)
        {
            if (mutitype == 2)
                return down.E_Version;
            else
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


            string version = "";
            if (context.Request["version"] == "null" || string.IsNullOrEmpty(context.Request["version"]))
            {
                version = "-1";
            }
            else
            {
                version = context.Request["version"];
            }

            int modetype = Convert.ToInt32(context.Request["modetype"]);
            int period = Convert.ToInt32(context.Request["period"]);

            string channelNames = context.Request["channelnames"];
            string channelids = context.Request["channelids"];
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            string singlevalue = context.Request["singlehiddenvalue"] ?? "";
            string singlename = context.Request["singlehiddenname"] ?? "";
            int statype = Convert.ToInt32(context.Request["stattype"]);

            int mutitype = Convert.ToInt32(context.Request["mutitype"]);
            
            if (1 == areatype)
            {
                areaid = -1;
            }
           

            if (mutitype==1)//渠道多选
            {
                projectsource = -1;
                if (isfortable && !string.IsNullOrEmpty(singlevalue))
                {
                    channelNames = singlename;
                    channelids = singlevalue;
                }
            }
            else if (mutitype == 2)//版本多选
            {
                projectsource = -1;
                if (isfortable && !string.IsNullOrEmpty(singlevalue))
                {
                    version = singlevalue;
                }
            }

            List<ChannelRight> list = new List<ChannelRight>();
            List<int> e_versionids = new List<int>();
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
            string[] versions = version.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var ver in versions)
            {
                e_versionids.Add(Convert.ToInt32(ver));
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
                                                                                                e_versionids,
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
            get { return "D_HttpDownStatDownAvgCountByVersionImpl"; }
        }

        public override string ServiceName
        {
            get { return "人均下载量统计"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.NoCheck; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/D_DownlaodStatistics/D_ResDownloadAvgCountByVersion.aspx"; }
        }
    }
}
