using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Sjqd_MileStoneConfig
    {
        public int ID { get; set; }

        public int SoftID { get; set; }

        /// <summary>
        /// 软件名称
        /// </summary>
        public string SoftName { get; set; }

        public MobileOption Platform { get; set; }

        /// <summary>
        /// 里程碑事件时间
        /// </summary>
        public DateTime MileStoneDate { get; set; }

        /// <summary>
        /// 标记说明
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 版本名称
        /// </summary>
        public string VersionName { get; set; }
    }
}