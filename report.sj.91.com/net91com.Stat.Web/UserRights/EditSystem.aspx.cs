using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using net91com.Core.Extensions;
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class EditSystem : URBasePage  
    {
        protected URBasicInfoService biService = new URBasicInfoService();

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {
            if (SystemID > 0)
            {
                try
                {
                    SystemInfo system = biService.GetSystem(SystemID);
                    if (system != null)
                    {
                        txtId.Text = system.ID.ToString();
                        txtName.Text = system.Name;
                        rblstatus.SelectedValue = ((int)system.Status).ToString();
                        txtDescription.Text = system.Description;
                        txtMd5Key.Text = system.Md5Key;
                        txtUrl.Text = system.Url;
                    }
                }
                catch (NotRightException)
                {
                    AlertBack("您没有权限执行此操作");
                }
            }
        }

        /// <summary>
        /// 角色ID
        /// </summary>
        protected int SystemID
        {
            get { return GetQueryString("id").ToInt32(0); }
        }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get { return string.IsNullOrEmpty(Request["ReturnUrl"]) ? "SystemManage.aspx" : Request["ReturnUrl"]; }
        }

        /// <summary>
        /// 添加/编辑角色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, System.EventArgs e)
        {
            string sysName = txtName.Text.Trim();
            if (sysName == "")
            {
                AlertBack("请输入系统名称！");
                return;
            }
            SystemInfo system = new SystemInfo
            {
                ID = SystemID,
                Description = this.txtDescription.Text,
                Name = sysName,
                AddTime = DateTime.Now,
                Status = (StatusOptions)Convert.ToInt32(rblstatus.SelectedValue),
                Url = txtUrl.Text.Trim()
            };
            try
            {
                //添加
                if (SystemID == 0)
                {
                    biService.AddSystem(system);
                    Response.Redirect(ReturnUrl);
                }
                else  //修改
                {
                    biService.UpdateSystem(system);
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