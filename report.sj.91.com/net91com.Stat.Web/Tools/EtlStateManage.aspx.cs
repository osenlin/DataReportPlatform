using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

using net91com.Reports.UserRights;
using net91com.Reports.Tools;

namespace net91com.Stat.Web.Tools
{
    public partial class EtlStateManage : ToolPageBase
    {
        protected Dictionary<int, string> typeDict;

        /// <summary>
        /// 返回页面地址
        /// </summary>
        protected string ReturnUrl;

        /// <summary>
        /// 当前输入的搜索关键字
        /// </summary>
        protected string Keyword;

        protected EtlStateTypeOptions Type;

        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveSuperAdminRight();

            ReturnUrl = HttpUtility.UrlEncode(Request.RawUrl);
            Type = string.IsNullOrEmpty(Request["type"]) ? EtlStateTypeOptions.None : (EtlStateTypeOptions)Convert.ToInt32(Request["type"]);
            Keyword = Request["keyword"] == null ? string.Empty : Request["keyword"];
            TEtlStatesService esService = new TEtlStatesService();
            typeDict = esService.GetEtlStateTypes();
            int pageIndex = string.IsNullOrEmpty(Request["page"]) ? 1 : Convert.ToInt32(Request["page"]);
            int pageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 15 : Convert.ToInt32(Request["pagesize"]);
            int count = 0;
            List<EtlState> list = esService.GetEtlStates(Type, Keyword, pageIndex, pageSize, ref count);
            repeaData.DataSource = list;
            repeaData.DataBind();
            AspNetPager1.RecordCount = count;
            AspNetPager1.PageSize = pageSize;
            AspNetPager1.CurrentPageIndex = pageIndex;


            ddlTypes.DataSource = esService.GetEtlStateTypes();
            ddlTypes.DataTextField = "Value";
            ddlTypes.DataValueField = "Key";
            ddlTypes.DataBind();
            ddlTypes.SelectedValue = ((int)Type).ToString();
        }

        #region 编辑系统HTML构造(GetOpHtml)

        /// <summary>
        /// 编辑系统HTML构造
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string GetOpHtml(EtlState state)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.AppendFormat(@"<a href=""javascript:eidt({0},{1})"" name=""upd"">修改</a>", state.ID, (int)state.Type);
            htmlBuilder.AppendFormat(@"<a href=""javascript:dele({0})"" name=""dele"">删除</a>", state.ID);
            htmlBuilder.AppendFormat(@"<input type=""hidden"" id=""input{0}"" value=""{1}"" />", state.ID, state.Key);
            return htmlBuilder.ToString();
        }

        #endregion
    }
}