using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Core;
using net91com.Core.Extensions;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services.sjqd;
using Ext.Net;
using Ext.Net.Utilities;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Core.Util;



namespace net91com.Stat.Web.Admin
{
    public partial class LanguageManage : Stat.Web.Base.ReportBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();


            if (!X.IsAjaxRequest)
            {
                if (!Page.IsPostBack)
                {
                    isSearch.Text = "1";
                 }
            }
            
        }

        protected void MyData_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            int count = 0;
            ///是查询
            if (isSearch.Text == "1")
            {
                
                hiddenlan.Text = txtlan.Text.Trim();
                hiddenlanname.Text = txtlanname.Text.Trim();
                var list = Sjqd_LanService.GetSjqd_LanList(hiddenlan.Text, hiddenlanname.Text, 0, pagecut.PageSize, out count);
                Store3.DataSource = list;
                Store3.DataBind();
                (Store3.Proxy[0] as PageProxy).Total = count;
                isSearch.Text = "0";
            }
            else
            {
                int PageSize = this.pagecut.PageSize;
                int IndexSize = e.Start;
                if (IndexSize == -1)
                {
                    IndexSize = 0;
                }
                var list = Sjqd_LanService.GetSjqd_LanList(hiddenlan.Text, hiddenlanname.Text, IndexSize, pagecut.PageSize, out count);
                Store3.DataSource = list;
                Store3.DataBind();
                (Store3.Proxy[0] as PageProxy).Total = count;
            }
        }
        protected void HandleChanges(object sender, BeforeStoreChangedEventArgs e)
        {
            ChangeRecords<Sjqd_Lan> list = e.DataHandler.ObjectData<Sjqd_Lan>();
            foreach (Sjqd_Lan updated in list.Updated)
            {
                Sjqd_LanService.UpdateLan(updated);
                if (Store3.UseIdConfirmation)
                {
                    e.ConfirmationList.ConfirmRecord(updated.ID.ToString());
                }
            }
            X.Msg.Alert("消息", "保存成功").Show();
            e.Cancel = true;


        }
    }
}