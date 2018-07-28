using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Services.Entity;
using net91com.Core;
using net91com.Core.Extensions;
using Newtonsoft.Json;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class ChannelRightManager : URBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Request["act"])
            {
                case "GetChannelUserIds":
                    GetChannelUserIds();
                    break;
                case "AddUserChannelRight":
                    AddUserChannelRight();
                    break;
                case "DeleteUserChannelRight":
                    DeleteUserChannelRight();
                    break;
            }
            if (!Page.IsPostBack)
            {
                List<Soft> SoftList = loginService.AvailableSofts;
                ddlSoft.DataSource = SoftList;
                ddlSoft.DataTextField = "Name";
                ddlSoft.DataValueField = "ID";
                ddlSoft.DataBind();               
            }
        }

        /// <summary>
        /// 渠道树JSON
        /// </summary>
        protected string ChannelTreeJson
        {
            get
            {
                List<Channel> channels = new URChannelsService().GetChannels(CurrentSoft.ID, false);
                StringBuilder jsonBuilder = new StringBuilder("[");
                foreach (Channel chl in channels)
                {
                    jsonBuilder.AppendFormat("{{\"id\":\"{0}_{1}\",\"pId\":\"{2}_{3}\",\"name\":\"{4}\",\"val\":{5},\"type\":{6}}},"
                        , (int)chl.ChannelType, chl.ID, (int)chl.ParentChannelType, chl.ParentID, (chl.Platform == 0 ? chl.Name : chl.Name + "(" + chl.Platform + ")").Replace("\"", ""), chl.ID, (int)chl.ChannelType);
                }
                return jsonBuilder.ToString().TrimEnd(',') + "]";
            }
        }

        /// <summary>
        /// 获取当前选择的产品
        /// </summary>
        protected Soft CurrentSoft
        {
            get
            {
                if (ddlSoft.SelectedValue == string.Empty)
                    return loginService.AvailableSofts[0];
                int softId = Convert.ToInt32(ddlSoft.SelectedValue);
                Soft soft = loginService.AvailableSofts.FirstOrDefault(a => a.ID == softId);
                if (soft == null)
                    throw new NotRightException();
                return soft;
            }
        }

        private List<User> users = null;
        /// <summary>
        /// 用户列表
        /// </summary>
        protected List<User> Users
        {
            get
            {
                if (users == null)
                    users = new URBasicInfoService().GetUsersBySoft(CurrentSoft.ID, new UserTypeOptions[] { UserTypeOptions.Channel, UserTypeOptions.ChannelPartner });
                return users;
            }
        }

        /// <summary>
        /// 已选择的用户ID列表
        /// </summary>
        /// <returns></returns>
        protected void GetChannelUserIds()
        {
            int softId = Convert.ToInt32(Request["softId"]);
            int channelId = Convert.ToInt32(Request["channelId"]);
            ChannelTypeOptions channelType = (ChannelTypeOptions)Convert.ToInt32(Request["channelType"]);
            List<int> userIds = new URChannelsService().GetUserIds(softId, new ChannelRight { ChannelID = channelId, ChannelType = channelType });
            StringBuilder jsonBuilder = new StringBuilder("[");
            userIds.ForEach((a) => { jsonBuilder.AppendFormat("{0},", a); });
            string result = jsonBuilder.ToString().TrimEnd(',') + "]";
            Response.Clear();
            Response.ContentType = "text/json";
            Response.Write(result);
            Response.End();
        }

        /// <summary>
        /// 添加用户渠道权限
        /// </summary>
        protected void AddUserChannelRight()
        {
            Response.Clear();
            Response.ContentType = "text/json";
            try
            {
                int userId = Convert.ToInt32(Request["userId"]);
                int softId = Convert.ToInt32(Request["softId"]);
                int channelId = Convert.ToInt32(Request["channelId"]);
                ChannelTypeOptions channelType = (ChannelTypeOptions)Convert.ToInt32(Request["channelType"]);
                new URChannelsService().AddUserChannelRight(userId, softId, new ChannelRight { ChannelID = channelId, ChannelType = channelType });
                Response.Write("{\"state\":1}");
            }
            catch (Exception ex)
            {
                net91com.Core.Util.LogHelper.WriteException("", ex);
                Response.Write("{\"state\":0}");
            }
            Response.End();
        }

        /// <summary>
        /// 添加用户渠道权限
        /// </summary>
        protected void DeleteUserChannelRight()
        {
            Response.Clear();
            Response.ContentType = "text/json";
            try
            {
                int userId = Convert.ToInt32(Request["userId"]);
                int softId = Convert.ToInt32(Request["softId"]);
                int channelId = Convert.ToInt32(Request["channelId"]);
                ChannelTypeOptions channelType = (ChannelTypeOptions)Convert.ToInt32(Request["channelType"]);
                new URChannelsService().DeleteUserChannelRight(userId, softId, new ChannelRight { ChannelID = channelId, ChannelType = channelType });
                Response.Write("{\"state\":1}");                
            }
            catch (Exception ex)
            {
                net91com.Core.Util.LogHelper.WriteException("", ex);
                Response.Write("{\"state\":0}");
            }
            Response.End();
        }
        
        /// <summary>
        /// 软件选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSoft_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}