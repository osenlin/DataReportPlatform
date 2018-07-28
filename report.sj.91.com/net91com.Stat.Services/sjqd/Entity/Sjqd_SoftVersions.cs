using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class Sjqd_SoftVersions
    {
        public int ID { get; set; }

        public int SoftID { get; set; }

        public int Platform { get; set; }

        public string Version { get; set; }

        public string E_Version { get; set; }

        public bool Hidden { get; set; }

        public bool HiddenForUL { get; set; }

        public bool IsStatisticsVersion { get; set; }
    }
}