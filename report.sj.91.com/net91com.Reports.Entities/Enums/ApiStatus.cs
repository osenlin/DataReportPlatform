using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.Enums
{
    public enum ApiStatus
    {
        [Description("调用成功")] Success = 0,
        [Description("数据还在统计中")] InStatistics = 1,
        [Description("数据还未统计")] NotStatistics = 2,
        [Description("参数缺失")] ParameterException = 10,
        [Description("Sign签名错误")] SignException = 20,
        [Description("服务端异常")] ServerException = 30,
    }
}