using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class AddUser : URBasePage 
    {
        protected URBasicInfoService biService = new URBasicInfoService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected int UserID
        {
            get { return Convert.ToInt32(GetQueryString("un", "0")); }
        }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get { return string.IsNullOrEmpty(Request["ReturnUrl"]) ? "UserManage.aspx" : Request["ReturnUrl"]; }
        }

        private void BindData()
        {
            if (UserID > 0)
            {
                try
                {
                    User user = biService.GetUser(UserID);
                    if (user != null)
                    {
                        txtName.Text = user.Account;
                        rblStatus.SelectedValue = ((int)user.Status).ToString();
                        rblType.SelectedValue = ((int)user.AccountType).ToString();
                        TxtTrueName.Text = user.TrueName;
                        beginTime.Text = user.BeginTime.ToString("yyyy-MM-dd");
                        endTime.Text = user.EndTime.ToString("yyyy-MM-dd");
                        ckbspecialuser.Checked = user.IsSpecialUser;
                        txtemail.Text = user.Email;
                        txtDept.Text = user.Department;
                        chbwhiteuser.Checked = user.IsWhiteUser;
                    }
                }
                catch (NotRightException)
                {
                    AlertBack("您没有权限执行此操作");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {           
            string userName = txtName.Text.Trim();
            if (string.IsNullOrEmpty(userName))
            {
                AlertBack("请输入用户名");
                return;
            }
            if (userName.Length < 6)
            {
                AlertBack("用户名的长度不能小于6个字符");
                return;
            }
            if (string.IsNullOrEmpty(TxtTrueName.Text.Trim()))
            {
                AlertBack("请输入真实姓名");
                return;
            }
            User user = new User()
            {
                ID = UserID,
                Account = userName,
                TrueName = TxtTrueName.Text.Trim(),
                AccountType = (UserTypeOptions)Convert.ToInt32(rblType.SelectedValue),
                Status = (StatusOptions)Convert.ToInt32(rblStatus.SelectedValue),
                BeginTime = Convert.ToDateTime(beginTime.Text),
                EndTime = Convert.ToDateTime(endTime.Text),
                IsSpecialUser = ckbspecialuser.Checked,
                Email = txtemail.Text.Trim(),
                IsWhiteUser = chbwhiteuser.Checked,
                Department = txtDept.Text.Trim()
            };
            try
            {
                //添加
                if (UserID == 0)
                {
                    biService.AddUser(user);
                    Response.Redirect(ReturnUrl);
                }
                else  //修改
                {
                    biService.UpdateUser(user);
                    Response.Redirect(ReturnUrl);
                }
            }
            catch (NotRightException)
            {
                AlertBack("您没有权限执行此操作");
            }
            catch (net91com.Core.ToUserException ex)
            {
                AlertBack(ex.Message);
            }
        }
    }
}