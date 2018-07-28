using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace net91com.Reports.Entities.TableTemplate
{
    public class BaiduReportTableTemplate
    {
        public class ReportTable
        {
            public string Name;

            public string TableStyle;

            public int RowCycleCount;

            public string RowCycleCountKey;

            public List<ReportTableRows> Rows;
        }

        public class ReportTableRows
        {
            /// <summary>
            /// Type=1 表示th 类型行 0 表示td 类型行
            /// </summary>
            public int Type { get; set; }

            public List<Cell> listCells;

            public string RowStyle;
        }

        public class Cell
        {
            public string Format { get; set; }

            public string Key { get; set; }

            /// <summary>
            /// 是否需要转化成对应值
            /// </summary>
            public bool NeedConvert { get; set; }

            public int Rowspan { get; set; }
            public int Colspan { get; set; }
            public int ColIndex { get; set; }
            public string Width { get; set; }

            public int NeedSpeacilStyle { get; set; }
        }
    }
}