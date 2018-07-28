using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class DownSpeedLog
    {
        #region 构造函数

            /// <summary>
            /// 来自数据库的数据构造
            /// </summary>
            public DownSpeedLog(IDataReader reader)
                : this()
            {
                LoadFromDb(reader);
            }

            /// <summary>
            /// 默认构造函数
            /// </summary>
            public DownSpeedLog()
            {
            }

            /// <summary>
            /// 来自数据库的数据构造
            /// </summary>
            public   void LoadFromDb(IDataReader reader)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    switch (reader.GetName(i).ToLower())
                    {
                        case "serverip":
                            ServerIP = reader.IsDBNull(i) ? "" : reader.GetString(i);
                            break;
                        case "nettype":
                            NetType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["nettype"]);
                            break;
                        case "avgspeed":
                            AvgSpeed = reader.IsDBNull(i) ? 0 : Convert.ToDecimal(reader["avgspeed"]);
                            break;
                        case "restime":
                            ResTime = reader.IsDBNull(i) ? 0 : Convert.ToDecimal(reader["restime"]);
                            break;
                        case "avgdns":
                            AvgDNS = reader.IsDBNull(i) ? 0 : Convert.ToDecimal(reader["avgdns"]);
                            break;
                        case "downcount":
                            DownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                            break;
                        
                    }
                }
            }

            #endregion

        #region 属性


            /// <summary>
            /// 服务器IP
            /// </summary>
            public string ServerIP { get; set; }
           /// <summary>
           /// 网络类型
           /// </summary>
            public int NetType { get; set; }

            public decimal AvgSpeed { get; set; }


            public decimal ResTime { get; set; }

            public decimal AvgDNS { get; set; }

            public int DownCount { get; set; }

            public object GetIndexValue(int index)
            {
                switch (index)
                {
                    case  1:
                        return AvgSpeed;
                    case 2:
                        return ResTime;
                    case 3:
                        return AvgDNS;
                    case 4:
                        return DownCount;
                    default:
                        return AvgSpeed; 
                }
            }

            #endregion
    }
}
