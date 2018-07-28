using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ApiEntity
{
    public class Api_HwImeiFirstLoginDate
    {
        public bool Success = false;
        public string Msg;
        public List<ImeiFirstLoginDate> Datas=new List<ImeiFirstLoginDate>(); 
    }

    public class ImeiFirstLoginDate
    {
        public string IMEI;
        public DateTime FirstLoginDate;
    }

    public class Api_RequestParas
    {
        public string sign;

        public List<string> imeis;
    }
}
