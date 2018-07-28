using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 用户留存统计
    /// </summary>
    public class Sjqd_StatRetainedUsers
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 新增日期
        /// </summary>
        public DateTime OriginalDate { get; set; }

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
        public int OriginalNewUserCount { get; set; }

        /// <summary>
        /// 留存数量
        /// </summary>
        public int RetainedUserCount { get; set; }
    }
}