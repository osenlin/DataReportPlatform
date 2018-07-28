using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_SummaryUsers_Daily
    {
        public string msg;

        public bool success;

        public List<Api_SummaryUsersCount> datas;
    }

    public class Api_SummaryUsersCount
    {
        public DateTime datatime;

        public long newcount;

        public long daucount;

        public string soft;
    }

    public class Api_SummaryDownData_Daily
    {
        public string msg;

        public bool success;

        public List<Api_SummaryDownCount> datas;
    }

    public class Api_SummaryDownCount
    {
        public DateTime datatime;

        public string soft;

        public long downtotalcount;

        public long downfromclient;

        public long downfromclient_one;

        public long downfromclient_two;

        public long downfromclient_search;

        public long downfrompc;

        public long downfrompc_one;

        public long downfrompc_two;

        public long downfrompc_search;

        public long downfromother;

        public long downfromweb;

        public long downfromapi;



        


    }
}
