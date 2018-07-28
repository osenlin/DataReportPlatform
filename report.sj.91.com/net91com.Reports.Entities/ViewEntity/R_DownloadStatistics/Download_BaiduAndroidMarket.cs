using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class Download_BaiduAndroidMarket
    {


        public List<MarketData> data = new List<MarketData>();

        public bool success;
       
    }
    public class MarketData
    {
        public string date;
        public string app_one_pc;
        public string app_one_wap;
        public string app_one_api;
        public string app_one_client;

        public string app_two_api;
        public string app_two_client;

        public string game_one_pc;
        public string game_one_wap;
        public string game_one_api;
        public string game_one_client;

        public string game_two_api;
        public string game_two_client;


    }
}
