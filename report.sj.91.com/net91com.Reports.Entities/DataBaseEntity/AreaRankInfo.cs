using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    /// <summary>
    /// 国家或地区排行信息实体
    /// </summary>
    public class AreaRankInfo
    {
        /// <summary>
        /// 国家或地区名称
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 新增用户数
        /// </summary>
        public int NewUserCount { get; set; }

        /// <summary>
        /// 活跃用户数
        /// </summary>
        public int ActiveUserCount { get; set; }

        /// <summary>
        /// 留存用户数
        /// </summary>
        public int RetainedUserCount { get; set; }

        /// <summary>
        /// 上个周期的新增用户数
        /// </summary>
        public int LastNewUserCount { get; set; }
    }
}