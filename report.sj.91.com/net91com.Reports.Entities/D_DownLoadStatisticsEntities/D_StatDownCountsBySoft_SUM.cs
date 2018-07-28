using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StatDownCountsBySoft_SUM
    {
        #region 属性

        public int SoftID { get; set; }

        public DateTime StatDate { get; set; }

        public int ResType { get; set; }

        public int Pcid { get; set; }

        public int Cid { get; set; }

        public int Platform { get; set; }

        public int SourceID { get; set; }

        public int VersionID { get; set; }

        public int ChannelID { get; set; }

        public int ResCount { get; set; }

        public string Area { get; set; }

        public int ProvinceID { get; set; }

        public int CountryID { get; set; }

        public string CountryName { get; set; }

        public string E_Version;

        public string ChannelName;

        /// <summary>
        /// 下载量
        /// </summary>
        public int DownCount { get; set; }

        public int GameDownCount { get; set; }

        /// <summary>
        /// 更新下载
        /// </summary>
        public int DownCountByUpdating { get; set; }

        /// <summary>
        /// 去除更新下载
        /// </summary>
        public int DownCountExceptUpdating { get; set; }

        /// <summary>
        /// 搜索下载数
        /// </summary>
        public int DownCountBySearching { get; set; }

        /// <summary>
        /// 静默更新
        /// </summary>
        public int DownCountBySlienceUpdating { get; set; }

        /// <summary>
        /// 一次分发
        /// </summary>
        public int DownCountExceptAllUpdating { get; set; }

        /// <summary>
        /// 一次分发新用户下载数
        /// </summary>
        public int NewUserDownCountExceptAllUpdating { get; set; }

        //一次分发总用户
        public int UserCountExceptAllUpdating { get; set; }
        //二次分发总用户
        public int UserCountUpdateing { get; set; }

        /// <summary>
        /// 所有价值用户（非静默）
        /// </summary>
        public int UserCountNoSilence { get; set; }

        /// <summary>
        /// 一般更新价值用户
        /// </summary>
        public int UserCountUpdateNoSilence { get; set; }

        /// <summary>
        /// 一般下载价值用户
        /// </summary>
        public int UserCountNoUpdateNoSearch { get; set; }
 
        /// <summary>
        /// 搜索价值用户
        /// </summary>
        public int UserCountSearch { get; set; }

        //静默分发总用户
        public int UserCountSilenceUpdateing { get; set; }
        /// <summary>
        /// 一次分发新用户数
        /// </summary>
        public int NewUserCountExceptAllUpdating { get; set; }

        #region 平均下载量字段
        /// <summary>
        /// 人均下载量
        /// </summary>
        public double AvgDown { get; set; }

        /// <summary>
        /// 新用户人均下载量
        /// </summary>
        public double AvgDownNew { get; set; }

        /// <summary>
        /// 活跃用户人均下载量
        /// </summary>
        public double AvgDownAct { get; set; }

        /// <summary>
        /// 人均一次分发
        /// </summary>
        public double AvgDownFirstAll { get; set; }

        /// <summary>
        /// 人均二次分发
        /// </summary>
        public double AvgDownSecAll { get; set; }

        /// <summary>
        /// 人均二次静默分发
        /// </summary>
        public double AvgDownSecSilence { get; set; }
        /// <summary>
        /// 新用户人均一次分发
        /// </summary>
        public double AvgDownFirstNew { get; set; }

        /// <summary>
        /// 活跃用户人均一次分发
        /// </summary>
        public double AvgDownFirstAct { get; set; } 
        #endregion

        #region 下载成功，失败字段
        /// <summary>
        /// 下载成功
        /// </summary>
        public int DownSuccessCount { get; set; }

        /// <summary>
        /// 静默下载成功
        /// </summary>
        public int DownSuccessCountBySlience { get; set; }

        /// <summary>
        /// 普通更新下载成功（部分表代表所有更新下载）
        /// </summary>
        public int DownSuccessCountByUpdateNoSlience { get; set; }

        /// <summary>
        /// 一次分发下载成功
        /// </summary>
        public int DownSuccessCountByExceptAllUpdate { get; set; }

        /// <summary>
        /// 下载失败
        /// </summary>
        public int DownFailCount { get; set; }

        /// <summary>
        /// 静默下载失败
        /// </summary>
        public int DownFailCountBySlience { get; set; }

        /// <summary>
        /// 普通更新下载失败
        /// </summary>
        public int DownFailCountByUpdateNoSlience { get; set; }

        /// <summary>
        /// 二次分发下载失败
        /// </summary>
        public int DownFailCountByExceptAllUpdate { get; set; }
        #endregion

        #region 安装情况
        /// <summary>
        /// 安装成功
        /// </summary>
        public int SetUpSuccessCount { get; set; }

        /// <summary>
        /// 静默更新安装成功
        /// </summary>
        public int SetUpSuccessCountBySlience { get; set; }

        /// <summary>
        /// 普通更新安装成功
        /// </summary>
        public int SetUpSuccessCountByUpdateNoSlience { get; set; }

        /// <summary>
        /// 安装失败
        /// </summary>
        public int SetUpFailCount { get; set; }

        /// <summary>
        /// 一个更新安装成功
        /// </summary>
        public int SetUpSuccessCountByExceptAllUpdate { get; set; }

        /// <summary>
        /// 静默更新安装失败
        /// </summary>
        public int SetUpFailCountBySlience { get; set; }

        /// <summary>
        /// 普通更新安装失败
        /// </summary>
        public int SetUpFailCountByUpdateNoSlience { get; set; }

        /// <summary>
        /// 一个更新安装失败
        /// </summary>
        public int SetUpFailCountByExceptAllUpdate { get; set; }
        #endregion

        /// <summary>
        /// 更新游戏下载
        /// </summary>
        public int GameDownCountByUpdating { get; set; }

        /// <summary>
        /// 搜索游戏下载
        /// </summary>
        public int GameDownCountBySearching { get; set; }

        public int ScheduleDownCount { get; set; }

        public int DownCountBySlience { get; set; }

        public int UserCount { get; set; }

        public int NewUserCount { get; set; }

        public int ActUserCount { get; set; }

        public int NewUserDownCount { get; set; }

        public int BrowseCount { get; set; }

        public string Country { get; set; }

        public string Province { get; set; }

        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public D_StatDownCountsBySoft_SUM(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public D_StatDownCountsBySoft_SUM()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "statdate":
                        int tempdate = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(tempdate/10000, tempdate%10000/100, tempdate%100);
                        break;
                    case "softid":
                        SoftID = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["softid"]));
                        break;
                    case "restype":
                        ResType = (reader.IsDBNull(i) ? 1 : Convert.ToInt32(reader["restype"]));
                        break;
                    case "pcid":
                        Pcid = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["pcid"]);
                        break;
                    case "cid":
                        Cid = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["cid"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["platform"]);
                        break;
                    case "sourceid":
                        SourceID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["sourceid"]);
                        break;
                    case "versionid":
                        VersionID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["versionid"]);
                        break;
                    case "e_version":
                        E_Version = reader.IsDBNull(i) ? "" : reader["e_version"].ToString();
                        break;
                    case "channelname":
                        ChannelName = reader.IsDBNull(i) ? "" : reader["channelname"].ToString();
                        break;
                    case "channelid":
                        ChannelID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["channelid"]);
                        break;
                    case "rescount":
                        ResCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["rescount"]);
                        break;
                    case "downsuccesscount":
                        DownSuccessCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downsuccesscount"]);
                        break;
                    case "downfailcount":
                        DownFailCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downfailcount"]);
                        break;
                    case "setupsuccesscount":
                        SetUpSuccessCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupsuccesscount"]);
                        break;
                    case "setupfailcount":
                        SetUpFailCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupfailcount"]);
                        break;
                    case "downcount":
                        DownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                        break;
                    case "downcountbyupdating":
                        DownCountByUpdating = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcountbyupdating"]);
                        break;
                    case "downcountbysearching":
                        DownCountBySearching = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcountbysearching"]);
                        break;
                    case "browsecount":
                        BrowseCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["browsecount"]);
                        break;
                    case "gamedowncount":
                        GameDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["gamedowncount"]);
                        break;
                    case "gamedowncountbyupdating":
                        GameDownCountByUpdating = reader.IsDBNull(i)
                                                      ? 0
                                                      : Convert.ToInt32(reader["gamedowncountbyupdating"]);
                        break;
                    case "gamedowncountbysearching":
                        GameDownCountBySearching = reader.IsDBNull(i)
                                                       ? 0
                                                       : Convert.ToInt32(reader["gamedowncountbysearching"]);
                        break;
                    case "area":
                        Area = reader.IsDBNull(i) ? "未知" : reader["area"].ToString();
                        break;
                    case "provinceid":
                        ProvinceID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["provinceid"]);
                        break;
                    case "countryid":
                        CountryID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["countryid"]);
                        break;
                    case "province":
                        Province = reader.IsDBNull(i) ? "未知" :reader["province"].ToString();
                        break;
                    case "country":
                        Country = reader.IsDBNull(i) ? "未知" : reader["country"].ToString();
                        break;
                    case "countryname":
                        CountryName = reader.IsDBNull(i)?"":reader["countryname"].ToString();
                        break;
                        
                    case "scheduledowncount":
                        ScheduleDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["scheduledowncount"]);
                        break;
                    case "downcountbyslienceupdating":
                        DownCountBySlienceUpdating = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["downcountbyslienceupdating"]);
                        break;
                    case "downcountexceptallupdating":
                        DownCountExceptAllUpdating = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["downcountexceptallupdating"]);
                        break;
                        
                    case "downcountbyslience":
                        DownCountBySlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["downcountbyslience"]);
                        break;
                    case "downsuccesscountbyslience":
                        DownSuccessCountBySlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["downsuccesscountbyslience"]);
                        break;
                    case "downfailcountbyslience":
                        DownFailCountBySlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["downfailcountbyslience"]);
                        break;
                    case "setupsuccesscountbyslience":
                        SetUpSuccessCountBySlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["setupsuccesscountbyslience"]);
                        break;
                    case "setupfailcountbyslience":
                        SetUpFailCountBySlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["setupfailcountbyslience"]);
                        break;

                    case "setupfailcountbyupdatenoslience":
                        SetUpFailCountByUpdateNoSlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["setupfailcountbyupdatenoslience"]);
                        break;
                    case "setupsuccesscountbyupdatenoslience":
                        SetUpSuccessCountByUpdateNoSlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["setupsuccesscountbyupdatenoslience"]);
                        break;
                    case "downfailcountbyupdatenoslience":
                        DownFailCountByUpdateNoSlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["downfailcountbyupdatenoslience"]);
                        break;
                    case "downsuccesscountbyupdatenoslience":
                        DownSuccessCountByUpdateNoSlience = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["downsuccesscountbyupdatenoslience"]);
                        break;
                    case "usercount":
                        UserCount = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["usercount"]);
                        break;
                    case "newusercount":
                        NewUserCount = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["newusercount"]);
                        break;
                    case "newuserdowncount":
                        NewUserDownCount = reader.IsDBNull(i)
                                                         ? 0
                                                         : Convert.ToInt32(reader["newuserdowncount"]);
                        break;
                    case "downsuccesscountbyexceptallupdate":
                        DownSuccessCountByExceptAllUpdate = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["downsuccesscountbyexceptallupdate"]);
                        break;
                    case "setupsuccesscountbyexceptallupdate":
                        SetUpSuccessCountByExceptAllUpdate = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["setupsuccesscountbyexceptallupdate"]);
                        break;
                    case "downfailcountbyexceptallupdate":
                        DownFailCountByExceptAllUpdate = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["downfailcountbyexceptallupdate"]);
                        break;
                    case "setupfailcountbyexceptallupdate":
                        SetUpFailCountByExceptAllUpdate = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["setupfailcountbyexceptallupdate"]);
                        break;
                    case "newuserdowncountexceptallupdating":
                        NewUserDownCountExceptAllUpdating = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["newuserdowncountexceptallupdating"]);
                        break;
                    case "usercountexceptallupdating":
                        UserCountExceptAllUpdating = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["usercountexceptallupdating"]);
                        break;
                    case "newusercountexceptallupdating":
                        NewUserCountExceptAllUpdating = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["newusercountexceptallupdating"]);
                        break;
                    case "usercountupdateing":
                        UserCountUpdateing = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["usercountupdateing"]);
                        break;
                    case "usercountsilenceupdateing":
                        UserCountSilenceUpdateing = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["usercountsilenceupdateing"]);
                        break;
                    case "usercountsearch":
                        UserCountSearch = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["usercountsearch"]);
                        break;
                    case "usercountnosilence":
                        UserCountNoSilence = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["usercountnosilence"]);
                        break;
                    case "usercountupdatenosilence":
                        UserCountUpdateNoSilence = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["usercountupdatenosilence"]);
                        break;
                    case "usercountnoupdatenosearch":
                        UserCountNoUpdateNoSearch = reader.IsDBNull(i)
                                                        ? 0
                                                        : Convert.ToInt32(reader["usercountnoupdatenosearch"]);
                        break;


                }
            }
            DownCountExceptUpdating = DownCount - DownCountByUpdating;
        }
    }
}
