using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 分固件版本统计
    /// </summary>
    public class Sjqd_StatUsersByGjbb
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
        /// 固件版本ID
        /// </summary>
        public int GjbbID { get; set; }

        /// <summary>
        /// 固件版本
        /// </summary>
        public string Gjbb { get; set; }

        ///// <summary>
        ///// 新增数量
        ///// </summary>
        //public int NewUserCount { get; set; }

        ///// <summary>
        ///// 活跃数量
        ///// </summary>
        //public int ActiveUserCount { get; set; }
        /// <summary>
        /// 使用用户数
        /// </summary>
        public int UseCount { get; set; }
    }
}