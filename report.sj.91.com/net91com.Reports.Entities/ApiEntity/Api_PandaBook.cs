using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_PandaBook
    {
        public List<PandaBookDatas> datas=new List<PandaBookDatas>();
        public bool success = false;
        public string message;
    }

    public class PandaBookDatas
    {
        public string datatype;
        public List<PandaBookData> data=new List<PandaBookData>();
    }

    public class PandaBookData
    {
        public DateTime statdate;
        public string platform;
        public string value;
    }
}
