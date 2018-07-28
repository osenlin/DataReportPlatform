using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResourceRecommendTotal
    {
          /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public ResourceRecommendTotal(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResourceRecommendTotal()
        {
        }


        #region 属性


        public string Account { get; set; }
        public int Date1Count { get; set; }
        public int Date2Count { get; set; }
        public int Date3Count { get; set; }
        public int Date4Count { get; set; }
        public int Date5Count { get; set; }
        public int Date6Count { get; set; }
        public int Date7Count { get; set; }
        public int DateAllCount { get; set; }

        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {

                    case "account":
                        Account = reader.GetString(i);
                        break;
                  
                    case "date1count":
                        Date1Count = reader.GetInt32(i);
                        break;
                    case "date2count":
                        Date2Count = reader.GetInt32(i);
                        break;
                    case "date3count":
                        Date3Count = reader.GetInt32(i);
                        break;
                    case "date4count":
                        Date4Count = reader.GetInt32(i);
                        break;
                    case "date5count":
                        Date5Count = reader.GetInt32(i);
                        break;
                    case "date6count":
                        Date6Count = reader.GetInt32(i);
                        break;
                    case "date7count":
                        Date7Count = reader.GetInt32(i);
                        break;
                    case "dateallcount":
                        DateAllCount = reader.GetInt32(i);
                        break;


                }
            }
        }

        public object GetIndexValue(int index)
        {
            switch (index)
            {
                case 0:
                    return Account;
                case 1:
                    return Date1Count;
                case 2:
                    return Date2Count;
                case 3:
                    return Date3Count;
                case 4:
                    return Date4Count;
                case 5:
                    return Date5Count;
                case 6:
                    return Date6Count;
                case 7:
                    return Date7Count;
                case 8:
                    return DateAllCount;
               

                default:
                    return Account;
            }
        }

       
    }
}
