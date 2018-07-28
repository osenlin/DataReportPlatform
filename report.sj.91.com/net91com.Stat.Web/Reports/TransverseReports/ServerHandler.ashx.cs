using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Stat.Web.Base;
using net91com.Core.Extensions;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Services.Entity;
using net91com.Core;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.sjqd;
using Newtonsoft.Json;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.TransverseReports
{
    /// <summary>
    /// ServerHandler 的摘要说明
    /// </summary>
    public class ServerHandler : HandlerBase
    {

        public override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);
            switch (GetQueryString("act").ToLower())
            {
                case "downgjbb":
                    DownloadGJBB();
                    break;
                case "lan":
                    DownloadLan();
                    break;
                case "softxh":
                    DownloadSBXH();
                    break;
                case "downresolution":
                    DownResolution();
                    break;
                case "downbrand":
                    DownloadBrand();
                    break;
                case "getlandetail":
                    GetLanDetailLine();
                    break;
                case "getgjbbdetail":
                    GetGjBBDetailLine();
                    break;
                case "getsbxhdetail":
                    GetSbXHDetailLine();
                    break;
                //分辨率每天明细
                case "getresolutionline":
                    GetResolutionDetailLine();
                    break;
                case "getbranddetail":
                    GetBrandDetailLine();
                    break;
            }
        }

        private void DownResolution()
        {
            net91com.Stat.Core.PeriodOptions Period = GetQueryString("zhouqi").ToEnum<net91com.Stat.Core.PeriodOptions>(net91com.Stat.Core.PeriodOptions.LatestOneWeek);
            int excelsoft = Convert.ToInt32(GetQueryString("soft"));
            int excelplatform = Convert.ToInt32(GetQueryString("platform"));
            CheckHasRight(excelsoft, "Reports/TransverseReports/ResolutionReport.aspx");
            List<Resolution> list = TerminalService.GetInstance().GetResolutions(excelsoft, excelplatform, (int)Period,
                Convert.ToInt32(UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes).ToString("yyyyMMdd"))
                );

            ThisResponse.ContentEncoding = System.Text.Encoding.GetEncoding("GBK");
            AddHead("分辨率分布.xls");
            ThisResponse.Write("分辨率\t用户数\t百分比\t\n");
            string temp = string.Empty;
            int allcount = list.Sum(p => p.UseCount);
            list.ForEach(resol =>
            {
                ThisResponse.Write((string.IsNullOrEmpty(resol.ResolutionStr) ? "未知分辨率" : resol.ResolutionStr) + "\t" + string.Format("{0:N0}", (resol.UseCount)) + "\t" + (Convert.ToDecimal(resol.UseCount) / allcount * 100).ToString("0.00") + "\t\n");
            });
            ThisResponse.End();
        }
        /// <summary>
        /// 下载设备型号分布数据
        /// </summary>
        private void DownloadSBXH()
        {
            
            net91com.Stat.Core.PeriodOptions Period = GetQueryString("zhouqi").ToEnum<net91com.Stat.Core.PeriodOptions>(net91com.Stat.Core.PeriodOptions.LatestOneWeek);
            int excelsoft = Convert.ToInt32(GetQueryString("soft"));
            int excelplatform = Convert.ToInt32(GetQueryString("platform"));
            CheckHasRight(excelsoft, "Reports/TransverseReports/SoftXhTransverse.aspx");
            List<Sjqd_StatUsersBySbxh> list = StatUsersBySbxhService.GetInStance().GetSoftSBXHTransverse(Period,
                Convert.ToInt32(UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes).ToString("yyyyMMdd")),
                excelsoft, (MobileOption)excelplatform);

            ThisResponse.ContentEncoding = System.Text.Encoding.GetEncoding("GBK");
            AddHead("机型分布.xls");
            ThisResponse.Write("机型\t用户数\t百分比\t\n");
            string temp = string.Empty;
            int allcount = list.Sum(p => p.UseCount);
            list.ForEach(gjbb =>
            {
                ThisResponse.Write((string.IsNullOrEmpty(gjbb.Sbxh) ? "未知" : gjbb.Sbxh) + "\t" + gjbb.UseCount + "\t" + (Convert.ToDecimal(gjbb.UseCount) / allcount * 100).ToString("0.00") + "\t\n");
            });
            ThisResponse.End();
        }

        private void DownloadBrand()
        {
            
            net91com.Stat.Core.PeriodOptions Period = GetQueryString("zhouqi").ToEnum<net91com.Stat.Core.PeriodOptions>(net91com.Stat.Core.PeriodOptions.LatestOneWeek);
            int excelsoft = Convert.ToInt32(GetQueryString("soft"));
            int excelplatform = Convert.ToInt32(GetQueryString("platform"));
            CheckHasRight(excelsoft, "Reports/TransverseReports/SoftBrandTransverse.aspx");
            List<Sjqd_StatUsersBySbxh> list =  StatUsersBySbxhService.GetInStance().GetSoftBrandTransverse(Period,
                Convert.ToInt32(UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes).ToString("yyyyMMdd")),
                excelsoft, (MobileOption)excelplatform);
            ThisResponse.ContentEncoding = System.Text.Encoding.GetEncoding("GBK");
            AddHead("品牌分布.xls");
            ThisResponse.Write("品牌\t用户数\t百分比\t\n");
            string temp = string.Empty;
            int allcount = list.Sum(p => p.UseCount);
            list.ForEach(gjbb =>
            {
                ThisResponse.Write((string.IsNullOrEmpty(gjbb.Brand) ? "未适配品牌" : gjbb.Brand) + "\t" + gjbb.UseCount + "\t" + (Convert.ToDecimal(gjbb.UseCount) / allcount * 100).ToString("0.00") + "\t\n");
            });
            ThisResponse.End();
        }

        /// <summary>
        /// 下载语言分布数据
        /// </summary>
        private void DownloadLan()
        {
            net91com.Stat.Core.PeriodOptions Period = GetQueryString("zhouqi").ToEnum<net91com.Stat.Core.PeriodOptions>(net91com.Stat.Core.PeriodOptions.LatestOneWeek);
            int excelsoft = Convert.ToInt32(GetQueryString("soft"));
            int excelplatform = Convert.ToInt32(GetQueryString("platform"));
            CheckHasRight(excelsoft, "Reports/TransverseReports/SoftLanTransverse.aspx");
            List<Sjqd_StatUsersByLan> list =StatUsersByLanService.GetInstance().GetSoftLanTransverse(Period,
                Convert.ToInt32(UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes).ToString("yyyyMMdd")), 
                excelsoft, (MobileOption)excelplatform);

            ThisResponse.ContentEncoding = System.Text.Encoding.GetEncoding("GBK");
            AddHead("语言分布.xls");
            ThisResponse.Write("语言\t用户数\t百分比\t\n");
            string temp = string.Empty;
            int allcount = list.Sum(p => p.UseCount);
            list.ForEach(gjbb =>
            {
                ThisResponse.Write((string.IsNullOrEmpty(gjbb.Lan) ? "未知" : gjbb.Lan) + "\t" + gjbb.UseCount + "\t" + (Convert.ToDecimal(gjbb.UseCount) / allcount * 100).ToString("0.00") + "\t\n");
            });
            ThisResponse.End();
        }

        /// <summary>
        /// 下载固件版本分布数据
        /// </summary>
        private void DownloadGJBB()
        {
            net91com.Stat.Core.PeriodOptions Period = GetQueryString("zhouqi").ToEnum<net91com.Stat.Core.PeriodOptions>(net91com.Stat.Core.PeriodOptions.LatestOneWeek);
            int excelsoft = Convert.ToInt32(GetQueryString("soft"));
            int excelplatform = Convert.ToInt32(GetQueryString("platform"));
            CheckHasRight(excelsoft, "Reports/TransverseReports/SoftGJBBTransverse.aspx");
            List<Sjqd_StatUsersByGjbb> list = StatUsersByGjbbService.GetInstance().GetSoftGJBBTransverse(Period,
                Convert.ToInt32(UtilityService.GetInstance().GetMaxTimeCache(Period, ReportType.StatTerminationDistribution, CacheTimeOption.TenMinutes).ToString("yyyyMMdd")), excelsoft, (MobileOption)excelplatform);
            int all = list.Sum(p => p.UseCount);
            ThisResponse.ContentEncoding = System.Text.Encoding.GetEncoding("GBK");
            AddHead("固件版本分布.xls");
            ThisResponse.Write("固件版本\t用户数\t百分比\t\n");
            string temp = string.Empty;
            list.ForEach(gjbb =>
            {
                ThisResponse.Write((string.IsNullOrEmpty(gjbb.Gjbb) ? "未知" : gjbb.Gjbb) + "\t" + gjbb.UseCount + "\t" + (Convert.ToDecimal(gjbb.UseCount) / all * 100).ToString("0.00") + "\t\n");
            });
            ThisResponse.End();
        }
       

        /// <summary>
        /// 固件版本每一天的曲线
        /// </summary>
        public void GetGjBBDetailLine()
        {
            string gjbb = GetQueryString("gjbbname");
            DateTime begintime = Convert.ToDateTime(GetQueryString("sdate"));
            DateTime endtime = Convert.ToDateTime(GetQueryString("edate"));
            if (begintime <= endtime.AddDays(-90))
            {
                begintime = endtime.AddDays(-90);
            }
            MobileOption plat = (MobileOption)Convert.ToInt32(GetQueryString("plat"));
            int soft = Convert.ToInt32(GetQueryString("soft"));

            List<Sjqd_StatUsersByGjbb> list = StatUsersByGjbbService.GetInstance().
                GetGjbbByDates(begintime, endtime, soft, (int)plat, gjbb);


            List<DateTime> datelist = list.Select(p => p.StatDate).ToList();
            string AxisJsonStr1 = string.Empty;
            string SeriesJsonStr1 = string.Empty;
            if (list.Count != 0)
            {
                SetxAxisJson(datelist, ref AxisJsonStr1);
                SeriesJsonStr1 = JsonConvert.SerializeObject(GetDataJsonListByGjbb(datelist, list));
            }
            else
            {
                AxisJsonStr1 = "{}";
                SeriesJsonStr1 = "[]";
            }
            string result = "{ x:" + AxisJsonStr1 + "," + "y:" + SeriesJsonStr1 + "}";
            HttpContext.Current.Response.Write(result);
            
            
        }
        /// <summary>
        /// 设备型号每天分布
        /// </summary>
        public void GetSbXHDetailLine()
        {
            string sbxhname = GetQueryString("sbxhname");
            DateTime begintime = Convert.ToDateTime(GetQueryString("sdate"));
            DateTime endtime = Convert.ToDateTime(GetQueryString("edate"));
            if (begintime <= endtime.AddDays(-90))
            {
                begintime = endtime.AddDays(-90);
            }
            MobileOption plat = (MobileOption)Convert.ToInt32(GetQueryString("plat"));
            int soft = Convert.ToInt32(GetQueryString("soft"));

            List<Sjqd_StatUsersBySbxh> list = StatUsersBySbxhService.GetInStance().GetSBXHByDates(begintime, endtime, soft, (int)plat, sbxhname);


            List<DateTime> datelist = list.Select(p => p.StatDate).ToList();
            string AxisJsonStr1 = string.Empty;
            string SeriesJsonStr1 = string.Empty;
            if (list.Count != 0)
            {
                SetxAxisJson(datelist, ref AxisJsonStr1);
                SeriesJsonStr1 = JsonConvert.SerializeObject(GetDataJsonListBySBXH(datelist, list));
            }
            else
            {
                AxisJsonStr1 = "{}";
                SeriesJsonStr1 = "[]";
            }
            string result = "{ x:" + AxisJsonStr1 + "," + "y:" + SeriesJsonStr1 + "}";
            HttpContext.Current.Response.Write(result);
        }
        /// <summary>
        /// 语言每天分布
        /// </summary>
        public void GetLanDetailLine()
        {
            string lanname = GetQueryString("lanname");
            DateTime begintime = Convert.ToDateTime(GetQueryString("sdate"));
            DateTime endtime = Convert.ToDateTime(GetQueryString("edate"));
            if (begintime <= endtime.AddDays(-90))
            {
                begintime = endtime.AddDays(-90);
            }
            MobileOption plat = (MobileOption)Convert.ToInt32(GetQueryString("plat"));
            int soft = Convert.ToInt32(GetQueryString("soft"));

            List<Sjqd_StatUsersByLan> list = StatUsersByLanService.GetInstance().
                GetLanByDates(begintime, endtime, soft, (int)plat, lanname);
             
             
            List<DateTime> datelist = list.Select(p => p.StatDate).ToList();
            string AxisJsonStr1 = string.Empty;
            string SeriesJsonStr1 = string.Empty;
            if (list.Count != 0)
            {
                SetxAxisJson(datelist, ref AxisJsonStr1);
                SeriesJsonStr1 = JsonConvert.SerializeObject(GetDataJsonListByLan(datelist, list));
            }
            else
            {
                AxisJsonStr1 = "{}";
                SeriesJsonStr1 = "[]";
            }
            string result = "{ x:" + AxisJsonStr1 + "," + "y:" + SeriesJsonStr1 + "}";
            HttpContext.Current.Response.Write(result);
            
        }

        /// <summary>
        /// 分辨率每天分布
        /// </summary>
        public void GetResolutionDetailLine()
        {
            string resolutionname = GetQueryString("resolutionname");
            DateTime begintime = Convert.ToDateTime(GetQueryString("sdate"));
            DateTime endtime = Convert.ToDateTime(GetQueryString("edate"));
            if (begintime <= endtime.AddDays(-90))
            {
                begintime = endtime.AddDays(-90);
            }
            MobileOption plat = (MobileOption)Convert.ToInt32(GetQueryString("plat"));
            int soft = Convert.ToInt32(GetQueryString("soft"));

            List<Resolution> list = TerminalService.GetInstance().GetResolutionsByDates
                (begintime, endtime, soft, (int)plat, resolutionname);
            List<DateTime> datelist = list.Select(p => p.StatDate).ToList();
            string AxisJsonStr1 = string.Empty;
            string SeriesJsonStr1 = string.Empty;
            if (list.Count != 0)
            {
                SetxAxisJson(datelist, ref AxisJsonStr1);
                SeriesJsonStr1 = JsonConvert.SerializeObject(GetDataJsonListByResolution(datelist, list));
            }
            else
            {
                AxisJsonStr1 = "{}";
                SeriesJsonStr1 = "[]";
            }
            string result = "{ x:" + AxisJsonStr1 + "," + "y:" + SeriesJsonStr1 + "}";
            HttpContext.Current.Response.Write(result);

        }

        /// <summary>
        /// 品牌每天分布
        /// </summary>
        public void GetBrandDetailLine()
        {
            string brandname = GetQueryString("brandname");
            DateTime begintime = Convert.ToDateTime(GetQueryString("sdate"));
            DateTime endtime = Convert.ToDateTime(GetQueryString("edate"));
            if (begintime <= endtime.AddDays(-90))
            {
                begintime = endtime.AddDays(-90);
            }
            MobileOption plat = (MobileOption)Convert.ToInt32(GetQueryString("plat"));
            int soft = Convert.ToInt32(GetQueryString("soft"));

            List<Sjqd_StatUsersBySbxh> list = StatUsersBySbxhService.GetInStance().GetBrandByDates
                (begintime, endtime, soft, (int)plat, brandname);
            List<DateTime> datelist = list.Select(p => p.StatDate).ToList();
            string AxisJsonStr1 = string.Empty;
            string SeriesJsonStr1 = string.Empty;
            if (list.Count != 0)
            {
                SetxAxisJson(datelist, ref AxisJsonStr1);
                SeriesJsonStr1 = JsonConvert.SerializeObject(GetDataJsonListByBrand(datelist, list));
            }
            else
            {
                AxisJsonStr1 = "{}";
                SeriesJsonStr1 = "[]";
            }
            string result = "{ x:" + AxisJsonStr1 + "," + "y:" + SeriesJsonStr1 + "}";
            HttpContext.Current.Response.Write(result);

        }


        //分辨率明细数据
        protected List<SeriesJsonModel> GetDataJsonListByResolution(List<DateTime> x_date, List<Resolution> ress)
        {

            List<SeriesJsonModel> result = new List<SeriesJsonModel>();
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            ///构造对应x轴上对应各个坐标点,一开始就是null ，先填充好
            int spanInt = -(x_date.Count / 25 + 1);
            for (int ii = 0; ii < x_date.Count; ii++)
            {
                DataLabels dl = null;
                sjModel2.data.Add(dl);
            }
            sjModel2.name = "分辨率分布";
            for (int i = 0; i < ress.Count; i++)
            {

                for (int j = 0; j < x_date.Count; j++)
                {
                    if (x_date[j] == ress[i].StatDate)
                    {
                        DataLabels dl = new DataLabels();
                        SmallDataLabels smalldata = new SmallDataLabels();
                        dl.y = ress[i].UseCount;
                        dl.dataLabels = smalldata; 
                        if (j % spanInt == 0)//这个间隔和x轴设置是一样的
                            smalldata.enabled = true;
                        ///替换掉以前的null
                        sjModel2.data[j] = dl;

                    }
                }

            }
            result.Add(sjModel2);
            return result;

        }
        //语言明细数据
        protected List<SeriesJsonModel> GetDataJsonListByLan(List<DateTime> x_date, List<Sjqd_StatUsersByLan> lans)
        {

            List<SeriesJsonModel> result = new List<SeriesJsonModel>();
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            ///构造对应x轴上对应各个坐标点,一开始就是null ，先填充好
            int spanInt = -(x_date.Count / 25 + 1);
            for (int ii = 0; ii < x_date.Count; ii++)
            {
                DataLabels dl = null;
                sjModel2.data.Add(dl);
            }
            sjModel2.name = "语言分布";
            for (int i = 0; i < lans.Count; i++)
            {

                for (int j = 0; j < x_date.Count; j++)
                {
                    if (x_date[j] == lans[i].StatDate)
                    {
                        DataLabels dl = new DataLabels();
                        SmallDataLabels smalldata = new SmallDataLabels();
                        dl.y = lans[i].UseCount;
                        dl.dataLabels = smalldata;
                        if (j % spanInt == 0)//这个间隔和x轴设置是一样的
                            smalldata.enabled = true;
                        ///替换掉以前的null
                        sjModel2.data[j] = dl;

                    }
                }

            }
            result.Add(sjModel2);
            return result;

        }
        //设备明细数据
        protected List<SeriesJsonModel> GetDataJsonListBySBXH(List<DateTime> x_date, List<Sjqd_StatUsersBySbxh> sbxh)
        {

            List<SeriesJsonModel> result = new List<SeriesJsonModel>();
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            ///构造对应x轴上对应各个坐标点,一开始就是null ，先填充好
            int spanInt = -(x_date.Count / 25 + 1);
            for (int ii = 0; ii < x_date.Count; ii++)
            {
                DataLabels dl = null;
                sjModel2.data.Add(dl);
            }
            sjModel2.name = "机型分布";
            for (int i = 0; i < sbxh.Count; i++)
            {

                for (int j = 0; j < x_date.Count; j++)
                {
                    if (x_date[j] == sbxh[i].StatDate)
                    {
                        DataLabels dl = new DataLabels();
                        SmallDataLabels smalldata = new SmallDataLabels();
                        dl.y = sbxh[i].UseCount;
                        dl.dataLabels = smalldata;
                        if (j % spanInt == 0)//这个间隔和x轴设置是一样的
                            smalldata.enabled = true;
                        ///替换掉以前的null
                        sjModel2.data[j] = dl;

                    }
                }

            }
            result.Add(sjModel2);
            return result;

        }
        //固件明细数据 
        protected List<SeriesJsonModel> GetDataJsonListByGjbb(List<DateTime> x_date, List<Sjqd_StatUsersByGjbb> gjbb)
        {

            List<SeriesJsonModel> result = new List<SeriesJsonModel>();
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            int spanInt = -(x_date.Count / 25 + 1);
            for (int ii = 0; ii < x_date.Count; ii++)
            {
                DataLabels dl = null;
                sjModel2.data.Add(dl);
            }
            sjModel2.name = "固件版本分布";
            for (int i = 0; i < gjbb.Count; i++)
            {

                for (int j = 0; j < x_date.Count; j++)
                {
                    if (x_date[j] == gjbb[i].StatDate)
                    {
                        DataLabels dl = new DataLabels();
                        SmallDataLabels smalldata = new SmallDataLabels();
                        dl.y = gjbb[i].UseCount;
                        dl.dataLabels = smalldata;
                        if (j % spanInt == 0)//这个间隔和x轴设置是一样的
                            smalldata.enabled = true;
                        ///替换掉以前的null
                        sjModel2.data[j] = dl;

                    }
                }

            }
            result.Add(sjModel2);
            return result;

        }

        //品牌明细数据(因为品牌数据也放到设备型号类里)
        protected List<SeriesJsonModel> GetDataJsonListByBrand(List<DateTime> x_date, List<Sjqd_StatUsersBySbxh> brands)
        {

            List<SeriesJsonModel> result = new List<SeriesJsonModel>();
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            ///构造对应x轴上对应各个坐标点,一开始就是null ，先填充好
            int spanInt = -(x_date.Count / 25 + 1);
            for (int ii = 0; ii < x_date.Count; ii++)
            {
                DataLabels dl = null;
                sjModel2.data.Add(dl);
            }
            sjModel2.name = "品牌分布";
            for (int i = 0; i < brands.Count; i++)
            {

                for (int j = 0; j < x_date.Count; j++)
                {
                    if (x_date[j] == brands[i].StatDate)
                    {
                        DataLabels dl = new DataLabels();
                        SmallDataLabels smalldata = new SmallDataLabels();
                        dl.y = brands[i].UseCount;
                        dl.dataLabels = smalldata;
                        if (j % spanInt == 0)//这个间隔和x轴设置是一样的
                            smalldata.enabled = true;
                        ///替换掉以前的null
                        sjModel2.data[j] = dl;

                    }
                }

            }
            result.Add(sjModel2);
            return result;

        }

        //操作系统明细数据
        protected List<SeriesJsonModel> GetDataJsonListByOsVersion(List<DateTime> x_date, List<Sjqd_StatUsersByOsVersion> osVersions)
        {

            List<SeriesJsonModel> result = new List<SeriesJsonModel>();
            ///构造一个json模型
            SeriesJsonModel sjModel2 = new SeriesJsonModel();
            ///构造对应x轴上对应各个坐标点,一开始就是null ，先填充好
            int spanInt = -(x_date.Count / 25 + 1);
            for (int ii = 0; ii < x_date.Count; ii++)
            {
                DataLabels dl = null;
                sjModel2.data.Add(dl);
            }
            sjModel2.name = "操作系统分布";
            for (int i = 0; i < osVersions.Count; i++)
            {

                for (int j = 0; j < x_date.Count; j++)
                {
                    if (x_date[j] == osVersions[i].StatDate)
                    {
                        DataLabels dl = new DataLabels();
                        SmallDataLabels smalldata = new SmallDataLabels();
                        dl.y = osVersions[i].UseCount;
                        dl.dataLabels = smalldata;
                        if (j % spanInt == 0)//这个间隔和x轴设置是一样的
                            smalldata.enabled = true;
                        ///替换掉以前的null
                        sjModel2.data[j] = dl;

                    }
                }

            }
            result.Add(sjModel2);
            return result;

        }
        
    }
}