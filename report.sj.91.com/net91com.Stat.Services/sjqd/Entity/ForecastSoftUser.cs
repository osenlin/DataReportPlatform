using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class ForecastSoftUser
    {
        public int Platform { get; set; }
        public int SoftId { get; set; }
        public int Period { get; set; }
        public DateTime StatDate { get; set; }
        public DateTime ForecaseDate { get; set; }
        public int NewUserCount { get; set; }
        public int TotalUserCount { get; set; }

        public int ChannelType { get; set; }
        public int ChannelID { get; set; }
    }
}