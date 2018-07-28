using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Core.Util;

namespace net91com.Stat.Web
{
    public partial class Login : System.Web.UI.Page
    {
        public string LoginUrl = "";
        protected void Page_Load(object sender, EventArgs e)
        {
 
            LoginUrl=ConfigHelper.GetSetting("UserLoginUrl");

        }
    }
}