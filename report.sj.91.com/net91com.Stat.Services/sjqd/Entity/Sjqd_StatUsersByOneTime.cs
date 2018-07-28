using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Sjqd_StatUsersByOneTime
    {
        /// <summary>
        /// 软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public int Platform { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        public int ChannelID { get; set; }

        /// <summary>
        /// 用户数目
        /// </summary>
        public int UserCount { get; set; }
    }
}