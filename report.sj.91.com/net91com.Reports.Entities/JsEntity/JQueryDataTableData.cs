using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.JsEntity
{
    public class JQueryDataTableData
    {
        public JQueryDataTableData()
        {
        }

        public string sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<List<string>> aaData { get; set; }
        public string Title { get; set; }
        public int iColumns { get; set; }

        public string begintime { get; set; }
        public string endtime { get; set; }
    }
}