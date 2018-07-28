using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class UserStatAll
    {
        /// <summary>
        /// 功能统计那边使用的id
        /// </summary>
        public int SoftOutID { get; set; }

        /// <summary>
        /// 标准id
        /// </summary>
        public int SoftId { get; set; }

        /// <summary>
        /// 软件名称
        /// </summary>
        public string SoftName { get; set; }

        /// <summary>
        /// 软件平台
        /// </summary>
        public int Platform { get; set; }

        /// <summary>
        ///新用户数
        /// </summary>
        public int DayNewNum { get; set; }

        /// <summary>
        /// 活跃用户
        /// </summary>
        public int DayActiveNum { get; set; }

        /// <summary>
        /// 启动次数（） 
        /// </summary>
        public double DayAvgStartNum { get; set; }

        /// <summary>
        /// 使用时长
        /// </summary>
        public double DayAvgSessionLength { get; set; }

        /// <summary>
        /// 总用户数
        /// </summary>
        public int AllUserNum { get; set; }

        ///总的session 次数
        public int AllSessions { get; set; }

        /// <summary>
        /// 所有时长
        /// </summary>
        public int AllSessionsLength { get; set; }

        public int OneTimeUsers { get; set; }

        public string OneTimeUsersPercent { get; set; }
    }
}