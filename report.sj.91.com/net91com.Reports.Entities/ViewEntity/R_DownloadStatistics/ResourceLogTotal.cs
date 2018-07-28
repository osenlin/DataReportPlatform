using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResourceLogTotal
    {
        /// <summary>
        /// 对应每个位置上的数据
        /// </summary>
        public List<LogDetailPosion> data1 { get; set; }
        public List<LogDetailPosion> data2 { get; set; }
        public List<LogDetailPosion> data3 { get; set; }
        public List<LogDetailPosion> data4 { get; set; }
        public List<LogDetailPosion> data5 { get; set; }
        public List<LogDetailPosion> data6 { get; set; }
        public List<LogDetailPosion> data7 { get; set; }
        public List<LogDetailPosion> data8 { get; set; }
        public List<LogDetailPosion> data9 { get; set; }
        public List<LogDetailPosion> data10 { get; set; }
        public List<LogDetailPosion> data11 { get; set; }
        public List<LogDetailPosion> data12 { get; set; }
        public List<LogDetailPosion> data13 { get; set; }
        public List<LogDetailPosion> data14 { get; set; }
        public int position { get; set; }
        /// <summary>
        /// 获取index上的数据
        /// </summary>
        /// <returns></returns>
        public void SetDataByIndex(int i,List<LogDetailPosion> list)
        {
            switch (i)
            { 
                case 1:
                    data1 = list;
                    break;
                case 2:
                    data2 = list;
                    break;
                case 3:
                    data3 = list;
                    break;
                case 4:
                    data4 = list;
                    break;
                case 5:
                    data5 = list;
                    break;
                case 6:
                    data6 = list;
                    break;
                case 7:
                    data7 = list;
                    break;
                case 8:
                    data8 = list;
                    break;
                case 9:
                    data9 = list;
                    break;
                case 10:
                    data10 = list;
                    break;
                case 11:
                    data11 = list;
                    break;
                case 12:
                    data12 = list;
                    break;
                case 13:
                    data13 = list;
                    break;
                case 14:
                    data14 = list;
                    break; 
            }
        }
    }
    /// <summary>
    /// 对应一个位置上映射的实体
    /// </summary>
    public class LogDetailPosion
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResName { get; set; }

        public int ResID { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 标签类型
        /// </summary>
        public int TagType { get; set; } 
        /// <summary>
        /// 每一天资源时间
        /// </summary>
        public DateTime DtBeginTime
        {
            get;
            set;
        } 
        public DateTime DtEndTime { get; set; }

        public string Memo { get; set; }

        public int TagID { get; set; }

        public Res91com.SoftModel.Enums.SoftADTypeOption ProjectType { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime EditTime { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatTime { get; set; }
        /// <summary>
        /// 修改原因
        /// </summary>
        public string EditReason { get; set; }
        /// <summary>
        /// 操作类型 1 表示新增 10 表示 修改排序
        /// </summary>
        public int OptionType { get; set; }

        public int DownCount { get; set; }
    }
}
