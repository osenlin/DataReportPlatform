using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using net91com.Stat.Services.Entity;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.Controls
{
    public class SelectChannelValue
    {
        public int ChannelValue { get; set; }

        public ChannelTypeOptions ChannelType { get; set; }

        public string ChannelText { get; set; }

        public int Platform { get; set; }

        public SelectChannelValue(int channelValue, ChannelTypeOptions type, string channelText, int platform)
        {
            this.ChannelValue = channelValue;
            this.ChannelType = type;
            this.ChannelText = channelText;
            this.Platform = platform;
        }
        public SelectChannelValue(int channelValue, ChannelTypeOptions type, string channelText)
        {
            this.ChannelValue = channelValue;
            this.ChannelType = type;
            this.ChannelText = channelText;
        }
    }

    public class SelectLinkTagValue
    {
        public int TagValue { get; set; }

        public bool IsCategory { get; set; }

        public string TagText { get; set; }

        public int Platform { get; set; }

        public SelectLinkTagValue(int tagValue, bool isCategory, string tagText, int platform)
        {
            this.TagValue = tagValue;
            this.IsCategory = isCategory;
            this.TagText = tagText;
            this.Platform = platform;
        }
        public SelectLinkTagValue(int tagValue, bool isCategory, string tagText)
        {
            this.TagValue = tagValue;
            this.IsCategory = isCategory;
            this.TagText = tagText;
        }
    }
}