using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Web.Base;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd;
using System.Text;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Core.Extensions;
using net91com.Core;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports
{
    public partial class DeveloperIndex : ReportBasePage
    {
        public DateTime dtnowtime;

        public List<string> TotalNumList = new List<string>();
        public DateTime DtSessionTime { get; set; }
        /// <summary>
        /// 获取跳转地址
        /// </summary>
        public string DefaultUrl { get; set; }
        public string DefaultParentUrl { get; set; }
        public string DefaltName { get;set; }
        private int _pageindex = 1;
        new  protected int PageIndex { get { return _pageindex; } set { _pageindex = value; } }
        private int _pagesize = 10;
        new protected int PageSize { get { return _pagesize; } set { _pagesize = value; } }
        UsersService userService = new UsersService(true);

        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID > 0).ToList(); }
        }

        List<UserStatAll> userStatAll;
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();

            var ds = UtilityService.GetInstance();
            dtnowtime = ds.GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.UserUseNewActivity, CacheTimeOption.TenMinutes);
            DtSessionTime = ds.GetMaxTimeCache(net91com.Stat.Core.PeriodOptions.Daily, ReportType.SoftSessionLen, CacheTimeOption.TenMinutes);
            ///得到计算时间
             if (!IsPostBack)
            {
              
                PageIndex = 1;
                AspNetPager1.CurrentPageIndex = PageIndex;
                AspNetPager1.PageSize = _pagesize;
                ///绑定中间数据
               
                
            }
            GetUrl();
            ///设置排序的一些东西
            Sorter();

            



        }
        /// <summary>
        /// 排序设置
        /// </summary>
        private void Sorter()
        {
            ///排序改变了
            if (HiddenSorterChange.Value != "0")
            {
                AspNetPager1.CurrentPageIndex = 1;
            }
        }

        private void BindMiddelData()
        {
            var middleData = userStatAll.GroupBy(p => new { platform = p.Platform })
                   .Select(p => new Result
                   {
                       Platform = p.Key.platform,
                       Count = p.Count(),
                       AllUserNum = p.Sum(l => l.AllUserNum),
                       DayNewNum = p.Sum(l => l.DayNewNum),
                       DayActive = p.Sum(l => l.DayActiveNum),
                       AllSessions = p.Sum(l => l.AllSessions),
                       AllSessionLength = p.Sum(l => l.AllSessionsLength)
                   }).OrderByDescending(p=>p.AllUserNum).ToList();
           
                ///总应用数
                TotalNumList.Add(Utility.SetNum(middleData.Sum(p => p.Count)));
                ///总累计用户数
                TotalNumList.Add(Utility.SetNum(middleData.Sum(p => p.AllUserNum)));
                ///总日新增用户
                TotalNumList.Add(Utility.SetNum(middleData.Sum(p => p.DayNewNum)));
                ///总日活跃用户
                TotalNumList.Add(Utility.SetNum(middleData.Sum(p => p.DayActive)));
            
                TableRepeat5.DataSource = middleData;
                TableRepeat5.DataBind();
              
             
           
        }
        /// <summary>
        /// 获取跳转地址
        /// </summary>
        private void GetUrl()
        {

            Right right = loginService.FindRight("reports/Default.aspx");
            if (right != null)
            {
                DefaultUrl = right.ID.ToString();
                DefaultParentUrl = right.ParentID.ToString();
                DefaltName = right.Name;
            }

        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
           PageIndex=  AspNetPager1.CurrentPageIndex;
           int count=0;
          
           Repeater1.DataSource=HandleListUsers(GetPageData(PageSize, PageIndex,out count));
           Repeater1.DataBind();
           ///总条数
           AspNetPager1.RecordCount = count;
           BindMiddelData();
        }
        /// <summary>
        /// 名称处理下，例如pad，名称都加(HD)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<UserStatAll> HandleListUsers(List<UserStatAll> list)
        {
            list.ForEach(p => p.SoftName = p.SoftName + ((p.Platform ==(int) MobileOption.IPAD || p.Platform == (int)MobileOption.AndroidPad) ? "(HD)" : ""));
            return list;
        }


        public List<UserStatAll> GetPageData(int pagesize, int pageindex, out int count)
        {
           
            
            ///所有的当天softuser 对象
            List<Sjqd_StatUsers> allsoftsuser = userService.GetSimpleSoftUserListCache(dtnowtime.Date, net91com.Stat.Core.PeriodOptions.Daily, CacheTimeOption.TenMinutes);
            var temp = from a in allsoftsuser
                       join b in AvailableSofts
                       on a.SoftID equals b.ID
                       where b.Platforms.Contains((MobileOption)a.Platform)
                       select new UserStatAll
                       {
                           SoftOutID = b.OutID,
                           SoftId = b.ID,
                           SoftName = b.Name,
                           Platform = (int)a.Platform,
                           AllUserNum = a.TotalUserCount,
                           DayNewNum = a.NewUserCount,
                           DayActiveNum = a.ActiveUserCount,
                           DayAvgSessionLength = a.AvgSessionLength/60,
                           DayAvgStartNum = a.AvgSessions ,
                           AllSessions =(int) a.AvgSessions * (a.NewUserCount + a.ActiveUserCount),
                           AllSessionsLength =(int) a.AvgSessionLength * (a.NewUserCount + a.ActiveUserCount)/60,
                       };
            

            //和用户能看到的软件内连接
            userStatAll = temp.ToList();
             //若无数据的情况下
            if (userStatAll.Count == 0)
            {
                for (int i = 0; i < AvailableSofts.Count; i++)
                {
                    for (int j = 0; j < AvailableSofts[i].Platforms.Count; j++)
                    {
                        UserStatAll r = new UserStatAll
                        {
                            SoftOutID = AvailableSofts[i].OutID,
                            SoftId = AvailableSofts[i].ID,
                            SoftName = AvailableSofts[i].Name,
                            Platform = (int)AvailableSofts[i].Platforms[j],
                            AllUserNum = 0,
                            DayNewNum = 0,
                            DayActiveNum = 0,
                            DayAvgSessionLength = 0,
                            DayAvgStartNum = 0,
                            AllSessions = 0,
                            AllSessionsLength = 0
                        };
                        userStatAll.Add(r);

                    }
                }
                
            }

            string softids = string.Join(",", AvailableSofts.Select(p => p.ID.ToString()).ToArray());
            List<Sjqd_StatUsersByOneTime> oneTimeUsers = new List<Sjqd_StatUsersByOneTime>();
                //new Sjqd_StatUsersByOneTimeService(true).GetSjqd_AllSoftsOneTimeUsers(softids);
            userStatAll = (from a in userStatAll
                           join b in oneTimeUsers
                           on new { soft = a.SoftId, plat = a.Platform }
                           equals  new { soft = b.SoftID, plat = b.Platform } into os
                           from tt in os.DefaultIfEmpty()
                           select new UserStatAll
                            {
                                SoftOutID = a.SoftOutID,
                                SoftId = a.SoftId,
                                SoftName = a.SoftName,
                                Platform =  a.Platform,
                                AllUserNum = a.AllUserNum,
                                DayNewNum = a.DayNewNum,
                                DayActiveNum = a.DayActiveNum,
                                DayAvgSessionLength = a.DayAvgSessionLength,
                                DayAvgStartNum = a.DayAvgStartNum,
                                AllSessions = a.AllSessions,
                                AllSessionsLength = a.AllSessionsLength,
                                OneTimeUsers = tt == null?0:tt.UserCount,
                                OneTimeUsersPercent = a.AllUserNum == 0 ? "" : (((tt == null ? 0 : (decimal)tt.UserCount) * 100 / a.AllUserNum).ToString("0.00") + "%")
                            }).ToList();

            count = userStatAll.Count;
            SetSorter();
            var result=userStatAll.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
            HiddenSorterChange.Value = "0";
            return result ;
        }
        ///开始排序,2升序,1降序
        private void SetSorter()
        {
            Func<UserStatAll, int> dele;

            switch (HiddenSorterField.Value)
            {

                case "1":
                    dele = p => p.Platform;
                    break;
                case "2":
                    dele = p => p.SoftName.GetHashCode();
                    break;
                case "3":
                    dele = p => p.AllUserNum;
                    break;
                case "4":
                    dele = p => p.OneTimeUsers;
                    break;
                case "5":
                    dele = p => p.DayNewNum;
                    break;
                case "6":
                    dele = p => p.DayActiveNum;
                    break;
                case "7":
                    dele = p => p.AllSessions;
                    break;
                case "8":
                    dele = p => p.AllSessionsLength;
                    break;
                default:
                    dele = p => p.SoftName.GetHashCode();
                    break;
            }
            ///首次登陆按照名字和平台
            
            if (HiddenSorterType.Value == "1")
            {
                userStatAll = userStatAll.OrderByDescending(dele).ToList();
            }
            else
            {
                userStatAll = userStatAll.OrderBy(dele).ToList();
            }
              
        }

        public string GetSrc(int obj)
        {
            string picsrc="";
            switch (obj)
            {
                case (int)MobileOption.iPhone:
                    picsrc = "../Images/PlatLogo/iphone.jpg";
                    break;
                case (int)MobileOption.Android:
                    picsrc = "../Images/PlatLogo/android.jpg";
                    break;
                case (int)MobileOption.AndroidPad:
                    picsrc = "../Images/PlatLogo/android.jpg";
                    break;
                case (int)MobileOption.AndroidTV:
                    picsrc = "../Images/PlatLogo/android.jpg";
                    break;
                case (int)MobileOption.IPAD:
                    picsrc = "../Images/PlatLogo/iphone.jpg";
                    break;
                case (int)MobileOption.M8:
                    picsrc = "../Images/PlatLogo/m8.jpg";
                    break;
                case (int)MobileOption.Oms:
                    picsrc = "../Images/PlatLogo/oms.jpg";
                    break;
                case (int)MobileOption.WP7:
                    picsrc = "../Images/PlatLogo/WP7.jpg";
                    break;
                case (int)MobileOption.WM:
                    picsrc = "../Images/PlatLogo/wm.jpg";
                    break;
                case (int)MobileOption.S60:
                    picsrc = "../Images/PlatLogo/symbian.jpg";
                    break;
                case (int)MobileOption.WebGame:
                    picsrc = "../Images/PlatLogo/webgame.jpg";
                    break;
                case (int)MobileOption.PC:
                    picsrc = "../Images/PlatLogo/pc.png";
                    break;
                case (int)MobileOption.Win8:
                    picsrc = "../Images/PlatLogo/win8.png";
                    break;
            }
            return picsrc;
        }
    }
 
    public class Result
    {
        public int Platform { get; set; }

        public int Count { get; set; }

        public int AllUserNum { get; set; }

        public int DayNewNum { get; set; }

        public int DayActive { get; set; }

        public int AllSessions { get; set; }

        public int AllSessionLength { get; set; }
    }
}