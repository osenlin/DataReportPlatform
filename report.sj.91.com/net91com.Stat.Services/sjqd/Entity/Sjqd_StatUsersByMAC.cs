using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Stat.Services.Entity;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 装机助手带来的量
    /// </summary>
    public class Sjqd_StatUsersByMAC
    {
        public int SoftID { get; set; }

        public MobileOption Platform { get; set; }

        public DateTime StatDate { get; set; }

        public net91com.Stat.Core.PeriodOptions Period { get; set; }

        /// <summary>
        /// Mac 地址
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// 新增量
        /// </summary>
        public int NewUserCount { get; set; }
    }
}