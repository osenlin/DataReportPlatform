using net91com.Reports.Services.CommonServices.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class PositionTag : System.Web.UI.UserControl
    {
        public string data;

        public string Positions
        {
            get
            {
                return selectPostionTag.Value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //data = PositionTagService.GetPositionTagTreeJson();

        }

    }
}