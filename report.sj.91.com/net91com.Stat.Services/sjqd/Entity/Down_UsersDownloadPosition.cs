using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Down_UsersDownloadPosition
    {
        /// <summary>
        /// 数据来源
        /// </summary>
        public int ProjectSource { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        public int ResType { get; set; }

        /// <summary>
        /// 位置id
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 位置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public string PageType { get; set; }

        /// <summary>
        /// 是否为专辑
        /// </summary>
        public bool ByTag { get; set; }

        /// <summary>
        /// 是否为检查更新
        /// </summary>
        public bool Updating { get; set; }

        ///2海外还是1国内
        public int AreaType { get; set; }
    }
}