using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace net91com.Stat.Web.UserRights
{
    public partial class RightSysDescript : URBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //只有管理员才能查看该页面
            loginService.HaveAdminRight();
        }
    }
}