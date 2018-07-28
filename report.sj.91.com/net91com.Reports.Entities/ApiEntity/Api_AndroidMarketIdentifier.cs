using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_AndroidMarketIdentifier
    {
        [JsonProperty("data")]
        public List<AndroidMarketIdentifiers> Data;
        [JsonProperty("success")]
        public bool Success; 
    }

    public class AndroidMarketIdentifiers
    {
        [JsonProperty("pn")]
        public string Identifiers;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("client")]
        public int Client;
        [JsonProperty("web")]
        public int Web;
        [JsonProperty("api")]
        public int Api;
        [JsonProperty("search")]
        public int Search;
        [JsonProperty("update")]
        public int Update;
        [JsonProperty("softcode")]
        public int ResId;
        [JsonProperty("sname")]
        public string ResName;
        [JsonProperty("cname")]
        public string CateName;



    } 

    public class  Identifier_Summary
    {

        public string Identifiers;

        public int ResId;

        public string ResName;

        public string CateName;

        public int TotalDownCount;

        public int DownCount_One;

        public int DownCount_Two;

        public int DownCount_Search;

        public int TotalDownCount_Avg;

        public int DownCount_One_Avg;

        public int DownCount_Two_Avg;

        public int DownCount_Search_Avg;

        public int TotalDownSuccess;

        public int TotalDownFail;

        public int TotalSetupSuccess;

        public int TotalSetupFail;

        public int StatDate;
    }
}
