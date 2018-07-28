using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 用户使用量，活跃用户，新增用户 使用的基类
    /// </summary>
    [Serializable]
    public class SoftUser
    {
        public int Period { get; set; }
        public DateTime StatDate { get; set; }
        public int Platform { get; set; }
        public int SoftId { get; set; }
        public int ChannelID { get; set; }
        public string ChannelName { get; set; }
        public int VersionID { get; set; }
        public string E_Version { get; set; }
        public int UseNum { get; set; }
        public int NewNum { get; set; }
        public int NewNum_Broken { get; set; }
        public int NewNum_NotBroken { get; set; }
        public int ActiveNum { get; set; }
        public int ActiveNum_Broken { get; set; }
        public int ActiveNum_NotBroken { get; set; }
        public int NewNum_ZJS { get; set; }
        public int ActiveNum_ZJS { get; set; }
        public int DownValueUsersForNew { get; set; }
        public int DownValueUsersForAct { get; set; }

        public int FuncValueUsersForNew { get; set; }
        public int FuncValueUsersForAct { get; set; }

        public int NewUserCount_Shanzhai { get; set; }

        public int ActiveUserCount_Shanzhai { get; set; }

        public int TotalUserCount_Shanzhai { get; set; }

        public int LostNum { get; set; }
        public int TotalNum { get; set; }


        public int OneTimeUsers { get; set; }

        /// <summary>
        /// 第一次计算新增用户
        /// </summary>
        public int FirstNewUserCount { get; set; }

        /// <summary>
        /// 第一次计算活跃用户
        /// </summary>
        public int FirstActiveUserCount { get; set; }

        /// <summary>
        /// 第一次计算使用用户
        /// </summary>
        public int FirstUseUserCount { get; set; }

        public double RetainRate1 { get; set; }

        public double RetainRate3 { get; set; }

        public double RetainRate6 { get; set; }

        /// <summary>
        /// 小时
        /// </summary>
        public int Hour { get; set; }

        /// 用户新增
        public string Growth { get; set; }

        ///使用量百分比
        public string UsePercent { get; set; }

        ///活跃用户百分比
        public string ActivityPercent { get; set; }

        ///流失率
        public string LostPercent { get; set; }

        /// <summary>
        /// 来自缓存的新增用户量
        /// </summary>
        public int NewUserCountFromCache { get; set; }

        /// <summary>
        /// 来自缓存的活跃用户量
        /// </summary>
        public int ActiveUserCountFromCache { get; set; }

        public int RetainedUserCount { get; set; }

        public int LastNewUserCount { get; set; }

        public int LastNewUserCount_Shanzhai { get; set; }

        /// <summary>
        /// 二次激活
        /// </summary>
        public int NewUserCount_SecAct { get; set; }

        /// <summary>
        /// 二次激活(百度)
        /// </summary>
        public int NewUserCount_SecAct2 { get; set; }
    }
}