using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownCountSumBySoftImpl : Abs_HttpDownLoad<D_StatDownCountsBySoft_SUM>
    {
         private int softid = 0;
         public override Result GetChart<T>(HttpContext context)
         {
             List<LineChartLine> lines = new List<LineChartLine>();
             int modetype = Convert.ToInt32(context.Request["modetype"]);
             int period = Convert.ToInt32(context.Request["period"]);


             DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
             DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
             string areatype = context.Request["areatype"];
             var lists = GetData<T>(context, false) as List<List<D_StatDownCountsBySoft_SUM>>;

             string showTypes = context.Request["showtype"];
             List<int> showTypesList =
                 showTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(p => Convert.ToInt32(p))
                          .ToList();

             string versionreq = "";
             if (context.Request["version"] == "null" || string.IsNullOrEmpty(context.Request["version"]) ||
                 context.Request["version"] == "-1")
             {
                 versionreq = "-1";
             }
             else
             {
                 versionreq = context.Request["version"];
             }
             List<string> versions = versionreq.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
             if (modetype == 2 && versions.Count > 1)
             {
                 List<List<D_StatDownCountsBySoft_SUM>> listtemp = new List<List<D_StatDownCountsBySoft_SUM>>();

                 if (lists.Count != 0)
                 {
               
                     for (int i  = 0; i < versions.Count; i++)
                     {
                         var linelist = lists[0].Where(p => p.E_Version == versions[i]).ToList();
                         if (linelist.Count!=0)
                         {
                             listtemp.Add(linelist); 
                         }
                     }
                 }

                 for (int j = 0; j < listtemp.Count; j++)
                 {
                     var appversion = GetLineName(modetype, listtemp[j][0], showTypesList[0], areatype);
                     List<LineChartPoint> points = listtemp[j].Select(p => new LineChartPoint
                     {
                         XValue = p.StatDate.ToString("yyyyMMdd"),
                         DataContext = "",
                         NumberType = 1,
                         Description = "",
                         YValue = GetIndexValue(p, showTypesList[0]).ToString()
                     }).ToList();
                     lines.Add(new LineChartLine { Name = appversion, Points = points, Show = true });
                 }
             }
             else
             {
                 for (int i = 0; i < showTypesList.Count; i++)
                 {
                     for (int j = 0; j < lists.Count; j++)
                     {
                         var appversion = GetLineName(modetype, lists[j][0], showTypesList[i], areatype);
                         List<LineChartPoint> points = lists[j].Select(p => new LineChartPoint
                         {
                             XValue = p.StatDate.ToString("yyyyMMdd"),
                             DataContext = "",
                             NumberType = 1,
                             Description = "",
                             YValue = GetIndexValue(p, showTypesList[i]).ToString()
                         }).ToList();
                         lines.Add(new LineChartLine { Name = appversion, Points = points, Show = true });
                     }
                 }
             }

             LineChart chart = new LineChart(GetXLine(begintime, endtime, period), lines);
             string result = "{ x:" + chart.GetXJson(p => { return ""; }) + ",y:" + chart.GetYJson(p => { return ""; }) +
                             ",title:'" + "下载量统计" + "'}";
             return Result.GetSuccessedResult(result, true);
         }

         protected override List<List<T>> GetData<T>(HttpContext context, bool isfortable = false)
         {
             DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
             DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
             int softs = Convert.ToInt32(context.Request["softs"]);
             softid = softs;
             int platform = Convert.ToInt32(context.Request["plat"]);
             int restype = Convert.ToInt32(context.Request["restype"]);
             string version = "";
             if (context.Request["version"] == "null" || string.IsNullOrEmpty(context.Request["version"]) ||
                 context.Request["version"] == "0")
             {
                 version = "";
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
             string areaids = context.Request["areaid"];
             string areaytpe = context.Request["areatype"];

             string statype = context.Request["showtype"];

             string multype = context.Request["multype"] ?? "";
             //2,渠道或者下载渠道多选或者版本多选 3 展示下载类型多选，4地区多选
             if (!multype.Equals("3") && !multype.Equals("4"))
             {
                 if (modetype == 1) //分渠道
                 {
                     projectsource = -1;
                     version = "";
                     if (isfortable && !string.IsNullOrEmpty(singlevalue))
                     {
                         channelNames = singlename;
                         channelids = singlevalue;
                     }
                 }
                 else if (modetype == 2) //分版本
                 {
                     channelids = "";
                     channelNames = "";
                     projectsource = -1;
                     if (isfortable && !string.IsNullOrEmpty(singlevalue))
                     {
                         version = singlevalue;
                     }
                 }
                 else //分项目来源
                 {
                     version = "";
                     channelids = "";
                     channelNames = "";
                     if (isfortable && !string.IsNullOrEmpty(singlevalue))
                     {
                         projectsource = Convert.ToInt32(singlevalue);
                     }
                 }
             }
             else if (multype.Equals("3"))
             {
                 statype = singlevalue;
             }
             else if (multype.Equals("4"))
             {
                 areaids = singlevalue;
             }

             List<string> statypelist = statype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
             List<string> areaidlist = areaids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
             List<ChannelRight> list = new List<ChannelRight>();
             List<string> e_versionids = new List<string>();
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
                 e_versionids.Add(ver);
             }

             List<List<D_StatDownCountsBySoft_SUM>> lists = new List<List<D_StatDownCountsBySoft_SUM>>();
             foreach (string areaiditem in areaidlist)
             {
                 string areaid = areaiditem;
                 string province = "-1";
                 //国内
                 if (areaytpe.Equals("1"))
                 {
                     if (areaiditem.Equals("-1"))
                     {
                         areaid = "-1";
                         //为国家ID
                         province = "-1";
                     }
                     else
                     {
                         areaid = "CN";
                         province =areaiditem;
                     }
                 }
                 for (int i = 0; i < list.Count; i++)
                 {
                     var result = D_StatDownCountSum_Service.Instance.GetD_StatDownBySoft_SUMByCache(restype, softs,
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
                                                                                                   areaid,
                                                                                                   province, 
                                                                                                   statypelist,
                                                                                                   new URLoginService(),
                                                                                                   Convert.ToInt32(areaytpe));
                     if (result.Count != 0)
                         lists.Add(result);

                 }
             }
             return lists as List<List<T>>;
         }

         protected override string DownloadExcelName
         {
             get { return "下载量统计.xls"; }
         }

         protected override string[] DownloadExcelDataTableTitleName
         {
             get
             {
                 if (softid == 46 || 85 == softid || 46 == softid)
                 {
                     return new string[]
                         {
                             "日期", "所有", "下载点击(去除更新)", "下载点击（来自更新）", "下载点击(来自搜索)",
                             "静默更新", "下载成功","下载成功（来自更新）", "下载成功（来自静默）", "下载失败" ,"下载失败（来自更新）","下载失败（来自静默）",
                             "安装成功","安装成功（来自更新）", "安装成功（来自静默）"
                             , "安装失败","安装失败（来自更新）", "安装失败（来自静默）","游戏下载","游戏一次下载","游戏二次下载"
                         };
                 }
                 else
                 {
                     return new string[]
                         {
                             "日期", "下载点击（所有）", "下载点击(去除更新)", "下载点击(静默更新)",
                             "来自搜索", "排期下载", "游戏下载（所有）", "游戏下载（去除更新）", "游戏下载（来自搜索）",
                             "下载成功", "下载失败"
                             , "安装成功", "安装失败"
                         };

                 }

             }
         }

         protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list,bool DetailEqChart=true)
         {
             var tempList = new List<List<string>>();

             if (list is List<D_StatDownCountsBySoft_SUM>)
             {
                 var temp = list as List<D_StatDownCountsBySoft_SUM>;
                 var list2 = temp.OrderByDescending(p => p.StatDate).ToList();
                 //输出表格
                 if (46==softid || 85 == softid || 58 == softid)
                 {
                     foreach (D_StatDownCountsBySoft_SUM obj in list2)
                     {
                         List<string> values = new List<string>();
                         values.Add(obj.StatDate.ToString("yyyy-MM-dd"));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCountExceptUpdating, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCountByUpdating, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCountBySearching, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCountBySlience, false));

                         values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCountByUpdateNoSlience, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCountBySlience, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownFailCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownFailCountByUpdateNoSlience, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownFailCountBySlience, false));

                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCountByUpdateNoSlience, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCountBySlience, false));

                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpFailCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpFailCountByUpdateNoSlience, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpFailCountBySlience, false));

                         values.Add(UtilityHelp.FormatNum<int>(obj.GameDownCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.GameDownCount-obj.GameDownCountByUpdating, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.GameDownCountByUpdating, false));
                         tempList.Add(values);
                     }
                     if (list2.Count!=0)
                     {
                         List<string> meavValues = new List<string>();
                         meavValues.Add("均值");
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCountExceptUpdating).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCountByUpdating).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCountBySearching).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCountBySlience).Average(), false));

                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownSuccessCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownSuccessCountByUpdateNoSlience).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownSuccessCountBySlience).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownFailCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownFailCountByUpdateNoSlience).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownFailCountBySlience).Average(), false));

                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpSuccessCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpSuccessCountByUpdateNoSlience).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpSuccessCountBySlience).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpFailCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpFailCountByUpdateNoSlience).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpFailCountBySlience).Average(), false));

                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.GameDownCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => (p.GameDownCount - p.GameDownCountByUpdating)).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.GameDownCountByUpdating).Average(), false));

                         tempList.Add(meavValues);
                     }

                 }
                 else
                 {
                     foreach (D_StatDownCountsBySoft_SUM obj in list2)
                     {
                         List<string> values = new List<string>();
                         values.Add(obj.StatDate.ToString("yyyy-MM-dd"));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCountExceptUpdating, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCountBySlience, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownCountBySearching, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.ScheduleDownCount, false));

                         values.Add(UtilityHelp.FormatNum<int>(obj.GameDownCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.GameDownCount - obj.GameDownCountByUpdating, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.GameDownCountBySearching, false));

                         values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.DownFailCount, false));

                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCount, false));
                         values.Add(UtilityHelp.FormatNum<int>(obj.SetUpFailCount, false));
                         tempList.Add(values);
                     }
                     if (list2.Count != 0)
                     {
                         List<string> meavValues = new List<string>();
                         meavValues.Add("均值");
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCountExceptUpdating).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCountBySlience).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownCountBySearching).Average(), false)); 
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.ScheduleDownCount).Average(), false));

                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.GameDownCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.GameDownCount - p.GameDownCountByUpdating).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.GameDownCountBySearching).Average(), false));

                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownSuccessCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.DownFailCount).Average(), false));

                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpSuccessCount).Average(), false));
                         meavValues.Add(UtilityHelp.FormatNum<double>(list2.Select(p => p.SetUpFailCount).Average(), false));
                         tempList.Add(meavValues);
                     }
                 }
             }

             return tempList;
         }

         public override string ServiceCategory
         {
             get { return "D_HttpResDownLoadSumBySoftImpl"; }
         }

         public override string ServiceName
         {
             get { return "新资源下载汇总按产品"; }
         }

         public override RightEnum RightType
         {
             get { return RightEnum.OnlyUrlRight; }
         }

         public override string RightUrl
         {
             get { return "reports_new/d_downlaodstatistics/d_resdownloadsumbysoft.aspx"; }
         }

         /// <summary>
         /// 获取展示类型名称
         /// </summary>
         /// <param name="showType"></param>
         /// <returns></returns>
         private string GetShowTypeString(int showType)
         {
             string showTypeString = string.Empty;
             switch (showType)
             {
                 case 1:
                     showTypeString = "下载点击(所有)";
                     break;
                 case 2:
                     showTypeString = "下载点击(去除更新)";
                     break;
                 case 3:
                     showTypeString = "下载点击(搜索带量)";
                     break;
                 case 4:
                     showTypeString = "下载点击(更新带量)";
                     break;
                 case 5:
                     showTypeString = "去除更新和搜索";
                     break;
                 case 6:
                     showTypeString = "游戏下载";
                     break;
                 case 7:
                     showTypeString = "游戏下载(去除更新)";
                     break;
                 case 8:
                     showTypeString = "下载成功";
                     break;
                 case 9:
                     showTypeString = "安装成功";
                     break;
                 case 10:
                     showTypeString = "排期下载";
                     break;
                 case 11:
                     showTypeString = "";
                     break;
             }
             return showTypeString;
         }
         private double GetIndexValue(D_StatDownCountsBySoft_SUM down, int showtype)
         {
             switch (showtype)
             {
                 case 1:
                     return down.DownCount;
                 case 2:
                     return down.DownCountExceptUpdating;
                 case 3:
                     return down.DownCountBySearching;
                 case 4:
                     return down.DownCountByUpdating;
                 case 5:
                     return down.DownCountExceptUpdating - down.DownCountBySearching;
                 case 6:
                     return down.GameDownCount;
                 case 7:
                     return down.GameDownCount - down.GameDownCountBySearching;
                 case 8:
                     return down.DownSuccessCount;
                 case 9:
                     return down.SetUpSuccessCount;
                 case 10:
                     return down.ScheduleDownCount;
                 default:
                     return 0;
             }
         }

         /// <summary>
         /// 获取曲线名称
         /// </summary>
         /// <param name="modetype"></param>
         /// <param name="down"></param>
         /// <param name="showtype"></param>
         /// <returns></returns>
         private string GetLineName(int modetype, D_StatDownCountsBySoft_SUM down, int showtype, string areatype)
         {
             string area_str = "";

             if (areatype.Equals("1"))
             {
                 if ("-1" == down.Province)
                 {
                     area_str = "不区分地区";
                 }
                 else
                 {
                     area_str =
                         B_BaseToolService.Instance.GetProvincesCache()
                                          .Where(p => p.EnShortName == down.Province)
                                          .Select(p => p.Name)
                                          .First();
                 }
             }
             else if (areatype.Equals("2"))
             {
                 if ("-1" == down.Country)
                 {
                     area_str = "不区分地区";
                 }
                 else
                 {
                     area_str =
                         B_BaseToolService.Instance.GetCountriesCache()
                                          .Where(p => p.EnShortName == down.Country)
                                          .Select(p => p.Name)
                                          .First();
                 }
             }

             if (modetype == 1)
                 return down.ChannelName + "_" + GetShowTypeString(showtype) + "_" + area_str;
             else if (modetype == 2)
             {
                 string versionname = down.E_Version;
                 if (down.E_Version=="-1")
                 {
                     versionname = "不区分版本";
                 }
                 return versionname + "_" + GetShowTypeString(showtype) + "_" + area_str;
             }
                
             else
                 return UtilityHelp.GetProjectSoureNameByID(down.SourceID) + "_" + GetShowTypeString(showtype) + "_" +
                        area_str;
         }

     }
}
