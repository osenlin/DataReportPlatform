using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_UserAnalysis
{
    public class ChannelAttribute
    {
        public ChannelAttributeType Type;

        public int ID;

        public string Name;

        public int PID;

        public ChannelAttributeType PType;
    }

    public enum ChannelAttributeType
    {
        None = 0,
        ChannelType = 1,
        FirstLevelChannelCate = 2,
        SecondLevelChannelCate = 3
    }
}