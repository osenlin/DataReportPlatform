using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using net91com.Core;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Stat.Core;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.UserRights;
using net91com.Reports.Entities;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.SjqdUserStat
{
    /// <summary>
    /// 用户统计相关的数据访问类
    /// </summary>
    public class StatUsers_DataAccess
    {
        //统计结果库连接串
        
        private string StatDB_MySQL_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        #region 获取渠道排行数据(GetRankOfChannels)

        /// <summary>
        /// 获取渠道排行
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfChannels(int softId, int platform, int channelId, int period,
                                                      DateTime statDate)
        {
            URChannelsService channelService = new URChannelsService();
            Dictionary<string, Channel> channelDict = channelId > 0
                        ? channelService.GetSubChannelDict(softId, ChannelTypeOptions.Customer, channelId, true)
                        : channelService.GetChannelDict(softId, true);

            string channelIdsString = string.Join(",", channelDict.Values.Where(a =>
                                                      (platform == (int)MobileOption.None || (int)a.Platform == platform) && a.ChannelType == ChannelTypeOptions.ChannelID)
                                                             .Select(a => a.ID.ToString())
                                                             .ToArray());
            if (string.IsNullOrEmpty(channelIdsString))
                return new List<Sjqd_StatUsers>();

            string platformWhere = platform == (int)MobileOption.None ? " and A.Platform<252" : " and A.Platform=?Platform";
            string cmdText = @"select A.ChannelID,A.NewUserCount-ifnull(A.NewUserCount_Shualiang,0) NewUserCount,A.NewUserCount+A.ActiveUserCount ActiveUserCount,ifnull(D.NewUserCount,0) LastNewUserCount
                                   ,A.TotalUserCount,ifnull(C.RetainedUserCount,0) RetainedUserCount,ifnull(D.NewUserCount,0) OriginalNewUserCount
                                from U_StatChannelUsers A
                                    left join U_StatRetainedUsers C on A.ChannelID=C.ChannelID and A.Platform=C.Platform and A.SoftID=C.SoftID and A.Period=C.Period and A.StatDate=C.StatDate and C.OriginalDate=?OriginalDate
                                    left join U_StatChannelUsers D on A.ChannelID=D.ChannelID and A.Platform=D.Platform and A.SoftID=D.SoftID and A.Period=D.Period and D.StatDate=?OriginalDate
                                where A.SoftID=?SoftID" + platformWhere + " and A.Period=?Period and A.StatDate=?StatDate and A.ChannelID in (" + channelIdsString + ")";
            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?SoftID", softId),
                    new MySqlParameter("?Platform", (int)platform),
                    new MySqlParameter("?Period", (int)period),
                    new MySqlParameter("?StatDate", statDate.ToString("yyyyMMdd")),
                    new MySqlParameter("?OriginalDate", GetLastStatDate(period, statDate).ToString("yyyyMMdd"))
                };
            List<Sjqd_StatUsers> users = new List<Sjqd_StatUsers>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    int chlId = Convert.ToInt32(reader["ChannelID"]);
                    Channel channel = channelDict[ChannelTypeOptions.ChannelID.ToString() + chlId.ToString()];
                    if (channel.ParentChannelType == ChannelTypeOptions.Category) continue;
                    string parentKey = channel.ParentChannelType.ToString() + channel.ParentID.ToString();
                    Channel parentChannel = null;
                    while (channelDict.ContainsKey(parentKey))
                    {
                        parentChannel = channelDict[parentKey];
                        if (parentChannel.ParentChannelType == ChannelTypeOptions.Category) break;
                        parentKey = parentChannel.ParentChannelType.ToString() + parentChannel.ParentID.ToString();
                    }
                    if (parentChannel != null)
                    {
                        Sjqd_StatUsers user = new Sjqd_StatUsers
                        {
                            ID = parentChannel.ID,
                            Name = parentChannel.Name,
                            ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"]),
                            NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                            TotalUserCount = Convert.ToInt32(reader["TotalUserCount"]),
                            RetainedUserCount = Convert.ToInt32(reader["RetainedUserCount"]),
                            LastNewUserCount = Convert.ToInt32(reader["LastNewUserCount"]),
                            OriginalNewUserCount = Convert.ToInt32(reader["OriginalNewUserCount"]),
                        };
                        users.Add(user);
                    }
                }
            }
            if (users.Count > 0)
            {
                double totalNewUserCount = (double)users.Sum(a => a.NewUserCount);
                double totalActiveUserCount = (double)users.Sum(a => a.ActiveUserCount);
                return (from a in users.GroupBy(a => new { a.ID, a.Name })
                        select new Sjqd_StatUsers
                        {
                            ID = a.Key.ID,
                            RetainedUserCount = a.Sum(v => v.RetainedUserCount),
                            Name = a.Key.Name,
                            ActiveUserCount = a.Sum(v => v.ActiveUserCount),
                            NewUserCount = a.Sum(v => v.NewUserCount),
                            TotalUserCount = a.Sum(v => v.TotalUserCount),
                            LastNewUserCount = a.Sum(v => v.LastNewUserCount),
                            OriginalNewUserCount = a.Sum(v => v.OriginalNewUserCount),
                            ActiveUserPercent = a.Sum(v => v.ActiveUserCount) / totalActiveUserCount,
                            NewUserPercent = a.Sum(v => v.NewUserCount) / totalNewUserCount,
                        }).ToList();
            }
            return new List<Sjqd_StatUsers>();
        }

        #endregion

        #region 获取版本排行数据(GetRankOfVersions)

        /// <summary>
        /// 获取版本排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfVersions(int softId, int platform, int channelId,
                                                      ChannelTypeOptions channelType, int period, DateTime statDate)
        {
            string cmdText =
                @"select ifnull(A.SoftVersion,'未知版本') Name,sum(A.NewUserCount) NewUserCount,sum(A.ActiveUserCount) ActiveUserCount,sum(C.OriginalNewUserCount) LastNewUserCount
                                   ,sum(C.RetainedUserCount) RetainedUserCount,sum(C.OriginalNewUserCount) OriginalNewUserCount
                                from U_StatUsersByVersion A                                  
                                    left join U_StatRetainedUsersByVersion C on A.SoftID=C.SoftID and A.Platform=C.Platform 
                                    and A.Period=C.Period and A.ChannelID=C.ChannelID and A.SoftVersion=C.SoftVersion 
                                    and A.StatDate=C.StatDate and C.OriginalDate=?OriginalDate
                                where A.SoftID=?SoftID and A.Platform=?Platform and A.Period=?Period and A.StatDate=?StatDate ";
            List<int> channelIds = GetChannelIds(softId, channelId, channelType);
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and A.ChannelID in (" + string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and A.ChannelID=-1";
            cmdText += " group by ifnull(A.SoftVersion,'未知版本')";

            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?SoftID"      , softId),
                    new MySqlParameter("?Platform"    ,  platform),
                    new MySqlParameter("?Period"      ,  period),
                    new MySqlParameter("?StatDate"    , statDate.ToString("yyyyMMdd")),
                    new MySqlParameter("?OriginalDate", GetLastStatDate(period, statDate).ToString("yyyyMMdd"))
                };
            List<Sjqd_StatUsers> users = new List<Sjqd_StatUsers>();
            using (MySqlCommand cmd = new MySqlCommand(cmdText, new MySqlConnection(StatDB_MySQL_ConnString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                cmd.Parameters.AddRange(parameters);
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Sjqd_StatUsers user = new Sjqd_StatUsers
                        {

                            Name = reader["Name"].ToString(),
                            ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"]),
                            NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                            RetainedUserCount =
                                reader["RetainedUserCount"] == DBNull.Value || reader["RetainedUserCount"] == null
                                    ? 0
                                    : Convert.ToInt32(reader["RetainedUserCount"]),
                            LastNewUserCount =
                                reader["LastNewUserCount"] == DBNull.Value || reader["LastNewUserCount"] == null
                                    ? 0
                                    : Convert.ToInt32(reader["LastNewUserCount"]),
                            OriginalNewUserCount =
                                reader["OriginalNewUserCount"] == DBNull.Value || reader["OriginalNewUserCount"] == null
                                    ? 0
                                    : Convert.ToInt32(reader["OriginalNewUserCount"]),
                        };
                        users.Add(user);
                    }
                }
            }

            if (users.Count > 0)
            {
                double totalNewUserCount = (double) users.Sum(a => a.NewUserCount);
                double totalActiveUserCount = (double) users.Sum(a => a.ActiveUserCount);
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].ActiveUserPercent = users[i].ActiveUserCount/totalActiveUserCount;
                    users[i].NewUserPercent = users[i].NewUserCount/totalNewUserCount;
                }
            }
            return users;
        }

        #endregion

        #region 获取地区排行数据(GetRankOfAreas)

        /// <summary>
        /// 获取国家排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfCountries(int softId, int platform, ChannelTypeOptions channelType,
                                                       int channelId, int period, DateTime startDate, DateTime endDate)
        {
            return GetRankOfAreas(softId, platform, 0, channelType, channelId, period, startDate, endDate);
        }

        /// <summary>
        /// 获取省排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfProvinces(int softId, int platform, ChannelTypeOptions channelType,
                                                       int channelId, int period, DateTime startDate, DateTime endDate)
        {
            return GetRankOfAreas(softId, platform, 1, channelType, channelId, period, startDate, endDate);
        }

        /// <summary>
        /// 获取市排行数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetRankOfCities(int softId, int platform, ChannelTypeOptions channelType,
                                                    int channelId, int period, DateTime startDate, DateTime endDate)
        {
            return GetRankOfAreas(softId, platform, 2, channelType, channelId, period, startDate, endDate);
        }

        /// <summary>
        /// 获取地区排行数据
        /// </summary>
        private List<Sjqd_StatUsers> GetRankOfAreas(int softId, int platform, int areaType,
                                                    ChannelTypeOptions channelType, int channelId, int period,
                                                    DateTime startDate, DateTime endDate)
        {
            string cmdText = @"select B.EnShortName,B.Name ,sum(A.NewUserCount) NewUserCount,sum(A.ActiveUserCount) ActiveUserCount
                                    ,sum(C.OriginalNewUserCount) LastNewUserCount,sum(C.RetainedUserCount) RetainedUserCount
                                 from U_StatUsersByArea A 
                                    inner join Cfg_Areas B on A.Area=B.EnShortName and B.EnShortName is not null and B.EnShortName<>'' and A.SoftID=?SoftID 
                                         and A.Period=?Period and A.StatDate between ?StartDate and ?EndDate and B.ParentID=0"
                             + (platform <= 0 ? " and A.Platform<252" : " and A.Platform=?Platform");
            List<int> channelIds = GetChannelIds(softId, channelId, channelType);
            cmdText += channelIds != null ? (channelIds.Count > 0
                                  ? " and A.ChannelID in (" + string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and A.ChannelID=-1";
            cmdText += @" left join U_StatRetainedUsersByArea C on A.SoftID=C.SoftID and A.Platform=C.Platform and A.Period=C.Period and A.ChannelID=C.ChannelID and A.Area=C.Area and A.StatDate=C.StatDate 
                         and C.OriginalDate between ?OriginalStartDate and ?OriginalEndDate and C.Period=?Period and C.SoftID=?SoftID and C.Platform=?Platform";
            switch ((PeriodOptions)period)
            {
                case PeriodOptions.Weekly:
                    cmdText += " and DATEDIFF(STR_TO_DATE(CAST(C.StatDate AS CHAR(8)),'%Y%m%d'),STR_TO_DATE(CAST(C.OriginalDate AS CHAR(8)),'%Y%m%d'))=7 ";
                    break;
                case PeriodOptions.NaturalMonth:
                    cmdText += " and DATEDIFF(STR_TO_DATE(CAST(C.StatDate AS CHAR(8)),'%Y%m%d'),STR_TO_DATE(CAST(C.OriginalDate AS CHAR(8)),'%Y%m%d')) between 28 and 31 ";
                    break;
                default: // PeriodOptions.Daily:
                    cmdText += " and DATEDIFF(STR_TO_DATE(CAST(C.StatDate AS CHAR(8)),'%Y%m%d'),STR_TO_DATE(CAST(C.OriginalDate AS CHAR(8)),'%Y%m%d'))=1 ";
                    break;
            }
            cmdText += " group by B.ID,B.Name";
            var parameters = new []
                {
                    new MySqlParameter("?SoftID"  , softId),
                    new MySqlParameter("?Platform", platform),
                    new MySqlParameter("?Period"  , period),
                    new MySqlParameter("?StartDate",startDate.ToString("yyyyMMdd")),
                    new MySqlParameter("?EndDate",  endDate.ToString("yyyyMMdd")),
                    new MySqlParameter("?OriginalStartDate", GetLastStatDate(period, startDate).ToString("yyyyMMdd")),
                    new MySqlParameter("?OriginalEndDate", GetLastStatDate(period, endDate).ToString("yyyyMMdd"))
                };
            List<Sjqd_StatUsers> users = new List<Sjqd_StatUsers>();
            using (MySqlCommand cmd = new MySqlCommand(cmdText,new MySqlConnection(StatDB_MySQL_ConnString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                cmd.Parameters.AddRange(parameters);
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Sjqd_StatUsers user = new Sjqd_StatUsers
                        {
                            IdName = reader["EnShortName"].ToString(),
                            Name = reader["Name"].ToString(),
                            ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"]),
                            NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                            RetainedUserCount = reader["RetainedUserCount"] == DBNull.Value || reader["RetainedUserCount"] == null ? 0 : Convert.ToInt32(reader["RetainedUserCount"]),
                            LastNewUserCount = reader["LastNewUserCount"] == DBNull.Value || reader["LastNewUserCount"] == null ? 0 : Convert.ToInt32(reader["LastNewUserCount"]),
                        };
                        users.Add(user);
                    }
                }
            }

            if (users.Count > 0)
            {
                double totalNewUserCount = (double) users.Sum(a => a.NewUserCount);
                double totalActiveUserCount = (double) users.Sum(a => a.ActiveUserCount);
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].ActiveUserPercent = users[i].ActiveUserCount/totalActiveUserCount;
                    users[i].NewUserPercent = users[i].NewUserCount/totalNewUserCount;
                }
            }
            return users;
        }

        #endregion

        #region 获取分渠道用户数据(GetStatUsersByChannel)

        /// <summary>
        /// 获取分渠道用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelIds"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="forPartner">是否提供给合作方</param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByChannel(int softId, int platform, List<int> channelIds, int period,
                                                          DateTime beginDate, DateTime endDate, bool forPartner)
        {
            string cmdText = "select StatDate";
            cmdText += forPartner
                           ? @",sum(case when Modulus=0 then NewUserCount-ifnull(NewUserCount_Shualiang,0) else (NewUserCount-ifnull(NewUserCount_Shualiang,0))*Modulus end) NewUserCount
                    ,sum(case when Modulus=0 then ActiveUserCount else ActiveUserCount*Modulus end) ActiveUserCount
                    ,sum(case when Modulus2=0 then ifnull(NewUserCount_SecAct,0) else ifnull(NewUserCount_SecAct,0)*Modulus2 end) as NewUserCount_SecAct
                    ,sum(case when Modulus2=0 then ifnull(NewUserCount_SecAct2,0) else ifnull(NewUserCount_SecAct2,0)*Modulus2 end) as NewUserCount_SecAct2"
                           : @",sum(NewUserCount-ifnull(NewUserCount_Shualiang,0)) NewUserCount
                    ,sum(ActiveUserCount) ActiveUserCount
                    ,sum(ifnull(NewUserCount_SecAct,0)) NewUserCount_SecAct
                    ,sum(ifnull(NewUserCount_SecAct2,0)) NewUserCount_SecAct2";
            cmdText += string.Format(@",0 NewUserCount_Shanzhai
                        ,0 ActiveUserCount_Shanzhai
                        ,sum(TotalUserCount) TotalUserCount
                        ,0 TotalUserCount_Shanzhai
                        ,0 DownNewUserCount
                        ,0 DownActiveUserCount
                    from U_StatChannelUsers 
                    where SoftID={0} and Period={1} and StatDate between {2:yyyyMMdd} and {3:yyyyMMdd} ", softId, period,
                                     beginDate, endDate);
            cmdText += platform > 0 ? "and Platform=" + platform.ToString() : "and Platform<252";
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : "";
            cmdText += " group by StatDate order by StatDate desc";
            List<Sjqd_StatUsers> statUsers = new List<Sjqd_StatUsers>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, cmdText))
            {
                while (reader.Read())
                {
                    int statDate = Convert.ToInt32(reader["StatDate"]);
                    statUsers.Add(
                        new Sjqd_StatUsers
                            {
                                StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                                NewUserCount_SecAct = Convert.ToInt32(reader["NewUserCount_SecAct"]),
                                NewUserCount_SecAct2 = Convert.ToInt32(reader["NewUserCount_SecAct2"]),
                                ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"]) + Convert.ToInt32(reader["NewUserCount"]),
                                TotalUserCount = Convert.ToInt32(reader["TotalUserCount"])
                            });
                }
            }
            return statUsers;
        }

        #endregion

        #region 获取分小时用户数据(GetStatUsersByHour)

        /// <summary>
        /// 获取分小时用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelIds"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByHour(int softId, int platform, List<int> channelIds, int period,
                                                       DateTime beginDate, DateTime endDate)
        {
            string cmdText =
                string.Format(@"select StatHour{0},sum(NewUserCount) NewUserCount,sum(ActiveUserCount) ActiveUserCount
                                            from U_StatUsersByHour 
                                            where SoftID={1} and StatDate between {2} and {3} {4} {5}
                                            group by {6}StatHour order by {6}StatHour",
                              period == (int) PeriodOptions.TimeOfDay ? ",20000101 StatDate" : ",StatDate"
                              , softId
                              , beginDate.ToString("yyyyMMdd")
                              , endDate.ToString("yyyyMMdd")
                              , platform > 0 ? "and Platform=" + platform : ""
                              ,
                              channelIds != null
                                  ? (channelIds.Count > 0
                                         ? " and ChannelID in (" +
                                           string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                         : " and 1=0")
                                  : " and ChannelID=-1"
                              , period == (int) PeriodOptions.TimeOfDay ? "" : "StatDate,");
            List<Sjqd_StatUsers> statUsers = new List<Sjqd_StatUsers>();
            using (MySqlCommand com = new MySqlCommand(cmdText, new MySqlConnection(StatDB_MySQL_ConnString)))
            {
                com.Connection.Open();
                com.CommandTimeout = 120;
                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int statDate = Convert.ToInt32(reader["StatDate"]);
                        Sjqd_StatUsers user = new Sjqd_StatUsers
                            {
                                StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                StatHour = Convert.ToInt32(reader["StatHour"]),
                                NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                                ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"])
                            };
                        user.StatDate = user.StatDate.AddHours(user.StatHour);
                        statUsers.Add(user);
                    }
                }
            }

            return statUsers;
        }

        #endregion

        #region 获取分地区用户数据(GetStatUsersByArea)

        /// <summary>
        /// 获取分地区用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelIds"></param>
        /// <param name="enshortname"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="forPartner"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByArea(int softId, int platform, List<int> channelIds, string enshortname,
                                                       int period, DateTime beginDate, DateTime endDate, bool forPartner)
        {
            B_Basic.B_BaseTool_DataAccess bt = new B_BaseTool_DataAccess();
            string cmdText = "select A.StatDate";
            cmdText += forPartner
                           ? @",sum(case when Modulus=0 then A.NewUserCount else A.NewUserCount*Modulus end) NewUserCount
                    ,sum(case when Modulus=0 then A.ActiveUserCount else A.ActiveUserCount*Modulus end) ActiveUserCount"
                           : @",sum(A.NewUserCount) NewUserCount,sum(A.ActiveUserCount) ActiveUserCount";
            cmdText += string.Format(@" from U_StatUsersByArea A
                        where A.SoftID={0} {1} and A.Period={2} and A.StatDate between {3:yyyyMMdd} and {4:yyyyMMdd} and A.Area='{5}'",
                                     softId,
                                     platform > 0 ? " and A.Platform=" + platform.ToString() : " and A.Platform<252",
                                     period, beginDate, endDate, enshortname);
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and A.ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and A.ChannelID=-1";
            cmdText += " group by A.StatDate";
            List<Sjqd_StatUsers> statUsers = new List<Sjqd_StatUsers>();
            using (MySqlCommand cmd = new MySqlCommand(cmdText, new MySqlConnection(StatDB_MySQL_ConnString)))
            {
                cmd.CommandTimeout = 120;
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int statDate = Convert.ToInt32(reader["StatDate"]);
                        statUsers.Add(
                            new Sjqd_StatUsers
                                {
                                    StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                    NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                                    ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"])                                   
                                });
                    }
                }
            }

            return statUsers;
        }

        #endregion

        #region 获取分版本用户数据(GetStatUsersByVersion)

        /// <summary>
        /// 获取分版本用户数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelIds"></param>
        /// <param name="versionId"></param>
        /// <param name="period"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatUsers> GetStatUsersByVersion(int softId, int platform, List<int> channelIds, string versionId,
                                                          int period, DateTime beginDate, DateTime endDate)
        {
            B_BaseTool_DataAccess bt=new B_BaseTool_DataAccess();
            string cmdText =
                string.Format(
                    @"select StatDate,sum(NewUserCount) NewUserCount,sum(ActiveUserCount) ActiveUserCount,sum(UpdatedUserCount) UpdatedUserCount
                            from U_StatUsersByVersion 
                              where SoftID={0} and Platform={1} and Period={2} and StatDate between {3:yyyyMMdd} and {4:yyyyMMdd} and SoftVersion='{5}'",
                    softId, (int)platform, period, beginDate, endDate, versionId);
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and ChannelID=-1";
            cmdText += " group by StatDate";
            List<Sjqd_StatUsers> statUsers = new List<Sjqd_StatUsers>();

            using (MySqlCommand cmd=new MySqlCommand(cmdText,new MySqlConnection(StatDB_MySQL_ConnString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 180;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int statDate = Convert.ToInt32(reader["StatDate"]);
                        statUsers.Add(
                            new Sjqd_StatUsers
                            {
                                StatDate = new DateTime(statDate / 10000, statDate / 100 % 100, statDate % 100),
                                NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                                ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"]),
                                UpdatedUserCount = Convert.ToInt32(reader["UpdatedUserCount"])
                            });
                    }
                }

            }

            return statUsers;
        }

        #endregion

        #region 获取分国家分版本(海外)用户数据(GetStatUsersByCountryByVersionEn)

        /// <summary>
        /// 获取分国家分版本(海外)用户数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<Sjqd_StatUsers>> GetStatUsersByCountryByVersionEn(int softId, int platform,
                                                                                         string versionIds,
                                                                                         string countryIds,
                                                                                         DateTime beginDate,
                                                                                         DateTime endDate)
        {
            string cmdText =
                string.Format(@"select StatDate,SoftVersion ,Area,sum(NewUserCount) NewUserCount,sum(ActiveUserCount) ActiveUserCount 
                                from U_StatUsersByAreaByVersion 
                                where SoftID={0} and Platform={1} and StatDate between {2:yyyyMMdd} and {3:yyyyMMdd} and SoftVersion in ({4}) and Area in ({5})
                                group by StatDate,SoftVersion,Area
                                order by StatDate", softId, (int) platform, beginDate, endDate, versionIds, countryIds);
            Dictionary<string, List<Sjqd_StatUsers>> result = new Dictionary<string, List<Sjqd_StatUsers>>();
            using (var reader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, cmdText))
            {
                while (reader.Read())
                {
                    string key = string.Format("{0}-{1}", reader["SoftVersion"], reader["Area"]);
                    if (!result.ContainsKey(key))
                        result.Add(key, new List<Sjqd_StatUsers>());
                    List<Sjqd_StatUsers> statUsers = result[key];
                    int statDate = Convert.ToInt32(reader["StatDate"]);
                    statUsers.Add(
                        new Sjqd_StatUsers
                            {
                                StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                NewUserCount = Convert.ToInt32(reader["NewUserCount"]),
                                ActiveUserCount = Convert.ToInt32(reader["ActiveUserCount"])
                            });
                }
            }
            return result;
        }

        #endregion

        #region 获取新增用户留存率数据(GetStatRetainedUsers)

        /// <summary>
        /// 获取新增用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelIds"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedUsers(int softId, int platform, List<int> channelIds,
                                                                 int period, DateTime fromDate, DateTime toDate)
        {
            if (period == (int) net91com.Stat.Core.PeriodOptions.Daily)
            {
                if ((toDate - fromDate).Days > 31)
                {
                    fromDate = toDate.AddDays(-31);
                }
            }

            string cmdText = string.Format(@"
            select 
            StatDate,OriginalDate,
            SUM(OriginalNewUserCount) OriginalNewUserCount,SUM(RetainedUserCount) RetainedUserCount 
            from U_StatRetainedUsers
            where SoftID={0} and Period={1} and OriginalDate between {2} and {3}", softId, period,
                                           fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"));

            cmdText += platform > 0 ? string.Format(" and Platform={0} ", platform) : " ";
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and ChannelID=-1";
            cmdText += " group by OriginalDate,StatDate order by OriginalDate desc,StatDate desc";


            List<Sjqd_StatRetainedUsers> retainedUsers = new List<Sjqd_StatRetainedUsers>();
            using (IDataReader reader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, cmdText))
            {
                while (reader.Read())
                {
                    int statDate = Convert.ToInt32(reader["StatDate"]);
                    int originalDate = Convert.ToInt32(reader["OriginalDate"]);
                    retainedUsers.Add(
                        new Sjqd_StatRetainedUsers
                            {
                                OriginalDate = new DateTime(originalDate/10000, originalDate/100%100, originalDate%100),
                                StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                OriginalNewUserCount = Convert.ToInt32(reader["OriginalNewUserCount"]),
                                RetainedUserCount = Convert.ToInt32(reader["RetainedUserCount"])
                            });
                }
            }
            return retainedUsers;
        }

        #endregion

        #region 获取活跃用户留存率数据(GetStatRetainedActiveUsers, MySQL)

        /// <summary>
        /// 获取活跃用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="channelIds"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedActiveUsers(int softId, int platform, List<int> channelIds,
                                                                       DateTime fromDate, DateTime toDate)
        {
            if ((toDate - fromDate).Days > 31)
                fromDate = toDate.AddDays(-31);

            string cmdText = string.Format(
                @"  select StatDate,OriginalDate,SUM(OriginalUserCount) OriginalUserCount,SUM(RetainedUserCount) RetainedUserCount 
                        from U_StatRetainedActiveUsers 
                        where SoftID={0} and OriginalDate between {1:yyyyMMdd} and {2:yyyyMMdd}", softId, fromDate,
                toDate);
            cmdText += platform > 0 ? " and Platform=" + platform.ToString() : " ";
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and ChannelID=-1";
            cmdText += " group by OriginalDate,StatDate order by OriginalDate desc,StatDate desc";
            List<Sjqd_StatRetainedUsers> retainedUsers = new List<Sjqd_StatRetainedUsers>();
            using (MySqlConnection statDbConn = new MySqlConnection(StatDB_MySQL_ConnString))
            {
                statDbConn.Open();
                MySqlCommand command = new MySqlCommand(cmdText, statDbConn);
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int statDate = Convert.ToInt32(reader["StatDate"]);
                        int originalDate = Convert.ToInt32(reader["OriginalDate"]);
                        retainedUsers.Add(
                            new Sjqd_StatRetainedUsers
                                {
                                    OriginalDate =
                                        new DateTime(originalDate/10000, originalDate/100%100, originalDate%100),
                                    StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                    OriginalNewUserCount = Convert.ToInt32(reader["OriginalUserCount"]),
                                    RetainedUserCount = Convert.ToInt32(reader["RetainedUserCount"])
                                });
                    }
                }
            }
            return retainedUsers;
        }

        #endregion

        #region 获取分地区新增用户留存率数据(GetStatRetainedUsersByArea)

        /// <summary>
        /// 获取分地区新增用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="areaId"></param>
        /// <param name="channelIds"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedUsersByArea(int softId, int platform, string areaId,
                                                                       List<int> channelIds, int period,
                                                                       DateTime fromDate, DateTime toDate)
        {
            B_BaseTool_DataAccess bt = new B_BaseTool_DataAccess();
            if (period == (int) net91com.Stat.Core.PeriodOptions.Daily)
            {
                if ((toDate - fromDate).Days > 31)
                {
                    fromDate = toDate.AddDays(-31);
                }
            }

            string cmdText = string.Format(@"select StatDate,OriginalDate,
            SUM(OriginalNewUserCount) OriginalNewUserCount,SUM(RetainedUserCount) RetainedUserCount 
                               from U_StatRetainedUsersByArea 
                               where SoftID={0}  and Area='{1}' and Period={2} and OriginalDate between {3} and {4}"
                                           , softId, areaId, period,
                                           fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"));
            cmdText += platform > 0 ? string.Format(" and Platform={0}", platform) : " and Platform<252";
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and ChannelID=-1";
            cmdText += " group by OriginalDate,StatDate order by OriginalDate desc,StatDate desc";

            List<Sjqd_StatRetainedUsers> retainedUsers = new List<Sjqd_StatRetainedUsers>();
            using (MySqlCommand cmd = new MySqlCommand(cmdText, new MySqlConnection(StatDB_MySQL_ConnString)))
            {
                cmd.Connection.Open();
                cmd.CommandTimeout = 300;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int statDate = Convert.ToInt32(reader["StatDate"]);
                        int originalDate = Convert.ToInt32(reader["OriginalDate"]);
                        retainedUsers.Add(
                            new Sjqd_StatRetainedUsers
                                {
                                    OriginalDate =
                                        new DateTime(originalDate/10000, originalDate/100%100, originalDate%100),
                                    StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                    OriginalNewUserCount = Convert.ToInt32(reader["OriginalNewUserCount"]),
                                    RetainedUserCount = Convert.ToInt32(reader["RetainedUserCount"])
                                });
                    }
                }
            }

            return retainedUsers;
        }

        #endregion

        #region 获取分地区活跃用户留存率数据(GetStatRetainedActiveUsersByArea)

        /// <summary>
        /// 获取分地区活跃用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="areaId"></param>
        /// <param name="channelIds"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedActiveUsersByArea(int softId, int platform, string areaId,
                                                                             List<int> channelIds, DateTime fromDate,
                                                                             DateTime toDate)
        {
            if ((toDate - fromDate).Days > 31)
                fromDate = toDate.AddDays(-31);

            string cmdText = string.Format(
                @"  select StatDate,OriginalDate,SUM(OriginalUserCount) OriginalUserCount,SUM(RetainedUserCount) RetainedUserCount 
                        from U_StatRetainedActiveUsersByArea
                        where SoftID={0} and OriginalDate between {1:yyyyMMdd} and {2:yyyyMMdd} and Area='{3}'", softId,
                fromDate, toDate, areaId);
            cmdText += platform > 0 ? " and Platform=" + platform.ToString() : " and Platform<252";
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and ChannelID=-1";
            cmdText += " group by OriginalDate,StatDate order by OriginalDate desc,StatDate desc";
            List<Sjqd_StatRetainedUsers> retainedUsers = new List<Sjqd_StatRetainedUsers>();
            using (MySqlConnection statDbConn = new MySqlConnection(StatDB_MySQL_ConnString))
            {
                statDbConn.Open();
                MySqlCommand command = new MySqlCommand(cmdText, statDbConn);
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int statDate = Convert.ToInt32(reader["StatDate"]);
                        int originalDate = Convert.ToInt32(reader["OriginalDate"]);
                        retainedUsers.Add(
                            new Sjqd_StatRetainedUsers
                                {
                                    OriginalDate =
                                        new DateTime(originalDate/10000, originalDate/100%100, originalDate%100),
                                    StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                    OriginalNewUserCount = Convert.ToInt32(reader["OriginalUserCount"]),
                                    RetainedUserCount = Convert.ToInt32(reader["RetainedUserCount"])
                                });
                    }
                }
            }
            return retainedUsers;
        }

        #endregion

        #region 获取分版本新增用户留存率数据(GetStatRetainedUsersByVersion)

        /// <summary>
        /// 获取分版本新增用户留存率数据
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platform"></param>
        /// <param name="versionId"></param>
        /// <param name="channelIds"></param>
        /// <param name="period"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Sjqd_StatRetainedUsers> GetStatRetainedUsersByVersion(int softId, int platform, string versionId,
                                                                          List<int> channelIds, int period,
                                                                          DateTime fromDate, DateTime toDate)
        {
            if (period == (int) PeriodOptions.Daily)
            {
                if ((toDate - fromDate).Days > 31)
                {
                    fromDate = toDate.AddDays(-31);
                }
            }
            B_BaseTool_DataAccess bt = new B_BaseTool_DataAccess();
            string cmdText =
                string.Format(@"select StatDate,OriginalDate,SUM(OriginalNewUserCount) OriginalNewUserCount,SUM(RetainedUserCount) RetainedUserCount 
                               from U_StatRetainedUsersByVersion 
                               where SoftID={0} and Platform={1} and SoftVersion='{2}'
                                   and Period={3} and OriginalDate between {4} and {5}",
                              softId, platform, versionId, period,
                              fromDate.ToString("yyyyMMdd"), toDate.ToString("yyyyMMdd"));
            cmdText += channelIds != null
                           ? (channelIds.Count > 0
                                  ? " and ChannelID in (" +
                                    string.Join(",", channelIds.Select(a => a.ToString()).ToArray()) + ")"
                                  : " and 1=0")
                           : " and ChannelID=-1";
            cmdText += " group by OriginalDate,StatDate order by OriginalDate desc,StatDate desc";


            List<Sjqd_StatRetainedUsers> retainedUsers = new List<Sjqd_StatRetainedUsers>();
            using (IDataReader reader = MySqlHelper.ExecuteReader(StatDB_MySQL_ConnString, cmdText))
            {
                while (reader.Read())
                {
                    int statDate = Convert.ToInt32(reader["StatDate"]);
                    int originalDate = Convert.ToInt32(reader["OriginalDate"]);
                    retainedUsers.Add(
                        new Sjqd_StatRetainedUsers
                            {
                                OriginalDate = new DateTime(originalDate/10000, originalDate/100%100, originalDate%100),
                                StatDate = new DateTime(statDate/10000, statDate/100%100, statDate%100),
                                OriginalNewUserCount = Convert.ToInt32(reader["OriginalNewUserCount"]),
                                RetainedUserCount = Convert.ToInt32(reader["RetainedUserCount"])
                            });
                }
            }
            return retainedUsers;
        }

        #endregion

        #region 获取排行最适配日期(GetDateOfRank)

        /// <summary>
        /// 获取排行最适配日期
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public DateTime GetDateOfRank(int softId, int period, DateTime statDate, string method)
        {
            string cmdText =
                string.Format(
                    "select max(StatDate) from {{0}}  where SoftID={0} and Period={1} and StatDate<={2}",
                    softId, period, statDate.ToString("yyyyMMdd"));
            switch (method)
            {
                case "GetRankOfChannels":
                    cmdText = string.Format(cmdText, "U_StatChannelUsers");
                    break;
                case "GetRankOfVersions":
                    cmdText = string.Format(cmdText, "U_StatUsersByVersion");
                    break;
                case "GetRankOfCities":
                case "GetRankOfCountries":
                case "GetRankOfProvinces":
                    cmdText = string.Format(cmdText, "U_StatUsersByArea");
                    break;
            }
            object result = MySqlHelper.ExecuteScalar(StatDB_MySQL_ConnString, cmdText);
            if (result != null && result != DBNull.Value)
            {
                int d = Convert.ToInt32(result);
                return new DateTime(d/10000, d/100%100, d%100);
            }
            return statDate;
        }

        #endregion

        #region 基础方法

        /// <summary>
        /// 获取上个周期的日期
        /// </summary>
        /// <param name="period"></param>
        /// <param name="statDate"></param>
        /// <returns></returns>
        private DateTime GetLastStatDate(int period, DateTime statDate)
        {
            DateTime lastStatDate;
            switch ((PeriodOptions) period)
            {
                case PeriodOptions.Daily:
                    lastStatDate = statDate.AddDays(-1);
                    break;
                case PeriodOptions.Weekly:
                    lastStatDate = statDate.AddDays(-7);
                    break;
                case PeriodOptions.Monthly:
                    lastStatDate = statDate.AddMonths(-1);
                    break;
                case PeriodOptions.NaturalMonth:
                    lastStatDate = statDate.AddDays(1).AddMonths(-1).AddDays(-1);
                    break;
                default:
                    lastStatDate = DateTime.MinValue;
                    break;
            }
            return lastStatDate;
        }

        /// <summary>
        /// /获取所有的渠道ID
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelId"></param>
        /// <param name="channelType"></param>
        /// <returns></returns>
        private List<int> GetChannelIds(int softId, int channelId, ChannelTypeOptions channelType)
        {
            URChannelsService channelService = new URChannelsService();
            List<int> channelIds = channelId > 0
                                       ? channelService.GetChannelIds(softId, channelType, new int[] {channelId})
                                       : null;
            return channelIds;
        }

        #endregion
    }
}