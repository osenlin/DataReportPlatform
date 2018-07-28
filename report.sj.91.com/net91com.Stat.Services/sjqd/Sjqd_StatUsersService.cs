using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Web;
using net91com.Stat.Core;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using System.Data.SqlClient;
using net91com.Core.Util;
using System.Data;
//using BY.AccessControlCore;
using net91com.Reports.UserRights;


namespace net91com.Stat.Services.sjqd
{
    public class Sjqd_StatUsersService
    {
        private static string connString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static Sjqd_StatUsersService service;
        private string _cachePreviousKey;
        private bool useCache = true;

        private Sjqd_StatUsersService()
        {
        }

        public static Sjqd_StatUsersService GetInstance()
        {
            if (service == null)
            {
                service = new Sjqd_StatUsersService();
                service._cachePreviousKey = "Sjqd_StatUsersService";
            }
            return service;
        }

        private string BuildCacheKey(params object[] args)
        {
            StringBuilder sbCacheKey = new StringBuilder(_cachePreviousKey);
            foreach (object obj in args)
            {
                sbCacheKey.Append("_");
                sbCacheKey.Append(obj);
            }
            return sbCacheKey.ToString();
        }
     
        public List<SoftUser> GetSoftUserListCache(DateTime begin, DateTime end, int softid, int platformid,
                                                   PeriodOptions period, URLoginService loginService,CacheTimeOption cachetime)
        {
             string cacheKey = BuildCacheKey("GetSoftUserListCache", begin, end, softid, platformid, period,
                                                ((loginService.LoginUser.AccountType ==
                                                  UserTypeOptions.Channel ||loginService.LoginUser.AccountType ==
                                                  UserTypeOptions.ChannelPartner)? loginService.LoginUser.ID.ToString(): ""));
            return CacheHelper.Get<List<SoftUser>>(cacheKey, CacheTimeOption.TenMinutes,
                                                   () =>
                                                   GetSoftUserList(begin, end, softid, platformid, period, loginService));
        }

        public List<SoftUser> GetSoftUserList(DateTime begin, DateTime end, int softid, int platformid,PeriodOptions period, URLoginService loginService)
        {
            List<SoftUser> lists = new List<SoftUser>();
            string sqlstr = "";
            ///无渠道商权限限制
            if (loginService.LoginUser.AccountType != UserTypeOptions.Channel &&
                loginService.LoginUser.AccountType != UserTypeOptions.ChannelPartner)
            {
                if (platformid != (int) MobileOption.None && platformid > 0)
                {
 
                    sqlstr =
                        @"  select period,StatDate,platform,softid ,
(NewUserCount+NewUserCountFromCache+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0)) as newnum,
NewUserCountFromCache,NewUserCount_Broken,NewUserCount_NotBroken,
ActiveUserCountFromCache+ActiveUserCount+ActiveUserCount_Shanzhai activenum,ActiveUserCountFromCache,
ActiveUserCount_Broken,ActiveUserCount_NotBroken,LostUserCount  lostnum ,
TotalUserCount+TotalUserCount_Shanzhai as  totalnum
                                    ,DownValueUsersForNew,
                                    DownValueUsersForAct,
                                    FuncValueUsersForNew,
                                    FuncValueUsersForAct,
                                    NewUserCount-ifnull(NewUserCount_Shualiang,0) FirstNewUserCount,
                                    ActiveUserCount FirstActiveUserCount,
                                    NewUserCount_ZJS,
                                    ActiveUserCount_ZJS,
                                    NewUserCount_Shanzhai,
                                    TotalUserCount_Shanzhai,
                                    ActiveUserCount_Shanzhai,
                                    IFNULL(NewUserCount_SecAct,0) NewUserCount_SecAct,
                                    IFNULL(NewUserCount_SecAct2,0) NewUserCount_SecAct2
                                    from  U_StatUsers 
                                    where softid=?softid and platform=?platform and period=?periodid and StatDate between ?begindate and ?enddate
							        order by StatDate desc";
                }
                else//不区分平台
                {
                    sqlstr =
                        @"  select period,StatDate,0 as platform,softid ,
sum(NewUserCount+NewUserCountFromCache+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0)) as newnum,
sum(NewUserCountFromCache) NewUserCountFromCache,sum(NewUserCount_Broken) NewUserCount_Broken,
sum(NewUserCount_NotBroken) NewUserCount_NotBroken, 
sum(ActiveUserCountFromCache+ActiveUserCount+ActiveUserCount_Shanzhai) as activenum,
sum(ActiveUserCountFromCache) ActiveUserCountFromCache, 
sum(ActiveUserCount_Broken) as ActiveUserCount_Broken, 
sum(ActiveUserCount_NotBroken) as ActiveUserCount_NotBroken,
sum(LostUserCount) as lostnum ,
sum(TotalUserCount+TotalUserCount_Shanzhai) as  totalnum
                                    ,sum(DownValueUsersForNew) DownValueUsersForNew,
                                    sum(DownValueUsersForAct) DownValueUsersForAct,
                                    sum(FuncValueUsersForNew) FuncValueUsersForNew,
                                    sum(FuncValueUsersForAct) FuncValueUsersForAct,
                                    sum(NewUserCount-ifnull(NewUserCount_Shualiang,0))  FirstNewUserCount,
                                    sum(ActiveUserCount) FirstActiveUserCount,
                                    sum(NewUserCount_ZJS)  NewUserCount_ZJS,
                                    sum(ActiveUserCount_ZJS)  ActiveUserCount_ZJS,
                                    sum(TotalUserCount_Shanzhai) TotalUserCount_Shanzhai,
                                    sum(NewUserCount_Shanzhai) NewUserCount_Shanzhai,
                                    sum(ActiveUserCount_Shanzhai) ActiveUserCount_Shanzhai,
                                    sum(IFNULL(NewUserCount_SecAct,0)) NewUserCount_SecAct,
                                    sum(IFNULL(NewUserCount_SecAct2,0)) NewUserCount_SecAct2
                                    from  U_StatUsers
                                    where softid=?softid and period=?periodid and platform<252 and StatDate between ?begindate and ?enddate
							        Group By period,StatDate,softid  order by StatDate desc";
                    
                }
            }
            else ///有渠道商权限限制
            {
                List<int> rangeChannelIds = loginService.GetAvailableChannelIds(softid);

                if (rangeChannelIds.Count == 0)
                    return new List<SoftUser>();

                string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());

                sqlstr =
                    string.Format(@" select period, StatDate,{1} softid ,sum(NewUserCount+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0)) as newnum,
                                0 NewUserCountFromCache,0 NewUserCount_Broken,
                                0 NewUserCount_NotBroken,
                                sum(ActiveUserCount+ActiveUserCount_Shanzhai) as activenum,
                                0 ActiveUserCountFromCache,
                                0 as ActiveUserCount_Broken,
                                0 as ActiveUserCount_NotBroken,
                                sum(LostUserCount) as lostnum ,
                                sum(TotalUserCount+TotalUserCount_Shanzhai) as  totalnum,
                                sum(DownValueUsersForNew) DownValueUsersForNew,
                                sum(DownValueUsersForAct) DownValueUsersForAct,
                                sum(FuncValueUsersForNew) FuncValueUsersForNew,
                                sum(FuncValueUsersForAct) FuncValueUsersForAct,
                                sum(NewUserCount-ifnull(NewUserCount_Shualiang,0))  FirstNewUserCount,
                                sum(ActiveUserCount) FirstActiveUserCount,
                                0  NewUserCount_ZJS,0 ActiveUserCount_ZJS,
                                sum(TotalUserCount_Shanzhai) TotalUserCount_Shanzhai,
                                sum(NewUserCount_Shanzhai) NewUserCount_Shanzhai,
                                sum(ActiveUserCount_Shanzhai) ActiveUserCount_Shanzhai,
                                sum(ifnull(NewUserCount_SecAct,0)) NewUserCount_SecAct,
                                sum(ifnull(NewUserCount_SecAct2,0)) NewUserCount_SecAct2
                                from Sjqd_StatChannelUsers with(nolock)
                                where SoftID=?softid and ChannelID in ({3}) 
                                and Period=?periodid and Platform<252 {0} and StatDate between  ?begindate and ?enddate
                                group by Period,StatDate,SoftID {2}
                                order by StatDate desc",
                                  (platformid != (int) MobileOption.None && platformid > 0)
                                      ? "  and platform=?platform "
                                      : "",
                                  (platformid != (int) MobileOption.None && platformid > 0)
                                      ? " platform,"
                                      : "0 as platform,",
                                  (platformid != (int) MobileOption.None && platformid > 0) ? " ,platform" : "",
                                  channelIdsString);
            }
            var  parameters = new []
                {
                    new MySqlParameter("?softid"  , softid),
                    new MySqlParameter("?platform", platformid),
                    new MySqlParameter("?periodid", (int) period),
                    new MySqlParameter("?begindate", int.Parse(begin.ToString("yyyyMMdd"))),
                    new MySqlParameter("?enddate" , int.Parse(end.ToString("yyyyMMdd")))
                };
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(connString, sqlstr, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(UserBind(dataReader, platformid, true, true, true));
                }
            }
            return lists;
        }

        /// <summary>
        /// 获取预测数据
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platformid"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public List<ForecastSoftUser> GetForecastSoftUser(int softid, int platformid,
                                                          net91com.Stat.Core.PeriodOptions period,
                                                          URLoginService loginService)
        {
            string sqlstr = string.Empty;
            if (loginService.LoginUser.AccountType != UserTypeOptions.ChannelPartner &&
                loginService.LoginUser.AccountType != UserTypeOptions.Channel)
            {
                if (platformid != (int) MobileOption.None && platformid > 0)
                {
                    sqlstr =
                        @"  select  period,StatDate,platform,softid ,(NewUserCount+NewUserCountFromCache+NewUserCount_Shanzhai) as newnum   
                                ,TotalUserCount+TotalUserCount_Shanzhai as  totalnum
                                      from  U_StatUsers 
                                      where softid=?softid and platform=?platform and period=?periodid 
							          Order by StatDate desc
                                     limit 4
                        
                    ";
                }
                else
                {
                    sqlstr =
                        @"  select   period,StatDate,0 as platform,softid 
                                    ,sum(NewUserCount+NewUserCountFromCache+NewUserCount_Shanzhai) as newnum  
                                    ,sum(TotalUserCount+TotalUserCount_Shanzhai) as  totalnum
                                      from  U_StatUsers 
                                      where softid=?softid  and period=?periodid and platform<252
							          Group By period,StatDate,softid Order by StatDate desc
                                      limit 4
                        ";
                }
            }
            else
            {
                List<int> rangeChannelIds = loginService.GetAvailableChannelIds(softid);

                if (rangeChannelIds.Count == 0)
                    return new List<ForecastSoftUser>();

                string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());

                sqlstr = string.Format(@" select   
                                period, StatDate,0 as platform, softid ,sum(NewUserCount+NewUserCount_Shanzhai) as newnum,
                                sum(TotalUserCount+TotalUserCount_Shanzhai) as  totalnum
                                from U_StatChannelUsers 
                                where SoftID=?softid  AND ChannelID in ({1})
                                and Period=?periodid and Platform<252 {0} 
                                group by Period,StatDate,SoftID 
                                order by StatDate desc
                                limit 4",
                                       (platformid != (int) MobileOption.None && platformid > 0)
                                           ? "  and platform=?platform "
                                           : "", channelIdsString);
            }
            var parameters = new []
                {
                    new MySqlParameter("?softid" , softid),
                    new MySqlParameter("?platform",platformid),
                    new MySqlParameter("?periodid",(int) period)
                };                              
            List<ForecastSoftUser> users = new List<ForecastSoftUser>();

            using (IDataReader dataReader = MySqlHelper.ExecuteReader(connString,sqlstr, parameters))
            {
                while (dataReader.Read())
                {
                    ForecastSoftUser softUser = new ForecastSoftUser();
                    softUser.Period = Convert.ToInt16(dataReader["period"]);
                    softUser.Platform = platformid;
                    softUser.SoftId = Convert.ToInt32(dataReader["softid"]);
                    int sDate = Convert.ToInt32(dataReader["StatDate"]);
                    softUser.StatDate = new DateTime(sDate/10000, sDate/100%100, sDate%100, 0, 0, 0);
                    switch (softUser.Period)
                    {
                        case (int) PeriodOptions.Daily:
                            softUser.ForecaseDate = softUser.StatDate.AddDays(1);
                            break;
                        case (int) PeriodOptions.Weekly:
                            softUser.ForecaseDate = softUser.StatDate.AddDays(7);
                            break;
                        case (int) PeriodOptions.Monthly:
                            softUser.ForecaseDate = softUser.StatDate.AddMonths(1);
                            break;
                        case (int) PeriodOptions.Of2Weeks:
                            softUser.ForecaseDate = softUser.StatDate.AddMonths(14);
                            break;
                        case (int) PeriodOptions.Of3Days:
                            softUser.ForecaseDate = softUser.StatDate.AddMonths(3);
                            break;
                    }
                    softUser.NewUserCount = Convert.ToInt32(dataReader["newnum"]);
                    softUser.TotalUserCount = Convert.ToInt32(dataReader["totalnum"]);
                    users.Add(softUser);
                }
            }
            return users;
        }

        public List<ForecastSoftUser> GetForecastSoftUserCache(int softid, int platformid,PeriodOptions period,
                                                               URLoginService loginService, CacheTimeOption cachetime)
        {

            string cacheKey = BuildCacheKey("GetForecastSoftUserCache", softid, platformid, period,
                                loginService == null
                                    ? ""
                                    : ((loginService.LoginUser.AccountType ==UserTypeOptions.Channel ||
                                        loginService.LoginUser.AccountType ==UserTypeOptions.ChannelPartner)
                                           ? loginService.LoginUser.ID.ToString(): ""));
            return CacheHelper.Get<List<ForecastSoftUser>>(cacheKey, CacheTimeOption.TenMinutes,
                                                             () =>
                                                             GetForecastSoftUser(softid, platformid, period,
                                                                                 loginService));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="platformid"></param>
        /// <param name="hasbrokenuser">是否包含有越狱字段数据</param>
        /// <param name="hasvalueuser">是否包含有价值用户字段</param>
        /// <param name="hasOldUser">是否包含未处理的新增活跃</param>
        /// <returns></returns>
        public SoftUser UserBind(IDataReader dataReader, int platformid, bool hasbrokenuser, bool hasvalueuser,
                                 bool hasOldUser)
        {
            SoftUser model = new SoftUser();
            object obj;
            obj = dataReader["period"];
            if (obj != null && obj != DBNull.Value)
            {
                model.Period = Convert.ToInt32(obj);
            }
            model.Platform = platformid;
            obj = dataReader["softid"];
            if (obj != null && obj != DBNull.Value)
            {
                model.SoftId = Convert.ToInt32(obj);
            }
            obj = dataReader["StatDate"];
            if (obj != null && obj != DBNull.Value)
            {
                int sDate = Convert.ToInt32(obj);
                model.StatDate = new DateTime(sDate/10000, sDate/100%100, sDate%100, 0, 0, 0);
            }
            obj = dataReader["newnum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.NewNum = Convert.ToInt32(obj);
            }
            //二次激活
            model.NewUserCount_SecAct = Convert.ToInt32(dataReader["NewUserCount_SecAct"]);
            model.NewUserCount_SecAct2 = Convert.ToInt32(dataReader["NewUserCount_SecAct2"]);
            if (hasvalueuser)
            {
                obj = dataReader["DownValueUsersForNew"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.DownValueUsersForNew = Convert.ToInt32(obj);
                }
                obj = dataReader["DownValueUsersForAct"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.DownValueUsersForAct = Convert.ToInt32(obj);
                }
                obj = dataReader["FuncValueUsersForNew"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.FuncValueUsersForNew = Convert.ToInt32(obj);
                }
                obj = dataReader["FuncValueUsersForAct"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.FuncValueUsersForAct = Convert.ToInt32(obj);
                }
            }
            if (hasbrokenuser)
            {
                obj = dataReader["NewUserCount_Broken"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.NewNum_Broken = Convert.ToInt32(obj);
                }
                obj = dataReader["NewUserCount_NotBroken"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.NewNum_NotBroken = Convert.ToInt32(obj);
                }
                obj = dataReader["ActiveUserCount_Broken"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.ActiveNum_Broken = Convert.ToInt32(obj);
                }
                obj = dataReader["ActiveUserCount_NotBroken"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.ActiveNum_NotBroken = Convert.ToInt32(obj);
                }
            }
            obj = dataReader["NewUserCount_ZJS"];
            if (obj != null && obj != DBNull.Value)
            {
                model.NewNum_ZJS = Convert.ToInt32(obj);
            }
            obj = dataReader["ActiveUserCount_ZJS"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ActiveNum_ZJS = Convert.ToInt32(obj);
            }
            if (hasOldUser)
            {
                obj = dataReader["FirstNewUserCount"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.FirstNewUserCount = Convert.ToInt32(obj);
                }
                obj = dataReader["FirstActiveUserCount"];
                if (obj != null && obj != DBNull.Value)
                {
                    model.FirstActiveUserCount = Convert.ToInt32(obj);
                }
            }

            obj = dataReader["activenum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ActiveNum = Convert.ToInt32(obj);
            }
            ///默认值是-1，对应数据库是空值
            model.LostNum = -1;
            obj = dataReader["lostnum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.LostNum = Convert.ToInt32(obj);
            }
            obj = dataReader["totalnum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.TotalNum = Convert.ToInt32(obj);
            }
            obj = dataReader["NewUserCount_Shanzhai"];
            if (obj != null && obj != DBNull.Value)
            {
                model.NewUserCount_Shanzhai = Convert.ToInt32(obj);
            }

            obj = dataReader["ActiveUserCount_Shanzhai"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ActiveUserCount_Shanzhai = Convert.ToInt32(obj);
            }

            obj = dataReader["TotalUserCount_Shanzhai"];
            if (obj != null && obj != DBNull.Value)
            {
                model.TotalUserCount_Shanzhai = Convert.ToInt32(obj);
            }


            ///使用量
            model.UseNum = model.ActiveNum + model.NewNum;
            ///第一次计算使用量
            model.FirstUseUserCount = model.FirstActiveUserCount + model.FirstNewUserCount;
            ///增长率
            float frontNum = model.TotalNum - model.NewNum;
            if (frontNum <= 0)
                model.Growth = "100%";
            else
                model.Growth = (model.NewNum/frontNum*100).ToString("0.00") + "%";
            ///活跃百分比和使用百分比
            if (model.TotalNum != 0)
            {
                model.ActivityPercent = (model.ActiveNum/(double) model.TotalNum*100).ToString("0.00") + "%";
                model.UsePercent = (model.UseNum/(double) model.TotalNum*100).ToString("0.00") + "%";
            }
            else
            {
                model.ActivityPercent = "100%";
                model.UsePercent = "100%";
            }
            ///流失率
            if (model.LostNum == -1)
            {
                model.LostPercent = "";
            }
            else
            {
                model.LostPercent = (model.LostNum/(double) model.UseNum*100).ToString("0.00") + "%";
            }
            obj = dataReader["NewUserCountFromCache"];
            if (obj != null && obj != DBNull.Value)
            {
                model.NewUserCountFromCache = Convert.ToInt32(obj);
            }
            obj = dataReader["ActiveUserCountFromCache"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ActiveUserCountFromCache = Convert.ToInt32(obj);
            }
            return model;
        }

        #region 综合页面需要的方法

        public List<SoftUser> GetAllSoftUsers(DateTime begin, DateTime end, int softid, int platformid,
                                              PeriodOptions period)
        {
            List<SoftUser> lists = new List<SoftUser>();
            string sqlstr = "";

            if (period != (int) MobileOption.None)
            {
                sqlstr =
                    @"  select period,StatDate,platform,softid ,
NewUserCountFromCache+NewUserCount+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0)  as newnum,
ActiveUserCountFromCache+ActiveUserCount+ActiveUserCount_Shanzhai as activenum,LostUserCount as lostnum ,
TotalUserCount+TotalUserCount_Shanzhai as  totalnum
                             from  U_StatUsers 
                             where softid=?softid and platform=?platform and period=?periodid and StatDate between ?begindate and ?enddate  ";
            }
            var parameters = new []
                {
                    new MySqlParameter("?softid",   softid),
                    new MySqlParameter("?platform", platformid),
                    new MySqlParameter("?periodid", (int) period),
                    new MySqlParameter("?begindate",begin.ToString("yyyyMMdd")),
                    new MySqlParameter("?enddate",  end.ToString("yyyyMMdd"))
                };
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(connString,sqlstr, parameters))
            {
                while (dataReader.Read())
                {
                    lists.Add(UserBind(dataReader));
                }
            }
            return lists;
        }

        //获取所有的softusers
        public List<SoftUser> GetAllSoftUsersCache(DateTime begin, DateTime end, int softid, int platformid,
                                                   net91com.Stat.Core.PeriodOptions period, CacheTimeOption cachetime)
        {
             string cacheKey = BuildCacheKey("GetAllSoftUsersCache", begin, end, softid, platformid, period);
            return CacheHelper.Get<List<SoftUser>>(cacheKey, CacheTimeOption.TenMinutes,
                                                   () => GetAllSoftUsers(begin, end, softid, platformid, period));

        }

     
        public List<SoftUser> GetUsersWithNoPeriodCache(DateTime begin, DateTime end, int softid, int platformid,
                                                        CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetUsersWithNoPeriodCache", begin, end, softid, platformid);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<SoftUser>>(cacheKey);
                }
                List<SoftUser> list = GetSoftUsersNoPeriod(begin, end, softid, platformid);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<SoftUser>>(cacheKey, list, cachetime, CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetSoftUsersNoPeriod(begin, end, softid, platformid);
            }
        }

        private List<SoftUser> GetSoftUsersNoPeriod(DateTime begin, DateTime end, int softid, int platformid)
        {
            List<SoftUser> lists = new List<SoftUser>();
            string sqlstr = "";

            sqlstr =
                @"  select period,StatDate,platform,softid ,
NewUserCountFromCache+NewUserCount+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0) as newnum,
ActiveUserCountFromCache+ActiveUserCount+ActiveUserCount_Shanzhai as activenum,LostUserCount as lostnum ,
TotalUserCount+TotalUserCount_Shanzhai as  totalnum
                                      from  U_StatUsers 
      where softid=?softid and platform=?platform   
and StatDate between ?begindate and ?enddate";

            var parameters = new []
                {
                    new MySqlParameter("?softid",    softid),
                    new MySqlParameter("?platform",  platformid),
                    new MySqlParameter("?begindate", begin.ToString("yyyyMMdd")),
                    new MySqlParameter("?enddate",   end.ToString("yyyyMMdd"))
                };
            using (MySqlCommand cmd = new MySqlCommand(sqlstr, new MySqlConnection(connString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                cmd.Parameters.AddRange(parameters);
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lists.Add(UserBind(dataReader));
                    }
                }
            }

            return lists;
        }

        ///获取历史最值
        public List<SoftUser> GetMaxNumCache(DateTime begin, DateTime end, int softid, int platformid,
                                             net91com.Stat.Core.PeriodOptions period, CacheTimeOption cachetime)
        {

            string cacheKey = BuildCacheKey("GetMaxNumCache", begin, end, softid, platformid, period);
            return CacheHelper.Get<List<SoftUser>>(cacheKey, CacheTimeOption.TenMinutes, () => GetMaxNum(begin, end, softid, platformid, period));           
        }

        ///获取历史最值
        private List<SoftUser> GetMaxNum(DateTime begin, DateTime end, int softid, int platformid,
                                         PeriodOptions period)
        {
            string sqlstr = "";
            if (platformid != (int) MobileOption.None)
            {
                sqlstr =
                    @"select StatDate,NewUserCount,ActiveUserCount from (
select * from(
                        select StatDate,NewUserCount+NewUserCountFromCache+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0) NewUserCount
                        ,ActiveUserCount+ActiveUserCountFromCache+ActiveUserCount_Shanzhai ActiveUserCount
                        from U_StatUsers 
                        where SoftID=?softid and Platform=?platform and Period=?periodid
                        order by NewUserCount desc 
                        limit 1 ) A
                        union
select * from (
                        select StatDate,NewUserCount+NewUserCountFromCache+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0) NewUserCount
                        ,ActiveUserCount+ActiveUserCountFromCache+ActiveUserCount_Shanzhai ActiveUserCount
                        from U_StatUsers 
                        where SoftID=?softid and Platform=?platform and Period=?periodid
                        order by ActiveUserCount desc
                        limit 1
                       ) A ) B";
            }
            var  parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid", softid),
                    new MySqlParameter("?platform",platformid),
                    new MySqlParameter("?periodid",(int) period)
                };
            List<SoftUser> softs = new List<SoftUser>();
            using (MySqlCommand cmd=new MySqlCommand(sqlstr,new MySqlConnection(connString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                cmd.Parameters.AddRange(parameters);
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        SoftUser softuser = new SoftUser();
                        if (dataReader["statdate"] != DBNull.Value && dataReader["statdate"] != null)
                        {
                            int temp = Convert.ToInt32(dataReader["statdate"]);
                            softuser.StatDate = new DateTime(temp / 10000, temp / 100 % 100, temp % 100, 0, 0, 0);
                        }
                        if (dataReader["newusercount"] != DBNull.Value && dataReader["newusercount"] != null)
                        {
                            softuser.NewNum = Convert.ToInt32(dataReader["newusercount"]);
                        }
                        if (dataReader["activeusercount"] != DBNull.Value && dataReader["activeusercount"] != null)
                        {
                            softuser.ActiveNum = Convert.ToInt32(dataReader["activeusercount"]);
                        }
                        softs.Add(softuser);
                    }
                }

            }
           
            return softs;
        }

        public SoftUser UserBind(IDataReader dataReader)
        {
            SoftUser model = new SoftUser();
            object obj;
            obj = dataReader["period"];
            if (obj != null && obj != DBNull.Value)
            {
                model.Period = Convert.ToInt32(obj);
            }
            obj = dataReader["platform"];
            if (obj != null && obj != DBNull.Value)
            {
                model.Platform = Convert.ToInt32(obj);
            }
            obj = dataReader["softid"];
            if (obj != null && obj != DBNull.Value)
            {
                model.SoftId = Convert.ToInt32(obj);
            }
            obj = dataReader["StatDate"];
            if (obj != null && obj != DBNull.Value)
            {
                int sDate = Convert.ToInt32(obj);
                model.StatDate = new DateTime(sDate/10000, sDate/100%100, sDate%100, 0, 0, 0);
            }
            obj = dataReader["newnum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.NewNum = Convert.ToInt32(obj);
            }
            obj = dataReader["activenum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ActiveNum = Convert.ToInt32(obj);
            }
            ///默认值是-1，对应数据库是空值
            model.LostNum = -1;
            obj = dataReader["lostnum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.LostNum = Convert.ToInt32(obj);
            }
            obj = dataReader["totalnum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.TotalNum = Convert.ToInt32(obj);
            }
            ///使用量
            model.UseNum = model.ActiveNum + model.NewNum;
            ///增长率
            float frontNum = model.TotalNum - model.NewNum;
            if (frontNum <= 0)
                model.Growth = "100%";
            else
                model.Growth = (model.NewNum/frontNum*100).ToString("0.00") + "%";
            ///活跃百分比和使用百分比
            if (model.TotalNum != 0)
            {
                model.ActivityPercent = (model.ActiveNum/(double) model.TotalNum*100).ToString("0.00") + "%";
                model.UsePercent = (model.UseNum/(double) model.TotalNum*100).ToString("0.00") + "%";
            }
            else
            {
                model.ActivityPercent = "100%";
                model.UsePercent = "100%";
            }
            ///流失率
            if (model.LostNum == -1)
            {
                model.LostPercent = "";
            }
            else
            {
                model.LostPercent = (model.LostNum/(double) model.UseNum*100).ToString("0.00") + "%";
            }
            return model;
        }

        #endregion

        #region 获取用户新增/活跃等数据(不需要登录的接口使用)

        /// <summary>
        /// 获取用户新增/活跃等数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        public List<SoftUser> GetStatUsers(int softId, int platform, net91com.Stat.Core.PeriodOptions period,
                                           DateTime startDate, DateTime endDate, CacheTimeOption cacheTime)
        {
            string cacheKey = string.Format("Sjqd_StatUsersService.GetStatUsers-{0}-{1}-{2}-{3}-{4}", softId, platform,
                                            period, startDate, endDate);
            return CacheHelper.Get<List<SoftUser>>(cacheKey, CacheTimeOption.TenMinutes, () => GetStatUsers(softId, platform, period, startDate, endDate));
        }

        /// <summary>
        /// 获取用户新增/活跃等数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<SoftUser> GetStatUsers(int softId, int platform, net91com.Stat.Core.PeriodOptions period,
                                           DateTime startDate, DateTime endDate)
        {
            string cmdText = string.Format(
                @"select StatDate
                                    ,SUM(NewUserCount+NewUserCountFromCache+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0)) NewUserCount
                                    ,SUM(ActiveUserCount+ActiveUserCountFromCache+ActiveUserCount_Shanzhai) ActiveUserCount
                                    ,Sum(TotalUserCount+TotalUserCount_Shanzhai) TotalNum
                                    ,Sum(ifnull(DownValueUsersForNew,0)) DownValueUsersForNew
                                    ,Sum(ifnull(DownValueUsersForAct,0)) DownValueUsersForAct
                                    ,SUM(NewUserCount+NewUserCount_Shanzhai-ifnull(NewUserCount_Shualiang,0)) FirstNewUserCount
                                    ,SUM(ActiveUserCount+ActiveUserCount_Shanzhai) FirstActiveUserCount
                               from Sjqd_StatUsers 
                               where SoftID=?SoftID and Period=?Period and StatDate between ?StartDate and ?EndDate{0}
                               group by StatDate
                               order by StatDate",
                platform != (int) MobileOption.None && platform > 0 ? " and Platform=?Platform" : " and Platform<252 ");

            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?SoftID"   ,       softId),
                    new MySqlParameter("?Platform" ,       platform),
                    new MySqlParameter("?Period"   , (int) period),
                    new MySqlParameter("?StartDate",       startDate.ToString("yyyyMMdd")),
                    new MySqlParameter("?EndDate"  ,       endDate.ToString("yyyyMMdd"))
                };
            List<SoftUser> result = new List<SoftUser>();

            using (MySqlCommand cmd=new MySqlCommand(cmdText,new MySqlConnection(connString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                cmd.Parameters.AddRange(parameters);
                using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int statDate = Convert.ToInt32(reader["StatDate"]);
                            result.Add(
                                new SoftUser
                                    {
                                        SoftId = softId,
                                        Platform = platform,
                                        Period = (int) period,
                                        StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                        NewNum = Convert.ToInt32(reader["NewUserCount"]),
                                        ActiveNum = Convert.ToInt32(reader["ActiveUserCount"]),
                                        TotalNum = Convert.ToInt32(reader["TotalNum"]),
                                        DownValueUsersForAct = Convert.ToInt32(reader["DownValueUsersForAct"]),
                                        DownValueUsersForNew = Convert.ToInt32(reader["DownValueUsersForNew"]),
                                        FirstActiveUserCount = Convert.ToInt32(reader["FirstActiveUserCount"]),
                                        FirstNewUserCount = Convert.ToInt32(reader["FirstNewUserCount"])
                                    });
                        }
                    }
            }

            return result;
        }

        #endregion
    }
}