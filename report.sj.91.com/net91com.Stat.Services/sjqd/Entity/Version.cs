using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace net91com.Stat.Services.sjqd.Entity
{
    /// <summary>
    /// 用于前台报表展示，之前遗留
    /// </summary>
    [Serializable]
    public class SoftVersion : ICloneable
    {
        public string VersionID { get; set; }
        public string VersionCode { get; set; }
        public Version Version { get; set; }

        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            var obj = formatter.Deserialize(stream) as SoftVersion;
            return obj;
        }
    }
}