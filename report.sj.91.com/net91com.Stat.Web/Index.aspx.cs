using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using net91com.Stat.Web.Base;
using System.Data;
//using BY.AccessControlCore;
using System.Web.Security;

using Ext.Net;
using Ext.Net.Utilities;
using net91com.Stat.Services;
using net91com.Stat.Services.Entity;
using net91com.Reports.UserRights; 

namespace net91com.Stat.Web
{
    public partial class Index : Page //StatPageBase
    {
        public string url = "";
        public string id = "";
        public string title = "";
        public string menuitemid = "";
        public string menupanleid = "";
        public string aboatusurl = "Reports_New/R_Other/AboatUs.aspx";
        //string systemkey = "892e200b6e89476d8c3e4e4c3759d2ac";

        private URLoginService loginService = new URLoginService();
        private User curUser;
       
        protected string TreeNodeString
        {
            get 
            {
                List<object> list = new List<object>();
                var rights = loginService.AvailableRights.Where(a => a.RightLevel == 1);
                for (int i = 0; i < rights.Count(); i++)
                {
                    Right right = rights.ElementAt(i);
                    list.Add(new { ID = right.ID.ToString(), Title = right.Name, pid = right.ID.ToString(), url = right.PageUrl });
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(list);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            curUser = loginService.LoginUser;

            label1.Text = "欢迎您：" + (string.IsNullOrEmpty(curUser.TrueName) ? curUser.Account : curUser.TrueName);

            //渠道内部用户及渠道合作方不能查看"关于我们"这个页面
            if (curUser.AccountType == UserTypeOptions.Channel || curUser.AccountType == UserTypeOptions.ChannelPartner)
            {
                btnaboatus.Visible = false;
                aboatusurl = "";
            }
            if (!X.IsAjaxRequest)
            {
                
                List<Soft> softs;
                try
                {
                    softs = loginService.AvailableSofts;
                }
                catch (NotRightException)
                {
                    softs = new List<Soft>();
                }
                var topRights = loginService.AvailableRights.Where(a => a.RightLevel == 0);
                for (int i = 0; i < topRights.Count(); i++)
                {
                    Right topRight = topRights.ElementAt(i);
                    MenuPanel mp = new MenuPanel();
                    mp.ID = "mp" + topRight.ID.ToString();
                    mp.Title = topRight.Name;
                    mp.Icon = Icon.BulletRight;
                    mp.Width = 200;
                    mp.AutoScroll = true;
                    var subRights = loginService.AvailableRights.Where(a => a.ParentID == topRight.ID);
                    for (int j = 0; j < subRights.Count(); j++)
                    {
                        Right subRight = subRights.ElementAt(j);
                        //如果是装机商带来量的报表,则必须要求有助手的权限
                        if (subRight.PageUrl.ToLower() == "reports/newuserbymac.aspx")
                        {
                            int[] array = { 68, 69, -9, 58, 9, 57, 60, 61, 71 };
                            if (!softs.Exists(a => array.Contains(a.ID))) continue;
                        }

                        Ext.Net.MenuItem mi = new Ext.Net.MenuItem();
                        mi.ID = subRight.ID.ToString();
                        mi.Text = subRight.Name;
                        mi.Icon = Icon.BulletStart;
                        if (string.IsNullOrEmpty(url))
                        {
                            url = subRight.PageUrl;
                            title = subRight.Name;
                            id = subRight.ID.ToString();
                            menupanleid = mp.ID;
                        }
                        
                        //唯一标识、访问链接地址、显示名称
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','{1}','{2}',this,{3});", subRight.ID, subRight.PageUrl, subRight.Name, mp.ID);
                        mp.Menu.Items.Add(mi);
                    }
                    Panel1.Items.Add(mp);
                }

                #region 系统管理

                UserTypeOptions accountType = loginService.LoginUser.AccountType;
                List<SystemInfo> adminSystems = loginService.AdminSystems;
                if ((accountType == UserTypeOptions.SuperAdmin || accountType == UserTypeOptions.Admin
                    || accountType == UserTypeOptions.ProductAdmin) && adminSystems.Count > 0)
                {
                    MenuPanel mp = new MenuPanel();
                    mp.ID = "SysManage";
                    mp.Title = "系统管理";
                    mp.Icon = Icon.BulletRight;
                    mp.Width = 200;
                    mp.AutoScroll = true;
                    //用户管理
                    Ext.Net.MenuItem mi = new Ext.Net.MenuItem();
                    mi.ID = "UserManage";
                    mi.Text = "用户管理";
                    mi.Icon = Icon.BulletStart;
                    mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/UserManage.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                    mp.Menu.Items.Add(mi);
                    if (accountType == UserTypeOptions.SuperAdmin)
                    {
                        //系统管理
                        mi = new Ext.Net.MenuItem();
                        mi.ID = "SystemManage";
                        mi.Text = "系统管理";
                        mi.Icon = Icon.BulletStart;
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/SystemManage.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                        mp.Menu.Items.Add(mi);
                    }
                    if (accountType != UserTypeOptions.ProductAdmin)
                    {
                        //角色管理
                        mi = new Ext.Net.MenuItem();
                        mi.ID = "RoleManage";
                        mi.Text = "角色管理";
                        mi.Icon = Icon.BulletStart;
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/RoleManage.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                        mp.Menu.Items.Add(mi);
                    }                                      
                    if (accountType == UserTypeOptions.SuperAdmin || accountType == UserTypeOptions.Admin)
                    {
                        //权限管理
                        mi = new Ext.Net.MenuItem();
                        mi.ID = "RightManage";
                        mi.Text = "权限管理";
                        mi.Icon = Icon.BulletStart;
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/RightManage.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                        mp.Menu.Items.Add(mi);                        
                        //产品管理
                        mi = new Ext.Net.MenuItem();
                        mi.ID = "SoftList";
                        mi.Text = "产品管理";
                        mi.Icon = Icon.BulletStart;
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/SoftList.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                        mp.Menu.Items.Add(mi);
                        Panel1.Items.Add(mp);
                        //项目来源管理
                        mi = new Ext.Net.MenuItem();
                        mi.ID = "ProjectSourceManage";
                        mi.Text = "项目来源管理";
                        mi.Icon = Icon.BulletStart;
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/ProjectSourceManage.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                        mp.Menu.Items.Add(mi);
                    }
                    //REPORT才有这个权限
                    if (adminSystems.Exists(a => a.ID == 1))
                    {
                        //渠道权限管理
                        mi = new Ext.Net.MenuItem();
                        mi.ID = "ChannelRightManager";
                        mi.Text = "渠道权限管理";
                        mi.Icon = Icon.BulletStart;
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/ChannelRightManager.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                        mp.Menu.Items.Add(mi);
                    }
                    if (accountType != UserTypeOptions.ProductAdmin)
                    {
                        //操作日志
                        mi = new Ext.Net.MenuItem();
                        mi.ID = "Log";
                        mi.Text = "操作日志";
                        mi.Icon = Icon.BulletStart;
                        mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/Log.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                        mp.Menu.Items.Add(mi);                        
                    }
                    //操作日志
                    mi = new Ext.Net.MenuItem();
                    mi.ID = "RightSysDescript";
                    mi.Text = "权限说明";
                    mi.Icon = Icon.BulletStart;
                    mi.Listeners.Click.Handler += string.Format("addTab(#{{TabPanel1}},'idClt{0}','UserRights/RightSysDescript.aspx','{1}',this,{2});", mi.ID, mi.Text, mp.ID);
                    mp.Menu.Items.Add(mi);
                    Panel1.Items.Add(mp);
                }                

                #endregion
            }
        }

        protected void FilterSpecialKey(object sender, EventArgs e)
        {
            X.Msg.Alert("xxx", "测试消息 FilterSpecialKeyu");
        }

        protected void TxtFieldKeyUp(object sender, EventArgs e)
        {
            
            Window wind = new Window(new Window.Config {
                Title = "测试 TxtSearchItme Window",
                Width = 200,
                Height = 450,
                BodyStyle = "background-color: #fff;" ,
                Padding = 5,
                Html = "看看看。。。。"
                
            });
            wind.ID = "windstest";
            X.WindowManager.BringToFront(wind);

            wind.Show(this);
           
          
        }
        protected void BtnExit(object sender, DirectEventArgs e)
        {
            X.Msg.Confirm("Message", "确定退出吗?", new MessageBoxButtonsConfig
            {
                Yes = new MessageBoxButtonConfig
                {
                    Handler = "CompanyX.DoYes()",
                    Text = "是"
                },
                No = new MessageBoxButtonConfig
                {
                    Text = "否"
                }
            }).Show();
           
        }

        [DirectMethod]
        public void DoYes()
        {
            loginService.Logout();
            Response.Redirect("Login.aspx");

        }
    }
}