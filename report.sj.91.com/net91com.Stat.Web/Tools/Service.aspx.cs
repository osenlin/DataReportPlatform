using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using Newtonsoft.Json;
using net91com.Reports.UserRights;
using net91com.Core.Extensions;
using net91com.Reports.Tools;

namespace net91com.Stat.Web.Tools
{
    public partial class Service : ToolPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string result = "{\"Message\":\"操作成功\",\"Code\":0}";
            try
            {
                switch (GetQueryString("act").ToLower())
                {
                    case "detlstate":
                        DeleteEtlState();
                        break;
                }
            }
            catch (NotRightException)
            {
                result = "{\"Message\":\"您没有权限执行此操作\",\"Code\":3}";
            }
            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 删除统计状态
        /// </summary>
        private void DeleteEtlState()
        {
            loginService.HaveSuperAdminRight();

            new TEtlStatesService().DeleteEtlState(Convert.ToInt32(Request["id"]));
        }
    }
}