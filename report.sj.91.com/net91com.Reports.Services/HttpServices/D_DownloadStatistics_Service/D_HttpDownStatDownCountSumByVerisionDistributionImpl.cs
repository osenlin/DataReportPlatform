using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    class D_HttpDownStatDownCountSumByVerisionDistributionImpl : Abs_HttpDownLoad<D_StatDownCountsBySoft_SUM>
    {
        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownCountSumByVerisionDistributionImpl"; }
        }

        public override string ServiceName
        {
            get { return "下载汇总按版本分布"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight;}
        }

        public override string RightUrl
        {
            get { return "reports_new/d_downlaodstatistics/D_ResDownLoadSumByVersionDistributionl.aspx"; }
        }

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                if (softid == 46 || 85 == softid || 46 == softid)
                {
                    return new string[]
                         {
                             "排名","版本名称", "所有", "下载点击(去除更新)", "下载点击（来自更新）", "下载点击(来自搜索)",
                             "静默更新", "下载成功","下载成功（来自更新）", "下载成功（来自静默）", "下载失败" ,"下载失败（来自更新）","下载失败（来自静默）",
                             "安装成功","安装成功（来自更新）", "安装成功（来自静默）"
                             , "安装失败","安装失败（来自更新）", "安装失败（来自静默）"
                         };
                }
                else
                {
                    return new string[]
                         {
                             "排名","版本名称", "下载点击（所有）", "下载点击(去除更新)", "下载点击(静默更新)",
                             "来自搜索", "排期下载", "游戏下载（所有）", "游戏下载（去除更新）", "游戏下载（来自搜索）",
                             "下载成功", "下载失败"
                             , "安装成功", "安装失败"
                         };

                }

            }
        }


        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list, bool DetailEqChart = true)
        {
            var tempList = new List<List<string>>();

            if (list is List<D_StatDownCountsBySoft_SUM>)
            {
                var temp = list as List<D_StatDownCountsBySoft_SUM>;
                var list2 = temp.OrderByDescending(p => p.StatDate).ToList();
                //输出表格
                if (46 == softid || 85 == softid || 58 == softid)
                {
                    int i = 1;
                    foreach (D_StatDownCountsBySoft_SUM obj in list2)
                    {
                        List<string> values = new List<string>();
                       
                        values.Add((i++).ToString());
                        values.Add(obj.E_Version);
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
                        tempList.Add(values);
                    }
                }
                else
                {
                    int i = 1;
                    foreach (D_StatDownCountsBySoft_SUM obj in list2)
                    {
                        List<string> values = new List<string>();
                       
                        values.Add((i++).ToString());
                        values.Add(obj.E_Version);
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
                }
            }

            return tempList;
        }


        public override Result GetChart<T>(HttpContext context)
        {
            var lists = GetData<T>(context, false) as List<List<D_StatDownCountsBySoft_SUM>>;

            List<D_StatDownCountsBySoft_SUM> lstEntity;
            var showtype = int.Parse(context.Request["showtype"]);
            StringBuilder sb = new StringBuilder("[");
            if (lists.Count != 0)
            {
                lstEntity = lists[0].Take(10).ToList();
                foreach (D_StatDownCountsBySoft_SUM item in lstEntity)
                {
                     sb.AppendFormat("['{0}',{1}],", item.E_Version,GetValueType(item,showtype));
                }
                sb.AppendFormat("['其他',{0}]]", lists[0].Sum(p => GetValueType(p, showtype)) - lstEntity.Sum(p => GetValueType(p, showtype)));
            }
            else
            {
                sb.Append("]");
            }
            string result = sb.ToString();
            return Result.GetSuccessedResult(result, true);
        }

        private double GetValueType(D_StatDownCountsBySoft_SUM down, int showtype)
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
                    return down.GameDownCount - down.DownCountByUpdating;
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

        private int softid;
        protected override List<List<T>> GetData<T>(HttpContext context, bool flag = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int softs = Convert.ToInt32(context.Request["softs"]);
            softid = softs;
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);

            int period = Convert.ToInt32(context.Request["period"]);

            string channelNames = context.Request["channelnames"];
            string channelids = context.Request["channelids"];
            List<ChannelRight> list = new List<ChannelRight>();
            string[] channels = channelids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] channelarray = channelNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (channels.Length == 0)
            {
                channels = new string[] { "Customer_0" };
                channelarray = new string[] { "不区分渠道" };
            }
            //非渠道选项
            if (channels.Length != channelarray.Length)
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
                var result = D_StatDownCountSum_Service.Instance.GetD_StatDownByVersionDistribution_SUMByCache(restype, softs,
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
                                                                                                new List<int>(), 
                                                                                                new URLoginService()
                                                                                                );
                if (result.Count != 0)
                    lists.Add(result);


            }
            return lists as List<List<T>>;


        }

        protected override string DownloadExcelName
        {
            get { return "下载量版本分布.xls"; }
        }
    }
}
