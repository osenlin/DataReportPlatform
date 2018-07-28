using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResDownLoadRankByStatType_Detail
    {
         #region 构造函数
        
            /// <summary>
            /// 来自数据库的数据构造
            /// </summary>
            public ResDownLoadRankByStatType_Detail(IDataReader reader)
                : this()
            {
                LoadFromDb(reader);
            }

            /// <summary>
            /// 默认构造函数
            /// </summary>
            public ResDownLoadRankByStatType_Detail()
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
                       
                        
                        case "appversion":
                            AppVersion = reader.IsDBNull(i) ? "" : reader["appversion"].ToString();
                            break;
                        case "appversionname":
                            AppVersionName = reader.IsDBNull(i) ? "" : reader["appversionname"].ToString();
                            break;
                        case "imei":
                            Imei = reader.IsDBNull(i) ? "" : reader["imei"].ToString();
                            break;
                        case "mobilemodel":
                            MobileModel = reader.IsDBNull(i) ? "" : reader["mobilemodel"].ToString();
                            break;
                        case "mobilefirmware":
                            MobileFirmware = reader.IsDBNull(i) ? "" : reader["mobilefirmware"].ToString();
                            break;
                        case "filesize":
                            FileSize = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["filesize"]);
                            break;
                        case "downsize":
                            DownSize = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downsize"]);
                            break;
                        case "errorcode":
                            ErrorCode = reader.IsDBNull(i) ? "" : reader["errorcode"].ToString();
                            break;
                        case "errortime":
                            ErrorTime = reader.IsDBNull(i) ? DateTime.MinValue : Convert.ToDateTime(reader["errortime"]);
                            break;
                        case "errorinfo":
                            ErrorInfo = reader.IsDBNull(i) ? "" : reader["errorinfo"].ToString();
                            break;
                        case "serverip":
                            ServerIp = reader.IsDBNull(i) ? "" : reader["serverip"].ToString();
                            break;
                        case "clientip":
                            ClientIp = reader.IsDBNull(i) ? "" : reader["clientip"].ToString();
                            break;
                        case "cdnurl":
                            CdnUrl = reader.IsDBNull(i) ? "" : reader["cdnurl"].ToString();
                            break;
                        case "cdnheader":
                            CdnHeader = reader.IsDBNull(i) ? "" : reader["cdnheader"].ToString();
                            break;
                        case "packagetype":
                            PackageType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["packagetype"]);
                            break;


                    }
                }
                if (PackageType == 1)
                    PackageTypeName = "增量包";
                else if(PackageType == 2)
                    PackageTypeName = "全量包";
                
            }

            
             
            #endregion

          #region 属性
            /// <summary>
            /// VersionCode
            /// </summary>
            public string AppVersion { get; set; }

            public string AppVersionName { get; set; } 

            public string Imei { get; set; }

            public string MobileModel { get; set; }

            public string MobileFirmware { get; set; }

            public long FileSize { get; set; }

            public long DownSize { get; set; }

            public string ErrorCode { get; set; }

            public DateTime ErrorTime { get; set; }

            public string ErrorInfo { get; set; } 

            public string ServerIp { get; set; }

            public string ClientIp { get; set; }
            /// <summary>
            /// 客户所在区域
            /// </summary>
            public string ClientArea     { get; set; }

            public string CdnUrl { get; set; }

            public string CdnHeader { get; set; }
            /// <summary>
            /// 打包方式
            /// </summary>
            public int PackageType { get; set; }

            public string PackageTypeName { get; set; }

            
            

        #endregion
    }
}
