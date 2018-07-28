using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Stat.Services.Entity;
using net91com.Core;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class UserOptionEntityBase
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 周期
        /// </summary>
        public net91com.Stat.Core.PeriodOptions Period { get; set; }

        /// <summary>
        /// 软件
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public MobileOption Platform { get; set; }

        /// <summary>
        /// 对应版本
        /// </summary>
        public int Version { get; set; }
    }

    /// <summary>
    /// 回话时长统计
    /// </summary>
    public class Sjqd_ULSessionLength
    {
        public int SessionLengthLevel { get; set; }

        /// <summary>
        /// 对应时长档次的人数概率
        /// </summary>
        public decimal Percent { get; set; }
    }

    /// <summary>
    /// 启动次数统计
    /// </summary>
    public class Sjqd_ULSessionsCount
    {
        /// <summary>
        /// 使用次数/天数
        /// </summary>
        public int SessionsLevel { get; set; }

        /// <summary>
        /// 用户数比例
        /// </summary>
        public decimal Percent { get; set; }
    }

    /// <summary>
    /// 单次时长统计
    /// </summary>
    public class Sjqd_ULSessionsSingle
    {
        /// <summary>
        /// 时长档次
        /// </summary>
        public int SessionLengthLevel { get; set; }

        /// <summary>
        /// 用户数比例
        /// </summary>
        public decimal Percent { get; set; }
    }

    public class Sjqd_ULSessionAvgUsers : UserOptionEntityBase
    {
        /// <summary>
        /// 当天使用人数
        /// </summary>
        public int UseUsers { get; set; }

        /// <summary>
        /// 平均每个用户启动次数
        /// </summary>
        public decimal AvgSessions { get; set; }

        /// <summary>
        /// 平均每个用户使用时长
        /// </summary>
        public decimal AvgSessionLength { get; set; }

        /// <summary>
        /// 累计启动次数
        /// </summary>
        public int AllSessions { get; set; }

        ///累计使用时长
        public long AllSessionLength { get; set; }

        /// <summary>
        /// 平均每次使用时长
        /// </summary>
        public decimal AvgLengthPerSession { get; set; }
    }
}