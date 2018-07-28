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
using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Admin
{

    public partial class SoftVersionManage : ReportBasePage
    {
        /// <summary>
        /// 可访问的产品列表
        /// </summary>
        protected override List<Soft> AvailableSofts
        {
            get { return loginService.AvailableSofts.Where(a => a.ID > 0).ToList(); }
        }
       
        protected void Page_Load(object sender, EventArgs e)
        {

            loginService.HaveUrlRight();
          
            if (!X.IsAjaxRequest)
            {
                if (!Page.IsPostBack)
                {
                    Store1.DataSource = AvailableSofts; 
                    Store1.DataBind();
                    comboxSofts.SelectedIndex = 0;
                    var platList = AvailableSofts[0].Platforms.Where(a => a != MobileOption.IPAD && a != MobileOption.AndroidPad).Select(p => new { PlatID = (int)p, PlatName = p.GetDescription() }).ToList();
                    Store2.DataSource = platList;
                    Store2.DataBind();
                    comboxSofts.SelectedItem.Value = AvailableSofts[0].ID.ToString();
                    comboxPlats.SelectedItem.Value = platList[0].PlatID.ToString();
                    isSearch.Text = "1";
                    //添加版本增加的代码
                    Store4.DataSource = AvailableSofts;
                    Store4.DataBind();
                    add_comboxSofts.SelectedIndex = 0;
                    Store5.DataSource = platList;
                    Store5.DataBind();
                    add_comboxSofts.SelectedItem.Value = AvailableSofts[0].ID.ToString();
                    add_comboxPlats.SelectedItem.Value = platList[0].PlatID.ToString();
                }
            }
         
        }
        [DirectMethod]
        public void SetPlat()
        {
            var plats = AvailableSofts.Find(p => p.ID == Convert.ToInt32(comboxSofts.SelectedItem.Value)).Platforms.Where(a => a != MobileOption.IPAD && a != MobileOption.AndroidPad).Select(p => new { PlatID = (int)p, PlatName = p.GetDescription() }).ToList();
            Store2.DataSource = plats;
            Store2.DataBind();
            comboxPlats.SelectedItem.Value = plats[0].PlatID.ToString();
            
            
        }

        [DirectMethod]
        public void SetPlat2()
        {
            var plats = AvailableSofts.Find(p => p.ID == Convert.ToInt32(add_comboxSofts.SelectedItem.Value)).Platforms.Where(a => a != MobileOption.IPAD && a != MobileOption.AndroidPad).Select(p => new { PlatID = (int)p, PlatName = p.GetDescription() }).ToList();
            Store5.DataSource = plats;
            Store5.DataBind();
            add_comboxPlats.SelectedItem.Value = plats[0].PlatID.ToString();
        }

        protected void MyData_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            //是查询
            if (isSearch.Text == "1")
            {
                hiddenplat.Text = comboxPlats.SelectedItem.Value;
                hiddensoft.Text = comboxSofts.SelectedItem.Value;
                hiddenversion.Text = txtversion.Text.Trim();
                var list = Sjqd_SoftVersionsService.GetSoftVersions(Convert.ToInt32(hiddensoft.Text), Convert.ToInt32(hiddenplat.Text), hiddenversion.Text);
                Store3.DataSource = list.Take(this.pagecut.PageSize + 1);
                Store3.DataBind();
                (Store3.Proxy[0] as PageProxy).Total = list.Count();
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
                var list = Sjqd_SoftVersionsService.GetSoftVersions(Convert.ToInt32(hiddensoft.Text), Convert.ToInt32(hiddenplat.Text), hiddenversion.Text);
                Store3.DataSource = list.Skip(IndexSize).Take(PageSize);
                Store3.DataBind();
                (Store3.Proxy[0] as PageProxy).Total = list.Count();
            }
        }

       

        protected void HandleChanges(object sender, BeforeStoreChangedEventArgs e)
        {
            ChangeRecords<Sjqd_SoftVersions> list = e.DataHandler.ObjectData<Sjqd_SoftVersions>();
            foreach (Sjqd_SoftVersions updated in list.Updated)
            {
                Sjqd_SoftVersionsService.UpdateSoftVersion(updated);
                if (Store3.UseIdConfirmation)
                {
                    e.ConfirmationList.ConfirmRecord(updated.ID.ToString());
                }
            }
            X.Msg.Alert("消息", "保存成功").Show();
            e.Cancel = true;
           
            
        }

        protected void OnSave(object sender, DirectEventArgs e)
        {
            
             int softid = Convert.ToInt32(add_comboxSofts.SelectedItem.Value);
            int platform = Convert.ToInt32(add_comboxPlats.SelectedItem.Value);
            string version = add_txtVersion.Value.ToString();
            bool isHidden = add_HiddenCheckbox.Checked;
            Sjqd_SoftVersions newVersion = new Sjqd_SoftVersions
            {
                SoftID = softid,
                Platform = platform,
                Version = version,
                Hidden = isHidden
            };
            int versionId = Sjqd_SoftVersionsService.AddSoftVersion(newVersion);
            if (versionId >0)
            {
                e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "1", false));
                //添加成功后在下方显示 
                X.Msg.Alert("消息", "添加成功").Show();
            }
            else
            {
                e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "0", false));
                X.Msg.Alert("消息", "添加失败").Show();
            }
        }

        /// <summary>
        /// 删除版本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnCommand(object sender, DirectEventArgs e)
        {
            if (e.ExtraParams["Command"].ToString() == "delete")
            {
                int id = Convert.ToInt32(e.ExtraParams["ID"]);
                //传入id 进行删除
                if (Sjqd_SoftVersionsService.DeleteSoftVersion(id) > 0)
                {
                    X.Msg.Alert("消息", "删除成功").Show();
                    e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "1", false));

                }
                else
                {
                    X.Msg.Alert("消息", "删除失败").Show();
                    e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "0", false));
                }
            }


        }

       

        
      
        

    }
}