using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.D_DownLoadStatisticsEntities
{
    public class D_StataUnionGamesCount
    {
        public DateTime _id { get; set; }
        /// <summary>
        /// 游戏广告
        /// </summary>
        public long cooperate { get; set; }
        /// <summary>
        /// 联运游戏
        /// </summary>
        public long oper_game { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int versionid { get; set; }


    }

    public class D_StataUnionGamesCountLst
    {
        public List<D_StataUnionGamesCount> data = new List<D_StataUnionGamesCount>();

        public bool success;
    }
}
