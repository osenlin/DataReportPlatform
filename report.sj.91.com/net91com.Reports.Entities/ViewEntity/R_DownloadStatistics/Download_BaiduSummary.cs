using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class Download_BaiduSummary
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Download_BaiduSummary(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        public Download_BaiduSummary()
        {

        }

        public DateTime StatDate;

        public long DownFromClient;

        public long DownFromClient_One;

        public long DownFromClient_Two;

        public long DownFromClient_Search;

        public long DownFromClient_ExceptSearch;

        public long DownFromPC;

        public long DownFromPC_One;

        public long DownFromPC_Two;

        public long DownFromPC_Search;

        public long DownFromPC_ExceptSearch;

        public long DownFromOther;

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {

            for (int i = 0; i < reader.FieldCount; i++)
            {


                switch (reader.GetName(i).ToLower())
                {
                    case "statdate":
                        int temp = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(temp/10000, temp%10000/100, temp%100);
                        break;
                    case "downfromclient":
                        DownFromClient   = reader.IsDBNull(i)
                                                             ? 0
                                                             : Convert.ToInt64(reader["downfromclient"]);
                        break;
                    case "downfromclient_one":
                        DownFromClient_One = reader.IsDBNull(i)
                                                              ? 0
                                                              : Convert.ToInt64(
                                                                  reader["downfromclient_one"]);
                        break;
                    case "downfromclient_exceptsearch":
                        DownFromClient_ExceptSearch = reader.IsDBNull(i)
                                                              ? 0
                                                              : Convert.ToInt64(
                                                                  reader["downfromclient_exceptsearch"]);
                        break;
                    case "downfrompc":
                        DownFromPC = reader.IsDBNull(i)
                                                                 ? 0
                                                                 : Convert.ToInt64(
                                                                     reader["downfrompc"]);
                        break;
                    case "downfrompc_one":
                        DownFromPC_One = reader.IsDBNull(i)
                                                      ? 0
                                                      : Convert.ToInt64(reader["downfrompc_one"]);
                        break;
                    case "downfrompc_exceptsearch":
                        DownFromPC_ExceptSearch = reader.IsDBNull(i)
                                                          ? 0
                                                          : Convert.ToInt64(reader["downfrompc_exceptsearch"]);
                        break;
                    case "downfromother":
                        DownFromOther = reader.IsDBNull(i)
                                                              ? 0
                                                              : Convert.ToInt64(
                                                                  reader["downfromother"]);
                        break;
                     
        
                }
               
            }
            DownFromClient_Two = DownFromClient - DownFromClient_One;
            DownFromClient_Search = DownFromClient - DownFromClient_ExceptSearch;
            DownFromPC_Two = DownFromPC - DownFromPC_One;
            DownFromPC_Search = DownFromPC - DownFromPC_ExceptSearch;

        }
    }
}
