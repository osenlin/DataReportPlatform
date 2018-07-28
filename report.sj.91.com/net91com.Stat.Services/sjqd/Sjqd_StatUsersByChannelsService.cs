using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Web;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using System.Data.SqlClient;
using net91com.Core.Util;
using System.Data;
//using BY.AccessControlCore;
using net91com.Reports.UserRights;

namespace net91com.Stat.Services.sjqd
{
    public class Sjqd_StatUsersByChannelsService
    {
        private static string connString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
        private static Sjqd_StatUsersByChannelsService service;
        private string _cachePreviousKey;
        private bool useCache = true;


        private Sjqd_StatUsersByChannelsService()
        {
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

        public static Sjqd_StatUsersByChannelsService GetInstance()
        {
            if (service == null)
            {
                service = new Sjqd_StatUsersByChannelsService();
                service._cachePreviousKey = "U_StatUsersByChannelsService";
            }
            return service;
        }

        /// <summary>
        /// 获取对应所选渠道下的，所有用户信息
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="softid"></param>
        /// <param name="platformid"></param>
        /// <param name="period"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="selectchannelvalue"></param>
        /// <param name="cachetime"></param>
        /// <returns></returns>
        public List<SoftUser> GetSoftUserChanelListCache(DateTime begin, DateTime end, int softid, int platformid,
                                                         net91com.Stat.Core.PeriodOptions period,
                                                         ChannelTypeOptions selectchanneltype, int selectchannelvalue,
                                                         string channleName, bool useModules,
                                                         net91com.Reports.UserRights.URLoginService loginService,
                                                         CacheTimeOption cachetime)
        {


                string cacheKey = BuildCacheKey("GetSoftUserChanelListCache", begin, end, softid, platformid, period,
                                                selectchanneltype, selectchannelvalue, useModules,
                                                loginService == null
                                                    ? ""
                                                    : ((loginService.LoginUser.AccountType ==
                                                        UserTypeOptions.Channel ||
                                                        loginService.LoginUser.AccountType ==
                                                        UserTypeOptions.ChannelPartner)
                                                           ? loginService.LoginUser.ID.ToString()
                                                           : ""));

                return CacheHelper.Get<List<SoftUser>>(cacheKey,CacheTimeOption.TenMinutes,
                            () => GetSoftUserChanelList(begin, end, softid, platformid, period, selectchanneltype,
                                                            selectchannelvalue, channleName, useModules, loginService));
               
        }


        /// <summary>
        /// 获取对应所选渠道下的，所有用户信息
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="softid"></param>
        /// <param name="platformid"></param>
        /// <param name="period"></param>
        /// <param name="selectchanneltype"></param>
        /// <param name="selectchannelvalue"></param>
        /// <returns></returns>
        public List<SoftUser> GetSoftUserChanelList(DateTime begin, DateTime end, int softid, int platformid,
                                                    net91com.Stat.Core.PeriodOptions period,
                                                    ChannelTypeOptions selectchanneltype, int selectchannelvalue,
                                                    string channeltext, bool useModules, URLoginService loginService)
        {
            List<int> rangeChannelIds = loginService == null
                                            ? new URChannelsService().GetChannelIds(softid, selectchanneltype,
                                                                                    new int[] {selectchannelvalue})
                                            : loginService.GetAvailableChannelIds(softid, selectchanneltype,
                                                                                  new int[] {selectchannelvalue});

            if (rangeChannelIds.Count == 0)
                return new List<SoftUser>();

            string channelIdsString = string.Join(",", rangeChannelIds.Select(a => a.ToString()).ToArray());
            string sql;
            if (!useModules)
            {
                sql =
                    string.Format(@"select * from ( select period,StatDate {5},{2} softid,sum(NewUserCount-ifnull(NewUserCount_Shualiang,0)) as newnum,sum(ActiveUserCount) as activenum
                                        ,0 lostnum 
                                        ,sum(TotalUserCount) totalnum
                                        ,0 DownValueUsersForNew
                                        ,0 DownValueUsersForAct
                                        ,0 FuncValueUsersForNew
                                        ,0 FuncValueUsersForAct
                                        ,'{0}' ChannelID
                                        ,'{1}' ChannelName
                                        ,0 NewUserCount_SecAct
                                        ,sum(ifnull(NewUserCount_SecAct2,0)) NewUserCount_SecAct2
                                    from  U_StatChannelUsers as A  
                                    where A.softid=?softid and   {3}  A.period=?period and A.StatDate between ?begindate and ?enddate and ChannelID in ({6})
                                    Group By A.period,A.StatDate {4})A order by StatDate desc",
                                  selectchannelvalue,
                                  channeltext,
                                  softid,
                                  platformid == 0 ? " A.platform<252  and " : " A.platform=?platform and ",
                                  platformid == 0 ? "" : ",A.platform",
                                  platformid == 0 ? ",0 platform" : ",platform",
                                  channelIdsString);
            }
            else
            {
                sql =
                    string.Format(@"select * from( select period,StatDate {5},{2} softid,sum(case when Modulus=0 then NewUserCount-ifnull(NewUserCount_Shualiang,0) else (NewUserCount-ifnull(NewUserCount_Shualiang,0))*Modulus end) as newnum,
                                    sum(case when Modulus=0 then ActiveUserCount else ActiveUserCount*Modulus end) as activenum
                                    ,0 as lostnum ,sum(TotalUserCount) as  totalnum,
                                    0 DownValueUsersForNew,
                                    0 DownValueUsersForAct,
                                    0 FuncValueUsersForNew,
                                    0 FuncValueUsersForAct, 
                                    '{0}' as  ChannelID,'{1}' as ChannelName
                                    ,0 as NewUserCount_SecAct
                                    ,sum(case when Modulus2=0 then ifnull(NewUserCount_SecAct2,0) else ifnull(NewUserCount_SecAct2,0)*Modulus2 end) as NewUserCount_SecAct2
                                    from  U_StatChannelUsers AS A 
                                    where A.softid=?softid and  {3} A.period=?period and A.StatDate between ?begindate and ?enddate and ChannelID in ({6})
                                    Group By A.period,A.StatDate {4})A order by StatDate desc", selectchannelvalue,
                                  channeltext, softid,
                                  platformid == 0 ? " A.platform<252  and " : " A.platform=?platform and ",
                                  platformid == 0 ? "" : ",A.platform", platformid == 0 ? ",0 platform" : ",platform",
                                  channelIdsString);
            }
            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?softid"   , softid),
                    new MySqlParameter("?platform" , platformid),
                    new MySqlParameter("?period"   , (int) period),
                    new MySqlParameter("?begindate", begin.ToString("yyyyMMdd")),
                    new MySqlParameter("?enddate"  , end.ToString("yyyyMMdd")), 
                };
            List<SoftUser> lists = new List<SoftUser>();
            using (MySqlCommand cmd=new MySqlCommand(sql,new MySqlConnection(connString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                cmd.Parameters.AddRange(parameters);
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lists.Add(UserBindSjQd(dataReader, platformid, true));
                    }
                }
            }
          
            return lists;
        }

        ///绑定softuser 类对象这个是统一渠道那边的
        public SoftUser UserBindSjQd(IDataReader dataReader, int platformid, bool hasvalueuser)
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
            obj = dataReader["activenum"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ActiveNum = Convert.ToInt32(obj);
            }
            //默认值是-1，对应数据库是空值
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
            //增加channelid 和channelname 字段

            obj = dataReader["ChannelID"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ChannelID = Convert.ToInt32(obj);
            }
            obj = dataReader["ChannelName"];
            if (obj != null && obj != DBNull.Value)
            {
                model.ChannelName = obj.ToString();
            }
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


            //使用量
            model.UseNum = model.ActiveNum + model.NewNum;
            //增长率
            float frontNum = model.TotalNum - model.NewNum;
            if (frontNum <= 0)
                model.Growth = "100%";
            else
                model.Growth = (model.NewNum/frontNum*100).ToString("0.00") + "%";
            //活跃百分比和使用百分比
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
            //流失率
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


        /// <summary>
        /// 获取所有子渠道商对应不分平台的数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cateOrCustomid"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public List<SoftUser> GetAllSonChannelCustomUser(ChannelTypeOptions type, string plat, int cateOrCustomid,
                                                         DateTime begintime, DateTime endtime,
                                                         net91com.Stat.Core.PeriodOptions period)
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                string cmdText = @"drop table if exists tmpList;
                                   drop table if exists tmpList2;
                                   CREATE TEMPORARY TABLE tmpList (ID INT, RootID INT, RootName VARCHAR(100),`Level` int);
                                   CREATE TEMPORARY TABLE tmpList2 (ID INT, RootID INT, RootName VARCHAR(100),`Level` int);";
                if (type == ChannelTypeOptions.Category)
                    cmdText += string.Format(@"insert into tmpList(ID,RootID,RootName,`Level`) select ID,ID,`Name`,1 from Cfg_ChannelCustomers where CID={0} and PID=0;", cateOrCustomid);
                else if (type == ChannelTypeOptions.Customer)
                    cmdText += string.Format(@"insert into tmpList(ID,RootID,RootName,`Level`) select ID,ID,`Name`,1 from Cfg_ChannelCustomers where PID={0};", cateOrCustomid);
                else
                    return new List<SoftUser>();
                cmdText += "insert into tmpList2 select * from tmpList;";
                MySqlHelper.ExecuteNonQuery(connection, cmdText);
                int level = 1;
                while (true)
                {
                    cmdText = string.Format(@"insert into tmpList(ID,RootID,RootName,`Level`) select A.ID,B.RootID,B.RootName,{0}+1 from Cfg_ChannelCustomers A inner join tmpList2 B on A.PID=B.ID and B.Level={0};
                                              truncate table tmpList2;
                                              insert into tmpList2 select * from tmpList where `Level`={0}+1;", level);
                    level++;
                    if (MySqlHelper.ExecuteNonQuery(connection, cmdText) == 0) break;
                }
                cmdText = string.Format(@"
                                    select Y.StatDate,Y.Period,0 SoftID,0 platform,X.RootID ChannelID,X.RootName ChannelName 
                                        ,SUM(NewUserCount) newnum,SUM(ActiveUserCount) activenum
                                        ,0 NewUserCount_SecAct,SUM(ifnull(NewUserCount_SecAct2,0)) NewUserCount_SecAct2
                                        ,SUM(LostUserCount) lostnum,sum(TotalUserCount) totalnum
                                    from ( 
                                        select A.ChannelID,B.RootID,B.RootName
	                                    from Cfg_Channels A inner join tmpList B 
                                        on A.CCID=B.ID and A.ChannelID is not null) X
	                                inner join U_StatChannelUsers Y on X.ChannelID=Y.ChannelID
                                        and Y.Period={0} and Y.StatDate between {1:yyyyMMdd} and {2:yyyyMMdd} {3}  
                                    group by Y.StatDate,Y.Period,X.RootName,X.RootID
                                    order by X.RootName;
                                    drop table tmpList;
                                    drop table tmpList2;"
                                        , (int)period, begintime, endtime
                                        , plat == "0" ? "and Y.platform<252" : ("and Y.platform in (" + plat + ")"));
                List<SoftUser> users = new List<SoftUser>();
                using (IDataReader reader = MySqlHelper.ExecuteReader(connection, cmdText))
                {
                    while (reader.Read())
                    {
                        users.Add(UserBindSjQd(reader, 0, false));
                    }
                }
                return users;
            }            
        }


        /// <summary>
        /// 获取所有子渠道商对应不分平台的数据
        /// </summary>
        /// <param name="fathercustomid"></param>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="cachetime"></param>
        /// <returns></returns>
        public List<SoftUser> GetAllSonChannelCustomUserCache(ChannelTypeOptions type, string plat, int cateOrCustomid,
                                                              DateTime begintime, DateTime endtime,
                                                              net91com.Stat.Core.PeriodOptions period,
                                                              CacheTimeOption cachetime)
        {
            if (useCache == true)
            {
                string cacheKey = BuildCacheKey("GetAllSonChannelCustomUserCache", type, plat, cateOrCustomid, begintime,
                                                endtime, period, cachetime);
                if (CacheHelper.Contains(cacheKey) && cachetime != CacheTimeOption.None)
                {
                    return CacheHelper.Get<List<SoftUser>>(cacheKey).ToList();
                }
                List<SoftUser> list = GetAllSonChannelCustomUser(type, plat, cateOrCustomid, begintime, endtime, period);
                if (list != null && cachetime != CacheTimeOption.None)
                {
                    CacheHelper.Set<List<SoftUser>>(cacheKey, list, cachetime, CacheExpirationOption.AbsoluteExpiration);
                }
                return list;
            }
            else
            {
                return GetAllSonChannelCustomUser(type, plat, cateOrCustomid, begintime, endtime, period);
            }
        }
    }
}