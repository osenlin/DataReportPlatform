using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace net91com.Stat.Web.Base
{
    public class MenuConfig : System.Web.UI.Page
    {
        /// <summary>
        /// 操作菜单项
        /// </summary>
        protected List<MenuItemEntity> MenuItems
        {
            get
            {
                List<MenuItemEntity> menuList = null;
                
                using (StreamReader srd = new StreamReader(Server.MapPath(".")+"/Scripts/MenuConfigs.txt"))
                {

                    string configString = srd.ReadToEnd().Trim();
                    configString = configString.Substring(configString.IndexOf("*/") + 2);
                    if (!string.IsNullOrEmpty(configString))
                    {
                        menuList = new List<MenuItemEntity>();
                        string[] menus = configString.Split(';');
                        foreach (string str in menus)
                        {
                            if (!string.IsNullOrEmpty(str.Trim()))
                            {
                                string[] mitems = str.Split(',');
                                if (mitems.Length == 4)
                                {
                                    MenuItemEntity m = new MenuItemEntity();
                                    for (int i = 0; i < 4; i++)
                                    {
                                        switch (i)
                                        {
                                            case 0: m.MenuId = Convert.ToInt32(mitems[0]); break;
                                            case 1: m.MenuName = mitems[1]; break;
                                            case 2: m.MenuUrl = mitems[2]; break;
                                            case 3: m.MenuSubId = Convert.ToInt32(mitems[3]); break;
                                        }
                                    }
                                    menuList.Add(m);
                                }
                            }
                        }
                    }
                }
                return menuList;
            }
        }
    }
    #region Menu
    /// <summary>
    /// 操作菜单实体
    /// </summary>
    public class MenuItemEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string MenuUrl { get; set; }

        /// <summary>
        /// 上级id
        /// </summary>
        public int MenuSubId { get; set; }
    }
    #endregion
}