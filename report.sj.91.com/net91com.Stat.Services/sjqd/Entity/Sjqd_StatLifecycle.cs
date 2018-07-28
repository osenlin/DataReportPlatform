using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Stat.Services.Entity;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 用户生命周期
    /// </summary>
    public class Sjqd_StatLifecycle : ICloneable
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 第几天
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// 软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 产品所属平台
        /// </summary>
        public MobileOption Platform { get; set; }

        /// <summary>
        /// 新增数量
        /// </summary>
        public int NewUserCount { get; set; }

        /// <summary>
        /// 留存数量
        /// </summary>
        public int RetainedUserCount { get; set; }

        public object Clone()
        {
            Sjqd_StatLifecycle cloneLifeCycle = new Sjqd_StatLifecycle();
            cloneLifeCycle.StatDate = this.StatDate;
            cloneLifeCycle.Days = this.Days;
            cloneLifeCycle.SoftID = this.SoftID;
            cloneLifeCycle.Platform = this.Platform;
            cloneLifeCycle.RetainedUserCount = this.RetainedUserCount;
            cloneLifeCycle.NewUserCount = this.NewUserCount;
            return cloneLifeCycle;
        }
    }
}