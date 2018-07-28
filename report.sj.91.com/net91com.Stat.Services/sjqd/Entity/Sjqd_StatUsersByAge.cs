using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Sjqd_StatUsersByAge
    {
        /// <summary>
        /// 使用间隔时长
        /// </summary>
        public int AgeDays { get; set; }


        /// <summary>
        /// 该占比
        /// </summary>
        public double Percent { get; set; }
    }
}