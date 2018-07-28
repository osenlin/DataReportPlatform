using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;

using net91com.Core;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Stat.Services.Entity;
using net91com.Reports.UserRights;
using net91com.Stat.Services;
using net91com.Reports.Services;
using net91com.Core.Extensions;

namespace net91com.Stat.Web.Base
{
    /// <summary>
    /// 报表查看页面基类
    /// </summary>
    public class ReportBasePage : Page 
    {
        protected URLoginService loginService = new URLoginService();

        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        protected User CurrentUser
        {
            get { return loginService.LoginUser; } 
        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected virtual List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts; }
        }

        /// <summary>
        /// 获取JSON格式的软件列表
        /// </summary>
        protected virtual string GetAvailableSoftsJson()
        {
            return loginService.GetAvailableSoftsJson();
        }

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected virtual List<Soft> AvailableInternalSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.SoftType == SoftTypeOptions.InternalSoft).ToList(); }
        }

        /// <summary>
        /// 获取装机助手的列表
        /// </summary>
        /// <param name="softs"></param>
        /// <returns></returns>
        protected virtual List<Soft> AvailableSjzsSofts
        {
            get
            {
                int[] filter = { 68, 69, 58, 9, 57, 60, 61, 71, 105550, 112, 116442 };
                List<Soft> result = (from a in AvailableSofts
                                     join b in filter
                                     on a.ID equals b
                                     select a).ToList();
                return result;
            }
        }

        /// <summary>
        /// 可访问的项目来源列表
        /// </summary>
        protected virtual List<ProjectSource> AvailableProjectSources
        {
            get { return loginService.AvailableProjectSources; }
        }

        /// <summary>
        /// 可访问的操作权限
        /// </summary>
        protected List<Right> AvailableRights
        {
            get { return loginService.AvailableRights; }
        }

        /// <summary>
        /// 可访问的资源列表
        /// </summary>
        protected List<int> AvailableResIds
        {
            get { return loginService.AvailableResIds; }
        }
        
        /// <summary>
        /// 获取指定名称的软件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Soft GetSoft(string name)
        {
            Soft soft = loginService.AvailableSofts.FirstOrDefault(a => a.Name == name);
            return soft;
        }

        /// <summary>
        /// 获取指定ID的软件
        /// </summary>
        /// <param name="softId"></param>
        /// <returns></returns>
        protected Soft GetSoft(int softId)
        {
            Soft soft = loginService.AvailableSofts.FirstOrDefault(a => a.ID == softId);
            return soft;
        }

      

        /// <summary>
        /// 获取Request.QueryString参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected string GetQueryString(string key, string defaultValue)
        {
            return string.IsNullOrEmpty(Request.QueryString[key]) ? defaultValue : Request.QueryString[key];
        }

        /// <summary>
        /// 获取Request.QueryString参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetQueryString(string key)
        {
            return GetQueryString(key, string.Empty);
        }

        /// <summary>
        /// 获取当前选择的软件列表
        /// </summary>
        protected List<Soft> SelectedSofts { get; private set; }

        /// <summary>
        /// 获取当前选择的平台列表
        /// </summary>
        protected List<MobileOption> SelectedPlatforms { get; private set; }

        /// <summary>
        /// 获取当前选择的统计周期
        /// </summary>
        protected net91com.Stat.Core.PeriodOptions Period { get; set; }

        /// <summary>
        /// 获取当前选择的开始日期
        /// </summary>
        protected DateTime BeginTime;

        /// <summary>
        /// 获取当前选择的结束日期
        /// </summary>
        protected DateTime EndTime;

        /// <summary>
        /// 验证当前选择的软件及其它参数是否在权限范围内
        /// </summary>
        /// <param name="selSoftIds">当前选择的软件列表,如果为空选择Cookie里指定的软件,否则选择有权限列表中第一个软件</param>
        /// <param name="selPlatforms">当前选择的平台列表,如果为空选择Cookie里指定的平台,否则选择有权限列表中第一个软件的第一个支持的平台</param>
        /// <param name="period"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="reportType"></param>
        protected void CheckParams(List<int> selSoftIds, List<MobileOption> selPlatforms, net91com.Stat.Core.PeriodOptions period, DateTime beginTime, DateTime endTime, ReportType reportType)
        {
            //如果用户一个产品权限都没有,则抛出无权限异常
            if (AvailableSofts.Count == 0)
                throw new NotRightException();
            List<int> selectedSoftIds = selSoftIds == null ? new List<int>() : selSoftIds;
            List<MobileOption> selectedPlatforms = selPlatforms == null ? new List<MobileOption>() : selPlatforms;
            //如果没有选择任何产品,使用Cookie指定的
            if (selectedSoftIds.Count == 0)
            {
                selectedSoftIds = UtilityHelp.GetDefaultSoftsFromCookie(out selectedPlatforms);
            }
            List<Soft> selectedSofts = (from a in AvailableSofts
                                        join b in selectedSoftIds
                                        on a.ID equals b
                                        select a).ToList();
            //如果客户端第一次请求或选择的软件都没有权限,使用第一个有权限的软件和第一个可以支持的平台
            if (selectedSofts.Count == 0)
                selectedSofts.Add(AvailableSofts[0]);
            if (selectedPlatforms.Count == 0)
                selectedPlatforms.Add(selectedSofts[0].Platforms[0]);
            //设置默认的软件和平台到Cookie
            UtilityHelp.SetDefaultSoftsToCookie(selectedSofts, selectedPlatforms);
            //设置相关属性
            SelectedSofts = selectedSofts;
            SelectedPlatforms = selectedPlatforms;
            Period = period;
            DateTime maxTime = UtilityService.GetInstance().GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, reportType, CacheTimeOption.TenMinutes);
            EndTime = endTime == DateTime.MinValue || endTime > maxTime ? maxTime : endTime;
            BeginTime = beginTime == DateTime.MinValue ? EndTime.AddDays(-30) : (beginTime > EndTime ? EndTime : beginTime);
            if (Period == net91com.Stat.Core.PeriodOptions.Hours && EndTime.Subtract(BeginTime).Days > 10)
            {
                BeginTime = EndTime.AddDays(-10);
            }
        }
        
        /// <summary>
        /// 提供打印的页面
        /// </summary>
        /// <param name="filename"></param>
        protected void AddHead(string filename)
        {
            if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("msie") > -1)
            {
                filename = HttpUtility.UrlPathEncode(filename);
            }
            if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
            }
            else
            {
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
            }
        }

        private Soft cookieSoft = null;
        /// <summary>
        /// cookie中存储的softid
        /// </summary>
        protected int CookieSoftid
        {
            get
            {
                if (cookieSoft == null)
                {
                    List<MobileOption> platforms;
                    List<int> softIds = UtilityHelp.GetDefaultSoftsFromCookie(out platforms);
                    List<Soft> softs = (from a in AvailableSofts
                                        join b in softIds on a.ID equals b
                                        select a).ToList();
                    if (softs.Count > 0)
                        cookieSoft = softs[0];
                    else if (AvailableSofts.Count > 0)
                        cookieSoft = AvailableSofts[0];
                    else
                        cookieSoft = new Soft();
                }
                return cookieSoft.ID;
            }
        }

        /// <summary>
        /// cookie中存储的平台
        /// </summary>
        protected int CookiePlatid
        {
            get
            {
                if (CookieSoftid != 0)
                {
                    List<MobileOption> platforms;
                    UtilityHelp.GetDefaultSoftsFromCookie(out platforms);
                    if (platforms.Count > 0)
                    {
                        foreach (MobileOption p in platforms)
                        {
                            if (cookieSoft.Platforms.Exists(a => a == p))
                                return (int)p;
                        }
                    }
                    if (cookieSoft.Platforms.Count > 0)
                        return (int)cookieSoft.Platforms[0];
                }
                else if (AvailableSofts.Count > 0)
                {
                    return (int)AvailableSofts[0].Platforms[0];
                }
                return 0;
            }
        }

        /// <summary>
        /// 将上次选中的软件和平台默认存储到cookie中
        /// </summary>
        protected void SetRequestCookie(int p_softid, int plat)
        {
            Soft soft = GetSoft(p_softid);
            List<Soft> selectedSofts = new List<Soft>();
            selectedSofts.Add(soft);
            List<MobileOption> selectedPlatforms = new List<MobileOption>();
            selectedPlatforms.Add((MobileOption)plat);
            UtilityHelp.SetDefaultSoftsToCookie(selectedSofts, selectedPlatforms);
        }




        /// <summary>
        /// 对于线性 最大点数
        /// </summary>
        private int lineMaxNum = 20;
        protected int LineMaxNum
        {
            set { lineMaxNum = value; }
            get { return lineMaxNum; }
        }
        /// <summary>
        /// 若超过点数，x轴隔多少显示
        /// </summary>
        private int lineMaxNumCoef = -2;
        protected int LineMaxNumCoef 
        { 
            set { lineMaxNumCoef = value; } 
            get { return lineMaxNumCoef; } 
        }

        private int _MaxPerNumber = 9;
        /// <summary>
        /// 最大显示饼图个数
        /// </summary>
        protected int MaxPerNumber
        {
            get { return _MaxPerNumber; }
            set { _MaxPerNumber = value; }
        }

        private int _MinPerRatio = 1;
        /// <summary>
        /// 饼图比率最小值
        /// </summary>
        protected int MinPerRatio
        {
            get { return _MinPerRatio; }
            set { _MinPerRatio = value; }
        }

        /// <summary>
        /// 前端页面显示提示信息
        /// </summary>
        /// <param name="message"></param>
        protected void AlertBack(string message)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "", string.Format("<script>alert(\"{0}\");</script>", message));
        }
        /// <summary>
        /// 获取平台
        /// </summary>
        /// <param name="shownoplatform">是否展示不区分平台</param>
        /// <param name="isSimple">是否是简化版,简化版就是iphone,android,ipad</param>
        /// <returns></returns>
        protected string GetPlatformHtml(bool shownoplatform,int noplatformvalue=-1,bool isSimple=false)
        {
            StringBuilder sb = null;
            var platform = UtilityHelp.GetDefaultPlatform(this.Context);
            if (shownoplatform)
                sb = new StringBuilder(string.Format("<option value='{0}' {1} >不区分平台</option>", noplatformvalue, noplatformvalue.ToString() == platform[0].ToString() ? "selected=\"selected\"" : ""));
            else
                sb = new StringBuilder();
            //所有平台都展现
            if (!isSimple)
            {
                var platList = UtilityHelp.GetMobileList(true);
                for (int i = 0; i < platList.Count; i++)
                {
                    sb.AppendFormat("<option value='{0}' {2}>{1}</option>", (int)platList[i], platList[i].GetDescription(), ((int)platList[i]).ToString() == platform[0].ToString());
                }

            }
            else//只展现3个平台  
            {
                sb.Append("<option value='4'>Android</option>");
                sb.Append("<option value='9'>AndroidPad</option>");
                sb.Append("<option value='1'>Iphone</option>");
                sb.Append("<option value='7'>Ipad</option>");
            }
            return sb.ToString();
        }

        protected string GetExtendResAttrHtml(int restype)
        {
            var map = B_BaseToolService.Instance.getExtendResAttr(restype);
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (Dictionary<string, string> dicitem in map)
            {
                if (i==0)
                {
                    sb.AppendFormat("<option value='{0}' select='selected'>{1}</option>", dicitem["type"], dicitem["name"]);
                }
                else
                {
                    sb.AppendFormat("<option value='{0}' >{1}</option>", dicitem["type"], dicitem["name"]);
                }           
            }
          
            return sb.ToString();
        }


        protected string GetAreaHtml(int type,bool shownodifferent)
        {
            List<B_AreaEntity> arealst;
            string strname = "不区分国家";
            switch (type)
            {
                case 1:
                    arealst = B_BaseToolService.Instance.GetCountriesCache();
                    break;
                case 2:
                    arealst = B_BaseToolService.Instance.GetProvincesCache();
                    strname = "不区分省份";
                    break;
                case 3:
                    arealst = B_BaseToolService.Instance.GetCitiesCache();
                    strname = "不区分市";
                    break;
                default:
                    arealst = B_BaseToolService.Instance.GetAreaCache();
                    break;
            }

            StringBuilder sb=new StringBuilder();
            if (shownodifferent)
                sb = new StringBuilder(string.Format("<option value='{0}' {1} >{2}</option>", -1,"selected=\"selected\"",strname));
            else
                sb = new StringBuilder();

            foreach (B_AreaEntity item in arealst)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", item.ID,item.Name);
            }
            return sb.ToString();

        }

        /// <summary>
        /// 获取软件的HTML
        /// </summary>
        /// <returns></returns>
        protected string GetSoftHtml()
        {
            return GetSoftHtml(AvailableSofts);
        }

        /// <summary>
        /// 获取软件的HTML
        /// </summary>
        /// <param name="AvailableSofts"></param>
        /// <returns></returns>
        protected string GetSoftHtml(List<Soft> AvailableSofts)
        {
            StringBuilder sb = new StringBuilder();
            var softs = AvailableSofts;
            for (int i = 0; i < softs.Count; i++)
            {
                sb.AppendFormat("<option value='{0}' {2} >{1}</option>", softs[i].ID, softs[i].Name, softs[i].ID == CookieSoftid ? "selected='selected'" : "");
            }

            return sb.ToString();
        }

        protected string GetSoftHtml(List<Soft> AvailableSofts,int stataloneid)
        {
            StringBuilder sb = new StringBuilder();
            var softs = AvailableSofts;
            for (int i = 0; i < softs.Count; i++)
            {
                if (softs[i].StatAloneID==stataloneid)
                {
                    sb.AppendFormat("<option value='{0}' {2} >{1}</option>", softs[i].ID, softs[i].Name, softs[i].ID == CookieSoftid ? "selected='selected'" : "");    
                }
            }
            return sb.ToString();
        }

        ///// <summary>
        ///// 获取资源类型
        ///// </summary>
        ///// <returns></returns>
        //protected List<ResourceTypeOption> GetResType()
        //{
        //    List<ResourceTypeOption> ResTypeList = new List<ResourceTypeOption>();
        //    ResTypeList.Add(ResourceTypeOption.Soft);
        //    ResTypeList.Add(ResourceTypeOption.Theme);
        //    ResTypeList.Add(ResourceTypeOption.Ring);
        //    ResTypeList.Add(ResourceTypeOption.Picture);
        //    ResTypeList.Add(ResourceTypeOption.CopyrightedSoft);
        //    ResTypeList.Add(ResourceTypeOption.EZine);
        //    ResTypeList.Add(ResourceTypeOption.EBook);
        //    ResTypeList.Add(ResourceTypeOption.Music);
        //    ResTypeList.Add(ResourceTypeOption.Video);
        //    ResTypeList.Add(ResourceTypeOption.College91);
        //    ResTypeList.Add(ResourceTypeOption.ItuneApp);
        //    ResTypeList.Add(ResourceTypeOption.ItuneFreeApp);
        //    ResTypeList.Add(ResourceTypeOption.WinPhone);
        //    ResTypeList.Add(ResourceTypeOption.General);
        //    ResTypeList.Add(ResourceTypeOption.Ad);
        //    ResTypeList.Add(ResourceTypeOption.PandaHomeChannelSoft);
        //    ResTypeList.Add(ResourceTypeOption.PandaHomeThemeModule);
        //    return ResTypeList;
        //}

        /// <summary>
        /// 获取资源类型html
        /// </summary>
        protected string GetResTypeHtml(int defaultvalue=-1)
        {
            StringBuilder sb = new StringBuilder();
            net91com.Reports.UserRights.URLoginService loginService = new net91com.Reports.UserRights.URLoginService();

            foreach (net91com.Reports.UserRights.ResourceType resType in loginService.GetResourceTypes())
            {
                if (resType.TypeID==defaultvalue)
                {
                    sb.AppendFormat("<option value='{0}' selected='selected'>{1}</option>", resType.TypeID, resType.TypeName); 
                }
                else
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", resType.TypeID, resType.TypeName); 
                }
                
            }
                
            return sb.ToString();
        }

        protected string GetResTypeHtml(List<int> lst)
        {
            StringBuilder sb = new StringBuilder();
            net91com.Reports.UserRights.URLoginService loginService = new net91com.Reports.UserRights.URLoginService();
            bool flag = true;
            foreach (net91com.Reports.UserRights.ResourceType resType in loginService.GetResourceTypes())
            {
                if (lst.Contains(resType.TypeID))
                {
                    if (flag)
                    {
                        sb.AppendFormat("<option value='{0}' selected='selected'>{1}</option>", resType.TypeID, resType.TypeName);
                        flag = false;
                    }
                    else
                    {
                        sb.AppendFormat("<option value='{0}'>{1}</option>", resType.TypeID, resType.TypeName); 
                    }
                    
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取不区分项目来源的html
        /// </summary>
        /// <param name="shownoproject">是否展示不区分来源</param>
        /// <param name="outdichtml">对应json字典输出</param>
        /// <returns></returns>
        protected string GetProjectSourceHtml(bool shownoproject, ref string outdichtml, ProjectSourceTypeOptions type = ProjectSourceTypeOptions.Domestic)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            var projectSources = AvailableProjectSources.Where(a => a.ProjectSourceType == type || a.ProjectSourceType == ProjectSourceTypeOptions.None).ToList();
            List<int> lst = UtilityHelp.GetDefaultProjectSources(this.Context, projectSources);
            var projectid = lst.Count <= 0 ? -1 : lst[0];
            bool isContainedInCookie = projectSources.Select(p => p.ProjectSourceID).Contains(projectid);
            for (int i = 0; i < projectSources.Count(); i++)
            {
                if (!shownoproject && projectSources[i].ProjectSourceID==0)
                    continue;
                if (isContainedInCookie)
                {
                    sb.AppendFormat("<option value='{0}' {2}>{1}</option>", projectSources[i].ProjectSourceID, projectSources[i].Name, projectid == projectSources[i].ProjectSourceID ? "selected=\"selected\"" : "");
                }
                else
                {
                    sb.AppendFormat("<option value='{0}' {2}>{1}</option>", projectSources[i].ProjectSourceID, projectSources[i].Name, i == 0 ? "selected=\"selected\"" : "");
                }
                
                sb2.AppendFormat("\"{0}\":\"{1}\",", projectSources[i].ProjectSourceID, projectSources[i].Name); 
            } 
            outdichtml = "{" + sb2.ToString().TrimEnd(',') + "}";
            return sb.ToString();
        }

        protected string GetSoftPlatHtmlJson()
        {
            StringBuilder sb = new StringBuilder("{");
            
            foreach (Soft item in AvailableSofts)
            {
                sb.AppendFormat("'{0}':[{1}],", item.ID,
                       string.Join(",", item.Platforms.Select(p => ((int)p).ToString()).ToArray()));
            } 
            return sb.ToString().TrimEnd(',') + "}";
        }

        protected string GetSoftAreaJson()
        {
            StringBuilder sb = new StringBuilder("{");
            foreach (Soft item in AvailableSofts)
            {
                sb.AppendFormat("'{0}':{1},", item.ID,item.SoftAreaType);
            }
            return sb.ToString().TrimEnd(',') + "}";
        }

        protected string GetProjectJson()
        {
            StringBuilder sb = new StringBuilder("{");

            foreach (var soft in AvailableSofts)
            {
                var strProjSoucre = new StringBuilder();
                foreach (var projectSource in soft.ProjectSources)
                {
                    strProjSoucre.AppendFormat("{{ 'Key':{0},'Value':'{1}' }},", projectSource.ProjectSourceID, projectSource.Name);
                }
                sb.AppendFormat("'{0}':[{1}],", soft.ID, strProjSoucre.ToString().TrimEnd(','));
            }

            return sb.ToString().TrimEnd(',') + "}";
        }

        private int _pageindex = 1;
        protected int PageIndex { get { return _pageindex; } set { _pageindex = value; } }

        private int _pagesize = 15;
        protected int PageSize { get { return _pagesize; } set { _pagesize = value; } }


        //protected void LimitChannelCustom(int softid)
        //{
        //    if (CurrentUser.AccountType == UserTypeOptions.Channel || CurrentUser.AccountType == UserTypeOptions.ChannelPartner)
        //    {
        //        List<ChannelRight> rights = loginService.GetAvailableChannels(softid);
        //        if (rights.Count == 0)
        //            Response.Redirect("/Reports/NoRight.aspx");
        //    }
        //}

        protected string GetAreaJson()
        {
            List<B_AreaEntity> provincelist = B_BaseToolService.Instance.GetProvincesCache();
            List<B_AreaEntity> countrylist = B_BaseToolService.Instance.GetCountriesCache();

            StringBuilder sb = new StringBuilder("{");

            //国内
            StringBuilder sbtemp = new StringBuilder();
            foreach (B_AreaEntity item in provincelist)
            {
                 sbtemp.AppendFormat("{0} {1},{2}'{3}","{","'key':"+item.ID,"'value':'"+item.Name,"},");
            }
            sb.AppendFormat("'{0}':[{1}],", 1, sbtemp.ToString().TrimEnd(','));
            //非国内
            sbtemp = new StringBuilder();
            foreach (B_AreaEntity item in countrylist)
            {
                sbtemp.AppendFormat("{0} {1},{2}'{3}", "{", "'key':" + item.ID, "'value':'" + item.Name, "},");
            }
            sb.AppendFormat("'{0}':[{1}],", 2, sbtemp.ToString().TrimEnd(','));

            return sb.ToString().TrimEnd(',') + "}";

        }

        protected string GetAreaJsonForEnShortName(int flag=0)
        {
            List<B_AreaEntity> provincelist = B_BaseToolService.Instance.GetProvincesCache().Where(p => p.EnShortName != null).ToList();
            List<B_AreaEntity> countrylist = B_BaseToolService.Instance.GetCountriesCache(flag).Where(p => p.EnShortName != null).ToList();

            StringBuilder sb = new StringBuilder("{");

            //国内
            StringBuilder sbtemp = new StringBuilder();
            foreach (B_AreaEntity item in provincelist)
            {
                sbtemp.AppendFormat("{0} {1},{2}'{3}", "{", "'key':'" + item.EnShortName+"'", "'value':'" + item.Name, "},");
            }
            sb.AppendFormat("'{0}':[{1}],", 1, sbtemp.ToString().TrimEnd(','));
            //非国内
            sbtemp = new StringBuilder();
            foreach (B_AreaEntity item in countrylist)
            {
                sbtemp.AppendFormat("{0} {1},{2}'{3}", "{", "'key':'" + item.EnShortName+"'", "'value':'" + item.Name, "},");
            }
            sb.AppendFormat("'{0}':[{1}],", 2, sbtemp.ToString().TrimEnd(','));

            return sb.ToString().TrimEnd(',') + "}";

        }

        protected string GetAreaJsonForSpecial(int flag = 0)
        {
            List<B_AreaEntity> provincelist = B_BaseToolService.Instance.GetProvincesCache().Where(p => p.EnShortName != null).ToList();
            List<B_AreaEntity> countrylist = B_BaseToolService.Instance.GetCountriesCache(flag).Where(p => p.EnShortName != null).ToList();

            StringBuilder sb = new StringBuilder("{");

            //国内
            StringBuilder sbtemp = new StringBuilder();
            foreach (B_AreaEntity item in provincelist)
            {
                sbtemp.AppendFormat("{0} {1},{2}'{3}", "{", "'key':'" + item.ID + "'", "'value':'" + item.Name, "},");
            }
            sb.AppendFormat("'{0}':[{1}],", 1, sbtemp.ToString().TrimEnd(','));
            //非国内
            sbtemp = new StringBuilder();
            foreach (B_AreaEntity item in countrylist)
            {
                sbtemp.AppendFormat("{0} {1},{2}'{3}", "{", "'key':'" + item.ID + "'", "'value':'" + item.Name, "},");
            }
            sb.AppendFormat("'{0}':[{1}],", 2, sbtemp.ToString().TrimEnd(','));

            return sb.ToString().TrimEnd(',') + "}";

        }

    }
}