using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    /// <summary>
    /// 用户量相关实体
    /// </summary>
    public class Sjqd_StatUsers
    {
        /// <summary>
        /// 维度ID
        /// </summary>
        public int ID { get; set; }

        public string IdName { get; set; }

        /// <summary>
        /// 维度名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }

        /// <summary>
        /// 小时
        /// </summary>
        public int StatHour { get; set; }

        /// <summary>
        /// 新增用户
        /// </summary>
        public int NewUserCount { get; set; }

        /// <summary>
        /// 山寨机新增用户
        /// </summary>
        public int NewUserCount_Shanzhai { get; set; }

        /// <summary>
        /// 二次激活用户数
        /// </summary>
        public int NewUserCount_SecAct { get; set; }

        /// <summary>
        /// 二次激活用户数(百度)
        /// </summary>
        public int NewUserCount_SecAct2 { get; set; }

        /// <summary>
        /// 有下载的新增用户数
        /// </summary>
        public int DownNewUserCount { get; set; }

        /// <summary>
        /// 活跃用户
        /// </summary>
        public int ActiveUserCount { get; set; }

        /// <summary>
        /// 山寨机活跃用户
        /// </summary>
        public int ActiveUserCount_Shanzhai { get; set; }

        /// <summary>
        /// 有下载的活跃用户
        /// </summary>
        public int DownActiveUserCount { get; set; }

        /// <summary>
        /// 百度DAU 用户（新用户+老用户）
        /// </summary>
        public int UseUserCount { get; set; }

        /// <summary>
        /// 升级用户数
        /// </summary>
        public int UpdatedUserCount { get; set; }

        /// <summary>
        /// 上周期新增用户
        /// </summary>
        public int LastNewUserCount { get; set; }

        /// <summary>
        /// 一次性用户
        /// </summary>
        public int OneTimeUserCount { get; set; }

        /// <summary>
        /// 累积用户
        /// </summary>
        public int TotalUserCount { get; set; }

        /// <summary>
        /// 山寨机累积用户
        /// </summary>
        public int TotalUserCount_Shanzhai { get; set; }

        /// <summary>
        /// 留存用户
        /// </summary>
        public int RetainedUserCount { get; set; }

        /// <summary>
        /// 日均留存率
        /// </summary>
        public double RetainedUserCountDailyRate { get; set; }


        /// <summary>
        /// 留存率指标新增用户
        /// </summary>
        public int OriginalNewUserCount { get; set; }

        /// <summary>
        /// 当前新增用户占总体的百分比
        /// </summary>
        public double NewUserPercent { get; set; }

        /// <summary>
        /// 当前活跃用户占总体的百分比
        /// </summary>
        public double ActiveUserPercent { get; set; }

        /// <summary>
        /// 下载量
        /// </summary>
        public long DownCount { get; set; }

        /// <summary>
        /// 下载用户数
        /// </summary>
        public int DownUserCount { get; set; }

        /// <summary>
        /// 一次下载量
        /// </summary>
        public int DownCountNotUpdate { get; set; }

        /// <summary>
        /// 一次用户数
        /// </summary>
        public int DownUserCountNotUpdate { get; set; }
    }
}