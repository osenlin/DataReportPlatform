using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;

using net91com.Reports.UserRights;
using net91com.Core.Extensions;

namespace net91com.Stat.Web.UserRights
{
    public partial class EditRight : URBasePage 
    {
        protected URBasicInfoService biService = new URBasicInfoService();

        /// <summary>
        /// 当前选择的权限对应的ID
        /// </summary>
        protected int RightID
        {
            get { return GetQueryString("rkey").ToInt32(0); }
        }

        /// <summary>
        /// 权限级别
        /// </summary>
        protected int Level
        {
            get { return GetQueryString("level").ToInt32(0); }
        }

        /// <summary>
        /// 当前选择的权限对应的父ID
        /// </summary>
        protected int ParentRightID
        {
            get { return GetQueryString("parentKey").ToInt32(0); }
        }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get { return string.IsNullOrEmpty(Request["ReturnUrl"]) ? "RightManage.aspx" : Request["ReturnUrl"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        #region 绑定数据(BindData)

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {
            switch (Level)
            {
                case -1:
                    rblType.SelectedValue = "0";
                    break;
                case 0:
                    rblType.SelectedValue = "1";
                    break;
                case 1:
                    rblType.SelectedValue = "2";
                    break;
            }
            if (Level == -1 || RightID > 0)
            {
                rblType.Enabled = false;
            }
            else
            {
                rblType.Items[0].Enabled = false;
            }
            ViewState["SorIndex"] = "-1";
            if (ParentRightID > 0 || Level == -1)
            {
                ddlSortIndex.DataSource = biService.GetRights(GetQueryString("sysId").ToInt32(0), ParentRightID);
                ddlSortIndex.DataTextField = "Name";
                ddlSortIndex.DataValueField = "SortIndex";
                ddlSortIndex.DataBind();
                txtURL.Enabled = true;
            }
            else if (RightID > 0)
            {
                Right right = biService.GetRight(RightID);
                int pkey = right.ParentID;
                txtName.Text = right.Name;
                rblType.SelectedValue = ((int)right.RightType).ToString();
                txtDescription.Text = right.Description;
                rblStatus.SelectedValue = ((int)right.Status).ToString();
                rblOnlyInternal.SelectedValue = right.OnlyInternal ? "1" : "0";
                txtURL.Text = right.PageUrl;
                ViewState["SorIndex"] = right.SortIndex.ToString();
                txtURL.Enabled = false;
                ViewState["ParentKey"] = pkey.ToString();

                foreach (Right r in biService.GetRights(GetQueryString("sysId").ToInt32(0), pkey))
                {
                    if (r.ID != RightID)
                    {
                        ListItem item = new ListItem(r.Name, r.SortIndex.ToString());
                        ddlSortIndex.Items.Add(item);
                    }
                }  
                ddlSortIndex.Items.Insert(0, new ListItem("保持不变", "-1"));
                ddlSortIndex.SelectedValue = "-1";
            }
            if (ddlSortIndex.Items.Count == 0)
            {
                ddlSortIndex.Enabled = false;
                ddlSortIndex.Items.Insert(0, new ListItem("-==无==-", "0"));
                rblPosition.Enabled = false;
            }
            else
            {
                if (RightID == 0)
                {
                    ddlSortIndex.SelectedIndex = ddlSortIndex.Items.Count - 1;
                }
            }
        }

        #endregion

        #region 创建模块(CreateRight)

        /// <summary>
        /// 创建模块
        /// </summary>
        private void CreateRight()
        {
            int rightLevel = Convert.ToInt32(rblType.SelectedValue);
            string rightName = txtName.Text.Trim();
            if (rightName == "")
            {
                AlertBack("请输入模块名称！");
                return;
            }
            string rightUrl = txtURL.Text.Trim();
            if (rightLevel == 1 && rightUrl == "")
            {
                AlertBack("请输入模块地址！");
                return;
            }
            
            int sortIndex = 0;
            if (ddlSortIndex.Enabled)
            {
                if (rblPosition.SelectedValue == "1")
                {
                    //之后
                    sortIndex = Convert.ToInt32(ddlSortIndex.SelectedValue) + 1;
                }
                else
                {
                    //之前
                    sortIndex = Convert.ToInt32(ddlSortIndex.SelectedValue);
                }
            }
            biService.AddRight(
                new Right
                {
                    RightLevel = rightLevel,
                    Description = txtDescription.Text.Trim(),
                    AddTime = DateTime.Now,
                    LastUpdateTime = DateTime.Now,
                    Name = rightName,
                    RightType = (RightTypeOptions)Convert.ToInt32(rblType.SelectedValue),
                    SortIndex = sortIndex,
                    Status = (StatusOptions)Convert.ToInt32(rblStatus.SelectedValue),
                    PageUrl = rightUrl,
                    ParentID = ParentRightID,
                    SystemID = GetQueryString("sysId").ToInt32(0),
                    OnlyInternal = rblOnlyInternal.SelectedValue == "1" ? true : false
                });

            Response.Redirect(ReturnUrl);

        }

        #endregion

        #region 修改模块(UpdateRight)

        /// <summary>
        /// 修改模块
        /// </summary>
        private void UpdateRight()
        {
            int rightLevel = Convert.ToInt32(rblType.SelectedValue);
            string rightName = txtName.Text.Trim();
            if (rightName == "")
            {
                AlertBack("请输入模块名称！");
                return;
            }
            string rightDescription = txtDescription.Text.Trim();
            string rightUrl = txtURL.Text.Trim();
            if (rightLevel == 1 && rightUrl == "")
            {
                AlertBack("请输入模块地址！");
                return;
            }
            // 原序号
            int sortIndex = Convert.ToInt32(ViewState["SorIndex"]);
            if (ddlSortIndex.SelectedValue != "-1")
            {
                // 新序号
                int sortIndex_new = Convert.ToInt32(ddlSortIndex.SelectedValue);
                if (rblPosition.SelectedValue == "1")
                {
                    // 新序号之后+1
                    if (sortIndex_new < sortIndex)
                    {
                        sortIndex_new++;
                    }
                }
                else
                {
                    // 新序号之前-1
                    if (sortIndex_new > sortIndex)
                    {
                        sortIndex_new--;
                    }
                }
                sortIndex = sortIndex_new;
            }

            biService.UpdateRight(
                new Right
                {
                    ID = RightID,
                    RightLevel = rightLevel,
                    Description = txtDescription.Text.Trim(),
                    AddTime = DateTime.Now,
                    LastUpdateTime = DateTime.Now,
                    Name = rightName,
                    RightType = (RightTypeOptions)Convert.ToInt32(rblType.SelectedValue),
                    SortIndex = sortIndex,
                    Status = (StatusOptions)Convert.ToInt32(rblStatus.SelectedValue),
                    PageUrl = rightUrl,
                    ParentID = ParentRightID,
                    SystemID = GetQueryString("sysId").ToInt32(0),
                    OnlyInternal = rblOnlyInternal.SelectedValue == "1" ? true : false
                });

            Response.Redirect(ReturnUrl);
        }

        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (RightID == 0)
                CreateRight();
            else
                UpdateRight();

        }
    }
}