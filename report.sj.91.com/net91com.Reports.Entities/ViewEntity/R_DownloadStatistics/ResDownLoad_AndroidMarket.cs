using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    /// <summary>
    /// 安卓市场接口的序列化类
    /// </summary>
    public class ResDownLoad_AndroidMarket
    {
        public List<MarketLoad> data=new List<MarketLoad>();

        public bool success;
    }

    public class MarketLoad
    {
        public DateTime _id { get; set; }

        public string count { get; set; }

        public string client { get; set; }

        public string web { get; set; }

        public string api { get; set; }

        public string search { get; set; }

        public string update { get; set; }

        public int Count { get; set; }

        public int Client { get; set; }

        public int Web { get; set; }

        public int Api { get; set; }

        public int Search { get; set; }

        public int Update { get; set; }

        public int wap_dau;
        /// <summary>
        /// wap dau
        /// </summary>
        public int Wap_Dau { get; set; }

        public void SetData()
        {
           
            Web = int.Parse(this.web, NumberStyles.Float);
            Api = int.Parse(this.api, NumberStyles.Float);
            Search = int.Parse(this.search, NumberStyles.Float); ;
            Update = int.Parse(this.update, NumberStyles.Float);
            Client = int.Parse(this.client, NumberStyles.Float);
            Count = Web + Api + Client;
            Wap_Dau = wap_dau;

        }
    }

    
}
