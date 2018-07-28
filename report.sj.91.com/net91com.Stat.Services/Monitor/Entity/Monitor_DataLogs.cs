using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.Monitor.Entity
{
    public class Monitor_DataLogs
    {
        /// <summary>
        /// 日志名称
        /// </summary>
        public string DataLogName { get; set; }

        /// <summary>
        /// 服务器ip
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime LogDate { get; set; }

        /// <summary>
        /// 日志时段
        /// </summary>
        public int LogHour { get; set; }

        /// <summary>
        /// 日志大小
        /// </summary>
        public int LogFileSize { get; set; }

        /// <summary>
        /// 真实时间
        /// </summary>
        public DateTime RealTime { get; set; }
    }
}