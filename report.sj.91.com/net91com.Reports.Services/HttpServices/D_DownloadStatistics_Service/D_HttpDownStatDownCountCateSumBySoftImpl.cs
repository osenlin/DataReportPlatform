using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownCountCateSumBySoftImpl : Abs_HttpDownLoad<D_StatDownCountsBySoft_SUM>
    {

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "日期", "所有（下载点击）"
                        , "去除更新（下载点击）", "来自搜索（下载点击）"
                        , "所有（下载成功）", "去除更新（下载成功）"
                        , "所有（下载失败）", "去除更新（下载失败）"
                        , "所有（安装成功）", "去除更新（安装成功）"
                        , "所有（安装失败）", "去除更新（安装失败）"
                    };
            }
        }

        public override Result GetChart<T>(HttpContext context)
        {
            List<LineChartLine> lines = new List<LineChartLine>();
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int softs = Convert.ToInt32(context.Request["softs"]);


            int downtype = Convert.ToInt32(context.Request["downtype"]);
            var lists = GetData<T>(context, false) as List<List<D_StatDownCountsBySoft_SUM>>;
            int period = Convert.ToInt32(context.Request["period"]);


            for (int j = 0; j < lists.Count; j++)
            {
                var linename = GetLineName(softs,lists[j][0].Platform, lists[j][0].Pcid, lists[j][0].ResType);
                List<LineChartPoint> points = lists[j].Select(p => new LineChartPoint
                {
                    XValue = p.StatDate.ToString("yyyyMMdd"),
                    NumberType =  1,
                    YValue = GetIndexValue(p, downtype).ToString()//这边要根据前台写的要求来
                }).ToList();
                lines.Add(new LineChartLine { Name = linename, Points = points, Show = true });
            }
            LineChart line = new LineChart(GetXLine(begintime, endtime, period), lines);
            string result = "{ x:" + line.GetXJson(p => { return ""; }) + ",y:" + line.GetYJson(p => { return ""; }) + ",title:'" + "下载量统计（分类）" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        private string GetLineName(int softid,int platform, int pcid,int restype)
        {
            string name = "";
            switch (platform)
            {
                case 1:
                    name = "IPhone";
                    break;
                case 4:
                    name = "Android";
                    break;
                case 7:
                    name = "IPad";
                    break;
                case 9:
                    name="AndroidPad";
                    break;
                default:
                    name = "不区分平台";
                    break;
            }
            int type = 1;
            if (softid==-46)
            {
                type = 2;
            }
            if (pcid==0)
            {
                name = name + "_" + "不区分大分类";
            }
            else
            {
                name = name + "_" + B_BaseToolService.Instance.GetResCateCache(type)
                                     .Where(p => p.CID == pcid && p.ResType == restype).First().CName;
            }

            return name;

        }

        private double GetIndexValue(D_StatDownCountsBySoft_SUM down, int downtype)
        {
            switch (downtype)
            {
                case -1:
                    return down.DownCount;
                case 3:
                    return down.DownCountBySearching;
                case 5:
                    return down.DownCountExceptAllUpdating;
                default:
                    return 0;
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
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownCount));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownCountExceptAllUpdating));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownCountBySearching));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCount));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCountByExceptAllUpdate));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownFailCount));
                    values.Add(UtilityHelp.FormatNum<int>(obj.DownFailCountByExceptAllUpdate));
                    values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCount));
                    values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCountByExceptAllUpdate));
                    values.Add(UtilityHelp.FormatNum<int>(obj.SetUpFailCount));
                    values.Add(UtilityHelp.FormatNum<int>(obj.SetUpFailCountByExceptAllUpdate));
                    tempList.Add(values);
                }
            }
            return tempList;
        }


        protected override List<List<T>> GetData<T>(HttpContext context, bool flag = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
         
            int softs = Convert.ToInt32(context.Request["softs"]);

            int restype = Convert.ToInt32(context.Request["restype"]);
            int period = Convert.ToInt32(context.Request["period"]);

            string plats = context.Request["plat"];
            List<string> lstplat = plats.Split(',').ToList();

            string  pcids = context.Request["pcid"];
            List<string> lstpcid = pcids.Split(',').ToList();

            int cid = Convert.ToInt32(context.Request["cid"]);
            //如果大分类多选了或者大分类不区分分类，子类一定是不区分
            if (lstpcid.Count>1 || (lstpcid.Count==1 && lstpcid[0]=="0") ||cid==-1)
            {
                cid = 0;
            }
            int downtype = Convert.ToInt32(context.Request["downtype"]);

            List<List<D_StatDownCountsBySoft_SUM>> lists = new List<List<D_StatDownCountsBySoft_SUM>>();

            List<D_StatDownCountsBySoft_SUM> result;
            foreach (string itemplat in lstplat)
            {
                foreach (string itempcid in lstpcid)
                {
                    result = D_StatDownCountSum_Service.Instance.GetD_StatDownCountCateSumByCache(restype, softs, int.Parse(itemplat),
                                                                                            begintime, endtime, period, int.Parse(itempcid), cid, downtype);

                    if (result.Count != 0)
                    {
                        lists.Add(result);
                    }
                }
            }
            

            return lists as List<List<T>>;
        }

        protected override string DownloadExcelName
        {
            get { return "下载统计（按分类）.xls"; }
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownCountCateSumBySoftImpl"; }
        }

        public override string ServiceName
        {
            get { return "下载统计（按分类）"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/D_DownlaodStatistics/D_ResDownLoadCateSumBySoft.aspx"; }
        }
    }
}
