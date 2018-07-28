using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownPositionDistributionImpl : Abs_HttpDownLoad<D_StatDownPositionDistribution>
    {
        private int diffdays = 1;
        private int isdiffpage = 1;
        public override Result GetChart<T>(HttpContext context)
        {

            var lists = GetData<T>(context, false) as List<List<D_StatDownPositionDistribution>>;

            List<D_StatDownPositionDistribution> lstEntity;
                   
            StringBuilder sb=new StringBuilder("[");
            if (lists.Count!=0)
            {
                lstEntity = lists[0].Take(10).ToList();
                foreach (D_StatDownPositionDistribution item in lstEntity)
                {
                    if (!string.IsNullOrEmpty(item.PageName))
                    {
                        if (!string.IsNullOrEmpty(item.PositionName))
                        {
                            sb.AppendFormat("['{0}',{1}],", item.PageName + "_" + item.PositionName + "_" + item.PageType, item.DownCount);
                        }
                        else
                        {
                            sb.AppendFormat("['{0}',{1}],", item.PageName + "_" + item.PageType, item.DownCount);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.PositionName)&&item.PositionName!="0")
                        {
                            sb.AppendFormat("['{0}',{1}],", item.PositionName + "_" + item.PageType, item.DownCount);
                        }
                        else if (string.IsNullOrEmpty(item.PositionName))
                        {
                            sb.AppendFormat("['{0}',{1}],", "未知", item.DownCount);   
                        }
                    }
                }
                sb.AppendFormat("['其他',{0}]]", lists[0].Sum(p => p.DownCount) - lstEntity.Sum(p => p.DownCount));
            }
            else
            {
                sb.Append("]");
            }
            string result = sb.ToString();
            return Result.GetSuccessedResult(result, true);
        }

        protected override List<List<T>> GetData<T>(HttpContext context, bool isfortable = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            TimeSpan dt = endtime.Subtract(begintime);
            diffdays = dt.Days+1;
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);

            string version = context.Request["version"];

            int period = Convert.ToInt32(context.Request["period"]);
            int IsUpdate = Convert.ToInt32(context.Request["downtype"]);

            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            int isdiffpagetype = Convert.ToInt32(context.Request["isdiffpagetype"]);
            isdiffpage = isdiffpagetype;
            int stattype = Convert.ToInt32(context.Request["stattype"]);
            string dostr=context.Request["do"]??"";
            if (dostr.Equals("get_excel"))
            {
                supdetaileqchart = false;
            }
            else
            {
                supdetaileqchart = true;
            }
            string  areaid = context.Request["areaid"];


            List<List<D_StatDownPositionDistribution>> lists = new List<List<D_StatDownPositionDistribution>>();

            var result = D_StatDownPositionDistribution_Service.Instance.GetD_StatDownPositionDistributionByCache(restype, softs,
                                                                                                      platform,
                                                                                                      projectsource,
                                                                                                      version,
                                                                                                      IsUpdate, begintime,
                                                                                                      endtime,
                                                                                                      period, 
                                                                                                      isdiffpagetype, 
                                                                                                      stattype,
                                                                                                      areaid);
            if (result.Count != 0)
                lists.Add(result);


            return lists as List<List<T>>;
        }

        protected override string DownloadExcelName
        {
            get { return "下载位置分布.xls"; }
        }

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                return new string[]
                    {
                        "位置", "位置编号", "统计量", "所占比例", "日均"
                    };
            }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list,bool isDetailTable)
        {
            var tempList = new List<List<string>>();
            if (list is List<D_StatDownPositionDistribution>)
            {
                var temp = list as List<D_StatDownPositionDistribution>;
                long downcountsum = temp.Sum(p => p.DownCount);
                foreach (D_StatDownPositionDistribution item in temp)
                {
                    List<string> values = new List<string>();

                    string pAndP = "";
                    string positionname = "未知";
                    if (!string.IsNullOrEmpty(item.PageName))
                    {
                        if (!string.IsNullOrEmpty(item.PositionName))
                        {
                            pAndP=item.PageName + "_" + item.PositionName;
                            positionname = item.PositionName;
                        }
                        else
                        {
                           pAndP=item.PageName;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.PositionName))
                        {
                            pAndP=item.PositionName;
                            positionname = item.PositionName;
                        }
                        else
                        {
                            pAndP= "未知";
                        }
                    }
                    if (isdiffpage==1)
                    {
                       pAndP= string.IsNullOrEmpty(item.PageType) ? pAndP : pAndP + "_" + item.PageType;
                    }
                    values.Add(pAndP);
                    values.Add(item.ByTag == 1 ? "专辑" : pAndP.Equals("未知")?"0":item.PositionId.ToString());
                    values.Add(UtilityHelp.FormatNum<long>(item.DownCount, true));
                    values.Add(UtilityHelp.FormatPercent<double>(item.DownCount*1.0/downcountsum,true));
                    values.Add(UtilityHelp.FormatNum<double>(item.DownCount*1.0/ diffdays, true));
                    if (isDetailTable)
                    {
                        values.Add(item.ByTag == 1 ? item.PageType + "_" + Guid.NewGuid().GetHashCode() :  item.PositionId.ToString());
                        values.Add(item.ByTag.ToString());
                    }
                    tempList.Add(values);
                }
            }
            return tempList;
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownPositionDistributionImpl"; }
        }

        public override string ServiceName
        {
            get { return "新下载位置分布"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.OnlyUrlRight; }
        }

        public override string RightUrl
        {
            get { return "reports_new/d_downlaodstatistics/d_resdownloadpositiondistribution.aspx"; }
        }
    }
}
