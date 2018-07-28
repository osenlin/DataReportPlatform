using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Core;
using net91com.Reports.DataAccess.OtherDataAccess;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;
using net91com.Stat.Core;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;
using Sjqd_StatUsers = net91com.Reports.Entities.DataBaseEntity.Sjqd_StatUsers;

namespace net91com.Reports.Services.HttpServices.R_Tool_Service
{
    public class HttpToolService : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "ToolService"; }
        }

        public override string ServiceName
        {
            get { return "通用工具页面"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.NoCheck; }
        }

        public override string RightUrl
        {
            get { return ""; }
        }

        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                    case "getyesterdayretain":
                        myresult = GetYesterdayRetain(context);
                        return myresult;
                    case "getcomplexplatuserdata":
                        myresult = GetComplexPlatUserData(context);
                        return myresult;
                }
            }
            return myresult;
        }
     
        private Result GetComplexPlatUserData(HttpContext context)
        {
            var loginService = new URLoginService();
            SetDownHead(context.Response, "复合平台用户下载量.xls", false, "gb2312");
            if (loginService.CheckUrlRight("tools/InternalCommonUse.aspx", "?act=getcomplexplatuserdata"))
            {
                List<SoftUser> lstsoftuser = GetComplexPlatUserData_GetData(context);
                Func<List<SoftUser>, List<List<string>>> func = p =>
                    {
                        var tempList = new List<List<string>>();
                        foreach (SoftUser item in p)
                        {
                            var values = new List<string>();
                            values.Add(item.StatDate.ToString("yyyy-MM-dd"));
                            values.Add(UtilityHelp.FormatNum(item.NewNum));
                            values.Add(UtilityHelp.FormatNum(item.ActiveNum));
                            tempList.Add(values);
                        }
                        return tempList;
                    };
                string html = GetTableHtml(new[] {"日期", "新增用户", "活跃用户"}, func(lstsoftuser));
                return Result.GetSuccessedResult(html, false, true);
            }
            else
            {
                return Result.GetSuccessedResult("", false, true);
            }
        }

        private List<SoftUser> GetComplexPlatUserData_GetData(HttpContext context)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int soft = Convert.ToInt32(context.Request["soft"]);
            int plat = Convert.ToInt32(context.Request["platform"]);
            int modetype = Convert.ToInt32(context.Request["modetype"]);
            string version = context.Request["version"];
            int realplat1 = 1;
            int realplat2 = 7;
            if (plat == 4)
            {
                realplat1 = 4;
                realplat2 = 9;
            }
            if (1 == modetype)
            {
                string channelNames = context.Request["channelnames"];
                string channelids = context.Request["channelids"];
                //默认加一项进去
                var list = new List<ChannelRight>();
                string[] channels = channelids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                string[] channelarray = channelNames.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                if (channels.Length == 0)
                {
                    channels = new[] {"Customer_0"};
                    channelarray = new[] {"不区分渠道"};
                }
                //非渠道选项
                if (channels.Length != channelarray.Length)
                    return new List<SoftUser>();
                var channelDic = new Dictionary<string, string>();
                for (int i = 0; i < channels.Count(); i++)
                {
                    string[] strs = channels[i].Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length == 2)
                    {
                        list.Add(new ChannelRight
                            {
                                ChannelType = (ChannelTypeOptions) Enum.Parse(typeof (ChannelTypeOptions), strs[0]),
                                ChannelID = Convert.ToInt32(strs[1])
                            });
                        channelDic.Add(channels[i], channelarray[i]);
                    }
                }
                Sjqd_StatUsersByChannelsService service = Sjqd_StatUsersByChannelsService.GetInstance();

                var lists = new List<SoftUser>();

                if (list.Count == 1 && list[0].ChannelID == 0)
                {
                    List<SoftUser> result = Sjqd_StatUsersService.GetInstance()
                                                                 .GetSoftUserListCache(begintime, endtime,
                                                                                       soft, realplat1
                                                                                       , PeriodOptions.Daily,
                                                                                       new URLoginService(),
                                                                                       CacheTimeOption.TenMinutes)
                                                                 .OrderBy(p => p.StatDate)
                                                                 .ToList();
                    List<SoftUser> result2 = Sjqd_StatUsersService.GetInstance()
                                                                  .GetSoftUserListCache(begintime, endtime,
                                                                                        soft, realplat2,
                                                                                        PeriodOptions.Daily,
                                                                                        new URLoginService(),
                                                                                        CacheTimeOption.TenMinutes)
                                                                  .OrderBy(p => p.StatDate)
                                                                  .ToList();
                    if (result.Count() != 0)
                        lists.AddRange(result);
                    if (result2.Count() != 0)
                    {
                        lists.AddRange(result2);
                    }
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        List<SoftUser> result = service.GetSoftUserChanelListCache(begintime, endtime, soft, realplat1,
                                                                                   PeriodOptions.Daily,
                                                                                   list[i].ChannelType,
                                                                                   list[i].ChannelID, channelDic[
                                                                                       list[i]
                                                                                           .ChannelType +
                                                                                       "_" +
                                                                                       list[i].ChannelID], false,
                                                                                   new URLoginService(),
                                                                                   CacheTimeOption.TenMinutes);
                        List<SoftUser> result2 = service.GetSoftUserChanelListCache(begintime, endtime, soft, realplat2,
                                                                                    PeriodOptions.Daily,
                                                                                    list[i].ChannelType,
                                                                                    list[i].ChannelID, channelDic[
                                                                                        list[i]
                                                                                            .ChannelType +
                                                                                        "_" +
                                                                                        list[i].ChannelID], false,
                                                                                    new URLoginService(),
                                                                                    CacheTimeOption.TenMinutes);
                        if (result.Count() != 0)
                            lists.AddRange(result);
                        if (result2.Count() != 0)
                        {
                            lists.AddRange(result2);
                        }
                    }
                }

                return
                    lists.GroupBy(p => p.StatDate)
                         .Select(
                             p =>
                             new SoftUser
                                 {
                                     StatDate = p.Key,
                                     NewNum = p.Sum(l => l.NewNum),
                                     ActiveNum = p.Sum(l => l.NewNum) + p.Sum(l => l.ActiveNum)
                                 })
                         .ToList();
            }
            else
            {
                List<string> lstver = version.Split(',').ToList();
                var lst =
                    new List<Sjqd_StatUsers>();

                var suService = new StatUsersService();
                for (int i = 0; i < lstver.Count; i++)
                {
                    List<Sjqd_StatUsers> lstverdata1 = suService.GetStatUsersByVersion(soft, realplat1,
                                                                                       ChannelTypeOptions.Category, 0,
                                                                                       lstver[i],
                                                                                       (int) PeriodOptions.Daily,
                                                                                       begintime, endtime);
                    if (lstverdata1.Count != 0)
                    {
                        lst.AddRange(lstverdata1);
                    }
                }


                //var lstverdata2 = sbv.GetUsersByVersionCache(soft, realplat1, begintime, endtime, PeriodOptions.Daily, lstver,
                //           CacheTimeOption.TenMinutes);

                for (int i = 0; i < lstver.Count; i++)
                {
                    List<Sjqd_StatUsers> lstverdata2 = suService.GetStatUsersByVersion(soft, realplat1,
                                                                                       ChannelTypeOptions.Category, 0,
                                                                                       lstver[i],
                                                                                       (int) PeriodOptions.Daily,
                                                                                       begintime, endtime);
                    if (lstverdata2.Count != 0)
                    {
                        lst.AddRange(lstverdata2);
                    }
                }
                return
                    lst.GroupBy(p => p.StatDate)
                       .Select(
                           p =>
                           new SoftUser
                               {
                                   StatDate = p.Key,
                                   NewNum = p.Sum(l => l.NewUserCount),
                                   ActiveNum = p.Sum(l => l.ActiveUserCount)
                               })
                       .ToList();
            }
        }


        private Result GetYesterdayRetain(HttpContext context)
        {
            List<Sjqd_StatChannelRetainedUsers> list = GetYesterdayRetainGetData(context);
            if (0 == list.Count)
            {
                list = new List<Sjqd_StatChannelRetainedUsers>();
            }
            SetDownHead(context.Response, "次日留存数据.xls", false, "gb2312");
            Func<List<Sjqd_StatChannelRetainedUsers>, List<List<string>>> func = p =>
                {
                    var tempList = new List<List<string>>();
                    foreach (Sjqd_StatChannelRetainedUsers item in p)
                    {
                        var values = new List<string>();
                        values.Add(item.OriginalDate.ToString());
                        values.Add(item.ChannelName.ToString());
                        values.Add(item.RetainedUserCount.ToString());
                        values.Add(item.OriginalNewUserCount == 0
                                       ? "100.00"
                                       : ((item.RetainedUserCount/(double) item.OriginalNewUserCount)*100).ToString(
                                           "0.00") + "%");
                        tempList.Add(values);
                    }
                    return tempList;
                };
            string html = GetTableHtml(new[] {"日期", "渠道信息", "留存量", "留存率"}, func(list));
            return Result.GetSuccessedResult(html, false, true);
        }

        private List<Sjqd_StatChannelRetainedUsers> GetYesterdayRetainGetData(HttpContext context)
        {
            DateTime begintime = Convert.ToDateTime(context.Request["begintime"]);
            DateTime endtime = Convert.ToDateTime(context.Request["endtime"]);
            int soft = Convert.ToInt32(context.Request["soft"]);
            int plat = Convert.ToInt32(context.Request["platform"]);
            string channelNames = context.Request["channelnames"];
            string channelids = context.Request["channelids"];
            //默认加一项进去
            var list = new List<ChannelRight>();
            string[] channels = channelids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            string[] channelarray = channelNames.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (channels.Length == 0)
            {
                channels = new[] {"Customer_0"};
                channelarray = new[] {"不区分渠道"};
            }
            //非渠道选项
            if (channels.Length != channelarray.Length)
                return new List<Sjqd_StatChannelRetainedUsers>();
            var channelDic = new Dictionary<string, string>();
            for (int i = 0; i < channels.Count(); i++)
            {
                string[] strs = channels[i].Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length == 2)
                {
                    list.Add(new ChannelRight
                        {
                            ChannelType = (ChannelTypeOptions) Enum.Parse(typeof (ChannelTypeOptions), strs[0]),
                            ChannelID = Convert.ToInt32(strs[1])
                        });
                    channelDic.Add(channels[i], channelarray[i]);
                }
            }
            var service = new RetainedUsersService(true);

            var lists = new List<Sjqd_StatChannelRetainedUsers>();
            for (int i = 0; i < list.Count; i++)
            {
                IEnumerable<Sjqd_StatChannelRetainedUsers> result = service.GetStatRetainedUsersCache(soft, plat,
                                                                                                      list[i].ChannelID,
                                                                                                      PeriodOptions
                                                                                                          .Daily,
                                                                                                      begintime, endtime,
                                                                                                      CacheTimeOption
                                                                                                          .TenMinutes,
                                                                                                      list[i]
                                                                                                          .ChannelType,
                                                                                                      new URLoginService
                                                                                                          ())
                                                                           .Where(
                                                                               p =>
                                                                               (p.StatDate - p.OriginalDate).Days == 1)
                                                                           .Select(
                                                                               l => new Sjqd_StatChannelRetainedUsers
                                                                                   {
                                                                                       ChannelName =
                                                                                           channelDic[channels[i]],
                                                                                       ChannelID = l.ChannelID,
                                                                                       StatDate = l.StatDate,
                                                                                       OriginalDate = l.OriginalDate,
                                                                                       RetainedUserCount =
                                                                                           l.RetainedUserCount,
                                                                                       OriginalNewUserCount =
                                                                                           l.OriginalNewUserCount,
                                                                                   });
                if (result.Count() != 0)
                    lists.AddRange(result);
            }
            return lists;
        }
    }
}