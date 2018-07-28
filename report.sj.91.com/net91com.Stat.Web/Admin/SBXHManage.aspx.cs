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
using net91com.Stat.Web.Base;

namespace net91com.Stat.Web.Admin
{
    public partial class SBXHManage : ReportBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loginService.HaveUrlRight();
            
            if (!X.IsAjaxRequest)
            {
                if (!Page.IsPostBack)
                {
                    
                    var platArray = new MobileOption[] { MobileOption.iPhone, MobileOption.Android, MobileOption.IPAD }.Select(p => new { PlatID = (int)p, PlatName = p.GetDescription() }).ToList();
                    Store2.DataSource = platArray;
                    Store2.DataBind();
                    comboxPlats.SelectedItem.Value = platArray[0].PlatID.ToString();
                    isSearch.Text = "1";

                }
            }
        }

        protected void MyData_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            int count=0;
            hiddenplat.Text = comboxPlats.SelectedItem.Value;
            hiddensbxh.Text = txtsbxh.Text.Trim();
            hiddensbxhname.Text = txtsbxhname.Text.Trim();
            ///是查询
            if (isSearch.Text == "1")
            {
                
                var list = Sjqd_SBXHService.GetSjqd_SBXHList(Convert.ToInt32(hiddenplat.Text), hiddensbxhname.Text, hiddensbxh.Text, 0, pagecut.PageSize + 1, out count);
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
                var list = Sjqd_SBXHService.GetSjqd_SBXHList(Convert.ToInt32(hiddenplat.Text), hiddensbxhname.Text, hiddensbxh.Text, IndexSize, pagecut.PageSize, out count);
                Store3.DataSource = list;
                Store3.DataBind();
                (Store3.Proxy[0] as PageProxy).Total = count;
            }
        }
        protected void HandleChanges(object sender, BeforeStoreChangedEventArgs e)
        {
            ChangeRecords<Sjqd_SBXH> list = e.DataHandler.ObjectData<Sjqd_SBXH>();
            foreach (Sjqd_SBXH updated in list.Updated)
            {
                Sjqd_SBXHService.UpdateSBXH(updated);
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