using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 用户基础统计信息
    /// </summary>
    public class Sjqd_StatUsers
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 统计周期
        /// </summary>
        public net91com.Stat.Core.PeriodOptions Period { get; set; }

        /// <summary>
        /// 软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 产品所属平台
        /// </summary>
        public MobileOption Platform { get; set; }

        /// <summary>
        /// 新增数量
        /// </summary>
        public int NewUserCount { get; set; }

        /// <summary>
        /// 活跃数量
        /// </summary>
        public int ActiveUserCount { get; set; }

        /// <summary>
        /// 总用户数
        /// </summary>
        public int TotalUserCount { get; set; }

        /// <summary>
        /// 流失用户量
        /// </summary>
        public int LostUserCount { get; set; }

        /// <summary>
        /// 平均启动次数
        /// </summary>
        public double AvgSessions { get; set; }

        /// <summary>
        /// 平均使用时长
        /// </summary>
        public double AvgSessionLength { get; set; }

        /// <summary>
        /// 升级用户
        /// </summary>
        public int UpdatedUserCount { get; set; }
    }
}