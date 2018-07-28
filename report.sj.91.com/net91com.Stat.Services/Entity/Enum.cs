using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace net91com.Stat.Services.Entity
{

    #region 状态类型

    public enum STATTypeOption
    {
        [Description("下载点击")] Download = 1,

        [Description("下载成功")] DownloadSuccess = 4,

        [Description("安装成功")] SetupSuccess = 5,

        [Description("安装失败")] SetupFail = 6,

        [Description("下载失败")] DownloadFail = 8,

        [Description("订阅下载")] DownloadFromSub = 7,

        [Description("浏览下载")] Browse = 2
    }

    #endregion

    #region  下载间隔天数枚举

    public enum DownFreqencyEnum
    {
        [Description("间隔1天")] OneDay = 1,
        [Description("间隔2-3天")] TwoToThree = 2,
        [Description("间隔4-7天")] FourToSeven = 3,
        [Description("间隔8-15天")] EightToFiF = 4,
        [Description("间隔16-29天")] SixTyToThirty = 5,
        [Description("间隔30天以上")] UpThriy = 6
    }

    #endregion

    #region 软件用龄枚举

    public enum StatUsersAgesEnum
    {
        [Description("1天至7天")] One = 1,
        [Description("8天至16天")] Two = 2,
        [Description("17天至一个月")] Three = 3,
        [Description("1个月至2个月")] Four = 4,
        [Description("2个月至6个月")] Five = 5,
        [Description("6个月至1年")] Seven = 6,
        [Description("1年至2年")] Eight = 7,
        [Description("2年以上")] Nine = 8
    }

    #endregion

    #region 软件使用天数枚举

    public enum StatUsersIntervalEnum
    {
        [Description("间隔1天")] One = 1,
        [Description("间隔2-3天")] Two = 2,
        [Description("间隔4-7天")] Three = 3,
        [Description("间隔8-15天")] Four = 4,
        [Description("间隔16-29天")] Five = 5,
        [Description("间隔30天以上")] Six = 6
    }

    #endregion

    #region 新老用户枚举

    public enum UserType
    {
        [Description("不区分用户")] NoUserType = -1,
        [Description("新用户")] NewUser = 0,
        [Description("老用户")] OldUser = 1
    }

    #endregion

    #region  下载个数枚举

    public enum DownCountMonthFreqencyEnum
    {
        [Description("下载1-5个")] One = 1,
        [Description("下载6-20个")] Two = 2,
        [Description("下载21-50个")] Three = 3,
        [Description("下载51-95个")] Four = 4,
        [Description("下载96-149个")] Five = 5,
        [Description("下载150个以上")] Six = 6
    }

    #endregion

    #region 下载

    public enum DownCountWeekFreqencyEnum
    {
        [Description("下载1-3个")] One = 1,
        [Description("下载4-10个")] Two = 2,
        [Description("下载11-25个")] Three = 3,
        [Description("下载26-45个")] Four = 4,
        [Description("下载46-69个")] Five = 5,
        [Description("下载70个以上")] Six = 6
    }

    #endregion

    #region

    public enum DownCountDayFreqencyEnum
    {
        [Description("下载1个")] One = 1,
        [Description("下载2-3个")] Two = 2,
        [Description("下载4-7个")] Three = 3,
        [Description("下载8-12个")] Four = 4,
        [Description("下载13-19个")] Five = 5,
        [Description("下载20个以上")] Six = 6
    }

    #endregion

    #region 各个报表枚举

    public enum ReportType
    {
        ///涉及到用户新增和是用报表
        UserUseNewActivity = 1,

        ///涉及到用户留存报表
        UserRetained = 2,
        //生命周期表
        UserLifecycle = 3,
        //单独新增表
        UserNew = 4,

        ///单独活跃
        UserActivity = 5,

        ///设备型号报表
        UserSbXh = 6,

        ///固件版本
        SoftGjBb = 7,

        ///软件语言分布
        SoftLan = 8,

        ///软件时长分布报表
        SoftSessionLen = 9,

        ///软件频率分布
        SoftSessionFrequeny = 10,
        //软件功能分布
        SoftFunction = 11,
        //软件用户涉及小时的表
        SoftUserForHour = 12,
        //分渠道统计
        SoftVersionSjqd = 13,
        //所用渠道商数据
        AllCustomStat = 14,
        //综合统计页面
        Default = 15,
        //资源下载
        ResDownStat = 16,
        //用户下载
        StatDownUser = 17,
        //用户下载间隔时间
        StatDownUserInterTime = 18,
        //用户下载间隔次数
        StatDownUserInterCount = 19,
        //终端分布
        StatTerminationDistribution = 20,
        //搜索关键字分布
        StatSearchKey = 21,
        //搜索次数分布
        StatSearchCount = 22,
        //PC终端分析
        StatTerminationDistributionForPC = 23,
        //装机商带量
        StatMacUsers = 24,
        //功能次数统计
        StatFunction = 25,
        //功能路径
        StatFunctionPathWay = 26,
        //软件使用时间间隔
        StatIntervalTime = 27,

        StatDownLoadSpeed = 28,

        /// <summary>
        /// 分渠道分国家
        /// </summary>
        StatUsersByAreaByChannel = 29
    }

    #endregion
}