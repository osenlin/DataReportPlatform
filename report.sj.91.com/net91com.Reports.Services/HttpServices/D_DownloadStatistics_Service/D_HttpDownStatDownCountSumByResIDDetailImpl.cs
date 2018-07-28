using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Res91com.ResourceDataAccess;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.D_DownloadStat;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public class D_HttpDownStatDownCountSumByResIDDetailImpl : Abs_HttpDownLoad<D_StatDownPositionDistribution>
    {

        private int isbrowsecount = 0;

        protected override List<List<T>> GetData<T>(HttpContext context, bool isfortable = false)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int softs = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["plat"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int version = Convert.ToInt32(context.Request["version"]);
            int areaid = int.Parse(context.Request["areaid"]);
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            int areatype = Convert.ToInt32(context.Request["areatype"]);
            int resselecttype = Convert.ToInt32(context.Request["resselecttype"]);
            string rescontext = context.Request["rescontext"];


            isbrowsecount = 0;
            if (projectsource == (int)ProjectOption.OP_REDIRECT_STAT)
            {
                isbrowsecount = 1;
            }

            List<List<D_StatDownPositionDistribution>> lists = new List<List<D_StatDownPositionDistribution>>();

            List<D_StatDownPositionDistribution> result;

            List<int> lstresid;
            if (resselecttype != 3)
            {
                lstresid = new List<int>() { int.Parse(rescontext) };
            }
            else
            {
                List<B_ResInfo> resInfos = new B_BaseTool_DataAccess().GetResInfo(rescontext, restype, areatype);
                lstresid = resInfos.Select(p => p.ResId).ToList();

            }

            result = D_StatDownPositionDistribution_Service.Instance.GetD_StatDownPositionByResIDCacheDetailCache(restype, softs,
                                                                                        platform, projectsource,
                                                                                        version, begintime, endtime,
                                                                                        areatype,areaid, lstresid);

            var result2 =
                B_BaseDownPositionService.Instance.GetB_DownPositionListByCache((ProjectSourceTypeOptions)areatype,
                                                                                restype, projectsource);

            var res = (from r1 in result
                       join r2 in result2 on new { Position = r1.PositionId }
                                            equals new { Position = r2.Position }
                       into tt
                       from t in tt.DefaultIfEmpty()
                       select new D_StatDownPositionDistribution
                       {
                           PositionId = r1.PositionId,
                           PositionName = t != null ? t.Name : "",
                           PageName = t != null ? t.PageName : "",
                           DownCount = r1.DownCount,
                           BrowseCount = r1.BrowseCount
                       }).ToList();


            if (res.Count != 0)
                lists.Add(res);

            return lists as List<List<T>>;
        }

        protected override string DownloadExcelName
        {
            get { return "明细位置.xls"; }
        }

        protected override string[] DownloadExcelDataTableTitleName
        {
            get
            {
                string name = "下载量";
                if (isbrowsecount == 1)
                {
                    name = "下载浏览";
                }
                return new string[]
                    {
                        "位置ID","位置名称",name
                    };
            }
        }

        protected override List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list, bool DetailEqChart = true)
        {
            var tempList = new List<List<string>>();
            if (list is List<D_StatDownPositionDistribution>)
            {
                var temp = list as List<D_StatDownPositionDistribution>;
                foreach (D_StatDownPositionDistribution item in temp)
                {
                    List<string> values = new List<string>();
                    values.Add(item.PositionId.ToString());
                    values.Add(item.PositionName);
                    if (isbrowsecount == 1)
                    {
                        values.Add(UtilityHelp.FormatNum<long>(item.BrowseCount, true));
                    }
                    else
                    {
                        values.Add(UtilityHelp.FormatNum<long>(item.DownCount, true));
                    }

                    tempList.Add(values);
                }
            }
            return tempList;
        }

        public override string ServiceCategory
        {
            get { return "D_HttpDownStatDownCountSumByResIDDetailImpl"; }
        }

        public override string ServiceName
        {
            get { return "下载位置明细"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.DefinedByYourself; }
        }


        public override bool Check(object obj)
        {
            return new URLoginService().FindRight(
                "reports_new/d_downlaodstatistics/D_ResDownLoadByResID.aspx") != null ? true : false;
        }

        public override string RightUrl
        {
            get { return "reports_new/d_downlaodstatistics/D_ResDownLoadByResID.aspx"; }
        }
    }
}