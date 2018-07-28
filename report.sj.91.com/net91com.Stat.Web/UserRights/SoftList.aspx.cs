using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

using net91com.Core;
using net91com.Core.Util;
using net91com.Core.Extensions;
using net91com.Reports.UserRights;
using Ext.Net;
using Ext.Net.Utilities;

namespace net91com.Stat.Web.UserRights
{
    public partial class SoftList : URBasePage 
    {
        protected URBasicInfoService biService = new URBasicInfoService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (!IsPostBack)
                {
                    SelectSoftType.SelectedItem.Value = "1";
                    isSearch.Text = "1";
                    platChecksGroup.Items.Clear();
                    foreach (MobileOption platform in new MobileOption[] { MobileOption.iPhone, MobileOption.Android, MobileOption.IPAD } )
                    {
                        var check = new Ext.Net.Checkbox(false, platform.ToString());
                        check.RawValue = (int)platform;
                        check.Checked = false;
                        check.ID = "plat_" + (int)platform;
                        check.Width = 90;
                        platChecksGroup.Items.Add(check);
                    }
                    btnSave.Disabled = true;
                }
            }
        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            int SoftID = Convert.ToInt32(e.ExtraParams["SoftID"]);
            var softinfo = biService.GetSoft(SoftID);
            string plats="";
            if (softinfo != null)
            {
                softType.SelectedItem.Value = ((int)softinfo.SoftType).ToString();
                SoftName.Text = softinfo.Name;
                SoftOutID.Text = softinfo.OutID.ToString();
                SoftPID.Text = softinfo.ID.ToString();
                softinfo.Platforms.ForEach((i) => { plats += i.ToString() + ","; });
                plats = plats.TrimEnd(',');
                SortNumID.Text = softinfo.SortIndex.ToString();
                onlyinternalselect.SelectedIndex = softinfo.OnlyInternal ? 0 : 1;
                softareatype.Text = softinfo.SoftAreaType.ToString();
            }
            e.ExtraParamsResponse.Add(new Ext.Net.Parameter("plats", plats));

        }
        /// <summary>
        /// 保存的结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnSave(object sender, DirectEventArgs e)
        {
            EditSoft(false, e);
        }

        protected void AddSoft(object sender, DirectEventArgs e)
        {
            EditSoft(true, e);
        }

        private void EditSoft(bool adding, DirectEventArgs e)
        {
            try
            {
                Soft soft = new Soft();
                string plats = e.ExtraParams[0].Value.TrimEnd('-');
                soft.Platforms = plats.Split('-').Select(p => (MobileOption)Convert.ToInt32(p)).ToList();
                soft.Name = SoftName.Text;
                soft.SoftType = (SoftTypeOptions)Convert.ToInt32(softType.SelectedItem.Value);
                soft.OutID = Convert.ToInt32(SoftOutID.Text);
                soft.ID = Convert.ToInt32(SoftPID.Text == "" ? "0" : SoftPID.Text);
                soft.SortIndex = Convert.ToInt32(SortNumID.Text);
                soft.Status = StatusOptions.Valid;
                soft.OnlyInternal = Convert.ToInt32(onlyinternalselect.SelectedItem.Value) > 0 ? true : false;
                soft.SoftAreaType = Convert.ToInt32(softareatype.SelectedItem.Value);
                if (adding)
                    biService.AddSoft(soft);
                else
                    biService.UpdateSoft(soft);
                e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "1", false));
                X.Msg.Alert("消息", "操作成功").Show();
            }
            catch (ToUserException ex)
            {
                e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "0", false));
                X.Msg.Alert("消息", ex.Message).Show();
            }
        }

        protected void MyData_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            int PageSize = this.pagecut.PageSize;
            int IndexSize = Convert.ToInt32(e.Parameters["start"]);
            if (IndexSize == -1)
            {
                IndexSize = 0;
            }
            int pageIndex = IndexSize / PageSize + 1;
            int recordCount = 0;
            List<Soft> list = biService.GetSofts((SoftTypeOptions)Convert.ToInt32(SelectSoftType.SelectedItem.Value), txtsoftName.Text, pageIndex, PageSize, ref recordCount);
            Store3.DataSource = list;
            Store3.DataBind();
            (Store3.Proxy[0] as PageProxy).Total = recordCount;
        }

        protected void OnCommand(object sender, DirectEventArgs e)
        {
            int SoftID = Convert.ToInt32(e.ExtraParams["SoftID"].ToString());
            biService.DeleteSoft(SoftID);
            e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "1", false));
            X.Msg.Alert("消息", "删除成功").Show();
        }
    }
}