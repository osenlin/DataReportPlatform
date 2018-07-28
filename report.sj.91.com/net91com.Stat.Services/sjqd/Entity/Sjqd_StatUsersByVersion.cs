using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using net91com.Core;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 分版本统计用户
    /// </summary>
    public class Sjqd_StatUsersByVersion
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 统计周期
        /// </summary>
        public PeriodOptions Period { get; set; }

        /// <summary>
        /// 软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 产品所属平台
        /// </summary>
        public MobileOption Platform { get; set; }

        /// <summary>
        /// 版本ID
        /// </summary>
        public int VersionID { get; set; }

        /// <summary>
        /// 版本名称
        /// </summary>
        public string VersionName { get; set; }

        /// <summary>
        /// 新增数量
        /// </summary>
        public int NewUserCount { get; set; }

        /// <summary>
        /// 活跃数量
        /// </summary>
        public int ActiveUserCount { get; set; }
        /// <summary>
        /// 使用用户
        /// </summary>
        public int UseCount { get; set; }

        /// <summary>
        /// 总用户数
        /// </summary>
        public int TotalUserCount { get; set; }

        /// <summary>
        /// 升级用户数
        /// </summary>
        public int UpdatedUserCount { get; set; }
    }
}
