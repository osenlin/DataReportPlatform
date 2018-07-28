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

namespace net91com.Stat.Web.UserRights
{
    public partial class Service : URBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string result = "{\"Message\":\"操作成功\",\"Code\":0}";
            try
            {
                switch (GetQueryString("act").ToLower())
                {
                    case "dright":
                        DeleteRight();
                        break;
                    case "drole":
                        DeleteRole();
                        break;
                    case "deleus":
                        DeleteUser();
                        break;
                    case "usstatus":
                        UpdateUserStatus();
                        break;
                    case "dsystem":
                        DeleteSystem();
                        break;
                    //case "usacttype":
                    //    UpdateUserAccountType();
                    //    break;
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

        //#region 修改用户账号类型

        ///// <summary>
        ///// 修改用户账号类型
        ///// </summary>
        //private void UpdateUserAccountType()
        //{
        //    int id = GetQueryString("uid").ToInt32(0);
        //    UserTypeOptions accountType = (UserTypeOptions)GetQueryString("st").ToInt32(0);
        //    new URBasicInfoService().UpdateAccountType(id, accountType);
        //}

        //#endregion

        #region 修改用户状态

        /// <summary>
        /// 修改用户状态
        /// </summary>
        private void UpdateUserStatus()
        {
            int id = GetQueryString("uid").ToInt32(0);
            StatusOptions status = (StatusOptions)GetQueryString("st").ToInt32(0);
            new URBasicInfoService().UpdateUserStatus(id, status);
        }

        #endregion

        #region 删除用户

        /// <summary>
        /// 删除用户
        /// </summary>
        private void DeleteUser()
        {
            int id = GetQueryString("rid").ToInt32(0);
            new URBasicInfoService().DeleteUser(id);
        }

        #endregion

        #region 删除角色

        /// <summary>
        /// 删除角色
        /// </summary>
        private void DeleteRole()
        {
            int id = GetQueryString("rid").ToInt32(0);
            new URBasicInfoService().DeleteRole(id);
        }

        #endregion

        #region 删除权限项

        /// <summary>
        /// 删除权限项
        /// </summary>
        private void DeleteRight()
        {
            int id = GetQueryString("rky").ToInt32(0);
            new URBasicInfoService().DeleteRight(id);
        }

        #endregion

        /// <summary>
        /// 删除系统
        /// </summary>
        private void DeleteSystem()
        {
            int id = GetQueryString("id").ToInt32(0);
            new URBasicInfoService().DeleteSystem(id);
        }
    }
}