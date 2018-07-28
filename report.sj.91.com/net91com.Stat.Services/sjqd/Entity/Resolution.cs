using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 分辨率
    /// </summary>
    public class Resolution
    {
        private int _IntStatDate;

        /// <summary>
        /// 统计时间
        /// </summary>
        public int IntStatDate
        {
            get { return _IntStatDate; }
            set { _IntStatDate = value; }
        }

        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime StatDate
        {
            get { return Convert.ToDateTime(_IntStatDate%10000 + "-" + _IntStatDate%10000/100 + "-" + _IntStatDate%100); }
        }

        private int _Period;

        /// <summary>
        /// 周期
        /// </summary>
        public int Period
        {
            get { return _Period; }
            set { _Period = value; }
        }

        private int _SoftID;

        /// <summary>
        /// 软件
        /// </summary>
        public int SoftID
        {
            get { return _SoftID; }
            set { _SoftID = value; }
        }

        private int _Platform;

        /// <summary>
        /// 平台
        /// </summary>
        public int Platform
        {
            get { return _Platform; }
            set { _Platform = value; }
        }

        private int _NewUserCount;

        /// <summary>
        /// 新增用户
        /// </summary>
        public int NewUserCount
        {
            get { return _NewUserCount; }
            set { _NewUserCount = value; }
        }

        private int _ActiveUserCount;

        /// <summary>
        /// 活跃用户
        /// </summary>
        public int ActiveUserCount
        {
            get { return _ActiveUserCount; }
            set { _ActiveUserCount = value; }
        }

        private int _UseCount;

        public int UseCount
        {
            get { return _UseCount; }
            set { _UseCount = value; }
        }

        private string _Resolution;

        /// <summary>
        /// 分辨率
        /// </summary>
        public string ResolutionStr
        {
            get { return _Resolution; }
            set { _Resolution = value; }
        }
    }
}