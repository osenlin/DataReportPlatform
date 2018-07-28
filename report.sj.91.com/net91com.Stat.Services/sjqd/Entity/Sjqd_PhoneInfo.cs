using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 提供给sjzs外部特例类
    /// </summary>
    public class Sjqd_PhoneInfo
    {
        public int ID { get; set; }

        public string name { get; set; }

        public int userCount { get; set; }

        public decimal userPercent { get; set; }

        /// <summary>
        /// 像素
        /// </summary>
        public string pixel { get; set; }

        /// <summary>
        ///  分辨率
        /// </summary>
        public string resolution { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public int price { get; set; }

        /// <summary>
        /// 频率
        /// </summary>
        public string frequency { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
    }
}