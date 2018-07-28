using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_DownCount
    {
        public string msg;

        public bool success;

        public List<Api_DownCountDetail> data;

    }

    public class Api_DownCountDetail
    {
        public DateTime date;

        public long downcount;

        public long downsuccesscount;

        public long downfailcount;

        public long setupcount;

        public long setupsuccesscount;

        public long setupfailcount;
    }
}
