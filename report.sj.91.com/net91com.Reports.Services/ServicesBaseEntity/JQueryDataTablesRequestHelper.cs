using System.Web;

namespace net91com.Reports.Services.ServicesBaseEntity
{
    // 排序的方向
    public enum SortDirection
    {
        Asc, // 升序
        Desc // 降序
    }

    // 排序列的定义
    public class SortColumn
    {
        public int Index { get; set; } // 列序号
        public SortDirection Direction { get; set; } // 列的排序方向
    }

    // 列定义
    public class Column
    {
        public string Name { get; set; } // 列名
        public bool Sortable { get; set; } // 是否可排序
        public bool Searchable { get; set; } // 是否可搜索
        public string Search { get; set; } // 搜索串
        public bool EscapeRegex { get; set; } // 是否正则
    }

    public class DataTablesRequest
    {
        private readonly int ColumnCount;
        private readonly Column[] columns;
        private readonly int displayLength;
        private readonly int displayStart;
        private readonly string echo;
        private readonly bool regex;
        private readonly HttpRequest request;
        private readonly string search;
        private readonly SortColumn[] sortColumns;
        private readonly int sortingCols;

        public DataTablesRequest(HttpRequest request)
        {
            this.request = request;

            echo = ParseStringParameter(sEchoParameter);
            displayStart = ParseIntParameter(iDisplayStartParameter);
            displayLength = ParseIntParameter(iDisplayLengthParameter);
            sortingCols = ParseIntParameter(iSortingColsParameter);

            search = ParseStringParameter(sSearchParameter);
            regex = ParseStringParameter(bRegexParameter) == "true";

            // 排序的列
            int count = iSortingCols;
            sortColumns = new SortColumn[count];
            for (int i = 0; i < count; i++)
            {
                var col = new SortColumn();
                col.Index = ParseIntParameter(string.Format("iSortCol_{0}", i));
                col.Direction = SortDirection.Asc;
                if (ParseStringParameter(string.Format("sSortDir_{0}", i)) == "desc")
                    col.Direction = SortDirection.Desc;
                sortColumns[i] = col;
            }

            ColumnCount = ParseIntParameter(iColumnsParameter);

            count = ColumnCount;
            columns = new Column[count];

            string[] names = ParseStringParameter(sColumnsParameter).Split(',');
            if (names.Length < count)
            {
                count = names.Length;
            }
            for (int i = 0; i < count; i++)
            {
                var col = new Column();
                col.Name = names[i];
                col.Sortable = ParseStringParameter(string.Format("bSortable_{0}", i)) == "true";
                col.Searchable = ParseStringParameter(string.Format("bSearchable_{0}", i)) == "true";
                col.Search = ParseStringParameter(string.Format("sSearch_{0}", i));
                col.EscapeRegex = ParseStringParameter(string.Format("bRegex_{0}", i)) == "true";
                columns[i] = col;
            }
        }

        #region

        private const string sEchoParameter = "sEcho";

        // 起始索引和长度
        private const string iDisplayStartParameter = "iDisplayStart";
        private const string iDisplayLengthParameter = "iDisplayLength";

        // 列数
        private const string iColumnsParameter = "iColumns";
        private const string sColumnsParameter = "sColumns";

        // 参与排序列数
        private const string iSortingColsParameter = "iSortingCols";
        private const string iSortColPrefixParameter = "iSortCol_"; // 排序列的索引
        private const string sSortDirPrefixParameter = "sSortDir_"; // 排序的方向 asc, desc

        // 每一列的可排序性
        private const string bSortablePrefixParameter = "bSortable_";

        // 全局搜索
        private const string sSearchParameter = "sSearch";
        private const string bRegexParameter = "bRegex";

        // 每一列的搜索
        private const string bSearchablePrefixParameter = "bSearchable_";
        private const string sSearchPrefixParameter = "sSearch_";
        private const string bEscapeRegexPrefixParameter = "bRegex_";

        #endregion

        public string sEcho
        {
            get { return echo; }
        }

        public int iDisplayStart
        {
            get { return displayStart; }
        }

        public int iDisplayLength
        {
            get { return displayLength; }
        }

        // 参与排序的列

        public int iSortingCols
        {
            get { return sortingCols; }
        }

        // 排序列

        public SortColumn[] SortColumns
        {
            get { return sortColumns; }
        }

        public int iColumns
        {
            get { return ColumnCount; }
        }

        public Column[] Columns
        {
            get { return columns; }
        }

        public string Search
        {
            get { return search; }
        }

        public bool Regex
        {
            get { return regex; }
        }

        #region 常用的几个解析方法

        private int ParseIntParameter(string name) // 解析为整数
        {
            int result = 0;
            string parameter = request[name];
            if (!string.IsNullOrEmpty(parameter))
            {
                int.TryParse(parameter, out result);
            }
            return result;
        }

        private string ParseStringParameter(string name) // 解析为字符串
        {
            return request[name];
        }

        private bool ParseBooleanParameter(string name) // 解析为布尔类型
        {
            bool result = false;
            string parameter = request[name];
            if (!string.IsNullOrEmpty(parameter))
            {
                bool.TryParse(parameter, out result);
            }
            return result;
        }

        #endregion
    }
}