using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class StatHourUser
    {
        public DateTime StatDate { get; set; }
        public int StatHour { get; set; }
        public int SoftID { get; set; }
        public int Platform { get; set; }
        public int NewUserCount { get; set; }
        public int ActiveUserCount { get; set; }
    }
}