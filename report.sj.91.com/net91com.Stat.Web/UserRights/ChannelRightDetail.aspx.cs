using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;
using System.Text;
using net91com.Stat.Services.Entity;
//using BY.AccessControlCore;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.UserRights
{
    public partial class ChannelRightDetail : URBasePage
    {     
        protected string nodeStr;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request["userId"]))
                throw new NotRightException();

            int userId = int.Parse(Request["userId"]);
            if (Request["act"] == "getchannels")
            {
                Response.ContentType = "text/json";                
                int softId = int.Parse(Request["softId"]);
                string json = BindData(userId, softId);
                Response.Clear();
                Response.Write(json);
                Response.End();
            }

            UserSofts = GetUserSofts(userId);
            if (UserSofts.Count == 0)
                throw new NotRightException();

            if (!IsPostBack)
            {
                int softid = UserSofts[0].ID;
                BindData(userId, softid);
            }
        }

        protected string BindData(int _uid,int _softid)
        {
            URChannelsService channelService = new URChannelsService();
            List<Channel> channels = channelService.GetChannels(_softid, false);
            List<ChannelRight> channelRights = channelService.GetUserChannelRights(_uid, _softid);
            StringBuilder jsonBuilder = new StringBuilder("[");
            foreach (Channel chl in channels)
            {
                bool selected = channelRights.Exists(a => a.ChannelType == chl.ChannelType && a.ChannelID == chl.ID);
                jsonBuilder.AppendFormat("{{\"id\":\"{0}_{1}\",\"pId\":\"{2}_{3}\",\"name\":\"{4}\",\"val\":{5},\"type\":{6},\"checked\":{7}}},"
                    , (int)chl.ChannelType, chl.ID, (int)chl.ParentChannelType, chl.ParentID, (chl.Platform == 0 ? chl.Name : chl.Name + "(" + chl.Platform + ")").Replace("\"", ""), chl.ID, (int)chl.ChannelType, selected.ToString().ToLower());
                
            }
            nodeStr = jsonBuilder.ToString().TrimEnd(',') + "]";
            return nodeStr;
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            int userId = int.Parse(Request["userId"]);
            URChannelsService channelService = new URChannelsService();
            foreach (Soft soft in UserSofts)
            {
                List<ChannelRight> channelRights = new List<ChannelRight>();
                if (Request["hiddencateids_" + soft.ID.ToString()] != "-1")
                {
                    channelRights.AddRange(Request["hiddencateids_" + soft.ID.ToString()].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => new ChannelRight { ChannelID = Convert.ToInt32(p), ChannelType = ChannelTypeOptions.Category }).ToList());

                    channelRights.AddRange(Request["hiddenchannelids_" + soft.ID.ToString()].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => new ChannelRight { ChannelID = Convert.ToInt32(p), ChannelType = ChannelTypeOptions.Customer }).ToList());

                    channelService.AddUserChannelRights(userId, soft.ID, channelRights);
                }
            }
            Response.Redirect(ReturnUrl);
        }

        /// <summary>
        /// 返回上级页面的地址
        /// </summary>
        protected string ReturnUrl
        {
            get { return string.IsNullOrEmpty(Request["ReturnUrl"]) ? "UserManage.aspx" : Request["ReturnUrl"]; }
        }

        protected List<Soft> UserSofts;
        /// <summary>
        /// 获取指定用户的可选择产品列表
        /// </summary>
        /// <returns></returns>
        private List<Soft> GetUserSofts(int userId)
        {
            List<Soft> softs = new URBasicInfoService().GetSofts();
            List<RightItem> softRights = new URRightsService().GetUserSoftRights(userId);
            List<Soft> userSofts = (from a in softs
                                         join b in softRights on a.ID equals b.RightID
                                         select a).ToList();
            return userSofts;
        }
    }
}