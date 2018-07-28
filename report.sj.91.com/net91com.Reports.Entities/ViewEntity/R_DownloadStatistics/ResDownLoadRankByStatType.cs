using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResDownLoadRankByStatType
    {
          #region 构造函数

            /// <summary>
            /// 来自数据库的数据构造
            /// </summary>
            public ResDownLoadRankByStatType(IDataReader reader)
                : this()
            {
                LoadFromDb(reader);
            }

            /// <summary>
            /// 默认构造函数
            /// </summary>
            public ResDownLoadRankByStatType()
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
                       
                        
                        case "resid":
                            ResID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["resid"]);
                            break;
                        case "statcount":
                            StatCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statcount"]);
                            break;
                        case "totalcount":
                            TotalCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["totalcount"]);
                            break;
                        case "percent":
                            Percent = reader.IsDBNull(i) ?0 : Convert.ToDecimal(reader["percent"]);
                            break;
                         
                    }
                }
                
            }

            
             
            #endregion

          #region 属性
         
            public int ResID { get; set; } 

            public string ResName { get; set; }

            public int StatCount { get; set; }

            public int TotalCount { get; set; }

            public decimal Percent { get; set; }

            

         #endregion
    }
}
