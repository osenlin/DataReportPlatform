using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.Entity
{
    public class TempSearchData
    {
        public DateTime dt { get; set; }

        /// <summary>
        /// 搜索次数
        /// </summary>
        public int SearchCount { get; set; }

        /// <summary>
        /// 搜索匹配次数
        /// </summary>
        public int SearchSussessCount { get; set; }

        /// <summary>
        /// 搜索不匹配次数
        /// </summary>
        public int SearchFailedCount { get; set; }
    }

    /// <summary>
    /// 功能构树节点
    /// </summary>
    public class FunctionNode
    {
        public string Code { get; set; }
        public long Count { get; set; }
        public string ParentCode { get; set; }
        public string CodeName { get; set; }

        /// <summary>
        /// 根的数目
        /// </summary>
        public long RootCount { get; set; }

        /// <summary>
        /// 根的code
        /// </summary>
        public string RootCode { get; set; }
    }
}