using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net91com.Reports.Tools
{
    #region 状态信息实体(EtlState)

    /// <summary>
    /// 状态信息实体
    /// </summary>
    public class EtlState
    {
        /// <summary>
        /// 状态ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 状态键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 状态值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public EtlStateTypeOptions Type { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
    }

    #endregion
}
