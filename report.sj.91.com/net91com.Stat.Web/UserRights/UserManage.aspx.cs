using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

using net91com.Core.Util;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public partial class UserManage : URBasePage
    {
        /// <summary>
        /// 当前选择的系统ID
        /// </summary>
        protected int SystemID;

        /// <summary>
        /// 当前选择的状态
        /// </summary>
        protected StatusOptions Status;
        /// <summary>
        /// 当前选择的用户类型
        /// </summary>
        protected UserTypeOptions AccountType;
        /// <summary>
        /// 当前输入的搜索关键字
        /// </summary>
        protected string Keyword;
        /// <summary>
        /// 返回页面地址
        /// </summary>
        protected string ReturnUrl;

        protected bool IsOnlyWhiteUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            SystemID = string.IsNullOrEmpty(Request["sysId"]) ? 0 : Convert.ToInt32(Request["sysId"]);
            List<SystemInfo> systems = new List<SystemInfo>();
            systems.Add(new SystemInfo { ID = 0, Name = "全部系统" });
            systems.AddRange(loginService.AdminSystems);
            ddlSystems.DataSource = systems;
            ddlSystems.DataTextField = "Name";
            ddlSystems.DataValueField = "ID";
            ddlSystems.DataBind();
            ddlSystems.SelectedValue = SystemID.ToString();
            ddlSystems.AutoPostBack = false;

            if (Request.QueryString["action"] == "ExportToExcel")
            {
                ExportToExcel();
                Response.End();
            }
            
            Status = string.IsNullOrEmpty(Request["status"]) ? StatusOptions.Valid : (StatusOptions)Convert.ToInt32(Request["status"]);
            AccountType = string.IsNullOrEmpty(Request["accountType"]) ? UserTypeOptions.General : (UserTypeOptions)Convert.ToInt32(Request["accountType"]);
            Keyword = Request["keyword"] == null ? string.Empty : Request["keyword"];
            IsOnlyWhiteUser = Request["checkOnlyWhite"] == null ? false : Convert.ToBoolean(Request["checkOnlyWhite"]) ;
            ReturnUrl = HttpUtility.UrlEncode(Request.RawUrl);
            BindData();
        }

        #region 绑定数据(BindData)

        /// <summary>
        /// 绑定数据(该方法已经包含权限验证)
        /// </summary>
        private void BindData()
        {
            URBasicInfoService biService = new URBasicInfoService();
            int pageIndex = string.IsNullOrEmpty(Request["page"]) ? 1 : Convert.ToInt32(Request["page"]);
            int pageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 15 : Convert.ToInt32(Request["pagesize"]);            
            int count = 0;
            List<User> list = biService.GetUsers(SystemID, Status, AccountType, Keyword, IsOnlyWhiteUser, pageIndex, pageSize, ref count);
            repeaData.DataSource = list;
            repeaData.DataBind();
            AspNetPager1.RecordCount = count;
            AspNetPager1.PageSize = pageSize;
            AspNetPager1.CurrentPageIndex = pageIndex;
        }

        #endregion

        #region 用户类型HTML构造(GetSelectHtml)

        /// <summary>
        /// 选择用户类型HTML构造
        /// </summary>
        /// <returns></returns>
        protected string GetSelectHtml()
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<select id=\"selAccountTypes\" name=\"selAccountTypes\">");
            if (loginService.LoginUser.AccountType == UserTypeOptions.SuperAdmin
                || loginService.LoginUser.AccountType == UserTypeOptions.Admin
                || loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin)
            {
                htmlBuilder.AppendFormat("<option value=\"{0}\"{1}>普通用户</option>"
                    , (int)UserTypeOptions.General
                    , AccountType == UserTypeOptions.General ? " selected=\"selected\"" : string.Empty); 
                if (loginService.LoginUser.AccountType == UserTypeOptions.SuperAdmin)
                {
                    htmlBuilder.AppendFormat("<option value=\"{0}\"{1}>超级管理员</option>"
                        , (int)UserTypeOptions.SuperAdmin
                        , AccountType == UserTypeOptions.SuperAdmin ? " selected=\"selected\"" : string.Empty);
                    htmlBuilder.AppendFormat("<option value=\"{0}\"{1}>管理员</option>"
                        , (int)UserTypeOptions.Admin
                        , AccountType == UserTypeOptions.Admin ? " selected=\"selected\"" : string.Empty);
                }
                if (loginService.LoginUser.AccountType == UserTypeOptions.SuperAdmin || loginService.LoginUser.AccountType == UserTypeOptions.Admin)
                {
                    htmlBuilder.AppendFormat("<option value=\"{0}\"{1}>产品管理员</option>"
                        , (int)UserTypeOptions.ProductAdmin
                        , AccountType == UserTypeOptions.ProductAdmin ? " selected=\"selected\"" : string.Empty);
                }
                htmlBuilder.AppendFormat("<option value=\"{0}\"{1}>渠道内部用户</option>"
                    , (int)UserTypeOptions.Channel
                    , AccountType == UserTypeOptions.Channel ? " selected=\"selected\"" : string.Empty);
                htmlBuilder.AppendFormat("<option value=\"{0}\"{1}>渠道合作方</option>"
                    , (int)UserTypeOptions.ChannelPartner
                    , AccountType == UserTypeOptions.ChannelPartner ? " selected=\"selected\"" : string.Empty);
            }
            htmlBuilder.Append("</select>");
            return htmlBuilder.ToString();
        }

        #endregion

        #region 编辑用户HTML构造(GetOpHtml)

        /// <summary>
        /// 编辑用户HTML构造
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected string GetOpHtml(User user)
        {
            if (loginService.CheckAdminRightForUserEdit(user))
            {
                StringBuilder htmlBuilder = new StringBuilder();
                if (user.AccountType != UserTypeOptions.SuperAdmin)
                {
                    htmlBuilder.AppendFormat(@"<a href=""javascript:eidt('usstatus',{0},'{1}')"">{2}</a>", user.ID
                        , (int)(user.Status == StatusOptions.Valid ? StatusOptions.Invalid : StatusOptions.Valid)
                        , user.Status == StatusOptions.Valid ? "禁用" : "启用");
                    htmlBuilder.AppendFormat(@"<a href=""AddUser.aspx?un={0}&ReturnUrl={1}"" name=""upd"">修改</a>", user.ID, ReturnUrl);
                    htmlBuilder.AppendFormat(@"<a href=""javascript:dele({0},'{1}')"" name=""dele"">删除</a>", user.ID, user.Account);
                }
                htmlBuilder.AppendFormat(@"<a href=""UserSystemManage.aspx?uid={0}&ReturnUrl={1}"" name=""system"">系统</a>", user.ID, ReturnUrl);
                htmlBuilder.AppendFormat(@"<a href=""UserRoleManage.aspx?sysId={0}&uid={1}&ReturnUrl={2}"" name=""role"">角色</a>", SystemID, user.ID, ReturnUrl);
                if (user.AccountType != UserTypeOptions.Channel && user.AccountType != UserTypeOptions.ChannelPartner)
                    htmlBuilder.AppendFormat(@"<a href=""RoleRightManage.aspx?sysId={0}&userId={1}&name={2}&ReturnUrl={3}"" name=""right"">操作权限</a>", SystemID, user.ID, user.Account, ReturnUrl);
                if (loginService.AdminSystems.Exists(a => a.ID == URBasicInfoService.REPORT_SYS_ID))
                {
                    htmlBuilder.AppendFormat(@"<a href=""RoleSoftRightManage.aspx?userId={0}&name={1}&ReturnUrl={2}"" name=""right"">产品权限</a>", user.ID, user.Account, ReturnUrl);
                    if (user.AccountType == UserTypeOptions.Channel || user.AccountType == UserTypeOptions.ChannelPartner)
                        htmlBuilder.AppendFormat(@"<a href=""ChannelRightDetail.aspx?userId={0}&name={1}&ReturnUrl={2}"" name=""right"">渠道权限</a>", user.ID, user.Account, ReturnUrl);
                }
                return htmlBuilder.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region 获取所有用户信息(ExportToExcel)

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        private void ExportToExcel()
        {
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            string fileName = string.Format("Users[{0:yyyyMMdd}].xls", DateTime.Now);
            if (Request.UserAgent.ToLower().IndexOf("msie") > -1)
            {
                fileName = HttpUtility.UrlPathEncode(fileName);
            }
            if (Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            }
            else
            {
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            }
            Dictionary<int, HashSet<int>> userSofts;
            List<Soft> allSofts;
            List<User> users = new URBasicInfoService().GetReportUsers(out userSofts, out allSofts);
            Response.Write(@"<table border=""1"" cellpadding=""0"" cellspacing=""0"">");
            Response.Write(@"<tr style=""text-align:center;font-weight:bold;""><td>ID</td><td>用户名</td><td>真实姓名</td><td>类型</td><td>邮箱</td><td>部门</td><td>最后登录时间</td><td>状态</td>");
            foreach (Soft soft in allSofts)
            {
                Response.Write(string.Format("<td>{0}</td>", soft.Name));
            }
            Response.Write("</tr>");
            foreach (User user in users)
            {
                Response.Write(string.Format(@"<tr style=""text-align:center;""><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6:yyyy-MM-dd HH:mm:ss}</td><td>{7}</td>"
                                            , user.ID
                                            , user.Account
                                            , user.TrueName
                                            , URBasicInfoService.GetUserTypeDescipt(user.AccountType)
                                            , user.Email
                                            , user.Department
                                            , user.LastLoginTime
                                            , user.Status == StatusOptions.Valid ? "启用" : "禁用"));
                foreach (Soft soft in allSofts)
                {
                    if (userSofts.ContainsKey(user.ID) && userSofts[user.ID].Contains(soft.ID))
                        Response.Write("<td>√</td>");
                    else
                        Response.Write("<td></td>");
                }
                Response.Write("</tr>");
            }
            Response.Write("</table>");
            Response.Flush();
        }

        #endregion
    }
}