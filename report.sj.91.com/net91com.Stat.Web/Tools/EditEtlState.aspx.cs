using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using net91com.Core.Extensions;
using net91com.Reports.UserRights;
using net91com.Reports.Tools;

namespace net91com.Stat.Web.Tools
{
    public partial class EditEtlState : ToolPageBase
    {
        protected TEtlStatesService esService = new TEtlStatesService();

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            //必须超管才能编辑
            loginService.HaveSuperAdminRight();

            if (!IsPostBack)
            {
                ddlTypes.DataSource = esService.GetEtlStateTypes();
                ddlTypes.DataTextField = "Value";
                ddlTypes.DataValueField = "Key";
                ddlTypes.DataBind();
                if (!string.IsNullOrEmpty(Request["type"]))
                {
                    ddlTypes.SelectedValue = Request["type"];
                    ddlTypes.Enabled = false;
                }
                BindData();
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {
            if (StateID > 0)
            {
                EtlState state = esService.GetEtlState(StateID);
                if (state != null)
                {
                    txtKey.Text = state.Key;
                    txtValue.Text = state.Value;
                    ddlTypes.SelectedValue = ((int)state.Type).ToString();
                    txtDescription.Text = state.Description;
                }
            }
        }

        /// <summary>
        /// 角色ID
        /// </summary>
        protected int StateID
        {
            get { return Convert.ToInt32(Request["id"]); }
        }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get { return string.IsNullOrEmpty(Request["ReturnUrl"]) ? "EtlStateManage.aspx" : Request["ReturnUrl"]; }
        }

        /// <summary>
        /// 添加/编辑角色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, System.EventArgs e)
        {
            string key = txtKey.Text.Trim();
            if (key == "")
            {
                AlertBack("请输入系统名称！");
                return;
            }
            EtlState state = new EtlState
            {
                Key = key,
                Description = txtDescription.Text,
                Value = txtValue.Text.Trim(),
                AddTime = DateTime.Now,
                Type = (EtlStateTypeOptions)Convert.ToInt32(ddlTypes.SelectedValue)
            };
            try
            {
                //添加
                if (StateID == 0)
                {
                    esService.AddEtlState(state);
                    Response.Redirect(ReturnUrl);
                }
                else  //修改
                {
                    state.ID = StateID;
                    esService.UpdateEtlState(state);
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