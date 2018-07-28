using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class SystemManage : URBasePage 
    {
        /// <summary>
        /// 返回页面地址
        /// </summary>
        protected string ReturnUrl;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ReturnUrl = HttpUtility.UrlEncode(Request.RawUrl);

            URBasicInfoService biService = new URBasicInfoService();
            List<SystemInfo> list = biService.GetSystems();
            repeaData.DataSource = list;
            repeaData.DataBind();
        }

        #region 编辑系统HTML构造(GetOpHtml)

        /// <summary>
        /// 编辑系统HTML构造
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        protected string GetOpHtml(SystemInfo system)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.AppendFormat(@"<a href=""javascript:eidt({0})"" name=""upd"">修改</a>", system.ID);
            htmlBuilder.AppendFormat(@"<a href=""javascript:dele({0})"" name=""dele"">删除</a>", system.ID);
            htmlBuilder.AppendFormat(@"<input type=""hidden"" id=""input{0}"" value=""{1}"" />", system.ID, system.Name);
            return htmlBuilder.ToString();
        }

        #endregion
    }
}