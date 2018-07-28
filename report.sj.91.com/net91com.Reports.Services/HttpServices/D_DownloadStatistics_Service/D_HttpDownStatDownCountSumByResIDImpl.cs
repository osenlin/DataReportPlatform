using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net91com.Core;
using net91com.Core.Util;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Entities.ViewEntity.R_DownloadStatistics;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownCountSumByIDImpl : Abs_HttpDownLoad<D_StatDownCountsBySoft_SUM>
    {


        protected override string DownloadExcelName
        {
            get { return "下载量查询（资源/位置）.xls"; }
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownCountSumByResIDImpl"; }
        }

        public override string ServiceName
        {
            get { return "下载量查询（资源/位置）"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "/Reports_New/D_DownlaodStatistics/D_ResDownLoadByResID.aspx"; }
        }
        //表示选择了资源名称，资源ID,资源包名中的一种
        private string resselecttype = "1";
        //海外库2，还是国内库1
        private int areatype = 1;
        //资源名称或资源ID或资源包的值
        private string rescontext = "";
        //是否显示最后一列
        private int showdetailposition = 1;

        protected override string[] DownloadExcelDataTableTitleName
        {
            get { return new string[] { "时间", "所有（下载点击）", "更新（下载点击）", "去除更新（下载点击）", "来自搜索（下载点击）", "下载成功", "下载成功(去除更新)", "下载失败", "安装成功", "安装成功(去除更新)", "安装失败", "下载浏览" }; }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list, bool DetailEqChart = true)
        {
            var lstlstname = new List<List<string>>();
            var temp = list as List<D_StatDownCountsBySoft_SUM>;
            foreach (D_StatDownCountsBySoft_SUM obj in temp)
            {
                var values = new List<string>();
                values.Add(obj.StatDate.ToString("yyyy-MM-dd"));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownCount));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownCountByUpdating));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownCount - obj.DownCountByUpdating));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownCountBySearching));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCount));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownSuccessCountByExceptAllUpdate));
                values.Add(UtilityHelp.FormatNum<int>(obj.DownFailCount));
                values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCount));
                values.Add(UtilityHelp.FormatNum<int>(obj.SetUpSuccessCountByExceptAllUpdate));
                values.Add(UtilityHelp.FormatNum<int>(obj.SetUpFailCount));
                values.Add(UtilityHelp.FormatNum<int>(obj.BrowseCount));
                if (DetailEqChart)
                {
                    if (showdetailposition == 1)
                    {
                        values.Add(obj.StatDate.ToString("yyyy-MM-dd") + "_" + obj.SoftID + "_" + obj.Platform + "_" + obj.ResType + "_" + obj.VersionID + "_" + obj.SourceID + "_" + areatype + "_" + resselecttype + "_" + rescontext+"_"+obj.Area);
                    }
                    else
                    {
                        values.Add("");
                    }
                }
                lstlstname.Add(values);
            }
            if (temp.Count > 0)
            {
                var values2 = new List<string>();
                values2.Add("汇总");
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.DownCount)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.DownCountByUpdating)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.DownCount - p.DownCountByUpdating)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.DownCountBySearching)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.DownSuccessCount)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.DownSuccessCountByExceptAllUpdate)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.DownFailCount)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.SetUpSuccessCount)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.SetUpSuccessCountByExceptAllUpdate)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.SetUpFailCount)));
                values2.Add(UtilityHelp.FormatNum<int>(temp.Sum(p => p.BrowseCount)));
                if (DetailEqChart)
                {
                    values2.Add("");
                }
                lstlstname.Add(values2);
            }

            return lstlstname;
        }

        public override Result GetChart<T>(System.Web.HttpContext context)
        {
            List<LineChartLine> lines = new List<LineChartLine>();
            int modetype = Convert.ToInt32(context.Request["modetype"]);
            int showtype = Convert.ToInt32(context.Request["showtype"]);
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            string stattype = context.Request["stattype"];
            List<int> stattypes = stattype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToList();
            int softid = Convert.ToInt32(context.Request["softid"]);
            int platform = Convert.ToInt32(context.Request["platform"]);

            var lists = GetData<T>(context, false) as List<List<D_StatDownCountsBySoft_SUM>>;
            var lstversion = B_BaseToolService.Instance.GetVersionCache(softid, platform);

            foreach (int itemstattype in stattypes)
            {
                for (int j = 0; j < lists.Count; j++)
                {
                    var linename = GetLineName(modetype, lists[j][0], itemstattype, lstversion);
                    List<LineChartPoint> points = lists[j].Select(p => new LineChartPoint
                    {
                        XValue = p.StatDate.ToString("yyyyMMdd"),
                        DataContext = "",
                        NumberType = showtype == 3 ? 3 : 1,
                        Description = "",
                        YValue = GetValue(p, itemstattype).ToString()
                    }).ToList();
                    lines.Add(new LineChartLine { Name = linename, Points = points, Show = true });
                }
            }
            LineChart line = new LineChart(GetXLine(begintime, endtime, 1), lines);
            string result = "{ x:" + line.GetXJson(p => { return ""; }) + ",y:" + line.GetYJson(p => { return ""; }) + ",title:'" + "下载量查询（资源/位置）" + "'}";
            return Result.GetSuccessedResult(result, true);
        }

        private string GetLineName(int modetype, D_StatDownCountsBySoft_SUM obj, int showtype, List<B_VersionEntity> lstversion)
        {
            string vername = "";
            if (obj.VersionID == -1)
            {
                vername = "不区分版本";
            }
            else
            {
                vername = lstversion.Where(p => p.ID == obj.VersionID).Select(p => p.Version).First();
            }

            if (modetype == 2)
                return vername + "_" + GetShowTypeString(showtype);
            else
                return GetShowTypeString(showtype);
        }

        private double GetValue(D_StatDownCountsBySoft_SUM down, int showtype)
        {
            switch (showtype)
            {
                case 1:
                    return down.DownCount;
                case 2:
                    return down.BrowseCount;
                case 4:
                    return down.DownSuccessCount;
                case 5:
                    return down.SetUpSuccessCount;
                case 6:
                    return down.SetUpFailCount;
                case 8:
                    return down.DownFailCount;
                default:
                    return 0;
            }
        }

        private string GetShowTypeString(int showType)
        {
            string showTypeString = string.Empty;
            switch (showType)
            {
                case 1:
                    showTypeString = "下载点击";
                    break;
                case 2:
                    showTypeString = "展示";
                    break;
                case 4:
                    showTypeString = "下载成功";
                    break;
                case 5:
                    showTypeString = "安装成功";
                    break;
                case 6:
                    showTypeString = "安装失败";
                    break;
                case 8:
                    showTypeString = "下载失败";
                    break;
            }
            return showTypeString;
        }

        private List<int> GetPositionList(string[] sts)
        {
            List<int> positonlist = new List<int>();
            Regex regex = new Regex("\\d+");
            for (int i = 0; i < sts.Length; i++)
            {
                if (regex.IsMatch(sts[i]))
                {
                    var value = int.Parse(sts[i]);
                    if (value != 0)
                        positonlist.Add(value);
                }
            }
            return positonlist;
        }



        protected override List<List<T>> GetData<T>(System.Web.HttpContext context, bool flag = false)
        {

            supdetaileqchart = false;

            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int sourceid = Convert.ToInt32(context.Request["projectsource"]);
            string versions = context.Request["version"];
            List<int> lstversion = versions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p)).ToList();
            int softid = Convert.ToInt32(context.Request["softid"]);
            int platform = Convert.ToInt32(context.Request["platform"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int resid = Convert.ToInt32(context.Request["resid"]);
            int areaid = Convert.ToInt32(context.Request["areaid"]);
            string position = context.Request["position"] == "" ? "0" : context.Request["position"];
            position = Regex.Replace(position, "[\r\n ]", "");
            string[] sts = position.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> lstpositon = GetPositionList(sts);

            //下面三个字段主要是为了获取明细位置提供参数
            resselecttype = context.Request["resselecttype"];
            string resname = context.Request["resname"];
            areatype = Convert.ToInt32(context.Request["areatype"]);


            if (sourceid == -1 || sourceid == 2)
            {
                showdetailposition = 0;
            }
            else
            {
                showdetailposition = 1;
            }
            var list = new List<List<ResourceDownLoad>>();
            List<int> lstresid;
            if (resid > 0)
            {
                lstresid = new List<int>() { resid };
                rescontext = resid + "";
            }
            else
            {
                List<B_ResInfo> resInfos = new B_BaseTool_DataAccess().GetResInfo(resname, restype, areatype);
                lstresid = resInfos.Select(p => p.ResId).ToList();
                rescontext = resname;
            }

            List<List<D_StatDownCountsBySoft_SUM>> lists = new List<List<D_StatDownCountsBySoft_SUM>>();
            foreach (int version in lstversion)
            {
                var result = D_StatDownCountSum_Service.Instance.GetD_StatDownCountSumByResIDCache(softid, platform, lstresid, restype,
                                                                        begintime, endtime, lstpositon, version,
                                                                        sourceid, areaid, areatype);
                if (result.Count != 0)
                {
                    lists.Add(result);
                }
                LogHelper.WriteInfo("result.count:" + result.Count);
            }
    
            return lists as List<List<T>>;
        }
    }
}