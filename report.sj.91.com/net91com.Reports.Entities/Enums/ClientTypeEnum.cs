using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.Enums
{
    public enum ClientTypeEnum
    {
        [Description("客户端")] Client = 1,
        [Description("web")] Web = 2,
        [Description("api")] Api = 3
    }
}