using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Tools
{
    #region 状态信息类型(EtlStateTypeOptions)

    /// <summary>
    /// 状态类型枚举
    /// </summary>
    public enum EtlStateTypeOptions
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = 0,

        /// <summary>
        /// 应用启动日志统计
        /// </summary>
        SoftLoginLogStat = 1,

        /// <summary>
        /// 使用日志统计
        /// </summary>
        UseLongLogStat = 2,

        /// <summary>
        /// 广告日志统计
        /// </summary>
        AdLogStat = 3
    }

    #endregion
}