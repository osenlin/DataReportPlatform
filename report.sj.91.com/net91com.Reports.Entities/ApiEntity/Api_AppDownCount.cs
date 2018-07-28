using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_AppDownCount
    {
        public string Identifier { get; set; }

        public string version { get; set; }

        public int downcount { get; set; }
    }
}
