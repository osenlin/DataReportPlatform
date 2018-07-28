using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Sjqd_StatUsersByInterval
    {
        /// <summary>
        /// 间隔天数
        /// </summary>
        public int IntervalDays { get; set; }


        /// <summary>
        /// 该间隔占比
        /// </summary>
        public double Percent { get; set; }
    }
}